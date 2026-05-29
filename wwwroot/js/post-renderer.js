/**
 * Post Renderer — handles syntax highlighting, copy buttons, and lang badges
 * for <pre><code> blocks inside post/comment content.
 */
(function (window) {
  'use strict';

  const COPY_SVG = `<svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="9" y="9" width="13" height="13" rx="2" ry="2"></rect><path d="M5 15H4a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2h9a2 2 0 0 1 2 2v1"></path></svg>`;
  const CHECK_SVG = `<svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><polyline points="20 6 9 17 4 12"></polyline></svg>`;

  let hljsReady = false;

  function loadHljsPacks() {
    if (hljsReady) return Promise.resolve();
    hljsReady = true;

    const packs = [
      'python', 'javascript', 'bash', 'sql', 'php',
      'cpp', 'yaml', 'json', 'xml', 'powershell',
      'java', 'csharp', 'css', 'html'
    ];

    const base = 'https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.9.0/languages/';
    const promises = packs.map(lang =>
      fetch(`${base}min/${lang}.min.js`)
        .then(r => r.ok ? r.text() : '')
        .then(text => {
          if (text) {
            try { eval(text); } catch (_) {}
          }
        })
        .catch(() => {})
    );

    return Promise.all(promises);
  }

  /**
   * Strips HTML tags to get plain text (for preview truncation).
   */
  function stripHtml(html) {
    const tmp = document.createElement('div');
    tmp.innerHTML = html;
    return tmp.textContent || tmp.innerText || '';
  }

  /**
   * Truncates plain text to a max length, appending "…".
   */
  function truncateText(text, maxLength) {
    if (text.length <= maxLength) return text;
    return text.slice(0, maxLength).trimEnd() + '…';
  }

  /**
   * Extracts the language hint from a <pre> class list or <code> class list.
   * Looks for "language-xxx" → returns "xxx", or "lang-xxx" → returns "xxx".
   */
  function extractLang(el) {
    const cls = Array.from(el.classList).join(' ');
    const match = cls.match(/language-(\w+)/) || cls.match(/lang-(\w+)/);
    return match ? match[1] : null;
  }

  /**
   * Injects a language badge (top-left) and a copy button (top-right)
   * into a <pre> block. Replaces the inner <code> with a wrapped version.
   */
  function enhanceCodeBlock(pre) {
    if (pre.dataset.enhanced) return;
    pre.dataset.enhanced = 'true';

    const code = pre.querySelector('code');
    if (!code) return;

    const lang = extractLang(code) || extractLang(pre) || '';
    const langDisplay = lang.toLowerCase();

    pre.style.position = 'relative';

    if (langDisplay) {
      const badge = document.createElement('span');
      badge.className = 'code-lang-badge';
      badge.textContent = langDisplay;
      pre.appendChild(badge);
    }

    const copyBtn = document.createElement('button');
    copyBtn.className = 'code-copy-btn';
    copyBtn.setAttribute('aria-label', 'Sao chép mã');
    copyBtn.innerHTML = `<span class="copy-icon">${COPY_SVG}</span><span class="copy-text">Sao chép</span>`;
    pre.appendChild(copyBtn);

    copyBtn.addEventListener('click', function () {
      const text = code.textContent || '';
      if (navigator.clipboard && navigator.clipboard.writeText) {
        navigator.clipboard.writeText(text).then(() => showCopied(copyBtn));
      } else {
        const ta = document.createElement('textarea');
        ta.value = text;
        ta.style.cssText = 'position:fixed;top:-9999px;left:-9999px;opacity:0';
        document.body.appendChild(ta);
        ta.select();
        try { document.execCommand('copy'); showCopied(copyBtn); } catch (_) {}
        document.body.removeChild(ta);
      }
    });

    hljs.highlightElement(code);
  }

  function showCopied(btn) {
    const icon = btn.querySelector('.copy-icon');
    const text = btn.querySelector('.copy-text');
    const origIcon = icon.innerHTML;
    icon.innerHTML = CHECK_SVG;
    text.textContent = 'Đã sao chép';
    btn.classList.add('copied');
    setTimeout(function () {
      icon.innerHTML = origIcon;
      text.textContent = 'Sao chép';
      btn.classList.remove('copied');
    }, 2000);
  }

  /**
   * Finds all <pre><code> blocks inside a given root element
   * and applies highlighting + UI enhancements.
   */
  function highlightCodeBlocks(root) {
    if (typeof hljs === 'undefined') return;
    root.querySelectorAll('pre code').forEach(function (block) {
      const pre = block.closest('pre');
      if (pre) enhanceCodeBlock(pre);
    });
  }

  /**
   * Renders a post's HTML content: highlights code blocks.
   * Call this after inserting the post body into the DOM.
   *
   * @param {string|Element} content - HTML string or root element
   * @param {Element} [root] - parent element to search for code blocks
   */
  function renderPost(content, root) {
    if (typeof hljs === 'undefined') {
      loadHljsPacks().then(function () {
        if (root) highlightCodeBlocks(root);
      });
    } else {
      if (root) highlightCodeBlocks(root);
    }
  }

  /**
   * Applies renderPost to all code blocks in the document.
   * Call once on DOMContentLoaded for post detail pages.
   */
  function renderAll() {
    if (typeof hljs === 'undefined') {
      loadHljsPacks().then(function () {
        highlightCodeBlocks(document);
      });
    } else {
      highlightCodeBlocks(document);
    }
  }

  window.PostRenderer = {
    renderPost: renderPost,
    renderAll: renderAll,
    stripHtml: stripHtml,
    truncateText: truncateText,
    highlightCodeBlocks: highlightCodeBlocks,
    loadHljsPacks: loadHljsPacks
  };

})(window);
