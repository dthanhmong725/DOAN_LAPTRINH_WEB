// CyberForum - Site Initialization
// Tabler Icons are used throughout the site
// All pages share these common utilities

// Initialize Tabler Icons if needed
document.addEventListener('DOMContentLoaded', () => {
  // AuthManager is loaded from auth.js
  if (typeof AuthManager !== 'undefined') {
    AuthManager.updateUI();
  }
});
