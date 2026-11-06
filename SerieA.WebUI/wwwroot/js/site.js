/**
 * Serie A Dashboard - Global Client Utilities & Icon System
 */

/**
 * Standard Lucide SVG Icon Generator
 * Renders an outline-style Lucide icon via SVG <use> symbol reference.
 * 
 * Sizing tokens:
 * - 'icon--xs' : 14px (compact indicators)
 * - 'icon--sm' : 16px (table actions, form buttons)
 * - 'icon--md' : 20px (navigation, section headers)
 * - 'icon--lg' : 24px (KPI badges, prominent status)
 * - 'icon--xl' : 32px (empty states, banners)
 * 
 * @param {string} name - Lucide symbol name (e.g. 'home', 'trophy', 'pencil', 'trash-2', 'check', 'x')
 * @param {string} [extraClass=''] - Additional CSS classes
 * @param {string} [title=''] - Accessible label/tooltip
 * @returns {string} SVG HTML markup
 */
window.lucideIcon = function (name, extraClass = '', title = '') {
    const cls = extraClass ? `icon ${extraClass}` : 'icon';
    const accessibility = title
        ? ` role="img" aria-label="${title}" title="${title}"`
        : ' aria-hidden="true"';
    return `<svg class="${cls}"${accessibility}><use href="#lucide-${name}"></use></svg>`;
};

// Mobile Responsive Sidebar Toggle
document.addEventListener('DOMContentLoaded', () => {
    const toggleBtn = document.querySelector('[data-sidebar-toggle]');
    const backdrop = document.querySelector('[data-sidebar-close]');

    if (toggleBtn) {
        toggleBtn.addEventListener('click', (e) => {
            e.stopPropagation();
            const isOpen = document.body.classList.toggle('sidebar-open');
            toggleBtn.setAttribute('aria-expanded', isOpen);
        });
    }

    if (backdrop) {
        backdrop.addEventListener('click', () => {
            document.body.classList.remove('sidebar-open');
            if (toggleBtn) toggleBtn.setAttribute('aria-expanded', 'false');
        });
    }

    // Close on Escape key
    document.addEventListener('keydown', (e) => {
        if (e.key === 'Escape' && document.body.classList.contains('sidebar-open')) {
            document.body.classList.remove('sidebar-open');
            if (toggleBtn) toggleBtn.setAttribute('aria-expanded', 'false');
        }
    });
});

// Global 401 Unauthorized Interceptor: Oturum süresi dolduğunda anında Login sayfasına yönlendir
const originalFetch = window.fetch;
window.fetch = async function (...args) {
    const response = await originalFetch.apply(this, args);
    if (response.status === 401) {
        window.location.href = '/Account/Login?returnUrl=' + encodeURIComponent(window.location.pathname);
    }
    return response;
};
