/**
 * Password Strength Checker
 * Evaluates password locally for instant feedback.
 * Optionally calls API for server-side validation.
 */
class PasswordStrengthChecker {

  static async check(password) {
    return this.checkLocally(password);
  }

  static checkLocally(password) {
    if (!password) {
      return {
        score: 0,
        level: 'Không có',
        suggestions: ['Mật khẩu không được để trống'],
        hasMinLength: false,
        hasUppercase: false,
        hasLowercase: false,
        hasDigit: false,
        hasSpecialChar: false,
        hasNoCommonPatterns: true
      };
    }

    const hasMinLength = password.length >= 8;
    const hasGoodLength = password.length >= 12;
    const hasGreatLength = password.length >= 16;
    const hasUppercase = /[A-Z]/.test(password);
    const hasLowercase = /[a-z]/.test(password);
    const hasDigit = /[0-9]/.test(password);
    const hasSpecial = /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?~`>]/.test(password);

    let score = 0;
    if (hasMinLength) score++;
    if (hasGoodLength) score++;
    if (hasGreatLength) score++;
    if (hasUppercase) score++;
    if (hasLowercase) score++;
    if (hasDigit) score++;
    if (hasSpecial) score += 2;

    const suggestions = [];
    if (!hasMinLength) suggestions.push('Ít nhất 8 ký tự');
    if (!hasGoodLength) suggestions.push('Nên dùng 12 ký tự trở lên');
    if (!hasUppercase) suggestions.push('Thêm chữ HOA (A-Z)');
    if (!hasLowercase) suggestions.push('Thêm chữ thường (a-z)');
    if (!hasDigit) suggestions.push('Thêm số (0-9)');
    if (!hasSpecial) suggestions.push('Thêm ký tự đặc biệt (!@#$%...)');

    let level = 'Rất yếu';
    if (score >= 8) level = 'Rất mạnh';
    else if (score >= 6) level = 'Mạnh';
    else if (score >= 4) level = 'Trung bình';
    else if (score >= 2) level = 'Yếu';

    return {
      score,
      level,
      suggestions,
      hasMinLength,
      hasUppercase,
      hasLowercase,
      hasDigit,
      hasSpecialChar: hasSpecial,
      hasNoCommonPatterns: true
    };
  }

  static getLevelClass(level) {
    const map = {
      'Không có': 'level-none',
      'Rất yếu': 'level-weak',
      'Yếu': 'level-weak',
      'Trung bình': 'level-fair',
      'Mạnh': 'level-strong',
      'Rất mạnh': 'level-very-strong',
      'Hoàn hảo': 'level-very-strong'
    };
    return map[level] || 'level-none';
  }

  static getLevelColor(level) {
    const map = {
      'Không có': 'var(--text-muted)',
      'Rất yếu': 'var(--accent-red)',
      'Yếu': '#ff7b54',
      'Trung bình': '#ffc107',
      'Mạnh': 'var(--accent)',
      'Rất mạnh': '#4ade80',
      'Hoàn hảo': '#4ade80'
    };
    return map[level] || 'var(--text-muted)';
  }

  static getSegmentCount(score) {
    // score is 0-10, divide into 4 tiers for 4 segments
    if (score >= 8) return 4;
    if (score >= 5) return 3;
    if (score >= 3) return 2;
    if (score >= 1) return 1;
    return 0;
  }

  static renderMeter(strength) {
    const segments = 4;
    const filled = this.getSegmentCount(strength.score);
    const levelClass = this.getLevelClass(strength.level);
    const color = this.getLevelColor(strength.level);

    let html = `<div class="pw-strength-meter" role="meter" aria-valuenow="${strength.score}" aria-valuemin="0" aria-valuemax="10" aria-label="Độ mạnh mật khẩu">`;
    for (let i = 1; i <= segments; i++) {
      const isFilled = i <= filled;
      const segColor = isFilled ? color : 'var(--bg-surface2)';
      html += `<div class="pw-meter-seg ${isFilled ? levelClass : ''}" style="background: ${segColor};"></div>`;
    }
    html += `</div>`;
    html += `<div class="pw-strength-label">`;
    html += `<span class="pw-level-text" style="color: ${color}; font-weight: 600; font-size: 0.8125rem;">${strength.level}</span>`;
    html += `</div>`;
    return html;
  }

  static renderRequirements(strength) {
    const items = [
      { key: 'hasMinLength', label: 'Ít nhất 8 ký tự' },
      { key: 'hasUppercase', label: 'Chữ HOA (A-Z)' },
      { key: 'hasLowercase', label: 'Chữ thường (a-z)' },
      { key: 'hasDigit', label: 'Số (0-9)' },
      { key: 'hasSpecialChar', label: 'Ký tự đặc biệt (!@#$%...)' }
    ];

    return items.map(item => `
      <div class="pw-req-item ${strength[item.key] ? 'valid' : ''}">
        <span class="pw-req-icon">
          ${strength[item.key]
            ? '<svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3" stroke-linecap="round" stroke-linejoin="round"><polyline points="20 6 9 17 4 12"></polyline></svg>'
            : '<svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3" stroke-linecap="round" stroke-linejoin="round"><line x1="18" y1="6" x2="6" y2="18"></line><line x1="6" y1="6" x2="18" y2="18"></line></svg>'}
        </span>
        <span class="pw-req-label">${item.label}</span>
      </div>
    `).join('');
  }

  static render(strength) {
    const meter = this.renderMeter(strength);
    const reqs = this.renderRequirements(strength);
    return `${meter}<div class="pw-requirements">${reqs}</div>`;
  }
}

/**
 * Password Strength Manager — binds input events to UI updates.
 */
class PasswordStrengthManager {
  constructor(config) {
    this.config = {
      passwordInput: null,
      confirmInput: null,
      strengthContainer: null,
      confirmContainer: null,
      submitBtn: null,
      minScore: 2,
      debounceMs: 100,
      ...config
    };
    this._debounceTimer = null;
    this._bound = false;
  }

  init() {
    const { passwordInput, strengthContainer } = this.config;
    if (!passwordInput || !strengthContainer) return;

    this._bound = true;
    passwordInput.addEventListener('input', () => this._onPasswordInput());
    passwordInput.addEventListener('paste', () => setTimeout(() => this._onPasswordInput(), 0));

    if (this.config.confirmInput && this.config.confirmContainer) {
      this.config.confirmInput.addEventListener('input', () => this._updateConfirmState());
      this.config.confirmInput.addEventListener('paste', () => setTimeout(() => this._updateConfirmState(), 0));
    }

    if (this.config.submitBtn) {
      this.config.submitBtn.addEventListener('click', (e) => {
        const pw = this.config.passwordInput.value;
        const strength = PasswordStrengthChecker.checkLocally(pw);
        if (strength.score < this.config.minScore) {
          e.preventDefault();
          Toast.show('Mật khẩu chưa đủ mạnh. Vui lòng kiểm tra lại.', 'warning');
        }
      });
    }

    this._updatePasswordToggle();
  }

  _onPasswordInput() {
    clearTimeout(this._debounceTimer);
    this._debounceTimer = setTimeout(async () => {
      const pw = this.config.passwordInput.value;
      const strength = await PasswordStrengthChecker.check(pw);
      this.config.strengthContainer.innerHTML = PasswordStrengthChecker.render(strength);
      this._updateConfirmState();
      this._updateSubmitState(strength);
    }, this.config.debounceMs);
  }

  _updateConfirmState() {
    const { confirmInput, confirmContainer } = this.config;
    if (!confirmInput || !confirmContainer) return;

    const pw = this.config.passwordInput.value;
    const confirm = confirmInput.value;

    if (!confirm) {
      confirmContainer.innerHTML = '';
      return;
    }

    const match = pw === confirm;
    const color = match ? 'var(--accent)' : 'var(--accent-red)';
    const icon = match
      ? '<svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3" stroke-linecap="round" stroke-linejoin="round"><polyline points="20 6 9 17 4 12"></polyline></svg>'
      : '<svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3" stroke-linecap="round" stroke-linejoin="round"><line x1="18" y1="6" x2="6" y2="18"></line><line x1="6" y1="6" x2="18" y2="18"></line></svg>';
    const label = match ? 'Mật khẩu khớp' : 'Mật khẩu không khớp';

    confirmContainer.innerHTML = `
      <div class="pw-confirm-indicator" style="display: flex; align-items: center; gap: 6px; font-size: 0.8125rem; color: ${color};">
        <span style="display: flex; align-items: center;">${icon}</span>
        <span>${label}</span>
      </div>
    `;
  }

  _updateSubmitState(strength) {
    const btn = this.config.submitBtn;
    if (!btn) return;
    const disabled = strength.score < this.config.minScore;
    btn.disabled = disabled;
    btn.style.opacity = disabled ? '0.5' : '1';
    btn.style.cursor = disabled ? 'not-allowed' : 'pointer';
  }

  _updatePasswordToggle() {
    const { passwordInput } = this.config;
    if (!passwordInput) return;
    const wrapper = passwordInput.closest('.pw-input-wrapper');
    if (!wrapper) return;
    const existing = wrapper.querySelector('.pw-toggle-btn');
    if (existing) return;

    const toggleBtn = document.createElement('button');
    toggleBtn.type = 'button';
    toggleBtn.className = 'pw-toggle-btn';
    toggleBtn.setAttribute('aria-label', 'Hiện/ẩn mật khẩu');
    toggleBtn.innerHTML = `<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path><circle cx="12" cy="12" r="3"></circle></svg>`;

    toggleBtn.addEventListener('click', function () {
      const isPassword = passwordInput.type === 'password';
      passwordInput.type = isPassword ? 'text' : 'password';
      this.innerHTML = isPassword
        ? `<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24"></path><line x1="1" y1="1" x2="23" y2="23"></line></svg>`
        : `<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path><circle cx="12" cy="12" r="3"></circle></svg>`;
      passwordInput.focus();
    });

    wrapper.appendChild(toggleBtn);
  }
}

// Auto-init on DOMContentLoaded for elements with data-strength-check
document.addEventListener('DOMContentLoaded', () => {
  const strengthContainers = document.querySelectorAll('[data-strength-check]');
  strengthContainers.forEach(container => {
    const form = container.closest('form');
    const pwInput = form ? form.querySelector('input[name="password"]') : null;
    const confirmInput = form ? form.querySelector('input[name="confirmPassword"], input[name="confirm_password"]') : null;
    const submitBtn = form ? form.querySelector('button[type="submit"]') : null;

    if (!pwInput) return;

    // Wrap input in a relative container for the toggle button
    const wrapper = pwInput.parentElement;
    if (!wrapper.classList.contains('pw-input-wrapper')) {
      wrapper.classList.add('pw-input-wrapper');
      wrapper.style.position = 'relative';
    }

    const confirmWrapper = confirmInput ? confirmInput.parentElement : null;
    if (confirmWrapper && !confirmWrapper.classList.contains('pw-input-wrapper')) {
      confirmWrapper.classList.add('pw-input-wrapper');
      confirmWrapper.style.position = 'relative';
    }

    const confirmContainer = confirmInput
      ? (confirmWrapper ? confirmWrapper.querySelector('.pw-confirm-container') || (() => {
          const el = document.createElement('div');
          el.className = 'pw-confirm-container';
          el.style.marginTop = '6px';
          confirmInput.insertAdjacentElement('afterend', el);
          return el;
        })() : null)
      : null;

    const strengthContainer = container;

    const mgr = new PasswordStrengthManager({
      passwordInput: pwInput,
      confirmInput: confirmInput,
      strengthContainer,
      confirmContainer,
      submitBtn
    });
    mgr.init();
  });
});
