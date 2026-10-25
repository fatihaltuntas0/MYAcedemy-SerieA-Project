document.addEventListener('DOMContentLoaded', () => {
  const modal = document.getElementById('match-modal');
  const deleteModal = document.getElementById('delete-match-modal');
  const form = document.getElementById('match-form');
  const deleteForm = document.getElementById('delete-match-form');
  const alertBox = document.getElementById('form-alert');

  const homeSelect = document.getElementById('m-home');
  const stadiumInput = document.getElementById('m-stadium');
  const statusSelect = document.getElementById('m-status');
  const scoreFields = document.getElementById('score-fields');

  // Ev Sahibi seçilince stadyumu otomatik doldur
  homeSelect.addEventListener('change', () => {
    const selectedOption = homeSelect.options[homeSelect.selectedIndex];
    const stadium = selectedOption.getAttribute('data-stadium');
    if (stadium) {
      stadiumInput.value = stadium;
    }
  });

  // Maç Durumuna göre skor alanlarını göster/gizle
  function toggleScoreFields() {
    const val = statusSelect.value;
    if (val === 'Canlı' || val === 'Tamamlandı') {
      scoreFields.style.display = 'flex';
    } else {
      scoreFields.style.display = 'none';
      document.getElementById('m-hs').value = '';
      document.getElementById('m-as').value = '';
    }
  }
  statusSelect.addEventListener('change', toggleScoreFields);

  // Modal Aç (Yeni Maç Ekle)
  document.querySelectorAll('[data-open-create]').forEach(b => b.addEventListener('click', () => {
    form.reset();
    document.getElementById('m-id').value = '';
    document.getElementById('match-modal-title').textContent = 'Yeni Maç Ekle';
    alertBox.style.display = 'none';

    // Varsayılan bugünün tarihi
    const today = new Date().toISOString().split('T')[0];
    document.getElementById('m-date').value = today;
    document.getElementById('m-time').value = '20:45';

    toggleScoreFields();
    modal.showModal();
  }));

  // Modal Kapat
  document.querySelectorAll('[data-modal-close]').forEach(b => b.addEventListener('click', () => {
    modal.close();
    deleteModal.close();
  }));

  // Düzenle Butonuna Tıklanınca
  document.querySelectorAll('[data-edit]').forEach(btn => {
    btn.addEventListener('click', () => {
      const match = JSON.parse(btn.getAttribute('data-match'));

      document.getElementById('m-id').value = match.id;
      document.getElementById('m-week').value = match.week;
      document.getElementById('m-status').value = match.status;
      document.getElementById('m-home').value = match.homeTeamId;
      document.getElementById('m-away').value = match.awayTeamId;
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

      document.getElementById('m-hs').value = match.homeScore !== null ? match.homeScore : '';
      document.getElementById('m-as').value = match.awayScore !== null ? match.awayScore : '';

      document.getElementById('match-modal-title').textContent = 'Maçı Düzenle';
      alertBox.style.display = 'none';

      toggleScoreFields();
      modal.showModal();
    });
  });

  // Sil Butonuna Tıklanınca
  document.querySelectorAll('[data-delete]').forEach(btn => {
    btn.addEventListener('click', () => {
      const id = btn.getAttribute('data-delete');
      const name = btn.getAttribute('data-name');

      document.getElementById('delete-match-id').value = id;
      document.getElementById('delete-match-text').textContent = `"${name}" karşılaşmasını silmek istediğinize emin misiniz?`;
      deleteModal.showModal();
    });
  });

  // Form Kaydet (Ekle / Güncelle)
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

    const res = await fetch(url, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload)
    });

    const result = await res.json();
    if (result.isSuccess) {
      window.location.reload();
    } else {
      alertBox.textContent = result.message + (result.errors ? ' ' + result.errors.join(', ') : '');
      alertBox.style.display = 'block';
    }
  });

  // Silmeyi Onayla
  deleteForm.addEventListener('submit', async (e) => {
    e.preventDefault();
    const id = document.getElementById('delete-match-id').value;
    const res = await fetch(`/Matches/Delete?id=${id}`, { method: 'POST' });
    const result = await res.json();
    if (result.isSuccess) {
      window.location.reload();
    } else {
      alert(result.message);
    }
  });
});
