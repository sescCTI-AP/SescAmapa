'use strict';

$('#formNovoAgendamnto, #formEditarAgendamnto').submit(function (e) {
    e.preventDefault();
});

$('#formNovoAgendamnto').validate({
    messages: {
        telefone: 'Preenchimento obrigatório',
        IdUnidadeOperacional: 'Preenchimento obrigatório',
        selectProcessoAgendamento: 'Preenchimento obrigatório',
        selectTurmaAgendamento: 'Preenchimeno obrigatório',
        selectHorario: 'Preenchimeno obrigatório'
    },
    submitHandler: function (form) {
        let statusCadastro = Number($('#statusCadastro').val());
        const agendamento = {
            CpfDependente: $('#idClienteSelecionado').val(),
            //IdClienteDependente: Number($('#idClienteSelecionado').val()),
            Telefone: telefone.value,
            IdProcessoAgendamentoEscolar: Number(selectProcessoAgendamento.value),
            IdGrupoTurmaEscolar: Number(selectTurmaAgendamento.value)
        };

        if (statusCadastro === 1) {
            let selectHorario = $('#selectHorario');
            if (selectHorario.val() != '' && selectHorario.val() != undefined) {
                agendamento.DataAgendadaMatricula = selectHorario.val();
                cadastrarAgendamento(agendamento, statusCadastro);
            } else {
                alert('Selecione uma data');
            }
        } else {
            cadastrarAgendamento(agendamento, statusCadastro);
        }
        return false;
    }
});

$('#formEditarAgendamnto').validate({
    messages: {
        selectProcessoAgendamentoEditar: 'Preenchimento obrigatório',
        selectTurmaAgendamentoEditar: 'Preenchimeno obrigatório',
        selectHorarioEditar: 'Preenchimeno obrigatório'
    },
    submitHandler: function (form) {
        let statusCadastro = Number($('#statusCadastroEditar').val());
        const agendamento = {
            Id: Number($('#idAgendamentoEditar').val()),
            CpfDependente: $('#idClienteSelecionado').val(),
            IdProcessoAgendamentoEscolar: Number(selectProcessoAgendamentoEditar.value),
            IdGrupoTurmaEscolar: Number(selectTurmaAgendamentoEditar.value)
        };

        if (statusCadastro === 1) {
            let selectHorario = $('#selectHorarioEditar');
            if (selectHorario.val() != '' && selectHorario.val() != undefined) {
                agendamento.DataAgendadaMatricula = selectHorario.val();
                cadastrarAgendamento(agendamento, statusCadastro);
            } else {
                alert('Selecione uma data');
            }
        } else {
            cadastrarAgendamento(agendamento, statusCadastro);
        }
        return false;
    }
});

$('#IdUnidadeOperacional').change(function () {
    const idUnidadeSelecionada = $(this).val();
    if (idUnidadeSelecionada != '' && idUnidadeSelecionada != undefined) {
        getProcessos(idUnidadeSelecionada, '#divProcessos');
        $('#divDadosDataHora, #divDadosDataHoraEditar').css("display", "none");
        $('#divTurmas, #divTurmasEditar').html('');
        $('#notificacao, #notificacaoEditar').removeClass('alert alert-warning text-center');
        $('#notificacao, #notificacaoEditar').html('');
    } else {
        $('#divProcessos, #divProcessosEditar').html('');
        $('#divTurmas, #divTurmasEditar').html('');
        $('#notificacao, #notificacaoEditar').removeClass('alert alert-warning text-center');
        $('#notificacao,  #notificacaoEditar').html('');
        $('#divDadosDataHora, #divDadosDataHoraEditar').css("display", "none");
        $('#divHorarios, #divHorarios').html('');
    }
});

function getAgendamento(id) {
    $.ajax({
        url: $("#urlGetAgendamento").val(),
        type: 'Post',
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(Number(id)),
        beforeSend: function () {
            $.LoadingOverlay("show",
                {
                    background: "rgba(176, 177, 178, 0.5)",
                    text: "Processando..."
                });
        },
        complete: function () {
            $.LoadingOverlay("hide");
        },
        success: function (data) {
            console.log(data);
            if (data.code === 1) {
                let { Agendamento } = data.data;

                $('#idAgendamentoEditar').val(id);
                $('#telefoneEditar').val(Agendamento.telefone);
                $('#unidadeEditar').val(Agendamento.unidade.nome);

                getProcessos(Agendamento.unidade.id, '#divProcessosEditar', Agendamento.idProcessoAgendamentoEscolar);
                getTurmas(Agendamento.idProcessoAgendamentoEscolar, '#divTurmasEditar', Agendamento.idGrupoTurmaEscolar);

                $('#modalEditarAgendamento').modal('show');
            } else {
                alert(data.message);
            }
        },
        error: function (err) {
            console.log(err);
            noty({
                text: 'Falha buscar agendamento',
                type: 'error',
                timeout: 2500,
                progressBar: true
            });
        }
    });
}

function getProcessos(id, idComponente, idProcessoSelecionado = null) {
    let divProcesso = document.querySelector(idComponente);

    $.ajax({
        url: $("#urlGetProcessos").val(),
        type: 'Post',
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(Number(id)),
        beforeSend: function () {
            $.LoadingOverlay("show",
                {
                    background: "rgba(176, 177, 178, 0.5)",
                    text: "Processando..."
                });
        },
        complete: function () {
            $.LoadingOverlay("hide");
        },
        success: function (data) {
            if (data.code === 1) {
                console.log(data);
                if (data.data.processos.length > 0) {
                    divProcesso.innerHTML = '';

                    let select = document.createElement("select");
                    select.setAttribute('id', idProcessoSelecionado === null ? 'selectProcessoAgendamento' : 'selectProcessoAgendamentoEditar');
                    select.setAttribute('name', idProcessoSelecionado === null ? 'selectProcessoAgendamento' : 'selectProcessoAgendamentoEditar');
                    select.setAttribute('class', 'form-control input-login');
                    select.setAttribute('required', '');

                    select.addEventListener('change',
                        (event) => {
                            //console.log(event.target)
                            let processoSelecionado = event.target;
                            console.log(processoSelecionado);
                            console.log('categoria: ' + $('#idCategoriaClienteSelecionado').val());
                            if (processoSelecionado.value != '' && processoSelecionado.value != undefined) {

                                VerificaDatasAberturaProcessoAgendamentoEscolar(processoSelecionado.value).then(function (result) {
                                    if (result) {
                                        let categoriasPermitidasAgendamento = processoSelecionado.options[processoSelecionado.selectedIndex]
                                            .getAttribute('categorias-permitidas').split(',');

                                        //console.log("c => " + categoriasPermitidasAgendamento);
                                        if (categoriasPermitidasAgendamento.includes($('#idCategoriaClienteSelecionado').val())) {
                                            if (idComponente.includes('Editar')) {
                                                getTurmas(processoSelecionado.value, '#divTurmasEditar');
                                            } else {
                                                getTurmas(processoSelecionado.value, '#divTurmas');
                                            }
                                        } else {
                                            $('#notificacao, #notificacaoEditar').addClass('alert alert-warning text-center');
                                            $('#notificacao, #notificacaoEditar').html('O Processo selecionado não permite Agendamento para a Categoria do Cliente');
                                        }
                                    } else {
                                        $('#notificacao, #notificacaoEditar').addClass('alert alert-warning text-center');
                                        $('#notificacao, #notificacaoEditar').html('Não há Agendamento disponível para o Processo selecionado');
                                    }
                                });
                            } else {
                                $('#notificacao, #notificacaoEditar').removeClass('alert alert-warning text-center');
                                $('#notificacao, #notificacaoEditar').html('');
                                $('#divTurmas, #divTurmasEditar').html('');
                                $('#divDadosDataHora, #divDadosDataHoraEditar').css("display", "none");
                                $('#divHorarios, #divHorariosEditar').html('');
                            }
                        });

                    let optionDefault = document.createElement("option");
                    optionDefault.setAttribute('value', '');
                    optionDefault.setAttribute('able', '');
                    optionDefault.appendChild(document.createTextNode('Selecione o processo'));
                    select.appendChild(optionDefault);

                    data.data.processos.forEach(processo => {
                        let option = document.createElement("option");
                        option.setAttribute('value', processo.id);
                        option.setAttribute('inicio-atendimento', processo.inicioAtendimento);
                        option.setAttribute('fim-atendimento', processo.fimAtendimento);
                        option.setAttribute('categorias-permitidas', processo.categoriasPermitidasAgendamento);
                        option.appendChild(document.createTextNode(`${processo.descricao}`));
                        if (idProcessoSelecionado !== null && processo.id == idProcessoSelecionado) {
                            option.selected = true;
                        }
                        select.appendChild(option);
                    });

                    let labelElement = document.createElement("label");
                    labelElement.setAttribute("for", "processoSelect");
                    labelElement.appendChild(document.createTextNode("PROCESSO"));
                    divProcesso.appendChild(labelElement);
                    divProcesso.appendChild(select);
                } else {
                    divProcesso.innerHTML = '';
                    noty({
                        text: 'Nenhum Processo de Agendamento aberto no momento',
                        type: 'warning',
                        timeout: 2500,
                        progressBar: true
                    });
                }
            }
        },
        error: function (err) {
            noty({
                text: 'Falha ao validar o pré-cadastro do cliente',
                type: 'error',
                timeout: 2500,
                progressBar: true
            });
        }
    });
}

function getTurmas(id, idComponente, idTurmaSelecionada = null) {
    let divTurma = document.querySelector(idComponente);
    let isEditar = idComponente.includes('Editar');

    $.ajax({
        url: $("#urlGetTurmas").val(),
        type: 'Post',
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(Number(id)),
        beforeSend: function () {
            $.LoadingOverlay("show",
                {
                    background: "rgba(176, 177, 178, 0.5)",
                    text: "Processando..."
                });
        },
        complete: function () {
            $.LoadingOverlay("hide");
        },
        success: function (data) {
            if (data.length > 0) {
                divTurma.innerHTML = '';
                let select = document.createElement("select");
                select.setAttribute('id', isEditar ? 'selectTurmaAgendamentoEditar' : 'selectTurmaAgendamento');
                select.setAttribute('name', isEditar ? 'selectTurmaAgendamentoEditar' : 'selectTurmaAgendamento');
                select.setAttribute('class', 'form-control input-login');
                select.setAttribute('single', '');
                select.setAttribute('required', '');

                let optionDefault = document.createElement("option");
                optionDefault.setAttribute('value', '');
                optionDefault.appendChild(document.createTextNode('Selecione a Turma'));
                select.appendChild(optionDefault);

                if (idTurmaSelecionada !== null) {
                    verificaVagasTurmaSelecionada(idTurmaSelecionada, '#horariosEditar', true);
                }

                select.addEventListener('change',
                    () => {
                        let turmaSelecionada = event.target.value;
                        //let turmaSelecionada = event.path[0].value;
                        if (turmaSelecionada != '' && turmaSelecionada != undefined) {
                            verificaVagasTurmaSelecionada(turmaSelecionada, isEditar ? '#horariosEditar' : '#horarios', isEditar);
                        } else {
                            $('#notificacao, #notificacaoEditar').removeClass('alert alert-warning text-center');
                            $('#notificacao, #notificacaoEditar').html('');
                            $('#divDadosDataHora, #divDadosDataHoraEditar').css("display", "none");
                            $('#divHorarios, #divHorariosEditar').html('');
                        }
                    });

                data.forEach(turma => {
                    let option = document.createElement("option");
                    option.setAttribute('value', turma.id);
                    option.appendChild(document.createTextNode(`${turma.descricao}`));
                    if (idTurmaSelecionada !== null && turma.id == idTurmaSelecionada) {
                        option.selected = true;
                    }
                    select.appendChild(option);
                });

                let labelElement = document.createElement("label");
                labelElement.setAttribute("for", "turmasSelect");
                labelElement.appendChild(document.createTextNode("GRUPO/TURMA"));
                divTurma.appendChild(labelElement);
                divTurma.appendChild(select);
            } else {
                divTurma.innerHTML = '';
            }
        },
        error: function (err) {
            noty({
                text: 'Falha ao validar o pré-cadastro do cliente',
                type: 'error',
                timeout: 2500,
                progressBar: true
            });
        }
    });







}

function getCalendario(idComponente, isEditar) {
    $(idComponente).html('');
    var id = $(isEditar ? '#selectProcessoAgendamentoEditar' : '#selectProcessoAgendamento').val();
    $.ajax({
        url: $("#urlGetDatasIndisponiveisAtendimento").val(),
        type: 'Post',
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(Number(id)),
        beforeSend: function () {
            $.LoadingOverlay("show",
                {
                    background: "rgba(176, 177, 178, 0.5)",
                    text: "Processando..."
                });
        },
        complete: function () {
            $.LoadingOverlay("hide");
        },
        success: function (response) {
            if (response.code === 1) {
                let dataInicioAtendimento =
                    stringToDate($(isEditar ? '#selectProcessoAgendamentoEditar option:selected' : '#selectProcessoAgendamento option:selected').attr('inicio-atendimento'));
                let dataMaximaAtendimentoProcesso =
                    stringToDate($(isEditar ? '#selectProcessoAgendamentoEditar option:selected' : '#selectProcessoAgendamento option:selected').attr('fim-atendimento'));
                let dataAtual = new Date();


                if (response.data.datasInvalidas.length > 0) {
                    let datasInvalidas = response.data.datasInvalidas.map(data => stringToDate(data));

                    console.log("datas invalidas: " + datasInvalidas);
                    $(isEditar ? '#filtroDataEditar' : '#filtroData').datetimepicker({
                        format: 'L',
                        inline: true,
                        sideBySide: true,
                        minDate: dataInicioAtendimento > dataAtual ? dataInicioAtendimento : dataAtual,
                        maxDate: dataMaximaAtendimentoProcesso,
                        disabledDates: datasInvalidas
                    });
                } else {
                    $(isEditar ? '#filtroDataEditar' : '#filtroData').datetimepicker({
                        format: 'L',
                        inline: true,
                        sideBySide: true,
                        minDate: dataInicioAtendimento > dataAtual ? dataInicioAtendimento : dataAtual,
                        maxDate: dataMaximaAtendimentoProcesso
                    });
                }

                const idProcessoSelecionado = $(isEditar ? '#selectProcessoAgendamentoEditar' : '#selectProcessoAgendamento').val();
                getHorarios(idProcessoSelecionado, dataInicioAtendimento > dataAtual ? dataInicioAtendimento : dataAtual, isEditar);
                $(isEditar ? '#filtroDataEditar' : '#filtroData').on("change.datetimepicker",
                    function (e) {
                        const dataSelecionada =
                            $(isEditar ? '#filtroDataEditar' : '#filtroData').data('datetimepicker').date().format('DD/MM/YYYY');
                        getHorarios(idProcessoSelecionado, dataSelecionada, isEditar);
                    });
                $(isEditar ? '#divDadosDataHoraEditar' : '#divDadosDataHora').css("display", "block");
            } else {
                alert(response.message);
            }
        },
        error: function (err) {
            noty({
                text: 'Falha ao verificar datas de agendamento.',
                type: 'error',
                timeout: 2500,
                progressBar: true
            });
        }
    });
}

function getHorarios(id, dataSelect, isEditar = false) {

    let divHorarios = document.querySelector(isEditar ? '#horariosEditar' : '#horarios');
    var dataEscolhida = moment(dataSelect, 'DD/MM/YYYY').format('YYYY/MM/DD');
    console.log("data > " + dataEscolhida);
    console.log("idProcesso> " + id);
    var processoData = {
        Id: id,
        DataEscolhida: dataEscolhida
    };
    $.ajax({
        url: $("#urlGetHorarios").val(),
        type: 'Post',
        dataType: "json",
        contentType: 'application/x-www-form-urlencoded',
        data: processoData,
        //data: JSON.stringify({ processoData: processo }),
        beforeSend: function () {
            $.LoadingOverlay("show",
                {
                    background: "rgba(176, 177, 178, 0.5)",
                    text: "Processando..."
                });
        },
        complete: function () {
            $.LoadingOverlay("hide");
        },
        success: function (response) {
            if (response.code === 1) {
                if (response.data.horarios.length > 0) {
                    divHorarios.innerHTML = '';

                    let select = document.createElement("select");
                    select.setAttribute('id', isEditar ? 'selectHorarioEditar' : 'selectHorario');
                    select.setAttribute('name', isEditar ? 'selectHorarioEditar' : 'selectHorario');
                    select.setAttribute('class', 'form-control input-login');
                    select.setAttribute('required', '');

                    let optionDefault = document.createElement("option");
                    optionDefault.setAttribute('value', '');
                    optionDefault.appendChild(document.createTextNode('Selecione o Horário'));
                    select.appendChild(optionDefault);

                    response.data.horarios.forEach(hora => {


                        let option = document.createElement("option");
                        //let horaFormatada = moment(hora,'L');
                        let horaFormatada = moment(hora).format('LT');
                        //console.log("h: " + hora + " / data: " + moment(dataEscolhida, 'DD/MM/YYYY'));
                        option.setAttribute('value', `${dataEscolhida} ${hora}`);
                        //option.setAttribute('value', `${moment(dataEscolhida, 'DD/MM/YYYY').format('DD/MM/YYYY')} ${hora}`);
                        option.appendChild(document.createTextNode(`${hora}`));
                        select.appendChild(option);
                    });

                    let labelElement = document.createElement("label");
                    labelElement.setAttribute("for", isEditar ? 'horarioSelectEditar' : 'horarioSelect');
                    labelElement.appendChild(document.createTextNode("Horários disponíveis para Agendamento"));

                    divHorarios.appendChild(labelElement);
                    divHorarios.appendChild(select);
                } else {
                    divHorarios.innerHTML = '';
                }
            } else {
                divHorarios.innerHTML = '';
                alert(response.message);
            }
        },
        error: function (err) {
            noty({
                text: 'Falha ao verificar horarios de agendamento.',
                type: 'error',
                timeout: 2500,
                progressBar: true
            });
        }
    });
}

function verificaVagasTurmaSelecionada(id, idComponenteCalendario, isEditar = false) {

    $.ajax({
        url: $("#urlVerificaVagasGrupoTurma").val(),
        type: 'Post',
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(Number(id)),
        beforeSend: function () {
            $.LoadingOverlay("show",
                {
                    background: "rgba(176, 177, 178, 0.5)",
                    text: "Processando..."
                });
        },
        complete: function () {
            $.LoadingOverlay("hide");
        },
        success: function (data) {
            if (data.code === 1) {
                if (!data.data.haVaga) {
                    $('#divDadosDataHora, #divDadosDataHoraEditar').css("display", "none");
                    $('#notificacaoEditar, #notificacao').addClass('alert alert-warning text-center');
                    $('#notificacaoEditar, #notificacao')
                        .html(
                            `O Grupo/Turma selecionado não possui vagas e você entrará para a <b>Fila de Espera</b>. Se surgir uma vaga, você será notificado
                                via Email e/ou Telefone.`);
                    $(isEditar ? '#statusCadastroEditar' : '#statusCadastro').val(2);
                } else {
                    $('#notificacao, #notificacaoEditar').removeClass('alert alert-warning text-center');
                    $('#notificacao,  #notificacaoEditar').html('');
                    $(isEditar ? '#statusCadastroEditar' : '#statusCadastro').val(1);
                    getCalendario(idComponenteCalendario, isEditar);
                }
            } else {
                alert(data.message);
            }
        },
        error: function (err) {
            noty({
                text: 'Falha ao verificar vagas disponiveis..',
                type: 'error',
                timeout: 2500,
                progressBar: true
            });
        }
    });
}

function cadastrarAgendamento(agendamento, status) {
    var agendamentoSubmit = {
        Agendamento: agendamento,
        Status: status
    };
    $.ajax({
        url: $('#formNovoAgendamnto').attr('action'),
        type: 'POST',
        dataType: "json",
        contentType: 'application/x-www-form-urlencoded',
        data: agendamentoSubmit,
        //data: JSON.stringify({ agendamentoMatriculaEscolar: agendamento, status: status }),
        beforeSend: function () {
            $.LoadingOverlay("show",
                {
                    background: "rgba(176, 177, 178, 0.5)",
                    text: "Processando..."
                });
        },
        complete: function () {
            $.LoadingOverlay("hide");
        },
        success: function (data) {
            switch (data.code) {
                case 0:
                case 2:
                    swal({
                        title: 'FALHA',
                        text: data.message,
                        icon: data.classAlert
                    });
                    break;
                case 1:
                    $('#cardCliente').load(location.href + ' #cardCliente>*');
                    swal({
                        title: 'SUCESSO',
                        text: data.message,
                        icon: data.classAlert
                    });
                    $('#modalAgendamento, #modalEditarAgendamento').modal('hide');
                    window.setTimeout(() => {
                        location.reload();
                    }, 2000);
                    break;
                case 3:
                    $('#selectTurmaAgendamentoEditar, #selectTurmaAgendamento').prop('selectedIndex', 0);
                    $('#divDadosDataHora, #divDadosDataHoraEditar').css("display", "none");
                    $('#notificacaoEditar, #notificacao').addClass('alert alert-warning text-center');
                    $('#notificacaoEditar, #notificacao').html(data.message);

                    break;
            }
        },
        error: function (err) {
            swal({
                title: 'FALHA',
                text: `Falha ao realizar o cadastro. Tente novamente. Se o error persistir entre em contato com o Sesc Pará
                        através da opção CONTATO, presente no menu da página inicial do site.`,
                icon: 'erro'
            });
        }
    });
}

function VerificaDatasAberturaProcessoAgendamentoEscolar(id) {
    let result = jQuery.Deferred();

    $.ajax({
        url: $("#urlVerificaDatasAgendamento").val(),
        type: 'Post',
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(Number(id)),
        beforeSend: function () {
            $.LoadingOverlay("show",
                {
                    background: "rgba(176, 177, 178, 0.5)",
                    text: "Processando..."
                });
        },
        complete: function () {
            $.LoadingOverlay("hide");
        },
        success: function (data) {
            if (data.code === 1) {
                result.resolve(data.data.isAbertoParaAgendamento);
            } else {
                alert(data.message);
                result.reject(false);
            }
        },
        error: function (err) {
            noty({
                text: 'Falha ao verificar datas de agendamento.',
                type: 'error',
                timeout: 2500,
                progressBar: true
            });
        }
    });








    //$.LoadingOverlay("show",
    //{
    //    background: "rgba(176, 177, 178, 0.5)",
    //    text: "Processando..."
    //});
    //$.getJSON($('#urlVerificaDatasAgendamento').val() + '/' + idProcesso,
    //    function (response) {
    //        $.LoadingOverlay("hide");
    //        if (response.Code === 1) {
    //            result.resolve(response.Data.IsAbertoParaAgendamento);
    //        } else {
    //            alert(response.Message);
    //            result.reject(false);
    //        }
    //    }).fail(function (err) {
    //        result.reject(false);
    //        $.LoadingOverlay("hide");
    //        alert('Falha ao verificar se o Processo permite Agendamento');
    //    });


    return result.promise();
}