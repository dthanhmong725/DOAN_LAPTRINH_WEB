using Microsoft.EntityFrameworkCore;
using DOAN_LAPTRINHWEB.Data;
using DOAN_LAPTRINHWEB.Interfaces;

namespace DOAN_LAPTRINHWEB.Services;

public class RateLimitService : IRateLimitService
{
    private readonly AppDbContext _context;
    private const int MaxRequests = 60;
    private const int WindowMinutes = 1;

    public RateLimitService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsRateLimitedAsync(string userId, string endpoint)
    {
        var windowStart = DateTime.UtcNow.AddMinutes(-WindowMinutes);

        var record = await _context.RateLimitRecords
            .FirstOrDefaultAsync(rlr =>
                rlr.UserId == userId &&
                rlr.Endpoint == endpoint &&
                rlr.WindowStart >= windowStart);

        if (record == null)
            return false;

        return record.RequestCount >= MaxRequests;
    }

    public async Task IncrementAsync(string userId, string endpoint)
    {
        var windowStart = DateTime.UtcNow.AddMinutes(-WindowMinutes);

        var record = await _context.RateLimitRecords
            .FirstOrDefaultAsync(rlr =>
                rlr.UserId == userId &&
                rlr.Endpoint == endpoint &&
                rlr.WindowStart >= windowStart);

        if (record == null)
        {
            _context.RateLimitRecords.Add(new Models.Entities.RateLimitRecord
            {
                UserId = userId,
                Endpoint = endpoint,
                RequestCount = 1,
                WindowStart = DateTime.UtcNow,
                LastRequest = DateTime.UtcNow
            });
        }
        else
        {
            record.RequestCount++;
            record.LastRequest = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        // Cleanup old records (older than 1 hour)
        var cutoff = DateTime.UtcNow.AddHours(-1);
        var oldRecords = await _context.RateLimitRecords
            .Where(rlr => rlr.WindowStart < cutoff)
            .ToListAsync();

        if (oldRecords.Any())
        {
            _context.RateLimitRecords.RemoveRange(oldRecords);
            await _context.SaveChangesAsync();
        }
    }
}
