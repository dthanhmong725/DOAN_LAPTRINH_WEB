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
    AuthManager.updateUI();
  }

  // Khởi tạo chuông thông báo & chuông tin nhắn trên MỌI trang có chứa nút #notificationBtn.
  // Đặt tập trung ở đây để đồng bộ hành vi, tránh phải lặp lại ở từng trang riêng lẻ.
  // NotificationBell/MessageBell tự bỏ qua nếu đã init hoặc chưa đăng nhập.
  if (typeof AuthManager !== 'undefined' && AuthManager.isAuthenticated()) {
    if (typeof NotificationBell !== 'undefined') {
      NotificationBell.init();
    }
    if (typeof MessageBell !== 'undefined') {
      MessageBell.init();
    }
  }
});
