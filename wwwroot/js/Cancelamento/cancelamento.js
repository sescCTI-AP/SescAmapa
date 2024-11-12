$('#multiSelectTurmas').hide();
let turmasSelecionadas = [];
let listaTurmas = [];
let atendente = 0;
let botao = document.querySelector('#botaoIr');

function getTurmas() {
    var iduop = $('#idUop').val();
    let divTurma = document.querySelector('#divTurmas');

    $("#load").html("<img src='/images/loading.gif' width=70>");
    $.ajax({
        url: '/admin/cancelamento/get-turmas/' + '?idUop=' + iduop + '&idAtendente=' + atendente,
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            $("#botaoIr").css("display", "block");
            if (data != null) {
                divTurma.innerHTML = '';

                let select = document.createElement("select");
                select.setAttribute('id', 'my-select');
                select.setAttribute('multiple', '');
                select.setAttribute('name', 'my-select[]');

                let optionDefault = document.createElement("option");
                optionDefault.setAttribute('value', '');
                optionDefault.setAttribute('able', '');
                select.appendChild(optionDefault);

                data.forEach(turma => {
                    let option = document.createElement("option");
                    option.setAttribute('value', turma.cdelement);
                    option.appendChild(document.createTextNode(`${turma.descricao}`));
                    select.appendChild(option);
                });

                let labelElement = document.createElement("label");
                labelElement.setAttribute("for", "turmaSelect");
                labelElement.appendChild(document.createTextNode("Turma"));
                divTurma.appendChild(labelElement);
                divTurma.appendChild(select);
                $('#my-select').multiSelect({
                    afterSelect: function (cdelement) {
                        turmasSelecionadas.push(cdelement[0]);
                    },
                    afterDeselect: function (cdelement) {
                        turmasSelecionadas.pop(cdelement[0]);
                    },
                });
                $('.ms-selectable ul').addClass("selectpicker");
                $('.ms-selectable ul').attr("data-live-search", "true");
                listaTurmas = turmasSelecionadas;
                select.addEventListener('change', () => {
                    $('#btnIr').css("display", "block");
                });
                $('#multiSelectTurmas').show();

            } else {
                divTurma.innerHTML = '';
            }
        }
    }).done(function () {
        $("#load").html("");
    });
}

function GetProgramas(idAtendente) {
    $("#table-programas tbody").empty();
    $.ajax({
        url: '/admin/cancelamento/get-programas-atendente' + '?idAtendente=' + idAtendente,
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            if (data != null) {
                if (data.code == 1) {
                    var results = data.atividade;                   
                    var cols = "";
                    if (results.length > 0) {
                        for (var i = 0; i < results.length; i++) {
                            let cdelement = results[i].cdprograma.toString().padStart(8, '0') + results[i].cdconfig.toString().padStart(8, '0') + results[i].sqocorrenc.toString().padStart(8, '0');
                            cols += "<tr>";
                            cols += "<td class='p-1'>" + results[i].dsusuario + "</td>";
                            cols += "<td class='p-1'> <a class=btn-rm-pg onclick=RemovePrograma('" + cdelement + "');> Remover </a> </td>";
                            cols += "</tr>";
                        }
                    }
                    $("#table-programas tbody").append(cols);
                }
            } else {
                $("#table-programas tbody").innerHTML = '';
            }
        }
    }).done(function () {

    });
}

$("#input-busca").keyup(function () {
    var user = $('#input-busca').val();
    if (user != '') {
        $('#lista-usuarios li').remove();
        $.ajax({
            url: '/admin/usuarios/buscar-usuario',
            type: 'get',
            data: { user: user },
            success: function (data) {
                console.log(data);
                if (data.code === 1) {
                    $('#validacao-busca').text('');
                    $.each(data.lista, function (index, value) {
                        $('#lista-usuarios').append('<li>' + value.nome + '<a class="badge-primrary ml-2" onclick="addVisualizador(' + value.id + ');">Adicionar</a></li>');
                    });
                }
                if (data.code === 2) {
                    $('#validacao-busca').text('Não foi encontrado nenhum usuário');
                }
            },
            error: function (err) {
                console.log("erro");
            }
        });
    } else {
        $('#validacao-busca').text('Informe um nome');
    }
});

function showModal(idAtendente) {
    $('#idUop').val('');
    $('#divTurmas').empty();
    atendente = idAtendente;
    $("#table-programas tbody").empty();
    GetProgramas(idAtendente);
    $('#modalPrograma').modal('show');
}

function addVisualizador(id) {
    $.ajax({
        url: '/admin/cancelamento/add-atendente',
        type: 'post',
        data: { idUsuario: id },
        beforeSend: function () {
            $.LoadingOverlay("show",
                {
                    background: "rgba(176, 177, 178, 0.5)",
                    text: "Aguarde..."
                });
        },
        complete: function () {
            $.LoadingOverlay("hide");
        },
        success: function (data) {
            if (data.code == 1) {
                $('#addVisualizador').modal('hide');
                $('#lista-colaboradres').load(location.href + ' #lista-colaboradres>*',
                    function () {
                        $('[data-toggle="tooltip"]').tooltip();
                    });
            } else if (data.code == 2) {
                swal({
                    title: 'ATENÇÃO',
                    text: 'Usuário já adicionado!',
                    icon: 'warning',
                    timer: 2000
                });
            } else {
                swal({
                    title: 'FALHA',
                    text: 'Usuário não encontrado!',
                    icon: 'error',
                    timer: 2000
                });
            }
        },
        error: function (err) {
            swal({
                title: 'FALHA',
                text: 'Falha ao remover usuário!',
                icon: 'error',
                timer: 2000
            });
        }
    });
}

function removerVisualizador(id) {
    swal({
        title: "EXCLUIR ATENDENTE",
        text: `Você realmente deseja remover esse atendente?`,
        icon: "warning",
        dangerMode: true,
        buttons: ["Não", "Sim"]
    })
        .then((value) => {
            if (value) {
                $.ajax({
                    url: '@Url.Action("RemoverAtendente")',
                    type: 'post',
                    data: { idUsuario: id },
                    beforeSend: function () {
                        $.LoadingOverlay("show",
                            {
                                background: "rgba(176, 177, 178, 0.5)",
                                text: "Aguarde..."
                            });
                    },
                    complete: function () {
                        $.LoadingOverlay("hide");
                    },
                    success: function (data) {
                        if (data.code == 1) {
                            $('#table-colaboradores').load(location.href + ' #table-colaboradores>*',
                                function () {
                                    $('[data-toggle="tooltip"]').tooltip();
                                });
                        } else if (data.code == 2) {
                            swal({
                                title: 'ATENÇÃO',
                                text: 'Registro não encontrado!',
                                icon: 'warning',
                                timer: 2000
                            });
                        } else {
                            swal({
                                title: 'FALHA',
                                text: 'Usuário não encontrado!',
                                icon: 'error',
                                timer: 2000
                            });
                        }
                    },
                    error: function (err) {
                        swal({
                            title: 'FALHA',
                            text: 'Falha ao remover atendente!',
                            icon: 'error',
                            timer: 2000
                        });
                    }
                });
            }
        });
}

function RemovePrograma(cdelement) {
    $.ajax({
        url: '/admin/cancelamento/remove-programa/' + '?idAtendente=' + atendente + "&cdelement=" + cdelement,
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            console.log(data);
            if (data.code == 1) {
                GetProgramas(atendente);
            } else {

            }
        }
    }).done(function () {

    });
}

function AddPrograma() {
    var turmas = document.querySelector('#my-select');
    var lista = JSON.stringify(turmasSelecionadas);

    let comp = {
        IdAtendente: atendente,
        ListaCdelement: turmasSelecionadas
    };
    $.ajax({
        url: '/admin/cancelamento/add-programa-atendente',
        type: 'POST',
        data: JSON.stringify(comp),
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            $("#botaoIr").css("display", "block");
            if (data.code == 1) {
                turmasSelecionadas = [];
                let divTurma = document.querySelector('#divTurmas');
                divTurma.innerHTML = '';
                $('#idUop').val('');
                GetProgramas(atendente);
            }
        }
    });
}
