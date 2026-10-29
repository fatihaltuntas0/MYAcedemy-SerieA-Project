/* admin-common.js - ortak yardımcılar (tüm admin sayfaları) */
const $=(s,r=document)=>r.querySelector(s), $$=(s,r=document)=>[...r.querySelectorAll(s)];
const esc=s=>String(s).replace(/[&<>"']/g,c=>({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[c]));
const fmtDate=iso=>iso.split('-').reverse().join('.');
const icon=id=>`<svg class="icon" aria-hidden="true"><use href="#i-${id}"/></svg>`;
const STATUS={NotStarted:'Başlamadı',Live:'Canlı',Finished:'Tamamlandı',Postponed:'Ertelendi'};
const teamCell=id=>`<span class="team"><span class="team-crest" style="--crest-a:${TEAMS[id].a};--crest-b:${TEAMS[id].b}" aria-hidden="true"></span>${esc(TEAMS[id].name)}</span>`;
const scoreBox=m=>m.status==='Finished'||m.status==='Live'?`<span class="score">${m.hs} - ${m.as}</span>`:'<span class="score score--empty">-</span>';
/* Puan durumu: yalnızca Finished maçlar. Sıralama: Puan > Averaj > Atılan Gol (GET /api/standings) */
function standings(upTo=99){const t={};Object.keys(TEAMS).forEach(k=>t[k]={id:k,o:0,g:0,b:0,m:0,ag:0,yg:0,p:0,form:[]});
 MATCHES.filter(m=>m.status==='Finished'&&m.week<=upTo).sort((x,y)=>x.week-y.week).forEach(m=>{
  [[m.home,m.hs,m.as],[m.away,m.as,m.hs]].forEach(([k,f,a])=>{const r=t[k];r.o++;r.ag+=f;r.yg+=a;
   const x=f>a?'W':f===a?'D':'L';r.form.push(x);if(x==='W'){r.g++;r.p+=3}else if(x==='D'){r.b++;r.p++}else r.m++;});});
 return Object.values(t).map(r=>({...r,av:r.ag-r.yg})).sort((a,b)=>b.p-a.p||b.av-a.av||b.ag-a.ag);}
function toast(msg,type='success'){const e=document.createElement('div');e.className='toast-a toast-a--'+type;e.setAttribute('role','status');
 e.innerHTML=icon(type==='success'?'check':'info')+'<span>'+esc(msg)+'</span>';$('[data-toasts]').appendChild(e);setTimeout(()=>e.remove(),3500);}
const tb=$('[data-sidebar-toggle]');const setSb=o=>{document.body.classList.toggle('sidebar-open',o);tb.setAttribute('aria-expanded',String(o))};
tb.addEventListener('click',()=>setSb(!document.body.classList.contains('sidebar-open')));$('[data-sidebar-close]').addEventListener('click',()=>setSb(false));
document.addEventListener('keydown',e=>{if(e.key==='Escape')setSb(false)});
/* Yerel önizleme: file:// ile açılınca menü bağlantıları dosyalara yönlenir */
if(location.protocol==='file:'){const map={'/Admin':'admin-home.html','/Admin/Matches':'../Admin_Paneli_2.html','/Admin/Teams':'../Admin_Paneli_1.html','/Admin/Weeks':'admin-weeks.html','/Admin/Standings':'admin-standings.html','/Admin/Users':'admin-users.html'};
 $$('a[href]').forEach(a=>{const h=a.getAttribute('href');if(map[h])a.setAttribute('href',map[h]);else if(h.startsWith('/Admin/Matches/Detail/'))a.setAttribute('href','admin-match-detail.html?id='+h.split('/').pop());});}
function modalWire(d){d.addEventListener('click',e=>{if(e.target===d)d.close()});$$('[data-modal-close]',d).forEach(b=>b.addEventListener('click',()=>d.close()))}
