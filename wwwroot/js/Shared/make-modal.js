//dialogClass => Add classes ao modal-dialog
//classeCor => Add a mesma cor de fundo ao header, body e footer do modal
//titulo => add html dentro da div header do modal
//corpo => add html dentro da div body do modal
//btnFecharTexto => add texto ao botão de fechar modal, se string vazia ('') então remove botão de fechar e desativa fechamento usando foco fora
function makeModal(dialogClass, classeCor, titulo, corpo, btnFecharTexto) {
    var displayFooterNone = "";
    if (btnFecharTexto == '') { displayFooterNone = 'd-none'; $('#modalGeral').modal({ backdrop: 'static', keyboard: false }) }

    var modalDialog = `<div class="modal-dialog ${dialogClass}" role="document"></div>`;
    var modalContent = `<div class= "modal-content" style = "opacity:.95!important"></div>`;
    var modalHeader = `<div class="modal-header ${classeCor}"><h4 class="text-center col">${titulo}</h4></div>`;
    var modalBody = `<div class="modal-body ${classeCor}">${corpo}</div>`;
    var modalFooter = `<div class="${displayFooterNone} modal-footer ${classeCor}"> <button id="modal-close-button" type="button" class="btn btn-secondary" data-dismiss="modal">${btnFecharTexto}</button> </div>`;


    $('#modalGeral').append(modalDialog);
    $('.modal-dialog').append(modalContent);
    $('.modal-content').append(modalHeader).addClass('bg-white');
    $('.modal-header').after(modalBody);
    $('.modal-body').after(modalFooter);
    $("#modalGeral").modal("show");

}

function destroyModalDialog() {
    $('#modalGeral').children().remove();
}

function hideModalGeral() {

    $('#modalGeral').modal('hide');

    //$('body').removeAttr("style").removeClass('modal-open');
    //$("#modalGeral").removeAttr("style").removeClass('show');
    //$(".modal-backdrop").remove();

}


$('#modalGeral').on('hidden.bs.modal', function (e) {
    $(this).children().remove();
})

//modal de loading
function loadingModal(mensagem) {
    makeModal('modal-dialog-centered',
        '',
        '',
        `<h4 class="pb-5 pl-3 text-center">${mensagem}<img src="/img/preloader.gif" alt="loading" height="30" /></h4>`,
        '');
}


