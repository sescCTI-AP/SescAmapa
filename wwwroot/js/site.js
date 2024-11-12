document.addEventListener('DOMContentLoaded', function () {
    let dropAreaFile = document.getElementById('drop-area-curriculo');

    if (!dropAreaFile) {
        return;
    }

    ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
        dropAreaFile.addEventListener(eventName, preventDefaults, false);
    });

    function preventDefaults(e) {
        e.preventDefault();
        e.stopPropagation();
    }

    ['dragenter', 'dragover'].forEach(eventName => {
        dropAreaFile.addEventListener(eventName, highlight, false);
    });

    ['dragleave', 'drop'].forEach(eventName => {
        dropAreaFile.addEventListener(eventName, unhighlight, false);
    });

    function highlight(e) {
        dropAreaFile.classList.add('highlight');
    }

    function unhighlight(e) {
        dropAreaFile.classList.remove('highlight');
    }

    dropAreaFile.addEventListener('drop', handleDrop, false);


});
    function handleDrop(e) {
        console.log(e);
        let dt = e.dataTransfer;
        let files = dt.files;

        handleFiles(files, 'd');
    }

    function handleFiles(files, tipo) {
        let fileList = document.getElementById('file-' + tipo);

        if (files.length > 0) {
            // Atualiza o texto para o nome do primeiro arquivo
            fileList.innerHTML = files[0].name;
        } else {
            // Caso nenhum arquivo seja selecionado, reverter para o texto original
            fileList.innerHTML = 'Arraste e solte arquivos aqui ou clique para selecionar arquivos';
        }
        ([...files]).forEach(uploadFile);
    }

    function uploadFile(file) {
        // Aqui você pode adicionar a lógica para manipular o arquivo,
        // como mostrá-lo na página ou enviá-lo a um servidor.
        console.log(file);
    }


// Mantenha `selectFile` fora do escopo de `DOMContentLoaded` para ser acessível globalmente
function selectFile(element) {
    document.querySelector('#' + element + '-file').click();
}
