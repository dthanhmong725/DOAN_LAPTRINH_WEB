async function initSettings() {
    // Đọc username từ data attribute - không cần inline script, không bị CSP block
    const settingsRoot = document.getElementById('settingsRoot');
    const username = settingsRoot ? settingsRoot.dataset.username : '';

    const tabs = document.querySelectorAll('.settings-tab');
    tabs.forEach(tab => {
        tab.addEventListener('click', () => {
            const tabId = tab.getAttribute('data-tab');
            document.querySelectorAll('.settings-panel').forEach(p => p.style.display = 'none');
            document.querySelectorAll('.settings-tab').forEach(t => t.classList.remove('active'));
            document.getElementById(`tab-${tabId}`).style.display = 'block';
            tab.classList.add('active');
        });
    });

    // 2. Load Profile Data
    try {
        if (username) {
            const r = await API.users.getProfile(username);
            if (r.success) {
                const u = r.data;
                const displayNameEl = document.getElementById('displayName');
                if (displayNameEl) displayNameEl.value = u.displayName || '';
                
                const bioEl = document.getElementById('bio');
                if (bioEl) bioEl.value = u.bio || '';
                
                const avatarUrlEl = document.getElementById('avatarUrl');
                if (avatarUrlEl) avatarUrlEl.value = u.avatarUrl || '';
                
                const coverPhotoUrlEl = document.getElementById('coverPhotoUrl');
                if (coverPhotoUrlEl) coverPhotoUrlEl.value = u.coverPhotoUrl || '';
            }
        }
    } catch (e) {
        console.warn('Failed to load profile data', e);
    }

    // 3. Save Profile Logic
    const btnSaveProfile = document.getElementById('btnSaveProfile');
    if (btnSaveProfile) {
        btnSaveProfile.addEventListener('click', async () => {
            const data = {
                displayName: document.getElementById('displayName').value.trim() || null,
                bio: document.getElementById('bio').value.trim() || null,
                avatarUrl: document.getElementById('avatarUrl').value.trim() || null,
                coverPhotoUrl: document.getElementById('coverPhotoUrl').value.trim() || null
            };
            try {
                // Ensure API object is available
                if (typeof API !== 'undefined' && API.users && API.users.updateProfile) {
                    const r = await API.users.updateProfile(data);
                    if (r && r.success) {
                        Toast.show('Đã lưu thay đổi!', 'success');
                    } else {
                        Toast.show(r?.message || 'Không thể lưu', 'error');
                    }
                } else {
                    Toast.show('API không khả dụng', 'error');
                }
            } catch (e) {
                Toast.show(e.message || 'Có lỗi xảy ra', 'error');
            }
        });
    }

    // 4. Appearance Settings (Theme/Font)
    const fontSizeSelect = document.getElementById('fontSizeSelect');
    if (fontSizeSelect) {
        // Set initial value from localStorage if exists
        const savedSize = localStorage.getItem('cf_font_size');
        if (savedSize) {
            fontSizeSelect.value = savedSize;
        }

        fontSizeSelect.addEventListener('change', (e) => {
            const size = e.target.value;
            document.documentElement.style.fontSize = size + 'px';
            localStorage.setItem('cf_font_size', size);
        });
    }
}

// Khởi chạy khi script được tải
initSettings();
