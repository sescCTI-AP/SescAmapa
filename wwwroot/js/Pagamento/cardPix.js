function geraPix(idForm) {
    var loadingImg = document.createElement('img');
    loadingImg.src = '/images/loading.gif';
    loadingImg.alt = 'Carregando...';
    loadingImg.id = 'loadingImg';

    var loadingDiv = document.createElement('div');
    loadingDiv.id = 'loading';
    loadingDiv.style.display = 'flex';

    loadingDiv.appendChild(loadingImg);
    document.body.appendChild(loadingDiv);

    var divForm = document.getElementById(idForm);
    var formData = new FormData();

    var camposInput = divForm.querySelectorAll('input');
    camposInput.forEach(function (input) {
        formData.append(input.name, input.value);
    });

    fetch('/cliente/cobrancas/gerar-pix', {
        method: 'POST',
        body: formData
    })
    .then(response => {
        const contentType = response.headers.get("content-type");

        if (contentType && contentType.includes("application/json")) {
            return response.json().then(data => {
                throw data; // Força o erro para cair no catch
            });
        }
        return response.text(); // Se for HTML (PartialView), retorna normalmente
    })
    .then(data => {
        // Garante que o loading seja removido antes de exibir a tela
        setTimeout(() => {
            var loading = document.getElementById('loading');
            if (loading) {
                loading.remove();
            }
        }, 300);

        // Exibe a PartialView (QR Code PIX)
        document.getElementById('modalContainer').innerHTML = data;
        $('#modal-recarga').modal('hide');
        var modal = document.getElementById('modal-pix');
        modal.classList.add('show');
        modal.style.display = 'block';
    })
    .catch(error => {
        // Caso seja um erro do Controller (horário inválido, por exemplo)
        if (error.message) {
            swal({
                title: "Atenção!",
                text: error.message,
                icon: "error",
                button: "OK"
            });
        } else {
            // Se for um erro inesperado
            swal({
                title: "Erro!",
                text: "Ocorreu um erro ao tentar gerar o PIX. Tente novamente.",
                icon: "error",
                button: "OK"
            });
        }
    })
    .finally(() => {
        // Força a remoção do loading em qualquer cenário após 1 segundo
        setTimeout(() => {
            var loading = document.getElementById('loading');
            if (loading) {
                loading.remove();
            }
        }, 1000);
    });
}

function closeModal() {
    var modal = document.getElementById('modal-pix');
    modal.classList.remove('show');
    modal.style.display = 'none';
}

function copiarCodigoPix() {
    console.log("Codigo copiado");
    var codigoPix = document.getElementById("codigo-pix");
    navigator.clipboard.writeText(codigoPix.value)
    .then(function () {
        swal({
            title: 'Código copiado',
            icon: 'success',
            timer: 2500
        });
    })
    .catch(function (err) {
        console.error('Erro ao copiar o código PIX: ', err);
    });
}