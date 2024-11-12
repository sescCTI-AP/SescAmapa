'use strict';

const btnCortarImagem = document.querySelector('#btnCortarImagem');

var $uploadCrop = $('#croppingArea').croppie({
    enableExif: true,
    viewport: {
        width: 1860,
        height: 556,
        type: 'square'
    },
    boundary: {
        width: 1860,
        height: 550
    }
});

function readFile(input) {
    if (validaArquivoImagem($('#upload'))) {
        if (typeof (FileReader) != "undefined") {
            let reader = new FileReader();

            reader.onload = function (e) {
                $uploadCrop.croppie('bind', {
                    url: e.target.result
                }).then(function () {
                    console.log('Imagem carregada!');
                });
            }
            reader.readAsDataURL(input.files[0]);
            btnCortarImagem.disabled = false;
        } else {
            btnCortarImagem.disabled = true;
            alert("Seu navegador não possui suporte ao FileReader API, portanto a imagem seleciona não pode ser exibida");
        }
    } else {
        $('#imagemRecortada').val('');
        $('#resultCrop').html();
        btnCortarImagem.disabled = true;
    }
}

$('#upload').on('change', function () {
    if (this.files.length > 0) {
        readFile(this);
    }
});

btnCortarImagem.addEventListener('click', () => {
    $uploadCrop.croppie('result', {
        type: 'canvas',
        size: 'viewport'
    }).then(function (resp) {
        recortarImagem({
            src: resp
        });
    });
});

function recortarImagem(result) {
    $("#btn-cadastrar-submit").show();
    $("#alert-cortar").hide();
    let html = '';
    if (result.html) {
        html = result.html;
    }
    if (result.src) {
        html = '<p>Imagem recortada</p><img src="' + result.src + '" class="img-thumbnail" style="max-width: 600px"/>';
    }
    $('#resultCrop').html(html);
    $('#imagemRecortada').val(result.src);
}


function loadImagemInCrop() {
    let urlImagemNoticiaAtual = $('#imagemAtual').val();
    console.log("url: " + urlImagemNoticiaAtual);
    $uploadCrop.croppie('bind', {
        url: urlImagemNoticiaAtual
    }).then(function () {
        //console.log('Imagem carregada!');
        btnCortarImagem.disabled = false;
        
        $('#resultCrop').html('<p>Imagem recortada</p><img src="' + urlImagemNoticiaAtual + '" class="img-thumbnail" style="max-width: 600px"/>');
    });
}