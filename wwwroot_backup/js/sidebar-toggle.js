// CyberForum - Sidebar Toggle (Thu gọn taskbar)
class SidebarToggle {
  static STORAGE_KEY = 'cf_sidebar_collapsed';
  static _initialized = false;

  static init() {
    if (this._initialized) return;
    this._initialized = true;

    // Áp dụng trạng thái đã lưu
    this._applySavedState();

    // Lắng nghe click trên tất cả nút có data-sidebar-toggle
    document.addEventListener('click', (e) => {
      const btn = e.target.closest('[data-sidebar-toggle]');
      if (btn) {
        e.preventDefault();
        this.toggle();
      }
    });

    // Theo dõi thay đổi kích thước: nếu xuống dưới 1024px thì tự động mở rộng
    try {
      const mq = window.matchMedia('(max-width: 1024px)');
      mq.addEventListener('change', (ev) => {
        if (ev.matches) document.body.classList.remove('cf-sidebar-collapsed');
      });
    } catch (_) {}
  }

  static toggle() {
    const collapsed = document.body.classList.toggle('cf-sidebar-collapsed');
    try { localStorage.setItem(this.STORAGE_KEY, collapsed ? '1' : '0'); } catch (_) {}
    this._updateToggleUI(collapsed);
  }

  static expand() {
    document.body.classList.remove('cf-sidebar-collapsed');
    try { localStorage.setItem(this.STORAGE_KEY, '0'); } catch (_) {}
    this._updateToggleUI(false);
  }

  static collapse() {
    document.body.classList.add('cf-sidebar-collapsed');
    try { localStorage.setItem(this.STORAGE_KEY, '1'); } catch (_) {}
    this._updateToggleUI(true);
  }

  static _applySavedState() {
    let saved = '0';
    try { saved = localStorage.getItem(this.STORAGE_KEY) || '0'; } catch (_) {}

    // Chỉ áp dụng trên màn hình >= 1024px (tránh xung đột với responsive mobile)
    if (window.innerWidth >= 1024 && saved === '1') {
      document.body.classList.add('cf-sidebar-collapsed');
      // Cập nhật UI sau khi DOM ready
      document.addEventListener('DOMContentLoaded', () => this._updateToggleUI(true));
    } else {
      document.addEventListener('DOMContentLoaded', () => this._updateToggleUI(false));
    }
  }

  static _updateToggleUI(collapsed) {
    document.querySelectorAll('[data-sidebar-toggle]').forEach(btn => {
      const icon = btn.querySelector('.ti, i');
      if (collapsed) {
        if (icon) icon.className = (icon.className || '').replace(/ti-layout-sidebar|ti-layout-sidebar-right-collapse|ti-chevron-left/g, '').trim() + ' ti ti-layout-sidebar-right-expand';
        btn.title = 'Mở rộng taskbar';
      } else {
        if (icon) icon.className = (icon.className || '').replace(/ti-layout-sidebar|ti-layout-sidebar-right-expand|ti-chevron-right/g, '').trim() + ' ti ti-layout-sidebar-left-collapse';
        btn.title = 'Thu gọn taskbar';
      }
    });
  }
}
