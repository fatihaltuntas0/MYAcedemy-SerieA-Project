document.addEventListener('DOMContentLoaded', () => {
    const userModal = document.getElementById('user-modal');
    const deleteModal = document.getElementById('delete-modal');
    const userForm = document.getElementById('user-form');
    const deleteForm = document.getElementById('delete-form');
    const alertBox = document.getElementById('form-alert');
    const btnSave = document.getElementById('btn-save-user');
    const btnDelete = document.getElementById('btn-confirm-delete');
    const passReq = document.getElementById('pass-req');
    const modalTitle = document.getElementById('user-modal-title');

    // Toast Notification Helper
    function showToast(message, type = 'success') {
        const toastContainer = document.querySelector('[data-toasts]') || document.querySelector('.toast-stack');
        if (!toastContainer) {
            console.log(`[Toast ${type}]: ${message}`);
            return;
        }

        const toast = document.createElement('div');
        toast.className = `toast-a ${type === 'error' ? 'toast-a--error' : 'toast-a--success'}`;
        toast.setAttribute('role', 'status');
        toast.innerHTML = `
            <svg class="icon" aria-hidden="true">
                <use href="${type === 'error' ? '#lucide-x' : '#lucide-check'}"></use>
            </svg>
            <span>${message}</span>
        `;

        toastContainer.appendChild(toast);
        requestAnimationFrame(() => toast.classList.add('is-visible'));

        setTimeout(() => {
            toast.classList.remove('is-visible');
            setTimeout(() => toast.remove(), 250);
        }, 3000);
    }

    // Clear form errors
    function clearErrors() {
        if (alertBox) {
            alertBox.style.display = 'none';
            alertBox.textContent = '';
        }
        document.querySelectorAll('.field-error').forEach(p => {
            p.textContent = '';
            p.style.display = 'none';
        });
        document.querySelectorAll('.form-field').forEach(f => f.classList.remove('has-error'));
    }

    // Set field error
    function setFieldError(fieldName, message) {
        const field = document.querySelector(`.form-field[data-field="${fieldName}"]`);
        if (field) {
            field.classList.add('has-error');
            const errP = field.querySelector('.field-error');
            if (errP) {
                errP.textContent = message;
                errP.style.display = 'block';
            }
        }
    }

    // Modal Close Buttons
    document.querySelectorAll('[data-modal-close]').forEach(btn => {
        btn.addEventListener('click', () => {
            if (userModal?.open) userModal.close();
            if (deleteModal?.open) deleteModal.close();
            clearErrors();
        });
    });

    // 1. Open Create User Modal
    document.querySelectorAll('[data-open-create]').forEach(btn => {
        btn.addEventListener('click', () => {
            userForm.reset();
            clearErrors();
            document.getElementById('u-id').value = '';
            modalTitle.textContent = 'Yeni Kullanıcı Ekle';
            if (passReq) passReq.style.display = 'inline';
            document.getElementById('u-pass').setAttribute('placeholder', 'En az 8 karakter');
            userModal.showModal();
            document.getElementById('u-name').focus();
        });
    });

    // 2. Open Edit User Modal
    document.querySelectorAll('[data-edit]').forEach(btn => {
        btn.addEventListener('click', () => {
            clearErrors();
            try {
                const user = JSON.parse(btn.getAttribute('data-user'));
                document.getElementById('u-id').value = user.id;
                document.getElementById('u-name').value = user.name || '';
                document.getElementById('u-email').value = user.email || '';
                document.getElementById('u-role').value = user.role || 'Editor';
                document.getElementById('u-active').value = user.status ? 'true' : 'false';
                document.getElementById('u-pass').value = '';
                document.getElementById('u-pass').setAttribute('placeholder', 'Değiştirmek istemiyorsanız boş bırakın');

                modalTitle.textContent = `Kullanıcıyı Düzenle: ${user.name}`;
                if (passReq) passReq.style.display = 'none';

                userModal.showModal();
                document.getElementById('u-name').focus();
            } catch (err) {
                console.error('Kullanıcı verisi ayrıştırılamadı:', err);
            }
        });
    });

    // 3. Save User Form (Create or Update)
    userForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        clearErrors();

        const id = document.getElementById('u-id').value.trim();
        const name = document.getElementById('u-name').value.trim();
        const email = document.getElementById('u-email').value.trim();
        const role = document.getElementById('u-role').value;
        const status = document.getElementById('u-active').value === 'true';
        const password = document.getElementById('u-pass').value;

        // Client-side validations
        let hasError = false;

        if (!name) {
            setFieldError('name', 'Ad Soyad alanı zorunludur.');
            hasError = true;
        }

        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!email) {
            setFieldError('email', 'E-posta adresi zorunludur.');
            hasError = true;
        } else if (!emailRegex.test(email)) {
            setFieldError('email', 'Geçerli bir e-posta adresi giriniz.');
            hasError = true;
        }

        const isNew = !id;
        if (isNew && (!password || password.length < 8)) {
            setFieldError('password', 'Şifre alanı zorunludur ve en az 8 karakter olmalıdır.');
            hasError = true;
        } else if (!isNew && password && password.length < 8) {
            setFieldError('password', 'Yeni şifre en az 8 karakter olmalıdır.');
            hasError = true;
        }

        if (hasError) {
            if (alertBox) {
                alertBox.textContent = 'Lütfen formdaki hataları kontrol ediniz.';
                alertBox.style.display = 'block';
            }
            return;
        }

        // Prepare request
        const url = isNew ? '/Users/Create' : `/Users/Update?id=${id}`;
        const payload = isNew ? {
            name,
            email,
            password,
            role,
            status
        } : {
            name,
            email,
            password: password || null,
            role,
            status
        };

        const originalBtnText = btnSave.innerHTML;
        btnSave.disabled = true;
        btnSave.innerHTML = `<span class="spinner-border spinner-border-sm me-1" role="status" aria-hidden="true"></span> Kaydediliyor...`;

        try {
            const response = await fetch(url, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(payload)
            });

            const result = await response.json();

            if (result.isSuccess) {
                showToast(isNew ? 'Kullanıcı başarıyla eklendi.' : 'Kullanıcı başarıyla güncellendi.', 'success');
                userModal.close();
                setTimeout(() => window.location.reload(), 400);
            } else {
                const errorMsg = result.message || 'İşlem gerçekleştirilemedi.';
                const details = result.errors && result.errors.length > 0 ? ` (${result.errors.join(', ')})` : '';
                if (alertBox) {
                    alertBox.textContent = errorMsg + details;
                    alertBox.style.display = 'block';
                }
            }
        } catch (err) {
            console.error('İstek hatası:', err);
            if (alertBox) {
                alertBox.textContent = 'Sunucuyla iletişim kurulurken bir hata oluştu.';
                alertBox.style.display = 'block';
            }
        } finally {
            btnSave.disabled = false;
            btnSave.innerHTML = originalBtnText;
        }
    });

    // 4. Open Delete Confirmation Modal
    document.querySelectorAll('[data-delete]').forEach(btn => {
        btn.addEventListener('click', () => {
            const id = btn.getAttribute('data-delete');
            const name = btn.getAttribute('data-name');
            const email = btn.getAttribute('data-email');

            document.getElementById('delete-id').value = id;
            const deleteText = document.getElementById('delete-text');
            if (deleteText) {
                deleteText.innerHTML = `<strong>${name}</strong> (${email}) kullanıcısı kalıcı olarak silinecektir.<br><small class="text-danger">Bu işlem geri alınamaz.</small>`;
            }

            deleteModal.showModal();
        });
    });

    // 5. Confirm Delete
    if (btnDelete) {
        btnDelete.addEventListener('click', async () => {
            const id = document.getElementById('delete-id').value;
            if (!id) return;

            const originalBtnText = btnDelete.innerHTML;
            btnDelete.disabled = true;
            btnDelete.innerHTML = `<span class="spinner-border spinner-border-sm me-1" role="status" aria-hidden="true"></span> Siliniyor...`;

            try {
                const response = await fetch(`/Users/Delete?id=${id}`, {
                    method: 'POST'
                });

                const result = await response.json();

                if (result.isSuccess) {
                    showToast('Kullanıcı başarıyla silindi.', 'success');
                    deleteModal.close();
                    setTimeout(() => window.location.reload(), 400);
                } else {
                    alert(result.message || 'Kullanıcı silinemedi.');
                }
            } catch (err) {
                console.error('Silme hatası:', err);
                alert('Kullanıcı silinirken bir sunucu hatası oluştu.');
            } finally {
                btnDelete.disabled = false;
                btnDelete.innerHTML = originalBtnText;
            }
        });
    }
});

