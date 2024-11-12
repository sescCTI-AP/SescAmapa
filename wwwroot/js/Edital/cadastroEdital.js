'use strict';
var botao = document.getElementById('btnCadastra');

var $validator = $("#formCreateEdital").validate({
    messages: {
        NumeroEdital: 'Preenchimento obrigatório',
        DataReuniao: 'Preenchimento obrigatório',
        Descricao: 'Preenchimento obrigatório',
    }
});

function validationChecking() {
    const $valid = $('#formCreateEdital').valid();
    if (!$valid) {
        $validator.focusInvalid();
        return false;
    }
}

const areasSelecionadas = [];
$('input[name=checkboxArea]').change(function () {
    if ($(this)[0].checked) {
        var area = $(this).val();
        areasSelecionadas.push({ Id: parseInt(area) });
    } else {
        const index = areasSelecionadas.indexOf($(this).val());
        areasSelecionadas.splice(index, 1);
    }
    document.querySelector('#idAreasSelecionadas').value = areasSelecionadas;
});

const cidadesSelecionadas = [];
$('input[name=checkboxCidade]').change(function () {
    if ($(this)[0].checked) {
        var cidade = $(this).val();
        cidadesSelecionadas.push({ Id: parseInt(cidade) });
    } else {
        const index = cidadesSelecionadas.indexOf($(this).val());
        cidadesSelecionadas.splice(index, 1);
    }
    document.querySelector('#idCidadesSelecionadas').value = cidadesSelecionadas;
});

$('input[name=checkboxExperiencia]').change(function () {
    console.log(document.getElementById('hasDocExperiencia').checked);

});


const cargo = [];
function addCargo() {
    var nomeCargo = $('#NomeCargo').val();
    var idCidade = $('#Cidade').val();
    var valor = $('#Valor').val();
    var projeto = $('#Projeto').val();
    var cidade = $('#Cidade :selected').text();
    var quantidade = $('#VagasCurriculo').val();
    var docExperiencia = document.getElementById('hasDocExperiencia').checked;
    var docFormacao = document.getElementById('hasDocFormacao').checked;
    if (nomeCargo !== '' && quantidade !== '') {
        cargo.push({
            Nome: nomeCargo,
            VagasCurriculo: quantidade,
            IdCidade: idCidade,
            Valor: valor,
            Projeto: projeto,
            HasDocExperiencia: docExperiencia,
            HasDocFormacao: docFormacao
        });

        var divNova = document.createElement("div");
        var conteudoNovo = document.createTextNode(nomeCargo + " | " + cidade + " | Limite de " + quantidade + " currículos" + " | " + valor + " | " + projeto + " | " + " | Doc. Experiência: " + docExperiencia + " | Doc. Formação: " + docFormacao);
        divNova.appendChild(conteudoNovo); //adiciona o nó de texto à nova div criada

        // adiciona o novo elemento criado e seu conteúdo ao DOM
        var divAtual = document.getElementById("div-cargos");
        divAtual.parentNode.insertBefore(divNova, divAtual);

        document.querySelector('#cargos').value = cargos;

        $('#NomeCargo').val('');
        $('#VagasCurriculo').val('');
        $('#Projeto').val('');
        $('#Valor').val('');
        document.getElementById('hasDocFormacao').checked = false;
        document.getElementById('hasDocExperiencia').checked = false;
    } else {
        noty({
            text: 'Informe o nome do cargo e a quantidade de curriculos',
            type: warning,
            timeout: 2500,
            progressBar: true
        });
    }
}

$('#formCreateEdital').submit(function (event) {
    event.preventDefault();
    const $valid = $('#formCreateEdital').valid();
    if ($valid) {
        let processo = {
            NumeroEdital: $('#NumeroEdital').val(),
         //   Cargo: $('#cargos').val(),
            //   Curso: $('#Curso').val(),
            Objeto: $('#Objeto').val(),
            Descricao: $('#Descricao').val(),
            IdTipoEdital: $('#IdTipoEdital').val(),
            IdModalidade: $('#IdModalidade').val(),
            IdStatusEdital: $('#IdStatusEdital').val(),
            DataReuniao: moment($('#DataReuniao').val(), 'DD/MM/YYYY').format('YYYY-MM-DD'),
            DataAberturaCurriculo: moment($('#DataAberturaCurriculo').val(), 'DD/MM/YYYY').format('YYYY-MM-DD'),
            DataFimCurriculo: moment($('#DataFimCurriculo').val(), 'DD/MM/YYYY').format('YYYY-MM-DD'),
        };

        console.log('processo' + JSON.stringify(processo));
        let dadosRequisicao = {
            Processo: processo,
            Areas: areasSelecionadas,
            Cidades: cidadesSelecionadas,
            Cargos: cargo
        };


        $.ajax({
            url: $('#formCreateEdital').attr('action'),
            type: 'POST',
            contentType: 'application/json', // Alterado para indicar que estamos enviando JSON
            data: JSON.stringify(dadosRequisicao), // Convertendo os dados para JSON
            beforeSend: function () {
                $(window).on('beforeunload', function () {
                    return "Se você atualizar a Página os dados serão perdidos";
                });
                botao.setAttribute('disabled', true);
                botao.style.backgroundColor = 'gray';
                botao.style.border = 'transparent';
                botao.innerHTML = '<img src="/images/static/loading.gif" height="25" alt="Carregando...">';
            },
            complete: function () {
                $(window).off('beforeunload');
                $.LoadingOverlay("hide");
            },
            success: function (data) {
                if (data.code == 1) {
                    iziToast.success({
                        title: 'SUCESSO!',
                        message: data.message,
                        theme: 'light',
                        position: 'topRight',
                    });
                    setTimeout(() => {
                        location.href = $('#urlDashboardClienteGeral').val();
                    }, 1500);
                } else {
                    botao.removeAttribute('disabled');
                    botao.style.backgroundColor = '';
                    botao.style.border = '';
                    botao.innerHTML = 'Salvar';

                    iziToast.warning({
                        title: 'Ops!',
                        message: data.message,
                        theme: 'light',
                        position: 'topRight',
                    });
                }
            },
            error: function (err) {
                botao.removeAttribute('disabled');
                botao.style.backgroundColor = '';
                botao.style.border = '';
                botao.innerHTML = 'Salvar';

                iziToast.error({
                    title: 'FALHA',
                    message: 'Falha ao enviar currículo, revise seus dados e tente novamente.',
                    theme: 'light',
                    position: 'topRight',
                });
            }
        });
    }
});
