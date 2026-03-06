const buton = document.getElementById("jsBtn");
const yazi = document.getElementById("degisecekYazi");
const haftaBaslik = document.getElementById("haftaBaslik");


buton.addEventListener("click", function() {
    yazi.innerText = "Javascript ile yapıldı";
    haftaBaslik.innerText = "2. Hafta";
    
});
const siralaBtn = document.getElementById("siralaBtn");
const tablo = document.querySelector(".dataTable tbody");

siralaBtn.addEventListener("click", function () {

    // Satırları al
    const satirlar = Array.from(tablo.querySelectorAll("tr"));

    // Vize notuna göre büyükten küçüğe sırala
    satirlar.sort(function (a, b) {
        const vizeA = parseInt(a.children[2].innerText);
        const vizeB = parseInt(b.children[2].innerText);
        return vizeB - vizeA;  // Büyükten küçüğe
    });

    // Tabloyu temizle
    tablo.innerHTML = "";

    // Sıralanmış satırları tekrar ekle
    satirlar.forEach(function (satir) {
        tablo.appendChild(satir);
    });

});
const degistirBtn = document.getElementById("degistirBtn");
const resim = document.getElementById("degisenResim");

degistirBtn.addEventListener("click", function () {
    resim.src = "js.png";
});
const kutu = document.getElementById("animBox");

let pozisyon = 0;
let yon = 1; // 1 = sağa , -1 = sola

function animasyon() {

    pozisyon += yon * 3;

    if (pozisyon >= 1100) {
        yon = -1; // sola dön
    }

    if (pozisyon <= 0) {
        yon = 1; // sağa dön
    }

    kutu.style.left = pozisyon + "px";
}

setInterval(animasyon, 20);

kutu.addEventListener("click", function () {
    // Zıplama animasyonu için sınıf ekle
    kutu.classList.add("jump");

    // Animasyon bitince sınıfı kaldır
    function animasyonBitti() {
        kutu.classList.remove("jump");
        kutu.removeEventListener("animationend", animasyonBitti);
    }

    kutu.addEventListener("animationend", animasyonBitti);
});