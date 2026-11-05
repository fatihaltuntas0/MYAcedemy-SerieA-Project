/**
 * Serie A standings (public page)
 * Zone filter, team search, row highlight (remembered in the browser),
 * period select, highlight tiles and crest fallback.
 */
(function () {
    'use strict';

    const PIN_KEY = 'serieA:standings:pinned';

    document.addEventListener('DOMContentLoaded', init);

    function init() {
        const root = document.querySelector('[data-standings]');
        if (!root || root.dataset.ready) return;   // If it is loaded twice, do not add duplicate listeners
        root.dataset.ready = '1';

        const rows = Array.from(root.querySelectorAll('.st-row'));
        const zoneLabels = Array.from(root.querySelectorAll('.st-zone'));
        const chips = Array.from(root.querySelectorAll('[data-zone-filter]'));
        const tiles = Array.from(root.querySelectorAll('.st-tile'));
        const ladderMarkers = Array.from(root.querySelectorAll('[data-ladder-team]'));
        const search = root.querySelector('[data-team-search]');
        const counter = root.querySelector('[data-visible-count]');
        const emptyRow = root.querySelector('[data-empty-row]');
        const clearBtn = root.querySelector('[data-clear]');
        const weekSelect = root.querySelector('[data-week-select]');
        const rowByTeam = new Map(rows.map((row) => [row.dataset.team, row]));
        const reduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

        let activeZone = 'all';

        // Simplifies Turkish characters: "Inter", "İnter", "inter" and "ınter" are treated as the same;
        // searches also work without typing "ş, ç, ğ, ö, ü".
        const fold = (value) =>
            (value || '')
                .toLowerCase()
                .normalize('NFD')
                .replace(/[\u0300-\u036f]/g, '')
                .replace(/ı/g, 'i')
                .trim();

        const keys = new Map(rows.map((row) => [row, fold(row.dataset.teamName + ' ' + row.dataset.team)]));

        function apply() {
            const query = fold(search ? search.value : '');
            let visible = 0;

            rows.forEach((row) => {
                const inZone = activeZone === 'all' || row.dataset.zone === activeZone;
                const matches = !query || keys.get(row).includes(query);
                const show = inZone && matches;
                row.hidden = !show;
                if (show) visible++;
            });

            // Zone headings: hidden while searching; with a zone filter only that zone's heading stays
            zoneLabels.forEach((label) => {
                label.hidden = Boolean(query) || (activeZone !== 'all' && label.dataset.zoneLabel !== activeZone);
            });

            if (counter) counter.textContent = String(visible);
            if (emptyRow) emptyRow.hidden = visible !== 0;
        }

        function setZone(zone) {
            activeZone = zone;
            chips.forEach((chip) => chip.setAttribute('aria-pressed', String(chip.dataset.zoneFilter === zone)));
            apply();
        }

        function resetFilters() {
            if (search) search.value = '';
            setZone('all');
        }

        chips.forEach((chip) => chip.addEventListener('click', () => setZone(chip.dataset.zoneFilter)));

        if (search) {
            search.addEventListener('input', apply);
            search.addEventListener('keydown', (e) => {
                if (e.key === 'Escape' && search.value) {
                    search.value = '';
                    apply();
                }
            });
        }

        if (clearBtn) {
            clearBtn.addEventListener('click', () => {
                resetFilters();
                if (search) search.focus();
            });
        }

        // Row highlighting: one team at a time, remembered in the browser
        function setPinned(code) {
            rows.forEach((row) => {
                const on = Boolean(code) && row.dataset.team === code;
                row.classList.toggle('is-pinned', on);
                const btn = row.querySelector('.st-name');
                if (btn) btn.setAttribute('aria-pressed', String(on));
            });
            ladderMarkers.forEach((marker) => {
                const on = Boolean(code) && marker.dataset.ladderTeam === code;
                marker.classList.toggle('is-pinned', on);
                marker.classList.toggle('is-active', on);
                marker.setAttribute('aria-pressed', String(on));
            });
            try {
                if (code) localStorage.setItem(PIN_KEY, code);
                else localStorage.removeItem(PIN_KEY);
            } catch (e) { /* If storage is blocked, carry on quietly */ }
        }

        // Clicking a row, or pressing the team-name button with the keyboard, runs the same action
        rows.forEach((row) => {
            row.addEventListener('click', () => {
                setPinned(row.classList.contains('is-pinned') ? null : row.dataset.team);
            });
        });

        // Clicking a highlight tile highlights that team and brings its table row into view
        tiles.forEach((tile) => {
            tile.addEventListener('click', () => {
                const row = rowByTeam.get(tile.dataset.team);
                if (!row) return;
                setPinned(tile.dataset.team);
                if (row.hidden) resetFilters();
                row.scrollIntoView({ block: 'center', behavior: reduceMotion ? 'auto' : 'smooth' });
            });
        });

        // Clicking a ladder marker highlights that team and scrolls smoothly to it
        ladderMarkers.forEach((marker) => {
            marker.addEventListener('click', () => {
                const team = marker.dataset.ladderTeam;
                const row = rowByTeam.get(team);
                if (!row) return;
                setPinned(marker.classList.contains('is-pinned') ? null : team);
                if (row.hidden) resetFilters();
                row.scrollIntoView({ block: 'center', behavior: reduceMotion ? 'auto' : 'smooth' });
            });
        });

        // Two-way hover synchronization between ladder and table
        function linkHover(team, on) {
            if (!team) return;
            const row = rowByTeam.get(team);
            if (row) row.classList.toggle('is-hover', on);
            ladderMarkers.forEach((m) => {
                if (m.dataset.ladderTeam === team) m.classList.toggle('is-hover', on);
            });
        }

        rows.forEach((row) => {
            row.addEventListener('mouseenter', () => linkHover(row.dataset.team, true));
            row.addEventListener('mouseleave', () => linkHover(row.dataset.team, false));
        });

        ladderMarkers.forEach((marker) => {
            marker.addEventListener('mouseenter', () => linkHover(marker.dataset.ladderTeam, true));
            marker.addEventListener('mouseleave', () => linkHover(marker.dataset.ladderTeam, false));
        });

        try {
            const saved = localStorage.getItem(PIN_KEY);
            if (saved) setPinned(saved);
        } catch (e) { /* ignore */ }

        // Reload the page when the period changes
        if (weekSelect && weekSelect.form) {
            weekSelect.addEventListener('change', () => weekSelect.form.submit());
        }

        // If a logo fails to load, fall back to the team-colored badge
        root.querySelectorAll('img[data-crest]').forEach((img) => {
            const swap = () => {
                img.hidden = true;
                const fallback = img.nextElementSibling;
                if (fallback) fallback.hidden = false;
            };
            if (img.complete && img.naturalWidth === 0) swap();
            else img.addEventListener('error', swap, { once: true });
        });
    }
})();