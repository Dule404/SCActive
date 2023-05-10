var tbEmail = document.getElementById('poljeEmail');
var tbPassword = document.getElementById('poljeSifra');
let btnPrijavise = document.querySelector('.btnRegistracija');

btnPrijavise.addEventListener('click', () => {
    btnPrijavise.removeEventListener('click', btnPrijavise);
    btnPrijavise.disabled = true;
    btnPrijavise.style.backgroundColor = 'grey';
    document.body.style.cursor = 'wait';
    var inputs = document.querySelectorAll('input');
    inputs.forEach(el => {
        el.classList.add('pointerEvent');
    });
    var btns = document.querySelectorAll('button');
    btns.forEach(el => {
        el.classList.add('pointerEvent');
    });
    var a = document.querySelector('.nn');
    a.classList.add('pointerEvent');
});