let increase = document.querySelector(".increase");
let decrease = document.querySelector(".decrease");
let quan = document.querySelector(".quan").innerText;
increase.addEventListener("click", () => {
    quan++;
    document.querySelector(".quan").innerText = quan;
});
decrease.addEventListener("click", () => {
    if (quan > 1) quan--;
    document.querySelector(".quan").innerText = quan;
});
let btns = document.querySelectorAll(".velicinaBtn");
btns.forEach((btn, index) => {
    btns[index].addEventListener('click', () => {
        btns.forEach((btn) => {
            btn.classList.remove('velicinaBtnClicked');
            btn.value = 0;
        });
        btns[index].classList.add('velicinaBtnClicked');
        btns[index].value = 1;
    });
});

