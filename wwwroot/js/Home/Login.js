const msgCapsLock = $('#msgCapsLock');


$(document).ready(function () {
    $('#Cpf').mask('999.999.999-99');
});


$('#esqueciasenha').on('click', function (e) {
    $('.limiter').addClass('blur');
})
$('.modal').on('hide.bs.modal', function (e) {
    console.log('hide');
    $('.limiter').removeClass('blur');
})


function capLock(e) {
    kc = e.keyCode ? e.keyCode : e.which;
    sk = e.shiftKey ? e.shiftKey : ((kc == 16) ? true : false);
    if (((kc >= 65 && kc <= 90) && !sk) || ((kc >= 97 && kc <= 122) && sk)) $('#msgCapsLock').css('display', 'block');
    else $('#msgCapsLock').css('display', 'none');;
}

$('#Senha').on('keypress', function (e) {capLock(e)});

$('#formRecuperarSenha').validate({
    messages: {
        cpfUsuarioRecuperarSenha: { required: 'Preenchimento obrigatório' }
    },
    errorClass: 'text-danger',
    submitHandler: function (form) {   
        var cpfInformado = $("#cpfUsuarioRecuperarSenha").val();
        $.ajax({
            url: form.action,
            type: 'POST',



            //dataType: 'json',
            //contentType: 'application/json; charset=utf-8',
            data: { cpf: cpfInformado },
            //data: JSON.stringify({ cpf: cpfInformado }),
            beforeSend: function () {
                $.LoadingOverlay("show",
                    {
                        background: "rgba(176, 177, 178, 0.5)",
                        text: "Enviando requisição..."
                    });
            },
            complete: function () {
                $.LoadingOverlay("hide");
            },
            success: function (data) {
                if (data.Code === 1) {
                    $('#modalRecuperarSenha').modal('hide');
                    swal({
                        title: data.Title,
                        text: data.Message,
                        icon: data.ClassAlert
                    });
                    form.reset();
                } else {
                    swal({
                        title: data.Title,
                        text: data.Message,
                        icon: data.ClassAlert
                    });
                }
            },
            error: function (err) {
                console.log('error: ', err);
                swal({
                    title: 'FALHA',
                    text: 'Falha ao requisitar a alteração de senha',
                    icon: 'error'
                });
            }
        });
        return false;
    }
});