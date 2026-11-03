/* ==========================================================================
   Serie A Match Detail & Event Management - Interactive Logic
   With Dynamic Player Roster & Position Grouping from team-rosters.json
   And Strict Domain Rules (Substituted players, Red cards, Minute order)
   ========================================================================== */

document.addEventListener('DOMContentLoaded', () => {
    const eventModal = document.getElementById('event-modal');
    const deleteModal = document.getElementById('delete-event-modal');
    const eventForm = document.getElementById('event-form');
    const deleteForm = document.getElementById('delete-event-form');
    const formAlert = document.getElementById('event-form-alert');
    const submitBtn = document.getElementById('ev-submit-btn');

    const typeSelect = document.getElementById('ev-type');
    const teamSelect = document.getElementById('ev-team');

    // Player inputs & dropdowns
    const playerSelect = document.getElementById('ev-player-select');
    const playerManualWrap = document.getElementById('ev-player-manual-wrap');
    const playerInput = document.getElementById('ev-player');
    const playerLabel = document.getElementById('ev-player-label');

    // Substitution Player Out inputs & dropdowns
    const playerOutWrap = document.getElementById('ev-field-player-out');
    const playerOutSelect = document.getElementById('ev-player-out-select');
    const playerOutManualWrap = document.getElementById('ev-player-out-manual-wrap');
    const playerOutInput = document.getElementById('ev-player-out');

    const cardWrap = document.getElementById('ev-field-card');
    const cardSelect = document.getElementById('ev-card-type');
    const minuteInput = document.getElementById('ev-minute');

    const matchId = document.getElementById('match-id-holder')?.value;

    // Existing match events context for real-time validation
    const container = document.getElementById('match-detail-container');
    const subsOut = JSON.parse(container?.getAttribute('data-subs-out') || '[]');
    const subsIn = JSON.parse(container?.getAttribute('data-subs-in') || '[]');
    const redCards = JSON.parse(container?.getAttribute('data-red-cards') || '[]');

    function parseMin(m) {
        if (!m) return 0;
        const [base, extra] = String(m).split('+').map(Number);
        return (base || 0) * 100 + (extra || 0);
    }

    // Position mapping & order
    const positionLabels = {
        'Goalkeeper': 'Kaleciler',
        'Defender': 'Defans',
        'Midfielder': 'Orta Saha',
        'Attacker': 'Forvetler',
        'Unknown': 'Diğer'
    };
    const positionOrder = ['Goalkeeper', 'Defender', 'Midfielder', 'Attacker', 'Unknown'];

    // Teams & Players Cache
    let teamsData = null;

    async function loadTeamsData() {
        if (teamsData) return teamsData;
        try {
            const res = await fetch('/data/team-rosters.json');
            if (res.ok) {
                const data = await res.json();
                teamsData = data.teams || data;
                return teamsData;
            }
        } catch (e) {
            console.warn('Could not load team-rosters.json', e);
        }
        return null;
    }

    // Preload teams dataset on load
    loadTeamsData();

    // Toast Notification Helper
    function showToast(message, type = 'success') {
        const toastContainer = document.querySelector('[data-toasts]');
        if (!toastContainer) return;

        const toastEl = document.createElement('div');
        toastEl.className = `toast-a toast-a--${type}`;
        toastEl.setAttribute('role', 'status');

        const iconName = type === 'success' ? 'check' : 'info';
        toastEl.innerHTML = `
            <svg class="icon" aria-hidden="true"><use href="#lucide-${iconName}"/></svg>
            <span>${message}</span>
        `;
        toastContainer.appendChild(toastEl);

        setTimeout(() => {
            toastEl.style.opacity = '0';
            toastEl.style.transform = 'translateY(8px)';
            setTimeout(() => toastEl.remove(), 300);
        }, 3500);
    }

    // Modal wireup (Backdrop click closes modal)
    [eventModal, deleteModal].forEach(dialog => {
        if (!dialog) return;
        dialog.addEventListener('click', (e) => {
            if (e.target === dialog) dialog.close();
        });
    });

    document.querySelectorAll('[data-modal-close]').forEach(btn => {
        btn.addEventListener('click', () => {
            eventModal?.close();
            deleteModal?.close();
        });
    });

    // Populate a <select> element with players grouped by position & match state
    function populatePlayerDropdown(selectEl, teamCode, placeholder = 'Oyuncu seçiniz...') {
        if (!selectEl) return;
        selectEl.innerHTML = '';

        if (!teamCode || !teamsData || !teamsData[teamCode]) {
            const opt = document.createElement('option');
            opt.value = '';
            opt.textContent = teamCode ? 'Bu takım için oyuncu kadrosu bulunamadı' : 'Lütfen önce takım seçiniz...';
            selectEl.appendChild(opt);

            const customOpt = document.createElement('option');
            customOpt.value = '__custom__';
            customOpt.textContent = 'Diğer (Manuel İsim Gir)...';
            selectEl.appendChild(customOpt);
            return;
        }

        const team = teamsData[teamCode];
        const players = team.players || [];

        // Default placeholder
        const defOpt = document.createElement('option');
        defOpt.value = '';
        defOpt.textContent = placeholder;
        selectEl.appendChild(defOpt);

        // Group players by position
        const grouped = {};
        players.forEach(p => {
            const pos = p.position || 'Unknown';
            if (!grouped[pos]) grouped[pos] = [];
            grouped[pos].push(p);
        });

        positionOrder.forEach(pos => {
            if (grouped[pos] && grouped[pos].length > 0) {
                const optgroup = document.createElement('optgroup');
                optgroup.label = positionLabels[pos] || pos;

                // Sort alphabetically by player name
                grouped[pos].sort((a, b) => a.name.localeCompare(b.name, 'tr'));

                grouped[pos].forEach(p => {
                    const opt = document.createElement('option');
                    opt.value = p.name;

                    let statusBadge = '';
                    const rc = redCards.find(c => c.player.toLowerCase() === p.name.toLowerCase());
                    const so = subsOut.find(s => s.player.toLowerCase() === p.name.toLowerCase());
                    const si = subsIn.find(s => s.player.toLowerCase() === p.name.toLowerCase());

                    if (rc) statusBadge = ` [🟥 Kırmızı - ${rc.minute}']`;
                    else if (so) statusBadge = ` [↓ Çıktı - ${so.minute}']`;
                    else if (si) statusBadge = ` [↑ Girdi - ${si.minute}']`;

                    const num = p.shirtNumber ? `#${p.shirtNumber} ` : '';
                    opt.textContent = `${num}${p.name}${statusBadge}`;
                    optgroup.appendChild(opt);
                });

                selectEl.appendChild(optgroup);
            }
        });

        // Always provide a fallback for manual entry
        const customOpt = document.createElement('option');
        customOpt.value = '__custom__';
        customOpt.textContent = 'Diğer (Manuel İsim Gir)...';
        selectEl.appendChild(customOpt);
    }

    // Refresh player dropdowns when team selection changes
    async function handleTeamChange() {
        await loadTeamsData();
        const selectedOption = teamSelect.options[teamSelect.selectedIndex];
        const teamCode = selectedOption?.getAttribute('data-code');

        populatePlayerDropdown(playerSelect, teamCode, 'Oyuncu seçiniz...');
        populatePlayerDropdown(playerOutSelect, teamCode, 'Çıkan oyuncuyu seçiniz...');

        // Reset manual input views
        if (playerManualWrap) playerManualWrap.style.display = 'none';
        if (playerInput) playerInput.value = '';

        if (playerOutManualWrap) playerOutManualWrap.style.display = 'none';
        if (playerOutInput) playerOutInput.value = '';
    }

    if (teamSelect) {
        teamSelect.addEventListener('change', handleTeamChange);
    }

    // Handle Manual Input Toggle when "__custom__" is selected
    if (playerSelect) {
        playerSelect.addEventListener('change', () => {
            if (playerSelect.value === '__custom__') {
                if (playerManualWrap) {
                    playerManualWrap.style.display = 'block';
                    playerInput?.focus();
                }
            } else {
                if (playerManualWrap) playerManualWrap.style.display = 'none';
                if (playerInput) playerInput.value = playerSelect.value;
            }
        });
    }

    if (playerOutSelect) {
        playerOutSelect.addEventListener('change', () => {
            if (playerOutSelect.value === '__custom__') {
                if (playerOutManualWrap) {
                    playerOutManualWrap.style.display = 'block';
                    playerOutInput?.focus();
                }
            } else {
                if (playerOutManualWrap) playerOutManualWrap.style.display = 'none';
                if (playerOutInput) playerOutInput.value = playerOutSelect.value;
            }
        });
    }

    // Open Event Modal
    document.querySelectorAll('[data-open-event-modal]').forEach(btn => {
        btn.addEventListener('click', async () => {
            if (!eventModal || !eventForm) return;
            eventForm.reset();
            clearValidationErrors();
            syncFormFields();

            if (playerManualWrap) playerManualWrap.style.display = 'none';
            if (playerOutManualWrap) playerOutManualWrap.style.display = 'none';

            await loadTeamsData();
            handleTeamChange();

            eventModal.showModal();
            teamSelect?.focus();
        });
    });

    // Dynamic Form Field Sync based on Event Type
    function syncFormFields() {
        if (!typeSelect) return;
        const type = typeSelect.value;

        if (type === 'Goal') {
            if (playerLabel) playerLabel.innerHTML = 'Golü Atan Oyuncu <span class="req">*</span>';
            if (playerOutWrap) playerOutWrap.style.display = 'none';
            if (cardWrap) cardWrap.style.display = 'none';
        } else if (type === 'Card') {
            if (playerLabel) playerLabel.innerHTML = 'Kart Gören Oyuncu <span class="req">*</span>';
            if (playerOutWrap) playerOutWrap.style.display = 'none';
            if (cardWrap) cardWrap.style.display = 'block';
        } else if (type === 'Substitution') {
            if (playerLabel) playerLabel.innerHTML = 'Oyuna Giren Oyuncu <span class="req">*</span>';
            if (playerOutWrap) playerOutWrap.style.display = 'block';
            if (cardWrap) cardWrap.style.display = 'none';
        }
    }

    if (typeSelect) {
        typeSelect.addEventListener('change', syncFormFields);
    }

    function clearValidationErrors() {
        if (formAlert) {
            formAlert.textContent = '';
            formAlert.style.display = 'none';
        }
        document.querySelectorAll('.form-field.has-error', eventForm).forEach(el => {
            el.classList.remove('has-error');
            const errEl = el.querySelector('.field-error');
            if (errEl) errEl.textContent = '';
        });
    }

    function setFieldError(fieldElement, message) {
        if (!fieldElement) return;
        fieldElement.classList.add('has-error');
        const errEl = fieldElement.querySelector('.field-error');
        if (errEl) errEl.textContent = message;
    }

    // Submit Event Form (Add Goal / Card / Substitution)
    if (eventForm) {
        eventForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            clearValidationErrors();

            const type = typeSelect.value;
            const teamId = teamSelect.value;

            // Determine Player Name
            let player = '';
            if (playerSelect.value === '__custom__') {
                player = playerInput.value.trim();
            } else {
                player = playerSelect.value.trim();
            }

            // Determine Player Out Name
            let playerOut = '';
            if (type === 'Substitution') {
                if (playerOutSelect.value === '__custom__') {
                    playerOut = playerOutInput.value.trim();
                } else {
                    playerOut = playerOutSelect.value.trim();
                }
            }

            const minute = minuteInput.value.trim();
            const cardType = cardSelect.value;

            let hasErrors = false;

            if (!teamId) {
                setFieldError(teamSelect.closest('.form-field'), 'Lütfen takımı seçin.');
                hasErrors = true;
            }

            if (!player) {
                const label = type === 'Substitution' ? 'Oyuna giren oyuncu' : 'Oyuncu';
                setFieldError(playerSelect.closest('.form-field'), `${label} seçiniz veya manuel isim giriniz.`);
                hasErrors = true;
            }

            if (type === 'Substitution') {
                if (!playerOut) {
                    setFieldError(playerOutSelect.closest('.form-field'), 'Oyundan çıkan oyuncuyu seçiniz.');
                    hasErrors = true;
                } else if (player.toLowerCase() === playerOut.toLowerCase()) {
                    setFieldError(playerOutSelect.closest('.form-field'), 'Giren ve çıkan oyuncu aynı olamaz.');
                    hasErrors = true;
                }
            }

            // Minute Regex Validation: 1 to 120 or stoppage format (e.g. 45, 90+3)
            const minRegex = /^\d{1,3}(\+\d{1,2})?$/;
            if (!minute) {
                setFieldError(minuteInput.closest('.form-field'), 'Dakika bilgisi zorunludur.');
                hasErrors = true;
            } else if (!minRegex.test(minute)) {
                setFieldError(minuteInput.closest('.form-field'), 'Geçerli bir dakika giriniz (örn: 45, 90+3).');
                hasErrors = true;
            } else {
                const baseMin = parseInt(minute.split('+')[0], 10);
                if (baseMin > 120) {
                    setFieldError(minuteInput.closest('.form-field'), 'Dakika 120\'den büyük olamaz.');
                    hasErrors = true;
                }
            }

            // Client-side Domain Rules Pre-check
            if (!hasErrors) {
                const currentMin = parseMin(minute);

                if (type === 'Goal') {
                    const rc = redCards.find(c => c.player.toLowerCase() === player.toLowerCase());
                    if (rc && currentMin >= parseMin(rc.minute)) {
                        setFieldError(playerSelect.closest('.form-field'), `"${player}" ${rc.minute}. dakikada kırmızı kart gördüğü için bu dakikada (${minute}') gol atamaz.`);
                        hasErrors = true;
                    }

                    const so = subsOut.find(s => s.player.toLowerCase() === player.toLowerCase());
                    if (so && currentMin >= parseMin(so.minute)) {
                        setFieldError(playerSelect.closest('.form-field'), `"${player}" ${so.minute}. dakikada oyundan çıktığı için bu dakikada (${minute}') gol atamaz.`);
                        hasErrors = true;
                    }

                    const si = subsIn.find(s => s.player.toLowerCase() === player.toLowerCase());
                    if (si && currentMin < parseMin(si.minute)) {
                        setFieldError(playerSelect.closest('.form-field'), `"${player}" henüz oyuna girmediği için (${minute}') gol atamaz. (Oyuna giriş: ${si.minute}').`);
                        hasErrors = true;
                    }
                } else if (type === 'Substitution') {
                    const so = subsOut.find(s => s.player.toLowerCase() === playerOut.toLowerCase());
                    if (so) {
                        setFieldError(playerOutSelect.closest('.form-field'), `"${playerOut}" daha önce ${so.minute}. dakikada zaten oyundan çıkmıştır.`);
                        hasErrors = true;
                    }

                    const si = subsIn.find(s => s.player.toLowerCase() === player.toLowerCase());
                    if (si) {
                        setFieldError(playerSelect.closest('.form-field'), `"${player}" daha önce ${si.minute}. dakikada zaten oyuna girmiştir.`);
                        hasErrors = true;
                    }

                    const rcOut = redCards.find(c => c.player.toLowerCase() === playerOut.toLowerCase());
                    if (rcOut && currentMin >= parseMin(rcOut.minute)) {
                        setFieldError(playerOutSelect.closest('.form-field'), `"${playerOut}" ${rcOut.minute}. dakikada kırmızı kart gördüğü için oyundan değiştirilemez.`);
                        hasErrors = true;
                    }
                } else if (type === 'Card') {
                    const rc = redCards.find(c => c.player.toLowerCase() === player.toLowerCase());
                    if (rc) {
                        setFieldError(playerSelect.closest('.form-field'), `"${player}" daha önce ${rc.minute}. dakikada zaten kırmızı kart görmüştür.`);
                        hasErrors = true;
                    }

                    const so = subsOut.find(s => s.player.toLowerCase() === player.toLowerCase());
                    if (so && currentMin > parseMin(so.minute)) {
                        setFieldError(playerSelect.closest('.form-field'), `"${player}" ${so.minute}. dakikada oyundan çıktığı için bu dakikada (${minute}') sahada kart göremez.`);
                        hasErrors = true;
                    }
                }
            }

            if (hasErrors) {
                if (formAlert) {
                    formAlert.textContent = 'Lütfen işaretli alanları kontrol ediniz.';
                    formAlert.style.display = 'block';
                }
                return;
            }

            // Prepare Payload & URL
            let endpoint = '';
            let payload = {};

            if (type === 'Goal') {
                endpoint = `/Matches/AddGoal?matchId=${matchId}`;
                payload = { teamId, playerName: player, minute };
            } else if (type === 'Card') {
                endpoint = `/Matches/AddCard?matchId=${matchId}`;
                payload = { teamId, playerName: player, minute, cardType };
            } else if (type === 'Substitution') {
                endpoint = `/Matches/AddSubstitution?matchId=${matchId}`;
                payload = { teamId, playerIn: player, playerOut, minute };
            }

            try {
                if (submitBtn) submitBtn.classList.add('is-loading');

                const response = await fetch(endpoint, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(payload)
                });

                const result = await response.json();

                if (result.isSuccess) {
                    eventModal.close();
                    showToast('Olay kaydı başarıyla eklendi.', 'success');
                    setTimeout(() => window.location.reload(), 400);
                } else {
                    if (formAlert) {
                        const errDetails = result.errors && result.errors.length > 0 ? ` (${result.errors.join(', ')})` : '';
                        formAlert.textContent = (result.message || 'Bir hata oluştu.') + errDetails;
                        formAlert.style.display = 'block';
                    }
                }
            } catch (err) {
                if (formAlert) {
                    formAlert.textContent = 'Sunucuyla bağlantı kurulurken hata oluştu.';
                    formAlert.style.display = 'block';
                }
            } finally {
                if (submitBtn) submitBtn.classList.remove('is-loading');
            }
        });
    }

    // Delete Event Logic
    let eventToDelete = null;

    document.querySelectorAll('[data-delete-event]').forEach(btn => {
        btn.addEventListener('click', () => {
            const id = btn.getAttribute('data-id');
            const type = btn.getAttribute('data-type');
            const label = btn.getAttribute('data-label') || 'bu olayı';

            eventToDelete = { id, type };

            const textEl = document.getElementById('delete-event-text');
            if (textEl) {
                textEl.textContent = `"${label}" kaydını silmek istediğinize emin misiniz? Bu işlem geri alınamaz.`;
            }

            deleteModal?.showModal();
        });
    });

    if (deleteForm) {
        deleteForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            if (!eventToDelete) return;

            let endpoint = '';
            if (eventToDelete.type === 'goal') {
                endpoint = `/Matches/DeleteGoal?goalId=${eventToDelete.id}`;
            } else if (eventToDelete.type === 'card') {
                endpoint = `/Matches/DeleteCard?cardId=${eventToDelete.id}`;
            } else if (eventToDelete.type === 'sub') {
                endpoint = `/Matches/DeleteSubstitution?subId=${eventToDelete.id}`;
            }

            const delSubmitBtn = document.getElementById('delete-submit-btn');

            try {
                if (delSubmitBtn) delSubmitBtn.classList.add('is-loading');

                const response = await fetch(endpoint, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' }
                });

                const result = await response.json();

                if (result.isSuccess) {
                    deleteModal.close();
                    showToast('Olay kaydı silindi.', 'success');
                    setTimeout(() => window.location.reload(), 400);
                } else {
                    alert(result.message || 'Silme işlemi başarısız oldu.');
                }
            } catch (err) {
                alert('Silme işlemi sırasında sunucu hatası oluştu.');
            } finally {
                if (delSubmitBtn) delSubmitBtn.classList.remove('is-loading');
            }
        });
    }
});
