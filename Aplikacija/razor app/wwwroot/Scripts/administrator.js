
let pContCount = 0;
let addPContCount = 0;
let showPContCount = 0;
let cContCount = 0;
let tContCount = 0;

let pLabel = document.querySelector('.prozvodiContainerShow');
let addLabel = document.querySelector('.InterfaceAddShow');
let showLabel = document.querySelector('.proizvodiShow');
let cLabel = document.querySelector('.clanoviContainerShow');
let tLabel = document.querySelector('.treneriContainerShow');
let addPCont = document.getElementById('addProizvod');
let showPCont = document.getElementById('showProizvod');
let cView = document.getElementById('clanoviView');
let tView = document.getElementById('treneriView');
let pView = document.getElementById('proizvodiView');
let addView = document.getElementById('addProizvodView');

pLabel.addEventListener("click", () => {
    if (pContCount == 0) {
        addPCont.style.display='block';
        showPCont.style.display = 'block';
    } else {
        addPCont.style.display = 'none';
        showPCont.style.display = 'none';
    }
    pContCount = (pContCount + 1) % 2;
});

addLabel.addEventListener("click", () => {
    if (addPContCount == 0) {
        addView.style.display = 'flex';
        addPCont.style.display = 'block';
    } else {
        addView.style.display = 'none';
    }
    addPContCount = (addPContCount + 1) % 2;
});

showLabel.addEventListener("click", () => {
    if (showPContCount == 0) {
        pView.style.display = 'block';
        showPCont.style.display = 'block';
    } else {
        pView.style.display = 'none';
    }
    showPContCount = (showPContCount + 1) % 2;
});

cLabel.addEventListener("click", () => {
    if (cContCount == 0) {
        cView.style.display = 'block';
    } else {
        cView.style.display = 'none';
    }
    cContCount = (cContCount + 1) % 2;
});

tLabel.addEventListener("click", () => {
    if (tContCount == 0) {
        tView.style.display = 'block';
    } else {
        tView.style.display = 'none';
    }
    tContCount = (tContCount + 1) % 2;
});



let imgsUrl = [null,null,null,null];

function doSomething()
{
    return new Promise((resolve,reject) => {

        let axa = new FormData();
        let axa2 = new FormData();
        let axa3 = new FormData();
        let axa4 = new FormData();
        axa.append("file",imgs[0]);
        axa2.append("file",imgs[1]);
        axa3.append("file",imgs[2]);
        axa4.append("file",imgs[3]);

        
        fetch('https://localhost:5001/Pictures/AddPicture', {
                method: "POST",
                body: axa
            }).then(res => {
                if (res.ok) {
                    res.json().then(data => {
                        imgsUrl[0] = data.url;
                        console.log(imgsUrl[0]);
                    })
                } else {
                    console.log("Nije dodata slika na azure.");
                }
            }).then(()=>{
                fetch('https://localhost:5001/Pictures/AddPicture', {
                    method: "POST",
                    body: axa2
                }).then(res => {
                    if (res.ok) {
                        res.json().then(data => {
                            imgsUrl[1] = data.url;
                            console.log(imgsUrl[1]);
                        })
                    } else {
                        console.log("Nije dodata slika na azure.");
                    }
                }).then(()=>{
                    fetch('https://localhost:5001/Pictures/AddPicture', {
                        method: "POST",
                        body: axa3
                    }).then(res => {
                        if (res.ok) {
                            res.json().then(data => {
                                imgsUrl[2] = data.url;
                                console.log(imgsUrl[2]);
                            })
                        } else {
                            console.log("Nije dodata slika na azure.");
                        }
                    }).then(()=>{
                        fetch('https://localhost:5001/Pictures/AddPicture', {
                            method: "POST",
                            body: axa4
                        }).then(res => {
                            if (res.ok) {
                                res.json().then(data => {
                                    imgsUrl[3] = data.url;
                                    console.log(imgsUrl[3]);
                                    resolve(imgsUrl);
                                })
                            } else {
                                console.log("Nije dodata slika na azure.");
                            }
                        })
                    })
                })
            })
    });
}


let imgs = ["","","",""];

let btnAddProduct = document.getElementById('dodajBtn');
btnAddProduct.addEventListener("click", () => {
    let ime = document.getElementById('ime').value;
    let cena = document.getElementById('cena').value;
    let opis = document.getElementById('opis').value;
    let kolicina = document.getElementById('kolicina').value;
    let kategorijaSelect = document.getElementById('kategorija');
    let kategorija = kategorijaSelect.options[kategorijaSelect.selectedIndex].value;

    doSomething(imgs).then(i => {
        console.log(imgsUrl);
       const data1 = {
           "ime": ime,
           "cena": cena,
           "kategorija": kategorija,
           "opis": opis,
           "slika0": imgsUrl[0],
           "slika1": imgsUrl[1],
           "slika2": imgsUrl[2],
           "slika3": imgsUrl[3],
           "velicina": "string",
           "kolicina": kolicina,
           "kvarljivo": true,
           "datumIstekaRoka": "2022-06-23T18:53:01.952Z",
           "createdDate": "2022-06-23T18:53:01.952Z"
       }

        fetch('https://localhost:5001/Admin/AddProduct', {
           method: 'POST',
           headers: {
               'Content-Type': 'application/json',
           },
           body: JSON.stringify(data1),
       }).then(res => {
           if (res.ok) {
               alert("Success");
           }
           else {
               alert("Failed! Check console for more informations.")
           }
       });
   })
});


let listInputsImg = document.querySelectorAll('.imageInput');
let listImg = [ 'img1' , 'img2','img3','img4'];
let inputTag = ['input1','input2','input3','input4' ];

listInputsImg.forEach((el,index)=>{
    el.addEventListener('change',()=>{
        previewFile(listImg[index],el,index);
    })
})



function previewFile(img,inputTag,index) {
    const preview = document.getElementById(img);
    const file = inputTag.files[0];
    const reader = new FileReader();
    imgs[index] = file;
    console.log(imgs);
    reader.addEventListener("load", function () {
        // convert image file to base64 string
        preview.src = reader.result;
    }, false);

    if (file) {
        reader.readAsDataURL(file);
    }
}
