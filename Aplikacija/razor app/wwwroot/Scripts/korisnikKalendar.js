let divKalendar = document.querySelector(".divDaniUMesecu");
let divMesec = document.querySelector(".mesec");
let datum = new Date();

const mesec = ["Januar", "Februar", "Mart", "April", "Maj", "Jun",
    "Jul", "Avgust", "Septembar", "Oktobar", "Novembar", "Decembar"
];
const dani = ['N', 'P', 'U', 'S', 'Č', 'P', 'S'];

divMesec.innerHTML = mesec[datum.getMonth()];


let brojDanaUMesecu;
if (datum.getMonth() === 0) {
    brojDanaUMesecu = 31;
} else if (datum.getMonth() === 1) {
    brojDanaUMesecu = 28;
} else if (datum.getMonth() === 2) {
    brojDanaUMesecu = 31;
} else if (datum.getMonth() === 3) {
    brojDanaUMesecu = 30;
} else if (datum.getMonth() === 4) {
    brojDanaUMesecu = 31;
} else if (datum.getMonth() === 5) {
    brojDanaUMesecu = 30;
} else if (datum.getMonth() === 6) {
    brojDanaUMesecu = 31;
} else if (datum.getMonth() === 7) {
    brojDanaUMesecu = 31;
} else if (datum.getMonth() === 8) {
    brojDanaUMesecu = 30;
} else if (datum.getMonth() === 9) {
    brojDanaUMesecu = 31;
} else if (datum.getMonth() === 10) {
    brojDanaUMesecu = 30;
} else if (datum.getMonth() === 11) {
    brojDanaUMesecu = 31;
}


let divPolje;
let divPoljeNedelja;
let divNedelja = document.createElement("div");
divNedelja.classList.add("divNedeljaUMesecu");

for (let j = 0; j < dani.length; j++) {
    divPoljeNedelja = document.createElement("div");
    divPoljeNedelja.innerHTML = dani[j];
    divPoljeNedelja.classList.add("divPoljeNedelja");
    divNedelja.appendChild(divPoljeNedelja);
}
divKalendar.appendChild(divNedelja);

for (let i = 0; i < 6; i++) {
    let divNedelja = document.createElement("div");
    divNedelja.classList.add("divNedeljaUMesecu");

    for (let j = 0; j < dani.length; j++) {
        divPolje = document.createElement("div");
        divPolje.className = "divPraznoPolje";
        divNedelja.appendChild(divPolje);
    }
    divKalendar.appendChild(divNedelja);
}

datum.setFullYear(2022, 5, 1);
divPolje = document.querySelectorAll(".divPraznoPolje");
let brojac = 1;
let dan;

dani.forEach((el, index) => {
    if (index === datum.getDay()) {
        dan = index;
    }
});

for (let i = 0; i < brojDanaUMesecu; i++) {
    divPolje[dan + i].innerHTML = i + 1;
    divPolje[dan + i].className = "divPolje";
    brojac++;
}
