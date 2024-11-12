const checkbox = document.getElementById('lb-aceite');
const btnConfirmar = document.getElementById('btn-submit');

checkbox.addEventListener('click', function () {
    console.log("clicou");
    if (this.checked) {
        btnConfirmar.disabled = false;
    } else {
        btnConfirmar.disabled = true;
    }
});