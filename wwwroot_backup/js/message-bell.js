// CyberForum - Message Bell Module
// Nút tin nhắn đã được đặt sẵn trong _Layout.cshtml, module này chỉ fetch badge count.
class MessageBell {
  static _pollInterval = null;
  static _POLL_MS = 15000;
  static _initialized = false;
  static _unreadCount = 0;

  // ============================================================
  // KHỞI TẠO & HỦY
  // ============================================================
  static init(force = false) {
    if (!force && !AuthManager.isAuthenticated()) return;
    if (this._initialized) return;
    this._initialized = true;

    this._fetchCount();
    this._pollInterval = setInterval(() => this._fetchCount(), this._POLL_MS);
  }

  static destroy() {
    if (this._pollInterval) { clearInterval(this._pollInterval); this._pollInterval = null; }
    this._initialized = false;
    this._unreadCount = 0;
    const badge = document.getElementById('msgBadge');
    if (badge) badge.classList.remove('show');
  }

  // ============================================================
  // FETCH DỮ LIỆU
  // ============================================================
  static async _fetchCount() {
    // Không check auth ở đây vì đã được xác nhận từ server-side (Razor) hoặc AuthManager
    try {
      const result = await API.chat.getUnreadCount();
      const count = result?.unreadCount ?? result?.UnreadCount ?? 0;
      this._renderBadge(count);
    } catch (e) {
      console.warn('[MessageBell] _fetchCount error:', e.message);
    }
  }

  // ============================================================
  // RENDER BADGE
  // ============================================================
  static _renderBadge(count) {
    const oldCount = this._unreadCount;
    this._unreadCount = count;
    const badge = document.getElementById('msgBadge');
    if (!badge) return;
    if (count > 0) {
      badge.textContent = count > 99 ? '99+' : count;
      badge.classList.add('show');
      if (count > oldCount) {
        badge.classList.remove('bounce');
        void badge.offsetWidth;
        badge.classList.add('bounce');
      }
    } else {
      badge.classList.remove('show');
    }
  }
}
