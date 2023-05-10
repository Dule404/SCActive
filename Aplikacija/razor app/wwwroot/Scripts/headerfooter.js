var btnClick = document.querySelector(".ham");
var btnx = document.querySelector(".x");
let nav = document.querySelector('.navigation');
btnClick.addEventListener('click', () => {
    nav.classList.add("menu-hide");
    btnClick.style.display = "none";
});
btnx.addEventListener('click', () => {
    nav.classList.remove("menu-hide");
    btnClick.style.display = "initial";
});