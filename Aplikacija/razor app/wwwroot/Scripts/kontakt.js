var tbName = document.getElementById('ImeKontakt');
var tbSurname = document.getElementById('PrezimeKontakt');
var tbEmail = document.getElementById('mailKontakt');
var tbContent = document.getElementById('porukaKontakt');
let btnPrijavise = document.querySelector('.btnPosalji');

btnPrijavise.addEventListener('click', () => {
    //fetch()
    btnPrijavise.removeEventListener('click', btnPrijavise);
    btnPrijavise.disabled = true;
    btnPrijavise.style.backgroundColor = '#e26a88';
    btnPrijavise.style.cursor = 'initial';
});