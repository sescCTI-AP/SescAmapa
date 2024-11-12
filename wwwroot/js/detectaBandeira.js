function detectaBandeira(numero) {
    var imgElement = document.getElementById('img-brand');
    numero = numero.replace(/\D/g, '');

    // Visa
    if (/^4[0-9]{12}(?:[0-9]{3})?$/.test(numero)) {
        imgElement.src = '/images/Brand/visto.png';
        imgElement.alt = 'visa';
        return 'Visa';
    }

    // MasterCard
    if (/^5[1-5][0-9]{14}$/.test(numero)) {
        imgElement.src = '/images/Brand/mastercard.png';
        imgElement.alt = 'mastercard';
        return 'MasterCard';
    }

    // American Express
    if (/^3[47][0-9]{13}$/.test(numero)) {
        imgElement.src = '/images/Brand/amex.png';
        imgElement.alt = 'amex';
        return 'American Express';
    }

    // Discover
    if (/^6(?:011|5[0-9]{2})[0-9]{12}$/.test(numero)) {
        imgElement.src = '/images/Brand/discover.png';
        imgElement.alt = 'discover';
        return 'Discover';
    }

    // JCB
    if (/^(?:2131|1800|35\d{3})\d{11}$/.test(numero)) {
        imgElement.src = '/images/Brand/jcb.png';
        imgElement.alt = 'jcb';
        return 'JCB';
    }

    // Elo
    if (/^(4011(78|79)|431274|438935|451416|457393|457631|457632|504175|506699|5067[0-6][0-9]|50677[0-8]|509[0-9]{3}|627780|636297|636368|6500(3[1-3]|[5-9][0-9])|6504[0-9]{2}|6505[0-2][0-9]|6505[3-9][0-9]|65070[0-9]|65071[0-8]|65072[0-7]|6509[0-1][0-9]|65092[0-7]|6516[5-9][0-9]|6517[0-9]{2}|6550[0-1][0-9]|6550[2-9][0-9]|6551[0-9]{2})[0-9]{10,11}$/.test(numero)) {
        imgElement.src = '/images/Brand/elo.png';
        imgElement.alt = 'elo';
        return 'Elo';
    }

    // Hipercard
    if (/^(606282|3841[0|4]0)[0-9]{6,12}$/.test(numero)) {
        imgElement.src = '/images/Brand/hiper.png';
        imgElement.alt = 'hiper';
        return 'Hipercard';
    }

    // Diners Club
    if (/^(3(?:0[0-5]|[689][0-9]))[0-9]{11,16}$/.test(numero)) {
        imgElement.src = '/images/Brand/dinners.png';
        imgElement.alt = 'dinners';
        return 'Diners Club';
    }

    // Outros ou não identificado
    return 'Bandeira não identificada';
}
