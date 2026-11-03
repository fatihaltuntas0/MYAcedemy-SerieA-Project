document.addEventListener('DOMContentLoaded', () => {
  const modal = document.getElementById('match-modal');
  const deleteModal = document.getElementById('delete-match-modal');
  const detailModal = document.getElementById('match-detail-modal');
  const form = document.getElementById('match-form');
  const deleteForm = document.getElementById('delete-match-form');
  const alertBox = document.getElementById('form-alert');

  const homeSelect = document.getElementById('m-home');
  const stadiumInput = document.getElementById('m-stadium');
  const statusSelect = document.getElementById('m-status');
  const scoreFields = document.getElementById('score-fields');

  // When the home team is selected, the stadium fills up automatically
  if (homeSelect) {
    homeSelect.addEventListener('change', () => {
      const selectedOption = homeSelect.options[homeSelect.selectedIndex];
      const stadium = selectedOption ? selectedOption.getAttribute('data-stadium') : '';
      if (stadium && stadiumInput) {
        stadiumInput.value = stadium;
      }
    });
  }

  // Show/hide scoreboards based on the game status
  function toggleScoreFields() {
    if (!statusSelect || !scoreFields) return;
    const val = statusSelect.value;
    if (val === 'Live' || val === 'Finished' || val === '1' || val === '2') {
      scoreFields.style.display = 'flex';
    } else {
      scoreFields.style.display = 'none';
      const hs = document.getElementById('m-hs');
      const as = document.getElementById('m-as');
      if (hs) hs.value = '';
      if (as) as.value = '';
    }
  }

  if (statusSelect) {
    statusSelect.addEventListener('change', toggleScoreFields);
  }

  // Modal Open (Add New Match)
  document.querySelectorAll('[data-open-create]').forEach(b => b.addEventListener('click', () => {
    if (!form || !modal) return;
    form.reset();
    const idField = document.getElementById('m-id');
    if (idField) idField.value = '';
    const title = document.getElementById('match-modal-title');
    if (title) title.textContent = 'Yeni Maç Ekle';
    if (alertBox) alertBox.style.display = 'none';

    // Default date for today
    const today = new Date().toISOString().split('T')[0];
    const dateField = document.getElementById('m-date');
    const timeField = document.getElementById('m-time');
    if (dateField) dateField.value = today;
    if (timeField) timeField.value = '20:45';

    toggleScoreFields();
    modal.showModal();
  }));

  // Modal Close - Closes the specific dialog the button belongs to
  document.querySelectorAll('[data-modal-close]').forEach(b => b.addEventListener('click', () => {
    const dialog = b.closest('dialog');
    if (dialog) {
      dialog.close();
    } else {
      if (modal && modal.open) modal.close();
      if (deleteModal && deleteModal.open) deleteModal.close();
      if (detailModal && detailModal.open) detailModal.close();
    }
  }));

  // When You Click the “Edit” Button
  document.querySelectorAll('[data-edit]').forEach(btn => {
    btn.addEventListener('click', () => {
      const matchRaw = btn.getAttribute('data-match');
      if (!matchRaw) return;

      const match = JSON.parse(matchRaw);

      document.getElementById('m-id').value = match.id || '';
      document.getElementById('m-week').value = match.week || 1;

      // Map enum numeric or text values to select element options
      let statusVal = match.status;
      if (statusVal === 0 || statusVal === 'NotStarted') statusVal = 'NotStarted';
      else if (statusVal === 1 || statusVal === 'Live') statusVal = 'Live';
      else if (statusVal === 2 || statusVal === 'Finished') statusVal = 'Finished';
      else if (statusVal === 3 || statusVal === 'Postponed') statusVal = 'Postponed';

      document.getElementById('m-status').value = statusVal;
      document.getElementById('m-home').value = match.homeTeamId || '';
      document.getElementById('m-away').value = match.awayTeamId || '';
      document.getElementById('m-stadium').value = match.stadium || '';

      if (match.matchDate) {
        const dt = new Date(match.matchDate);
        const yyyy = dt.getFullYear();
        const mm = String(dt.getMonth() + 1).padStart(2, '0');
        const dd = String(dt.getDate()).padStart(2, '0');
        const hh = String(dt.getHours()).padStart(2, '0');
        const min = String(dt.getMinutes()).padStart(2, '0');

        document.getElementById('m-date').value = `${yyyy}-${mm}-${dd}`;
        document.getElementById('m-time').value = `${hh}:${min}`;
      }

      document.getElementById('m-hs').value = (match.homeScore !== null && match.homeScore !== undefined) ? match.homeScore : '';
      document.getElementById('m-as').value = (match.awayScore !== null && match.awayScore !== undefined) ? match.awayScore : '';

      document.getElementById('match-modal-title').textContent = 'Maçı Düzenle';
      if (alertBox) alertBox.style.display = 'none';

      toggleScoreFields();
      if (modal) modal.showModal();
    });
  });

  // When You Click the “Delete” Button
  document.querySelectorAll('[data-delete]').forEach(btn => {
    btn.addEventListener('click', () => {
      const id = btn.getAttribute('data-delete');
      const name = btn.getAttribute('data-name');

      document.getElementById('delete-match-id').value = id;
      document.getElementById('delete-match-text').textContent = `"${name}" karşılaşmasını silmek istediğinize emin misiniz?`;
      if (deleteModal) deleteModal.showModal();
    });
  });

  // Save Form (Add / Update)
  if (form) {
    form.addEventListener('submit', async (e) => {
      e.preventDefault();

      const id = document.getElementById('m-id').value;
      const isEdit = !!id;

      const dateVal = document.getElementById('m-date').value;
      const timeVal = document.getElementById('m-time').value || '00:00';
      const isoDateTime = dateVal ? `${dateVal}T${timeVal}:00.000Z` : new Date().toISOString();

      const hsVal = document.getElementById('m-hs').value;
      const asVal = document.getElementById('m-as').value;

      const payload = {
        week: parseInt(document.getElementById('m-week').value),
        status: document.getElementById('m-status').value,
        homeTeamId: document.getElementById('m-home').value,
        awayTeamId: document.getElementById('m-away').value,
        stadium: document.getElementById('m-stadium').value,
        matchDate: isoDateTime,
        homeScore: hsVal !== '' ? parseInt(hsVal) : null,
        awayScore: asVal !== '' ? parseInt(asVal) : null
      };

      const url = isEdit ? `/Matches/Update?id=${id}` : '/Matches/Create';

      try {
        const res = await fetch(url, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(payload)
        });

        const result = await res.json();
        if (result.isSuccess) {
          window.location.reload();
        } else {
          if (alertBox) {
            alertBox.textContent = (result.message || 'Hata oluştu.') + (result.errors ? ' ' + result.errors.join(', ') : '');
            alertBox.style.display = 'block';
          }
        }
      } catch (err) {
        console.error('Save match error:', err);
        if (alertBox) {
          alertBox.textContent = 'Sunucuyla iletişim kurulurken bir hata oluştu.';
          alertBox.style.display = 'block';
        }
      }
    });
  }

  // Confirm Deletion
  if (deleteForm) {
    deleteForm.addEventListener('submit', async (e) => {
      e.preventDefault();
      const id = document.getElementById('delete-match-id').value;
      try {
        const res = await fetch(`/Matches/Delete?id=${id}`, { method: 'POST' });
        const result = await res.json();
        if (result.isSuccess) {
          window.location.reload();
        } else {
          alert(result.message || 'Silme işlemi başarısız oldu.');
        }
      } catch (err) {
        console.error('Delete match error:', err);
        alert('Sunucuyla iletişim kurulurken bir sorun oluştu.');
      }
    });
  }

  // --- Match Details Modal Handler ---
  document.querySelectorAll('[data-detail]').forEach(btn => {
    btn.addEventListener('click', async () => {
      const matchId = btn.getAttribute('data-detail');
      if (!matchId || !detailModal) return;

      try {
        const res = await fetch(`/Matches/GetDetails?id=${matchId}`);
        const result = await res.json();
        if (!result.isSuccess || !result.data) {
          alert(result.message || 'Maç detayı alınamadı.');
          return;
        }

        const m = result.data;

        // Fill header info
        document.getElementById('detail-week').textContent = `${m.week}. Hafta`;
        const dt = new Date(m.matchDate);
        document.getElementById('detail-date').textContent = dt.toLocaleDateString('tr-TR', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' });
        document.getElementById('detail-stadium').textContent = m.stadium || 'Stadyum';

        document.getElementById('detail-home-name').textContent = m.homeTeamName;
        document.getElementById('detail-away-name').textContent = m.awayTeamName;

        const homeLogo = document.getElementById('detail-home-logo');
        if (m.homeTeamLogoUrl) {
          homeLogo.src = m.homeTeamLogoUrl;
          homeLogo.style.display = 'inline-block';
        } else {
          homeLogo.style.display = 'none';
        }

        const awayLogo = document.getElementById('detail-away-logo');
        if (m.awayTeamLogoUrl) {
          awayLogo.src = m.awayTeamLogoUrl;
          awayLogo.style.display = 'inline-block';
        } else {
          awayLogo.style.display = 'none';
        }

        // Score & Status
        if (m.homeScore !== null && m.awayScore !== null) {
          document.getElementById('detail-score').textContent = `${m.homeScore} - ${m.awayScore}`;
        } else {
          document.getElementById('detail-score').textContent = `- : -`;
        }

        const statusBadge = document.getElementById('detail-status-badge');
        if (m.status === 1 || m.status === 'Live') {
          statusBadge.className = 'badge-a badge-a--danger';
          statusBadge.textContent = 'Canlı';
        } else if (m.status === 2 || m.status === 'Finished') {
          statusBadge.className = 'badge-a badge-a--success';
          statusBadge.textContent = 'Tamamlandı';
        } else {
          statusBadge.className = 'badge-a';
          statusBadge.textContent = 'Oynanmadı';
        }

        // Render Timeline Events
        renderMatchEvents(m);

        const fullPageBtn = document.getElementById('detail-full-page-btn');
        if (fullPageBtn) {
          fullPageBtn.href = `/Matches/Detail/${matchId}`;
        }

        detailModal.showModal();

      } catch (err) {
        console.error('Failed to load match detail:', err);
        alert('Maç detayları yüklenirken bir sorun oluştu.');
      }
    });
  });

  // Timeline Event Renderer
  function renderMatchEvents(match) {
    const list  = document.getElementById('detail-events-list');
    const empty = document.getElementById('detail-no-events');
    if (!list) return;
    list.replaceChildren();

    // "45+2" -> 4502, "23" -> 2300 (sıralama anahtarı)
    const minuteKey = (m) => {
      const [base, extra] = String(m).split('+').map(Number);
      return (base || 0) * 100 + (extra || 0);
    };
    const isRed = (c) => c === 2 || String(c).toLowerCase().includes('red');

    // Tüm olayları tek listede topla (aynı dakikada önce gol, sonra kart, sonra değişiklik)
    const events = [
      ...(match.goals || []).map(g => ({
        type: 'goal', teamId: g.teamId, player: g.playerName, minute: String(g.minute)
      })),
      ...(match.cards || []).map(c => ({
        type: isRed(c.cardType) ? 'red' : 'yellow',
        teamId: c.teamId, player: c.playerName, minute: String(c.minute)
      })),
      ...(match.substitutions || []).map(s => ({
        type: 'sub', teamId: s.teamId, playerIn: s.playerIn, playerOut: s.playerOut, minute: String(s.minute)
      }))
    ]
      .map((e, i) => ({ ...e, order: i, key: minuteKey(e.minute) }))
      .sort((a, b) => a.key - b.key || a.order - b.order);

    if (events.length === 0) {
      list.style.display = 'none';
      if (empty) empty.style.display = 'block';
      return;
    }
    list.style.display = '';
    if (empty) empty.style.display = 'none';

    // küçük yardımcı: element oluştur (textContent kullanır, XSS güvenli)
    const el = (tag, cls, text) => {
      const n = document.createElement(tag);
      if (cls) n.className = cls;
      if (text !== undefined) n.textContent = text;
      return n;
    };

    const buildIcon = (e) => {
      if (e.type === 'goal') {
        const s = el('span', 'tl-icon');
        s.innerHTML = window.lucideIcon ? window.lucideIcon('trophy', 'icon--xs') : '<svg class="icon icon--xs"><use href="#lucide-trophy"/></svg>';
        return s;
      }
      if (e.type === 'sub') {
        const s = el('span', 'tl-icon');
        s.innerHTML = window.lucideIcon ? window.lucideIcon('arrow-left-right', 'icon--xs') : '<svg class="icon icon--xs"><use href="#lucide-arrow-left-right"/></svg>';
        return s;
      }
      return el('span', 'tl-card tl-card--' + (e.type === 'red' ? 'red' : 'yellow'));
    };

    const buildText = (e) => {
      const box = el('div', 'tl-text');
      if (e.type === 'sub') {
        box.append(el('span', 'tl-sub-in',  '↑ ' + e.playerIn));
        box.append(el('span', 'tl-sub-out', '↓ ' + e.playerOut));
      } else {
        box.append(el('span', 'tl-player', e.player));
      }
      return box;
    };

    let halfTimeAdded = false;

    events.forEach((e, idx) => {
      // İlk ikinci-yarı olayından önce (ve öncesinde olay varsa) devre arası ayracı
      if (!halfTimeAdded && e.key >= 4600) {
        halfTimeAdded = true;
        if (idx > 0) {
          const ht = el('div', 'tl-row');
          ht.append(el('div', 'tl-ht', 'Devre Arası'));
          ht.firstChild.style.gridColumn = '1 / -1';
          list.append(ht);
        }
      }

      const isHome = e.teamId === match.homeTeamId;
      const row  = el('div', 'tl-row');
      const side = el('div', 'tl-side ' + (isHome ? 'tl-side--home' : 'tl-side--away'));

      // Ev sahibi: yazı + ikon (ikon çizgiye yakın), deplasman: ikon + yazı
      if (isHome) side.append(buildText(e), buildIcon(e));
      else        side.append(buildIcon(e), buildText(e));

      row.append(side, el('div', 'tl-min', e.minute + "'"));
      list.append(row);
    });
  }
});

