// CyberForum - Site Initialization
// Tabler Icons are used throughout the site
// All pages share these common utilities

// Apply theme ASAP to prevent flash (FOUC). Đặt ở đầu file để chạy ngay khi load.
(function () {
  try {
    var saved = localStorage.getItem('cf_theme');
    var theme = (saved === 'dark' || saved === 'light')
      ? saved
      : (window.matchMedia && window.matchMedia('(prefers-color-scheme: light)').matches ? 'light' : 'dark');
    document.documentElement.setAttribute('data-theme', theme);
  } catch (_) {
    document.documentElement.setAttribute('data-theme', 'dark');
  }
})();

/**
 * SiteUtils - Shared site-level helpers (theme, logout buttons, sidebar logout mover)
 */
class SiteUtils {
  /**
   * Gắn click handler cho tất cả nút đăng xuất.
   * Hỗ trợ cả #logoutBtn trong user dropdown và bất kỳ nút nào có data-action="logout"
   * hoặc onclick="AuthManager.logout()"
   */
  static bindLogoutButtons() {
    document.querySelectorAll('#logoutBtn, [data-action="logout"], [data-logout]').forEach(btn => {
      // Bỏ qua nếu đã gắn rồi
      if (btn.dataset.logoutBound === '1') return;
      btn.dataset.logoutBound = '1';

      // Bỏ onclick inline để chỉ handler mới chạy
      btn.removeAttribute('onclick');

      btn.addEventListener('click', (e) => {
        e.preventDefault();
        e.stopPropagation();
        if (typeof AuthManager === 'undefined') return;
        if (!AuthManager.isAuthenticated()) {
          window.location.href = '/auth/login';
          return;
        }
        if (typeof Toast !== 'undefined') {
          Toast.show('Đang đăng xuất...', 'info', 1500);
        }
        // Đóng user dropdown nếu có
        const dropdown = document.getElementById('userDropdown');
        if (dropdown) dropdown.style.display = 'none';
        setTimeout(() => AuthManager.logout(), 200);
      });
    });

    // Với các nút logout có onclick trực tiếp (legacy), wrap lại
    document.querySelectorAll('[onclick*="AuthManager.logout"]').forEach(btn => {
      if (btn.dataset.logoutBound === '1') return;
      btn.dataset.logoutBound = '1';
      const oldOnclick = btn.getAttribute('onclick');
      btn.removeAttribute('onclick');
      btn.addEventListener('click', (e) => {
        e.preventDefault();
        if (typeof AuthManager !== 'undefined') AuthManager.logout();
      });
    });
  }

  /**
   * Nếu page có user-dropdown với #logoutBtn, gỡ logout khỏi dropdown
   * vì đã chuyển xuống phần quản trị. Nhưng nếu user chưa đăng nhập thì không có
   * gì trong admin sidebar, nên vẫn giữ nút logout ở dropdown cho user bình thường.
   *
   * Logic: nếu có .nav-section.admin-section (do page đặt) thì thêm nút logout vào đó
   * cho cả user thường; nếu không thì giữ nguyên dropdown.
   */
  static setupAdminLogout() {
    if (typeof AuthManager === 'undefined' || !AuthManager.isAuthenticated()) return;

    // Tìm section admin-section trong sidebar (do page đặt sẵn)
    const adminSection = document.querySelector('.nav-section.admin-logout-section, .nav-section.requires-auth:has(.admin-logout-anchor)');
    if (adminSection && !document.getElementById('adminLogoutBtn')) {
      // Kiểm tra xem đã có logout trong admin section chưa
      if (!adminSection.querySelector('[data-action="logout"]')) {
        const logoutLink = document.createElement('a');
        logoutLink.href = 'javascript:void(0)';
        logoutLink.id = 'adminLogoutBtn';
        logoutLink.setAttribute('data-action', 'logout');
        logoutLink.className = 'nav-item';
        logoutLink.style.cssText = 'padding: 9px 16px; color: var(--accent-red); display: flex; align-items: center; gap: 8px; margin-top: 8px; border-top: 1px solid var(--border);';
        logoutLink.innerHTML = '<i class="ti ti-logout"></i><span>Đăng xuất</span>';
        adminSection.appendChild(logoutLink);
        // Gắn handler
        logoutLink.addEventListener('click', (e) => {
          e.preventDefault();
          if (typeof AuthManager !== 'undefined') AuthManager.logout();
        });
      }
    }
  }

  /**
   * Tự động bind tất cả handlers chung của site. Gọi 1 lần ở mỗi page.
   */
  static init() {
    // Đợi DOM ready nếu script chạy sớm
    const run = () => {
      this.bindLogoutButtons();
      this.setupAdminLogout();
    };
    if (document.readyState === 'loading') {
      document.addEventListener('DOMContentLoaded', run, { once: true });
    } else {
      run();
    }

    // Re-bind sau khi AuthManager cập nhật UI (vì user-menu có thể được show/hide)
    if (typeof AuthManager !== 'undefined') {
      const originalUpdateUI = AuthManager.updateUI.bind(AuthManager);
      AuthManager.updateUI = function() {
        originalUpdateUI();
        // Bind lại cho các element mới
        setTimeout(() => SiteUtils.bindLogoutButtons(), 50);
      };
    }
  }
}

document.addEventListener('DOMContentLoaded', () => {
  if (typeof ThemeManager !== 'undefined') {
    ThemeManager.init();
    ThemeManager.injectToggleButton();
  }
  if (typeof SidebarToggle !== 'undefined') SidebarToggle.init();
  if (typeof SiteUtils !== 'undefined') SiteUtils.init();

  if (typeof AuthManager !== 'undefined') {
    AuthManager.restoreSession();
    AuthManager.updateUI();
  }

  // Đọc trạng thái auth từ server (Razor đã set trong data-server-auth).
  // Dùng data attribute thay vì inline script để tránh bị CSP block.
  const serverAuth = document.body.dataset.serverAuth === 'true';
  const jsAuth = typeof AuthManager !== 'undefined' && AuthManager.isAuthenticated();
  const isAuthed = serverAuth || jsAuth;

  if (isAuthed) {
    if (typeof NotificationBell !== 'undefined' && !NotificationBell._initialized) {
      NotificationBell.init(true);
    }
    if (typeof MessageBell !== 'undefined' && !MessageBell._initialized) {
      MessageBell.init(true);
    }
  }
});

// ANIMATED BACKGROUND ORBS & CANVAS
document.addEventListener("DOMContentLoaded", function () {
    const prefersReducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    if (!prefersReducedMotion) {
        const canvas = document.getElementById('bg-canvas');
        if (!canvas) return; // Bảo vệ tránh lỗi ở các trang không có canvas
        
        const ctx = canvas.getContext('2d');
        let particles = [];
        let width, height;
        
        // Màu sắc của các hạt phân tử
        const colors = [
            'rgba(244, 113, 181, 0.6)', // Pink
            'rgba(45, 212, 191, 0.5)',  // Teal
            'rgba(245, 200, 66, 0.4)'   // Gold
        ];
        function resize() {
            width = canvas.width = window.innerWidth;
            height = canvas.height = window.innerHeight;
        }
        class Particle {
            constructor() {
                this.x = Math.random() * width;
                this.y = Math.random() * height;
                this.r = 1.5 + Math.random() * 1.5;
                this.vx = (Math.random() - 0.5) * 0.8;
                this.vy = (Math.random() - 0.5) * 0.8;
                this.color = colors[Math.floor(Math.random() * colors.length)];
            }
            update() {
                this.x += this.vx;
                this.y += this.vy;
                if (this.x < 0 || this.x > width) this.vx *= -1;
                if (this.y < 0 || this.y > height) this.vy *= -1;
            }
            draw() {
                ctx.beginPath();
                ctx.arc(this.x, this.y, this.r, 0, Math.PI * 2);
                ctx.fillStyle = this.color;
                ctx.fill();
            }
        }
        function initParticles() {
            particles = [];
            const count = window.innerWidth < 768 ? 40 : 80;
            for (let i = 0; i < count; i++) {
                particles.push(new Particle());
            }
        }
        function animateCanvas() {
            ctx.clearRect(0, 0, width, height);
            for (let i = 0; i < particles.length; i++) {
                particles[i].update();
                particles[i].draw();
                for (let j = i + 1; j < particles.length; j++) {
                    const dx = particles[i].x - particles[j].x;
                    const dy = particles[i].y - particles[j].y;
                    const dist = Math.sqrt(dx * dx + dy * dy);
                    
                    if (dist < 120) {
                        ctx.beginPath();
                        ctx.moveTo(particles[i].x, particles[i].y);
                        ctx.lineTo(particles[j].x, particles[j].y);
                        const opacity = 1 - (dist / 120);
                        ctx.strokeStyle = `rgba(255, 255, 255, ${opacity * 0.06})`;
                        ctx.lineWidth = 1;
                        ctx.stroke();
                    }
                }
            }
            requestAnimationFrame(animateCanvas);
        }
        let lastWidth = window.innerWidth;
        window.addEventListener('resize', () => {
            resize();
            if (Math.abs(window.innerWidth - lastWidth) > 50) {
                initParticles();
                lastWidth = window.innerWidth;
            }
        });
        resize();
        initParticles();
        animateCanvas();
    }
});
