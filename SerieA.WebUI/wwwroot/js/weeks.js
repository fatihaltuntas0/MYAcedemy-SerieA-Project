// weeks.js - Haftalar ve Fikstür Paneli Etkileşimi
document.addEventListener('DOMContentLoaded', () => {
  const panel = document.getElementById('week-detail-panel');
  const panelTitle = document.getElementById('week-panel-title');
  const panelSub = document.getElementById('week-panel-sub');
  const tbody = document.getElementById('week-matches-tbody');
  const btnClose = document.getElementById('btn-close-panel');

  if (!panel || !tbody) return;

  const STATUS_MAP = {
    'NotStarted': { label: 'Planlandı',   cls: 'badge-status--planned' },
    'Live':       { label: 'Canlı',       cls: 'badge-status--live' },
    'Finished':   { label: 'Tamamlandı',  cls: 'badge-status--finished' },
    'Postponed':  { label: 'Ertelendi',   cls: 'badge-status--planned' }
  };

  const formatDate = (isoString) => {
    if (!isoString) return '-';
    const d = new Date(isoString);
    const day = String(d.getDate()).padStart(2, '0');
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const year = d.getFullYear();
    return `${day}.${month}.${year}`;
  };

  const formatTime = (isoString) => {
    if (!isoString) return '';
    const d = new Date(isoString);
    const hours = String(d.getHours()).padStart(2, '0');
    const mins = String(d.getMinutes()).padStart(2, '0');
    return `${hours}:${mins}`;
  };

  // Haftayı İncele (Göz butonu) tıklama dinleyicisi
  document.querySelectorAll('[data-view-week]').forEach(btn => {
    btn.addEventListener('click', async () => {
      const week = btn.getAttribute('data-view-week');
      if (!week) return;

      panelTitle.textContent = `${week}. Hafta Karşılaşmaları`;
      if (panelSub) panelSub.textContent = `Serie A 2026/27 Sezonu · ${week}. Hafta Fikstürü`;
      
      // Skeleton Loading (ui-states)
      tbody.innerHTML = Array.from({ length: 6 }).map(() => `
        <tr class="skeleton-row">
          <td style="padding-left: 24px;"><div class="skeleton" style="width: 80px; height: 18px;"></div></td>
          <td style="text-align: right;"><div class="skeleton" style="width: 130px; height: 20px; margin-left: auto;"></div></td>
          <td style="text-align: center;"><div class="skeleton" style="width: 64px; height: 26px; border-radius: 9999px; margin: 0 auto;"></div></td>
          <td style="text-align: left;"><div class="skeleton" style="width: 130px; height: 20px;"></div></td>
          <td style="padding-right: 24px;"><div class="skeleton" style="width: 110px; height: 18px;"></div></td>
        </tr>
      `).join('');
      
      panel.hidden = false;
      panel.scrollIntoView({ behavior: 'smooth', block: 'nearest' });

      try {
        const response = await fetch(`/Weeks/GetWeekMatches?week=${week}`);
        const result = await response.json();

        if (!result.isSuccess || !result.data || result.data.length === 0) {
          tbody.innerHTML = `
            <tr>
              <td colspan="5">
                <div class="empty-state">
                  <div class="empty-state-icon">
                    <svg class="icon"><use href="#lucide-calendar" /></svg>
                  </div>
                  <h3 class="empty-state-title">${week}. Hafta İçin Maç Bulunamadı</h3>
                  <p class="empty-state-desc">Bu haftaya ait fikstürde henüz tanımlanmış bir maç kaydı bulunmuyor.</p>
                </div>
              </td>
            </tr>`;
          return;
        }

        const matches = result.data;
        tbody.innerHTML = matches.map(m => {
          const hasScore = m.homeScore !== null && m.homeScore !== undefined && m.awayScore !== null && m.awayScore !== undefined;
          const isLive = m.status === 'Live' || m.status === 1;

          const scoreHtml = hasScore 
            ? `<span class="match-score-pill ${isLive ? 'match-score-pill--live' : 'match-score-pill--finished'}">${m.homeScore} : ${m.awayScore}</span>`
            : `<span class="match-score-pill match-score-pill--scheduled">VS</span>`;

          const homeLogo = m.homeTeamLogoUrl 
            ? `<img class="match-team-logo" src="${m.homeTeamLogoUrl}" alt="${m.homeTeamName}" onerror="this.style.display='none';" />` 
            : '';

          const awayLogo = m.awayTeamLogoUrl 
            ? `<img class="match-team-logo" src="${m.awayTeamLogoUrl}" alt="${m.awayTeamName}" onerror="this.style.display='none';" />` 
            : '';

          return `
            <tr>
              <td style="padding-left: 24px;">
                <div class="match-datetime-cell">
                  <span class="match-date-val">${formatDate(m.matchDate)}</span>
                  <span class="match-time-val">${formatTime(m.matchDate)}</span>
                </div>
              </td>
              <td>
                <div class="match-team-home">
                  <span class="match-team-name">${m.homeTeamName}</span>
                  ${homeLogo}
                </div>
              </td>
              <td style="text-align: center;">${scoreHtml}</td>
              <td>
                <div class="match-team-away">
                  ${awayLogo}
                  <span class="match-team-name">${m.awayTeamName}</span>
                </div>
              </td>
              <td style="padding-right: 24px;">
                <div class="match-stadium-cell">
                  <svg class="icon" aria-hidden="true"><use href="#lucide-map-pin" /></svg>
                  <span>${m.stadium || '-'}</span>
                </div>
              </td>
            </tr>
          `;
        }).join('');

      } catch (error) {
        console.error('Error fetching week matches:', error);
        tbody.innerHTML = `
          <tr>
            <td colspan="5">
              <div class="empty-state empty-state--error">
                <div class="empty-state-icon">
                  <svg class="icon"><use href="#lucide-alert-circle" /></svg>
                </div>
                <h3 class="empty-state-title">Karşılaşmalar Yüklenemedi</h3>
                <p class="empty-state-desc">Sunucudan maç listesi alınırken bir hata meydana geldi. Lütfen tekrar deneyin.</p>
                <button type="button" class="btn-a btn-a--secondary btn-a--sm" onclick="document.querySelector('[data-view-week=\\'${week}\\']')?.click()">
                  Yeniden Dene
                </button>
              </div>
            </td>
          </tr>`;
      }
    });
  });

  if (btnClose) {
    btnClose.addEventListener('click', () => {
      panel.hidden = true;
    });
  }
});
