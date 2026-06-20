// CyberForum - Message Bell Module
// Hiển thị nút chuông tin nhắn (badge số chưa đọc) ngay cạnh nút Thông báo.
// Bấm vào sẽ điều hướng thẳng tới /chat.html (không có dropdown xem trước).
class MessageBell {
  static _pollInterval = null;
  static _POLL_MS = 15000;
  static _initialized = false;
  static _unreadCount = 0;

  // ============================================================
  // KHỞI TẠO & HỦY
  // ============================================================
  static init() {
    if (!AuthManager.isAuthenticated()) return;
    if (this._initialized) return;
    this._initialized = true;

    this._injectUI();
    this._fetchCount();
    this._pollInterval = setInterval(() => this._fetchCount(), this._POLL_MS);
  }

  static destroy() {
    if (this._pollInterval) { clearInterval(this._pollInterval); this._pollInterval = null; }
    this._initialized = false;
    this._unreadCount = 0;
    const btn = document.getElementById('messageBellBtn');
    if (btn) btn.remove();
  }

  // ============================================================
  // INJECT UI (nút chuông + badge)
  // ============================================================
  static _injectUI() {
    if (document.getElementById('messageBellBtn')) return;

    // Tìm nút thông báo hiện có để chèn ngay cạnh (sau khi NotificationBell đã đổi id)
    const notifBtn = document.getElementById('notifBellBtn') || document.getElementById('notificationBtn');
    if (!notifBtn) return;

    const btn = document.createElement('button');
    btn.id = 'messageBellBtn';
    btn.className = notifBtn.className; // dùng chung style btn-ghost btn-icon với chuông thông báo
    btn.title = 'Tin nhắn';
    btn.style.position = 'relative';
    btn.innerHTML = `
      <i class="ti ti-mail"></i>
      <span id="msgBadge" class="nb-badge"></span>
    `;

    btn.addEventListener('click', () => {
      window.location.href = '/chat.html';
    });

    notifBtn.insertAdjacentElement('afterend', btn);
  }

  // ============================================================
  // FETCH DỮ LIỆU
  // ============================================================
  static async _fetchCount() {
    if (!AuthManager.isAuthenticated()) return;
    try {
      const result = await API.chat.getUnreadCount();
      const count = result?.unreadCount ?? result?.UnreadCount ?? 0;
      this._renderBadge(count);
    } catch (e) {
      console.warn('[MessageBell] _fetchCount error:', e.message);
    }
  }

  // ============================================================
  // RENDER UI
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
