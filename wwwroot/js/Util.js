function validaArquivoImagem(fileInput) {
    let validos = /(\.jpg|\.png|\.gif|\.JPG|\.JPEG|\.Jpeg|\.jpeg)$/i;
    let nome = fileInput.get(0).files["0"].name;
    if (validos.test(nome)) {
        return true;
    }
    noty({
        text: 'Somente formatos de imagens: .JPG, .JPEG e PNG são permitidos',
        type: 'warning',
        timeout: 2500,
        progressBar: true
    });
    return false;
}

function imageZoom(id) {
    var img = $('#caminho-' + id).val();
    var imgPreview = document.getElementById("img-preview");
    imgPreview.src = img;
    $('#modal-image').modal('show');
}