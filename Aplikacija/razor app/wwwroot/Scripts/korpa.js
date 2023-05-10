
let btnPlati = document.querySelector(".plati");
let btnPoruci = document.querySelector(".poruci");
let divPlacanje = document.querySelector(".naplata");
let divKorpa = document.querySelector(".korpa");
let popup = document.querySelector('.popup');
let overlay = document.querySelector('.overlay');
const mediaIncrease = window.matchMedia('(max-width:1000px)')
//div Proizvodi
let proizvodi = document.querySelectorAll('.mainDivProizvod');

const increaseWidth = () => {
    divKorpa.style.width = '100%';
}

btnPlati.addEventListener("click", () => {
    if (proizvodi.length == 0) {
        popup.innerText = "Vaša korpa je prazna"
        popup.style.display = 'flex';
        overlay.style.display = 'flex';

        let btnClose = document.createElement('button');
        btnClose.type = 'button';
        btnClose.id = "closePopup";
        btnClose.innerText = "OK";
        popup.appendChild(btnClose)

        btnClose.addEventListener("click", () => {
            popup.style.display = 'none';
            overlay.style.display = 'none';
            btnClose.removeEventListener("click");
        });
    }
    else {
        btnPlati.style.display = 'none';
        btnPoruci.style.display = 'flex';
        divPlacanje.style.display = 'flex';
        mediaIncrease.addListener(increaseWidth);
        window.addEventListener('resize', () => {
            if (window.innerWidth > 1000)
                divKorpa.style.width = '40%';
        });
        divKorpa.style.width = '40%';
        divKorpa.style.paddingLeft = '2%';
        divKorpa.querySelector(".kolone").style.display = 'none';
        divKorpa.querySelector(".placanje").style.padding = '0';
        btnPlati.innerText = "Poručite";
        //div Proizvodi

        proizvodi.forEach((proizvod) => {
            proizvod.classList.add('mainDivProizvodClicked');
            proizvod.querySelector('.infoDivProizvod').classList.add('infoDivProizvodClicked');
            proizvod.querySelector('.cena').classList.add('cenaClicked');
            proizvod.querySelector('.ukupno').classList.add('ukupnoClicked');
            proizvod.querySelector('.decrease').classList.add('decreaseClicked');
            proizvod.querySelector('.increase').classList.add('increaseClicked');
            proizvod.querySelector('.quan').classList.add('quanClicked');
            proizvod.querySelector('.naziv').classList.add('nazivClicked');
            proizvod.querySelector('.complexBtn').classList.add('complexBtnClicked');
        });
    }
});

let ime = document.querySelector('#ime');
let prezime = document.querySelector('#prezime');
let ulica = document.querySelector('#ulica');
let grad = document.querySelector('#grad');
let postanskiBroj = document.querySelector('#posBroj');
let brojTelefona = document.querySelector('#brTelefona');
let email = document.querySelector('#email');

const validate = () => {

    let btnClose = document.createElement('button');
    btnClose.id = "closePopup";
    btnClose.innerText = "OK";
    btnClose.type = 'button';

    let btnShop = document.createElement('button');
    btnShop.id = "closePopup2";
    btnShop.innerText = "OK";
    
    if (ime.value === "") {
        popup.innerText = "Molimo Vas popunite polje Ime";
        popup.appendChild(btnClose)
    } else if (prezime.value === "") {
        popup.innerText = "Molimo Vas popunite polje Preziime";
        popup.appendChild(btnClose)
    } else if (ulica.value === "") {
        popup.innerText = "Molimo Vas popunite polje Ulica i kućni broj/broj stana*";
        popup.appendChild(btnClose)
    } else if (grad.value === "") {
        popup.innerText = "Molimo Vas popunite polje Grad";
        popup.appendChild(btnClose)
    } else if (postanskiBroj.value === "") {
        popup.innerText = "Molimo Vas popunite polje Poštanski broj";
        popup.appendChild(btnClose)
    } else if (brojTelefona.value === "") {
        popup.innerText = "Molimo Vas popunite polje Broj telefona";
        popup.appendChild(btnClose)
    } else {
        popup.classList.remove('popupBg');
        popup.innerText = "Vaša porudžbina je sada u procesu slanja.\nHvala Vam na poverenju.";
        popup.appendChild(btnShop);
    }

    btnClose.addEventListener("click", () => {
        let popup = document.querySelector('.popup');
        let overlay = document.querySelector('.overlay');
        popup.style.display = 'none';
        overlay.style.display = 'none';
        btnClose.removeEventListener('click',this.listener);
    });

    btnShop.addEventListener("click", () => {
        console.log(document.querySelector('.forma'));
        document.querySelector('.forma').submit();
    });
} 

btnPoruci.addEventListener("click", () => {
    popup.classList.add('popupBg');
    popup.style.display = 'block';
    overlay.style.display = 'block';

    validate();
});
