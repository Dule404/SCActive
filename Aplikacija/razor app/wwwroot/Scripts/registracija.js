var email = document.getElementById('poljeEmail');
var password = document.getElementById('poljeSifra');
var name = document.getElementById('poljeIme');
var surname = document.getElementById('poljePrezime');
var birthdate = document.getElementById('poljeDTRodjenja');
var phone = document.getElementById('phone');
var select = document.getElementById('sel');
let btnReg = document.querySelector('.btnRegistracija');
let msg = document.querySelector('.poruka');

btnReg.addEventListener('click', () => {
    msg.innerHTML = ""
    if (validateInputs())
        register();
});

function validateInputs() {
    if (email.value != "" && password.value != "" && name.value != "" && surname.value != "" && birthdate.value != "") {
        if (checkPass(password.value))
            return true;
        else {
            msg.innerHTML = "Lozinka mora biti minimum 7 karatera dugacka a maksimum 15, sa minimum jednim brojem i specijalnim karakterom";
        }
    } else
        msg.innerHTML = "Sva oznacena polja ne smeju biti prazna!";
}

function checkPass(pass) {
    var paswd = /^(?=.*[0-9])(?=.*[!@#$%^&*])[a-zA-Z0-9!@#$%^&*]{7,15}$/;
    if (password.value.match(paswd)) {
        return true;
    } else {
        return false;
    }
}

