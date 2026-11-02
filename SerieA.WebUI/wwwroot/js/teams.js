document.addEventListener('DOMContentLoaded', () => {
  const modal = document.getElementById('team-modal');
  const deleteModal = document.getElementById('delete-modal');
  const form = document.getElementById('team-form');
  const deleteForm = document.getElementById('delete-form');
  const alertBox = document.getElementById('form-alert');

  let currentLogoUrl = '';

  // Modal Aç/Kapat Butonları
  document.querySelectorAll('[data-open-create]').forEach(b => b.addEventListener('click', () => {
    form.reset();
    document.getElementById('t-id').value = '';
    currentLogoUrl = '';
    document.getElementById('team-modal-title').textContent = 'Yeni Takım Ekle';
    alertBox.style.display = 'none';
    modal.showModal();
  }));

  document.querySelectorAll('[data-modal-close]').forEach(b => b.addEventListener('click', () => {
    modal.close();
    deleteModal.close();
  }));

  // Düzenle Butonuna Tıklanınca
  document.querySelectorAll('[data-edit]').forEach(btn => {
    btn.addEventListener('click', () => {
      const team = JSON.parse(btn.getAttribute('data-team'));
      currentLogoUrl = team.logoUrl || '';
      document.getElementById('t-id').value = team.id;
      document.getElementById('t-name').value = team.name;
      document.getElementById('t-code').value = team.code;
      document.getElementById('t-city').value = team.city;
      document.getElementById('t-region').value = team.region.toLowerCase();
      document.getElementById('t-stadium').value = team.stadium;
      document.getElementById('t-capacity').value = team.stadiumCapacity;
      document.getElementById('t-founded').value = new Date(team.foundedYear).getFullYear();
      document.getElementById('t-colorA').value = team.colorOne || '#1E5BD8';
      document.getElementById('t-colorB').value = team.colorTwo || '#FFFFFF';
      document.getElementById('t-status').value = team.status.toString();

      document.getElementById('team-modal-title').textContent = 'Takımı Düzenle: ' + team.name;
      alertBox.style.display = 'none';
      modal.showModal();
    });
  });

  // Sil Butonuna Tıklanınca
  document.querySelectorAll('[data-delete]').forEach(btn => {
    btn.addEventListener('click', () => {
      const id = btn.getAttribute('data-delete');
      const name = btn.getAttribute('data-name');
      document.getElementById('delete-id').value = id;
      document.getElementById('delete-text').textContent = `"${name}" takımını silmek (pasife almak) istediğinize emin misiniz?`;
      deleteModal.showModal();
    });
  });

  // Form Kaydet (Ekle / Güncelle)
  form.addEventListener('submit', async (e) => {
    e.preventDefault();
    const id = document.getElementById('t-id').value;
    const isEdit = !!id;

    const payload = {
      name: document.getElementById('t-name').value,
      code: document.getElementById('t-code').value.toUpperCase(),
      city: document.getElementById('t-city').value,
      region: document.getElementById('t-region').value,
      stadium: document.getElementById('t-stadium').value,
      stadiumCapacity: parseInt(document.getElementById('t-capacity').value) || 0,
      foundedYear: `${document.getElementById('t-founded').value || 1900}-01-01T00:00:00.000Z`,
      colorOne: document.getElementById('t-colorA').value,
      colorTwo: document.getElementById('t-colorB').value,
      status: document.getElementById('t-status').value === 'true',
      logoUrl: currentLogoUrl || "https://placeholder.com/logo.png"
    };

    const url = isEdit ? `/Teams/Update?id=${id}` : '/Teams/Create';
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

  // Confirm Delete
  deleteForm.addEventListener('submit', async (e) => {
    e.preventDefault();
    const id = document.getElementById('delete-id').value;
    const btnConfirm = document.getElementById('btn-confirm-delete');
    if (!id) return;

    try {
      if (btnConfirm) btnConfirm.classList.add('is-loading');
      const res = await fetch(`/Teams/Delete?id=${id}`, { method: 'POST' });
      const result = await res.json();

      if (result.isSuccess) {
        deleteModal.close();
        window.location.reload();
      } else {
        const errorMsg = result.message || (result.errors ? result.errors.join(', ') : 'Silme işlemi başarısız oldu.');
        alert(errorMsg);
      }
    } catch (err) {
      console.error('Delete team error:', err);
      alert('Takım silinirken sunucuyla iletişim hatası oluştu.');
    } finally {
      if (btnConfirm) btnConfirm.classList.remove('is-loading');
    }
  });
});
