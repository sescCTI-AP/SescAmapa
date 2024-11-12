
exibeQrCode = function (xhr) {
    //console.log(xhr.responseJSON);

    limparCamposPix();
    var down = document.getElementById('divQrCodePix');
    var img = document.createElement('img');
    img.src = xhr.responseJSON.TextoImagem
    img.setAttribute('class', 'qrcodepix');
    img.setAttribute('width', '200');
    down.appendChild(img);
    $('#codigoPix').val(xhr.responseJSON.Codigo);
    $('#nomeAtividade').text(xhr.responseJSON.Atividade);
    $('#referencia').text('Referencia: ' + xhr.responseJSON.Referencia);
    $('#valorCobranca').text('R$ ' + xhr.responseJSON.Valor);
    $('#modalQrCode').modal('show');
};
function limparCamposPix() {
    $('#nomeAtividade').text('');
    $('#referencia').text('');
    $('#valorCobranca').text('');
    $('#codigoPix').val('');
    const div = document.querySelector("#divQrCodePix");
    div.innerHTML = "";
}
function copiarTexto() {
    var ele = $("#codigoPix").select();
    document.execCommand('copy');
    swal({
        text: 'Código copiado',
        icon: 'success',
        timer: 2000
    });
}
function onAjaxBegin() {
    $("#divLoading").removeClass("ShowLoader").addClass("ShowLoader");
}

function onAjaxComplete() {
    $("#divLoading").removeClass("ShowLoader").addClass("HideLoader");
}
