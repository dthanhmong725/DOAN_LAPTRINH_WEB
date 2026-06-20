// CyberForum - Notification Bell Module v3
// Sử dụng CSS classes (notification-bell.css) thay vì inline styles để dễ theme switching.
class NotificationBell {
  static _pollInterval = null;
  static _POLL_MS = 15000;
  static _isOpen = false;
  static _notifications = [];
  static _unreadCount = 0;
  static _initialized = false;

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

    document.addEventListener('click', (e) => {
      const btn = document.getElementById('notifBellBtn');
      const panel = document.getElementById('notifDropdown');
      if (!panel || !btn) return;
      if (!panel.contains(e.target) && !btn.contains(e.target)) {
        this._closeDropdown();
      }
    });
  }

  static destroy() {
    if (this._pollInterval) { clearInterval(this._pollInterval); this._pollInterval = null; }
    this._initialized = false;
    this._unreadCount = 0;
    this._notifications = [];
    this._isOpen = false;
    const badge = document.getElementById('notifBadge');
    if (badge) badge.classList.remove('show');
    const panel = document.getElementById('notifDropdown');
    if (panel) panel.remove();
  }

  // ============================================================
  // INJECT UI (badge + dropdown)
  // ============================================================
  static _injectUI() {
    const btn = document.getElementById('notificationBtn');
    if (!btn) return;

    btn.id = 'notifBellBtn';
    btn.style.position = 'relative';

    if (!document.getElementById('notifBadge')) {
      const badge = document.createElement('span');
      badge.id = 'notifBadge';
      badge.className = 'nb-badge';
      btn.appendChild(badge);
    }

    if (document.getElementById('notifDropdown')) return;

    const panel = document.createElement('div');
    panel.id = 'notifDropdown';
    panel.className = 'nb-dropdown';
    panel.innerHTML = `
      <div class="nb-header">
        <span class="nb-title">
          <i class="ti ti-bell-ringing"></i> Thông báo
        </span>
        <button id="notifMarkAllBtn" class="nb-mark-all" title="Đánh dấu tất cả đã đọc">
          <i class="ti ti-checks"></i> Đọc tất cả
        </button>
      </div>
      <div id="notifList" class="nb-list"></div>
    `;

    document.body.appendChild(panel);

    btn.addEventListener('click', (e) => {
      e.stopPropagation();
      this._isOpen ? this._closeDropdown() : this._openDropdown();
    });

    document.getElementById('notifMarkAllBtn')?.addEventListener('click', (e) => {
      e.stopPropagation();
      this._markAllRead();
    });
  }

  // ============================================================
  // VỊ TRÍ DROPDOWN
  // ============================================================
  static _positionDropdown() {
    const btn = document.getElementById('notifBellBtn');
    const panel = document.getElementById('notifDropdown');
    if (!btn || !panel) return;
    const rect = btn.getBoundingClientRect();
    panel.style.top = (rect.bottom + 8) + 'px';
    const panelWidth = 380;
    let left = rect.right - panelWidth;
    if (left < 8) left = 8;
    panel.style.right = 'auto';
    panel.style.left = left + 'px';
  }

  // ============================================================
  // FETCH DỮ LIỆU
  // ============================================================
  static async _fetchCount() {
    if (!AuthManager.isAuthenticated()) return;
    try {
      const result = await API.notifications.getUnreadCount();
      const count = result?.data?.unreadCount ?? result?.Data?.unreadCount ?? result?.data?.UnreadCount ?? 0;
      this._renderBadge(count);
    } catch (e) {
      console.warn('[NotificationBell] _fetchCount error:', e.message);
    }
  }

  static async _fetchNotifications() {
    const list = document.getElementById('notifList');
    if (list) {
      list.innerHTML = `
        <div class="nb-loading">
          <div class="nb-spinner"></div>
        </div>`;
    }
    try {
      const result = await API.notifications.getAll({ page: 1, pageSize: 20 });
      const ok = result?.success ?? result?.Success ?? false;
      if (ok) {
        this._notifications = result.data ?? result.Data ?? [];
        this._renderList();
      } else {
        throw new Error(result?.message || result?.Message || 'API trả về thất bại');
      }
    } catch (e) {
      console.error('[NotificationBell] _fetchNotifications error:', e);
      if (list) list.innerHTML = `
        <div class="nb-empty">
          <i class="ti ti-wifi-off" style="font-size: 2rem;"></i>
          <span>Không thể tải thông báo</span>
          <span style="font-size: 0.7rem; opacity: 0.5;">${e.message || ''}</span>
        </div>`;
    }
  }

  // ============================================================
  // RENDER UI
  // ============================================================
  static _renderBadge(count) {
    const oldCount = this._unreadCount;
    this._unreadCount = count;
    const badge = document.getElementById('notifBadge');
    if (!badge) return;
    if (count > 0) {
      badge.textContent = count > 99 ? '99+' : count;
      badge.classList.add('show');
      // Bounce animation nếu tăng so với trước
      if (count > oldCount) {
        badge.classList.remove('bounce');
        void badge.offsetWidth;
        badge.classList.add('bounce');
      }
    } else {
      badge.classList.remove('show');
    }
  }

  static _renderList() {
    const list = document.getElementById('notifList');
    if (!list) return;

    if (!this._notifications.length) {
      list.innerHTML = `
        <div class="nb-empty">
          <i class="ti ti-bell-off"></i>
          <span>Chưa có thông báo nào</span>
        </div>`;
      return;
    }

    list.innerHTML = this._notifications.map(n => this._renderItem(n)).join('');

    list.querySelectorAll('.nb-item').forEach(el => {
      el.addEventListener('click', () => {
        const id = parseInt(el.dataset.id);
        const postId = el.dataset.postId ? parseInt(el.dataset.postId) : null;
        const username = el.dataset.username || null;
        const typeKey = el.dataset.type || '';
        this._handleItemClick(id, postId, username, typeKey);
      });
    });
  }

  static _renderItem(n) {
    const icons = {
      PostUpvote:      { icon: 'ti-thumb-up',       color: 'var(--accent)',       bg: 'rgba(0, 229, 160, 0.15)' },
      PostDownvote:    { icon: 'ti-thumb-down',     color: 'var(--accent-red)',   bg: 'rgba(255, 90, 95, 0.15)' },
      Comment:         { icon: 'ti-message-circle', color: 'var(--accent-blue)',  bg: 'rgba(56, 139, 253, 0.15)' },
      CommentUpvote:   { icon: 'ti-thumb-up',       color: 'var(--accent)',       bg: 'rgba(0, 229, 160, 0.15)' },
      CommentDownvote: { icon: 'ti-thumb-down',     color: 'var(--accent-red)',   bg: 'rgba(255, 90, 95, 0.15)' },
      Mention:         { icon: 'ti-at',             color: 'var(--accent-purple)',bg: 'rgba(188, 140, 255, 0.15)' },
      Follow:          { icon: 'ti-user-plus',      color: 'var(--accent-blue)',  bg: 'rgba(56, 139, 253, 0.15)' },
    };

    const typeKey = n.typeLabel || n.TypeLabel || '';
    const cfg = icons[typeKey] || { icon: 'ti-bell', color: 'var(--text-muted)', bg: 'rgba(255, 255, 255, 0.05)' };
    const actorName = n.actorUsername || n.ActorUsername || '?';
    const initials = actorName.charAt(0).toUpperCase();
    const avatarUrl = n.actorAvatar || n.ActorAvatar;
    const message = n.message || n.Message || 'Thông báo mới';
    const timeAgo = n.timeAgo || n.TimeAgo || '';
    const isRead = n.isRead ?? n.IsRead ?? false;
    const postId = n.postId || n.PostId || '';

    const avatarHtml = avatarUrl
      ? `<img src="${avatarUrl}" alt="${actorName}">`
      : `<span>${initials}</span>`;

    return `
      <div class="nb-item ${isRead ? '' : 'unread'}" data-id="${n.id || n.Id}" data-post-id="${postId}" data-username="${actorName !== '?' ? actorName : ''}" data-type="${typeKey}" role="button" tabindex="0">
        <div class="nb-avatar-wrap">
          <div class="nb-avatar">${avatarHtml}</div>
          <div class="nb-type-icon" style="background:${cfg.bg}; color:${cfg.color};">
            <i class="ti ${cfg.icon}"></i>
          </div>
        </div>
        <div class="nb-body">
          <div class="nb-message">${this._escapeHtml(message)}</div>
          <div class="nb-time" style="color:${cfg.color};">
            <i class="ti ti-clock"></i> ${timeAgo}
          </div>
        </div>
        ${!isRead ? '<div class="nb-dot"></div>' : ''}
      </div>`;
  }

  // ============================================================
  // ACTIONS
  // ============================================================
  static async _handleItemClick(id, postId, username, typeKey) {
    const item = document.querySelector(`.nb-item[data-id="${id}"]`);
    if (item && item.classList.contains('unread')) {
      item.classList.remove('unread');
      this._unreadCount = Math.max(0, this._unreadCount - 1);
      this._renderBadge(this._unreadCount);
      try { await API.notifications.markAsRead(id); } catch (e) {}
    }
    if (postId) {
      this._closeDropdown();
      window.location.href = `/post.html?id=${postId}`;
    } else if (typeKey === 'Follow' && username) {
      this._closeDropdown();
      window.location.href = `/profile.html?username=${encodeURIComponent(username)}`;
    }
  }

  static async _markAllRead() {
    const btn = document.getElementById('notifMarkAllBtn');
    if (btn) {
      btn.disabled = true;
      btn.innerHTML = '<i class="ti ti-loader-2" style="animation:spin 0.8s linear infinite;"></i> Đang xử lý...';
    }

    try {
      await API.notifications.markAllAsRead();
      this._unreadCount = 0;
      this._renderBadge(0);

      this._notifications.forEach(n => { n.isRead = true; n.IsRead = true; });
      this._renderList();

      if (typeof Toast !== 'undefined') Toast.show('Đã đánh dấu tất cả là đã đọc', 'success');
    } catch (e) {
      if (typeof Toast !== 'undefined') Toast.show('Không thể cập nhật thông báo', 'error');
    } finally {
      if (btn) {
        btn.disabled = false;
        btn.innerHTML = '<i class="ti ti-checks"></i> Đọc tất cả';
      }
    }
  }

  // ============================================================
  // OPEN / CLOSE
  // ============================================================
  static _openDropdown() {
    const panel = document.getElementById('notifDropdown');
    if (!panel) return;

    this._positionDropdown();
    this._isOpen = true;
    panel.classList.add('open');
    this._fetchNotifications();
  }

  static _closeDropdown() {
    const panel = document.getElementById('notifDropdown');
    if (!panel) return;
    this._isOpen = false;
    panel.classList.remove('open');
  }

  // ============================================================
  // HELPER
  // ============================================================
  static _escapeHtml(str) {
    return (str || '')
      .replace(/&/g, '&amp;')
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;')
      .replace(/"/g, '&quot;');
  }
}
