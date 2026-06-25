// CyberForum - Theme Manager
// Quản lý theme sáng/tối, lưu vào localStorage
class ThemeManager {
  static STORAGE_KEY = 'cf_theme';
  static DEFAULT_THEME = 'dark'; // Cyber forum mặc định dark

  static _currentTheme = null;
  static _listeners = [];

  /**
   * Khởi tạo theme manager.
   * Đọc theme từ localStorage; nếu chưa có, dùng prefers-color-scheme của hệ thống;
   * nếu không xác định được thì dùng DEFAULT_THEME.
   */
  static init() {
    const saved = this._readSaved();
    const theme = saved || this._detectSystemTheme();
    this.apply(theme);
  }

  /**
   * Áp dụng theme cụ thể ('dark' | 'light')
   */
  static apply(theme) {
    if (theme !== 'dark' && theme !== 'light') theme = this.DEFAULT_THEME;
    this._currentTheme = theme;
    document.documentElement.setAttribute('data-theme', theme);

    // Cập nhật highlight.js stylesheet nếu có
    this._swapHighlightTheme(theme);

    // Cập nhật icon & title của nút toggle (nếu có)
    this._updateToggleUI(theme);

    try { localStorage.setItem(this.STORAGE_KEY, theme); } catch (_) {}

    this._listeners.forEach(fn => { try { fn(theme); } catch (_) {} });
  }

  static toggle() {
    const next = this._currentTheme === 'dark' ? 'light' : 'dark';
    this.apply(next);
  }

  static getTheme() {
    return this._currentTheme || this.DEFAULT_THEME;
  }

  static onChange(fn) {
    this._listeners.push(fn);
  }

  // ============================================================
  // HELPERS
  // ============================================================
  static _readSaved() {
    try {
      const v = localStorage.getItem(this.STORAGE_KEY);
      if (v === 'dark' || v === 'light') return v;
    } catch (_) {}
    return null;
  }

  static _detectSystemTheme() {
    try {
      if (window.matchMedia && window.matchMedia('(prefers-color-scheme: light)').matches) {
        return 'light';
      }
    } catch (_) {}
    return this.DEFAULT_THEME;
  }

  static _swapHighlightTheme(theme) {
    try {
      const link = document.getElementById('hljs-theme');
      if (!link) return;
      link.href = theme === 'light'
        ? 'https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.9.0/styles/github.min.css'
        : 'https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.9.0/styles/atom-one-dark.min.css';
    } catch (_) {}
  }

  static _updateToggleUI(theme) {
    document.querySelectorAll('[data-theme-toggle]').forEach(btn => {
      const icon = btn.querySelector('.ti, i');
      const label = btn.querySelector('[data-theme-label]');
      if (theme === 'light') {
        if (icon) { icon.className = (icon.className || '').replace(/ti-sun/g, '') + ' ti ti-moon-stars'; }
        if (label) label.textContent = 'Chế độ tối';
        btn.title = 'Chuyển sang chế độ tối';
      } else {
        if (icon) { icon.className = (icon.className || '').replace(/ti-moon-stars/g, '') + ' ti ti-sun'; }
        if (label) label.textContent = 'Chế độ sáng';
        btn.title = 'Chuyển sang chế độ sáng';
      }
    });
  }

  /**
   * Tự động inject nút theme toggle vào DOM.
   * - Nếu có sẵn [data-theme-toggle]: chỉ bind event
   * - Nếu có #themeToggleAnchor: tạo nút và chèn vào anchor
   * - Mặc định: tìm notification bell và chèn ngay trước/sau
   */
  static injectToggleButton() {
    // Trường hợp 1: đã có sẵn nút, chỉ cần bind
    const existing = document.querySelector('[data-theme-toggle]');
    if (existing) {
      existing.addEventListener('click', (e) => {
        e.preventDefault();
        e.stopPropagation();
        this.toggle();
      });
      this._updateToggleUI(this.getTheme());
      return;
    }

    // Trường hợp 2: có anchor sẵn
    const anchor = document.getElementById('themeToggleAnchor');
    if (anchor) {
      const btn = this._createToggleButton();
      anchor.appendChild(btn);
      this._updateToggleUI(this.getTheme());
      return;
    }

    // Trường hợp 3: tự động tìm vị trí thích hợp
    // Ưu tiên: kế bên notification bell
    const notifBtn = document.getElementById('notificationBtn') || document.getElementById('notifBellBtn');
    if (notifBtn && notifBtn.parentElement) {
      // Nếu notification bell đã có trong DOM, chèn theme toggle trước nó
      const btn = this._createToggleButton();
      notifBtn.parentElement.insertBefore(btn, notifBtn);
      this._updateToggleUI(this.getTheme());
      return;
    }

    // Fallback: chèn vào navbar-right
    const navbarRight = document.querySelector('.navbar-right, .navbar .user-menu, nav');
    if (navbarRight) {
      const btn = this._createToggleButton();
      navbarRight.insertBefore(btn, navbarRight.firstChild);
      this._updateToggleUI(this.getTheme());
    }
  }

  static _createToggleButton() {
    const btn = document.createElement('button');
    btn.className = 'btn btn-ghost btn-icon theme-toggle-nav';
    btn.setAttribute('data-theme-toggle', '');
    btn.setAttribute('aria-label', 'Chuyển theme sáng/tối');
    btn.title = 'Chuyển theme sáng/tối';
    btn.innerHTML = '<i class="ti ti-sun"></i>';
    btn.addEventListener('click', (e) => {
      e.preventDefault();
      e.stopPropagation();
      this.toggle();
    });
    return btn;
  }
}
