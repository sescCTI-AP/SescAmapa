"use strict";

if ($('[data-toggle="tooltip"]').length > 0) {
    $('[data-toggle="tooltip"]').tooltip();
}

function phoneNumber(obj) {
    obj.mask("(99) 9999-9999?9").focusout(function (event) {
        var target, phone, element;
        target = (event.currentTarget) ? event.currentTarget : event.srcElement;
        phone = target.value.replace(/\D/g, '');
        element = $(target);
        element.unmask();
        if (phone.length > 10) {
            element.mask("(99) 99999-9999");
        } else {
            element.mask("(99) 9999-9999?9");
        }
    });
}

function dateFormat(date) {
    const newDate = new Date();
    newDate.setTime(date.replace('/Date(', '').replace(')/', ''));
    return setZero(newDate.getDate()) + '/' + setZero(newDate.getMonth() + 1) + '/' + newDate.getFullYear() + ' às ' + setZero(newDate.getHours()) + ':' + setZero(newDate.getMinutes());
}

function stringToDate(dateStr) {
    return new Date(dateStr.split('/')[2], dateStr.split('/')[1] - 1, dateStr.split('/')[0]);
}

function validaDataInicioFim(dataInicio, dataFim) {
    const d1 = new Date(moment(dataInicio, 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm'));
    const d2 = new Date(moment(dataFim, 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm'));
    return moment(d2).isAfter(d1);
}

function replaceAll(str, needle, replacement) {
    var i = 0;
    while ((i = str.indexOf(needle, i)) != -1) {
        str = str.replace(needle, replacement);
    }
    return str;
}

$('.showPreviewImagemUpload').change(function () {
    if (typeof (FileReader) !== 'undefined') {

        var imageHolder = $('#image-holder');
        imageHolder.empty();

        const reader = new FileReader();
        reader.onload = function (e) {
            $("<img />",
                {
                    "src": e.target.result,
                    "style": "height: 300px",
                    "class": "thumb-image"
                }).appendTo(imageHolder);
        }
        imageHolder.show();
        reader.readAsDataURL($(this)[0].files[0]);
    } else {
        alert("Este navegador nao suporta FileReader.");
    }
});

$('.showPreviewImagemUpload2').change(function () {
    if (typeof (FileReader) !== 'undefined') {

        var imageHolder = $('#image-holder2');
        imageHolder.empty();

        const reader = new FileReader();
        reader.onload = function (e) {
            $("<img />",
                {
                    "src": e.target.result,
                    "style": "height: 300px",
                    "class": "thumb-image"
                }).appendTo(imageHolder);
        }
        imageHolder.show();
        reader.readAsDataURL($(this)[0].files[0]);
    } else {
        alert("Este navegador nao suporta FileReader.");
    }
});


$('form').each(function () {
    if ($(this).data('validator'))
        $(this).data('validator').settings.ignore = ".note-editor *";
});


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

function initPagination() {
    const pagination = document.querySelector('#paginacao');

    if (pagination != null) {
        const listLi = pagination.children[0].children[0].children[0].children;
        for (let i = 0; i < listLi.length; i++) {
            listLi[i].classList.add("page-item");
            listLi[i].children[0].className = 'page-link';
        }
    }
}

//Torna o texto maiúsculo e remove os acentos
function FormataTexto(texto) {
    let textMaiusculo = texto.toUpperCase();
    return textMaiusculo.normalize('NFD').replace(/[\u0300-\u036f]/g, "");
}

$(document).ready(function () {
    //window.location.href = '#inicio';

    setTimeout(function () {
        $('.load').fadeOut('fast');
    }, 1000);
    setTimeout(function () {
        $('.load-back').fadeOut('slow');
    }, 1000);

    $('a[href="#top"]').click(function () {
        $('html, body').animate({ scrollTop: 0 }, 800);
        return false;
    });

    //$("#sidebar").mCustomScrollbar({
    //    theme: "minimal"
    //});

    $('#dismiss, .overlay').on('click', function () {
        $('#sidebar').removeClass('active');
        $('.overlay').removeClass('active');
    });

    $('#sidebarCollapse').on('click', function () {
        $('#sidebar').addClass('active');
        $('.overlay').addClass('active');
        $('.collapse.in').toggleClass('in');
        $('a[aria-expanded=true]').attr('aria-expanded', 'false');
    });

    //Google analitics
    window.dataLayer = window.dataLayer || [];
    function gtag() { dataLayer.push(arguments); }
    gtag('js', new Date());

    gtag('config', 'UA-147621804-1');
});

function getEnderecoByCep(cep) {
    if (/[0-9]{5}\-[0-9]{3}/g.test(cep)) {
        $.ajax({
            url: `https://viacep.com.br/ws/${cep}/json/unicode/`,
            type: 'GET',
            dataType: "json",
            contentType: 'application/json; charset-8',
            beforeSend: function () {
                $.LoadingOverlay("show",
                    {
                        background: "rgba(176, 177, 178, 0.5)",
                        text: "Buscando endereço..."
                    });
            },
            complete: function () {
                $.LoadingOverlay("hide");
            },
            success: function (endereco) {
                if (endereco.erro) {
                    alert('Endereço não encontrado, verifique o CEP digitado e tente novamente.');
                } else {
                    $('#Logradouro').val(FormataTexto(endereco.logradouro));
                    $('#Bairro').val(FormataTexto(endereco.bairro));
                    $('#Cidade').val(FormataTexto(endereco.localidade));
                    $('#Estado').val(endereco.uf);
                }
            },
            error: function (err) {
                alert('Falha ao buscar o endereço. Insira endereço manualmente');
            }
        });
    }
}