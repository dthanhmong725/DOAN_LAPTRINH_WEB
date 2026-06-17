# CyberForum - Apply EF Migrations
# Chạy script này để áp dụng các migrations mới lên database

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  CyberForum - Apply Migrations" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Set-Location $PSScriptRoot

try {
    Write-Host "[1/2] Đang áp dụng migrations..." -ForegroundColor Yellow
    dotnet ef database update
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "[2/2] Hoàn tất!" -ForegroundColor Green
        Write-Host ""
        Write-Host "Đã áp dụng migrations thành công lên database." -ForegroundColor Green
        Write-Host ""
        Write-Host "Các bảng mới được tạo:" -ForegroundColor Cyan
        Write-Host "  - ReputationHistories (lịch sử điểm uy tín)" -ForegroundColor White
        Write-Host "  - ChatMessageReactions (reactions cho tin nhắn)" -ForegroundColor White
        Write-Host ""
        Write-Host "Các cột mới được thêm vào ChatMessages:" -ForegroundColor Cyan
        Write-Host "  - PinnedAt, PinnedById" -ForegroundColor White
    } else {
        Write-Host ""
        Write-Host "[LỖI] Không thể áp dụng migrations." -ForegroundColor Red
        Write-Host "Vui lòng kiểm tra connection string trong appsettings.json" -ForegroundColor Red
        exit 1
    }
}
catch {
    Write-Host ""
    Write-Host "[LỖI] $_" -ForegroundColor Red
    exit 1
}
