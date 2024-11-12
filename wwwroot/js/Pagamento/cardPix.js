function geraPix(idForm) {
    var loadingImg = document.createElement('img');
    loadingImg.src = '/images/loading.gif';
    loadingImg.alt = 'Carregando...';
    loadingImg.id = 'loadingImg';

    var loadingDiv = document.createElement('div');
    loadingDiv.id = 'loading';
    loadingDiv.style.display = 'flex'; // Inicialmente oculto

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
            if (!response.ok) {
                throw new Error('Erro ao carregar a PartialView.');
            }
            return response.text();
        })
        .then(data => {
            document.getElementById('modalContainer').innerHTML = data;
            $('#modal-recarga').modal('hide');
            var modal = document.getElementById('modal-pix');
            modal.classList.add('show');
            modal.style.display = 'block';
            setTimeout(function () {
                document.getElementById('loading').style.display = 'none';
            }, 1000);

        })
        .catch(error => {
             console.error('Erro:', error);
            document.getElementById('loading').style.display = 'none';
        });
    //document.getElementById('loading').style.display = 'none';
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