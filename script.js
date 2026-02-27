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