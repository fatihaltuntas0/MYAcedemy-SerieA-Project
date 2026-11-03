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
window.lucideIcon = function(name, extraClass = '', title = '') {
    const cls = extraClass ? `icon ${extraClass}` : 'icon';
    const accessibility = title 
        ? ` role="img" aria-label="${title}" title="${title}"` 
        : ' aria-hidden="true"';
    return `<svg class="${cls}"${accessibility}><use href="#lucide-${name}"></use></svg>`;
};
