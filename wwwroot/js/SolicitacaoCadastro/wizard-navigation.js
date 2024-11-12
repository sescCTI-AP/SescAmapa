function validateAndMoveNext(currentPageId, nextPageId) {
    var fields = document.querySelectorAll('#' + currentPageId + ' input, #' + currentPageId + ' select');
    var allValid = true;

    Array.prototype.forEach.call(fields, function (field) {
        // Remove qualquer mensagem de erro.
        var errorMessage = field.parentNode.querySelector('.error-message');
        if (errorMessage) {
            errorMessage.remove();
        }

        // Verifica validade do campo.
        if (!field.checkValidity()) {
            allValid = false;

            // cria span para mensagem de erro.
            var span = document.createElement('span');
            span.className = 'error-message';
            span.textContent = 'Preenchimento obrigatório';
            field.parentNode.appendChild(span);
        }

        // Adiciona um event listener para remover mensagens de erro caso comece a digitar.
        field.addEventListener('input', function () {
            var errorMessage = field.parentNode.querySelector('.error-message');
            if (errorMessage) {
                errorMessage.remove();
            }
        });
    });

    if (allValid) {
        document.getElementById(currentPageId).style.display = 'none';
        document.getElementById(nextPageId).style.display = 'block';
    }
}






function navigation(num) {
    // Lista de ids das divs
    var ids = ['page-w-1', 'page-w-2', 'page-w-3', 'page-w-4'];

    // Percorre a lista de ids
    for (var i = 0; i < ids.length; i++) {
        var div = document.getElementById(ids[i]);

        // Se o id atual é igual ao número passado, seta o display para block
        if (i === num - 1) {
            div.style.display = 'block';
        } else { // Senão, seta o display para none
            div.style.display = 'none';
        }
    }
}

class magicFocus {
    constructor(parent) {
        if (!parent) return;

        this.parent = parent;
        this.focus = document.createElement('div');
        this.focus.classList.add('magic-focus');
        this.parent.classList.add('has-magic-focus');
        this.parent.appendChild(this.focus);

        let inputs = this.parent.querySelectorAll('input, textarea, select');
        for (let input of inputs) {
            input.addEventListener('focus', () => window.magicFocus.show());
            input.addEventListener('blur', () => window.magicFocus.hide());
        }
    }

    show() {
        let el = document.activeElement;
        if (!['INPUT', 'SELECT', 'TEXTAREA'].includes(el.nodeName)) return;

        clearTimeout(this.reset);

        if (['checkbox', 'radio'].includes(el.type)) {
            el = document.querySelector(`[for=${el.id}]`);
        }

        this.focus.style.top = `${el.offsetTop || 0}px`;
        this.focus.style.left = `${el.offsetLeft || 0}px`;
        this.focus.style.width = `${el.offsetWidth || 0}px`;
        this.focus.style.height = `${el.offsetHeight || 0}px`;
    }

    hide() {
        let el = document.activeElement;
        if (!['INPUT', 'SELECT', 'TEXTAREA', 'LABEL'].includes(el.nodeName)) {
            this.focus.style.width = 0;
        }

        this.reset = setTimeout(() => {
            window.magicFocus.focus.removeAttribute('style');
        }, 200);
    }
}

// initialize
window.magicFocus = new magicFocus(document.querySelector('.form'));

$(function () {
    $('.select').customSelect();
});
