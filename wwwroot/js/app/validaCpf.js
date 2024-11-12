function validarCPF(cpf) {
    cpf = cpf.replace(/[^\d]+/g, ''); // Remove tudo que não é número
    if (cpf == '') return false; // CPF vazio
    // Verifica se o CPF possui 11 dígitos
    if (cpf.length != 11) return false;
    // Verifica se todos os dígitos são iguais
    if (/^(.)\1+$/.test(cpf)) return false;
    var soma = 0;
    // Calcula a soma dos nove primeiros dígitos do CPF
    for (var i = 0; i < 9; i++) {
        soma += parseInt(cpf.charAt(i)) * (10 - i);
    }
    var resto = soma % 11;
    // Verifica o primeiro dígito verificador
    if (resto == 0 || resto == 1) {
        var dv1 = 0;
    } else {
        var dv1 = 11 - resto;
    }
    // Calcula a soma dos dez primeiros dígitos do CPF
    soma = 0;
    for (var i = 0; i < 10; i++) {
        soma += parseInt(cpf.charAt(i)) * (11 - i);
    }
    resto = soma % 11;
    // Verifica o segundo dígito verificador
    if (resto == 0 || resto == 1) {
        var dv2 = 0;
    } else {
        var dv2 = 11 - resto;
    }
    // Verifica se os dígitos verificadores estão corretos
    if (cpf.charAt(9) == dv1 && cpf.charAt(10) == dv2) {
        return true; // CPF válido
    } else {
        return false; // CPF inválido
    }
}
