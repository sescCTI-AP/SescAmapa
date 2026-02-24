using Newtonsoft.Json;

namespace PagamentoApi.Models.BB
{
    public class BoletoRequest
    {
        /// <summary>
        /// Número do convênio de Cobrança do Cliente. Identificador determinado pelo sistema Cobrança para controlar a emissão de boletos, 
        /// liquidação, crédito de valores ao Beneficiário e intercâmbio de dados com o cliente.
        /// </summary>
        /// <value>
        /// Número do convênio de Cobrança do Cliente. Identificador determinado pelo sistema Cobrança 
        /// para controlar a emissão de boletos, liquidação, crédito de valores ao Beneficiário e intercâmbio de dados com o cliente.
        /// </value>
        [JsonProperty(PropertyName = "numeroConvenio")]
        public long? NumeroConvenio { get; set; }

        /// <summary>
        /// Características do serviço de boleto bancário e como ele deve ser tratado pelo banco.
        /// </summary>
        /// <value>Características do serviço de boleto bancário e como ele deve ser tratado pelo banco.</value>
        [JsonProperty(PropertyName = "numeroCarteira")]
        public int? NumeroCarteira { get; set; }

        /// <summary>
        /// Número da variação da carteira do convênio de cobrança.
        /// </summary>
        /// <value>Número da variação da carteira do convênio de cobrança.</value>
        [JsonProperty(PropertyName = "numeroVariacaoCarteira")]
        public int? NumeroVariacaoCarteira { get; set; }

        /// <summary>
        /// Identifica  a característica dos boletos dentro das modalidades de cobrança existentes no banco.    
        /// Domínio: 01 - SIMPLES; 04 - VINCULADA
        /// </summary>
        /// <value>Identifica  a característica dos boletos dentro das modalidades de cobrança existentes no banco.   
        ///  Domínio: 01 - SIMPLES; 04 - VINCULADA</value>
        [JsonProperty(PropertyName = "codigoModalidade")]
        public int? CodigoModalidade { get; set; }

        /// <summary>
        /// Data de emissão do boleto (formato \"dd.mm.aaaaa\"). 
        /// </summary>
        /// <value>Data de emissão do boleto (formato \"dd.mm.aaaaa\"). </value>
        [JsonProperty(PropertyName = "dataEmissao")]
        public string DataEmissao { get; set; }

        /// <summary>
        /// Data de vencimento do boleto (formato \"dd.mm.aaaaa\").
        /// </summary>
        /// <value>Data de vencimento do boleto (formato \"dd.mm.aaaaa\").</value>
        [JsonProperty(PropertyName = "dataVencimento")]
        public string DataVencimento { get; set; }

        /// <summary>
        /// Valor de cobrança > 0.00, emitido em Real (formato decimal separado por \".\"). Valor do boleto no registro. 
        /// Deve ser maior que a soma dos campos “VALOR DO DESCONTO DO TÍTULO” e “VALOR DO ABATIMENTO DO TÍTULO”, se informados. 
        /// Informação não passível de alteração após a criação. No caso de emissão com valor equivocado, sugerimos cancelar e emitir novo boleto.
        /// </summary>
        /// <value>Valor de cobrança > 0.00, emitido em Real (formato decimal separado por \".\"). Valor do boleto no registro. 
        /// Deve ser maior que a soma dos campos “VALOR DO DESCONTO DO TÍTULO” e “VALOR DO ABATIMENTO DO TÍTULO”, se informados. 
        /// Informação não passível de alteração após a criação. No caso de emissão com valor equivocado, sugerimos cancelar e emitir novo boleto.</value>
        [JsonProperty(PropertyName = "valorOriginal")]
        public double? ValorOriginal { get; set; }

        /// <summary>
        /// Valor de dedução do boleto >= 0.00 (formato decimal separado por \".\").
        /// </summary>
        /// <value>Valor de dedução do boleto >= 0.00 (formato decimal separado por \".\").</value>
        [JsonProperty(PropertyName = "valorAbatimento")]
        public double? ValorAbatimento { get; set; }

        /// <summary>
        /// Quantos dias após a data de vencimento do boleto para iniciar o processo de cobrança através de protesto. (valor inteiro >= 0).
        /// </summary>
        /// <value>Quantos dias após a data de vencimento do boleto para iniciar o processo de cobrança através de protesto. (valor inteiro >= 0).</value>
        [JsonProperty(PropertyName = "quantidadeDiasProtesto")]
        public double? QuantidadeDiasProtesto { get; set; }

        /// <summary>
        /// Quantos dias após a data de vencimento do boleto para iniciar o processo de negativação através da opção escolhida 
        /// no campo orgaoNegativador. (valor inteiro >= 0).
        /// </summary>
        /// <value>Quantos dias após a data de vencimento do boleto para iniciar o processo de negativação através da opção escolhida 
        /// no campo orgaoNegativador. (valor inteiro >= 0).</value>
        [JsonProperty(PropertyName = "quantidadeDiasNegativacao")]
        public int? QuantidadeDiasNegativacao { get; set; }

        /// <summary>
        /// Código do Órgão Negativador.  Domínio: 10 - SERASA
        /// </summary>
        /// <value>Código do Órgão Negativador.  Domínio: 10 - SERASA</value>
        [JsonProperty(PropertyName = "orgaoNegativador")]
        public int? OrgaoNegativador { get; set; }

        /// <summary>
        /// Indicador de que o boleto pode ou não ser recebido após o vencimento. Campo não obrigatório  
        /// Se não informado, será assumido a informação de limite de recebimento que está definida no convênio.  
        /// Quando informado \"S\" em conjunto com o campo \"numeroDiasLimiteRecebimento\", será definido a quantidade de dias (corridos) 
        /// que este boleto ficará disponível para pagamento após seu vencimento.  Obs.: Se definido \"S\" e o campo \"numeroDiasLimiteRecebimento\"
        ///  ficar com valor zero também será assumido a informação de limite de recebimento que está definida no convênio.  
        /// Quando informado \"N\", fica definindo que o boleto NÃO permite pagamento em atraso, portanto só aceitará pagamento 
        /// até a data do vencimento ou o próximo dia útil, quando o vencimento ocorrer em dia não útil.  Quando informado qualquer 
        /// valor diferente de \"S\" ou \"N\" será assumido a informação de limite de recebimento que está definida no convênio.
        /// </summary>
        /// <value>
        /// Indicador de que o boleto pode ou não ser recebido após o vencimento. Campo não obrigatório  
        /// Se não informado, será assumido a informação de limite de recebimento que está definida no convênio. 
        ///  Quando informado \"S\" em conjunto com o campo \"numeroDiasLimiteRecebimento\", 
        /// será definido a quantidade de dias (corridos) que este boleto ficará disponível para pagamento após seu vencimento. 
        ///  Obs.: Se definido \"S\" e o campo \"numeroDiasLimiteRecebimento\" ficar com valor zero também será assumido 
        /// a informação de limite de recebimento que está definida no convênio.  Quando informado \"N\", 
        /// fica definindo que o boleto NÃO permite pagamento em atraso, portanto só aceitará pagamento até a data do vencimento
        ///  ou o próximo dia útil, quando o vencimento ocorrer em dia não útil.  Quando informado qualquer valor diferente de \"S\" ou \"N\" 
        /// será assumido a informação de limite de recebimento que está definida no convênio.
        /// </value>
        [JsonProperty(PropertyName = "indicadorAceiteTituloVencido")]
        public string IndicadorAceiteTituloVencido { get; set; }

        /// <summary>
        /// Número de dias limite para recebimento. Informar valor inteiro > 0.
        /// </summary>
        /// <value>Número de dias limite para recebimento. Informar valor inteiro > 0.</value>
        [JsonProperty(PropertyName = "numeroDiasLimiteRecebimento")]
        public int? NumeroDiasLimiteRecebimento { get; set; }

        /// <summary>
        /// Código para  identificar se o boleto de cobrança foi aceito (reconhecimento da dívida pelo Pagador).  Domínios: A - ACEITE N - NAO ACEITE
        /// </summary>
        /// <value>Código para  identificar se o boleto de cobrança foi aceito (reconhecimento da dívida pelo Pagador).  
        /// Domínios: A - ACEITE N - NAO ACEITE
        /// </value>
        [JsonProperty(PropertyName = "codigoAceite")]
        public string CodigoAceite { get; set; }

        /// <summary>
        /// Código para identificar o tipo de boleto de cobrança.  Domínios: 1- CHEQUE 2- DUPLICATA MERCANTIL 3- DUPLICATA MTIL POR INDICACAO 4- DUPLICATA DE SERVICO 5- DUPLICATA DE SRVC P/INDICACAO 6- DUPLICATA RURAL 7- LETRA DE CAMBIO 8- NOTA DE CREDITO COMERCIAL 9- NOTA DE CREDITO A EXPORTACAO 10- NOTA DE CREDITO INDULTRIAL 11- NOTA DE CREDITO RURAL 12- NOTA PROMISSORIA 13- NOTA PROMISSORIA RURAL 14- TRIPLICATA MERCANTIL 15- TRIPLICATA DE SERVICO 16- NOTA DE SEGURO 17- RECIBO 18- FATURA 19- NOTA DE DEBITO 20- APOLICE DE SEGURO 21- MENSALIDADE ESCOLAR 22- PARCELA DE CONSORCIO 23- DIVIDA ATIVA DA UNIAO 24- DIVIDA ATIVA DE ESTADO 25- DIVIDA ATIVA DE MUNICIPIO 31- CARTAO DE CREDITO 32- BOLETO PROPOSTA 33- BOLETO APORTE 99- OUTROS.
        /// </summary>
        /// <value>
        /// Código para identificar o tipo de boleto de cobrança.  Domínios: 1- CHEQUE 2- DUPLICATA MERCANTIL 
        /// 3- DUPLICATA MTIL POR INDICACAO 4- DUPLICATA DE SERVICO 5- DUPLICATA DE SRVC P/INDICACAO 6- DUPLICATA RURAL
        ///  7- LETRA DE CAMBIO 8- NOTA DE CREDITO COMERCIAL 9- NOTA DE CREDITO A EXPORTACAO 10- NOTA DE CREDITO INDULTRIAL 
        /// 11- NOTA DE CREDITO RURAL 12- NOTA PROMISSORIA 13- NOTA PROMISSORIA RURAL 14- TRIPLICATA MERCANTIL 
        /// 15- TRIPLICATA DE SERVICO 16- NOTA DE SEGURO 17- RECIBO 18- FATURA 19- NOTA DE DEBITO 20- APOLICE DE SEGURO 
        /// 21- MENSALIDADE ESCOLAR 22- PARCELA DE CONSORCIO 23- DIVIDA ATIVA DA UNIAO 24- DIVIDA ATIVA DE ESTADO 
        /// 25- DIVIDA ATIVA DE MUNICIPIO 31- CARTAO DE CREDITO 32- BOLETO PROPOSTA 33- BOLETO APORTE 99- OUTROS.
        /// </value>
        [JsonProperty(PropertyName = "codigoTipoTitulo")]
        public int? CodigoTipoTitulo { get; set; }

        /// <summary>
        /// Descrição do tipo de boleto.
        /// </summary>
        /// <value>Descrição do tipo de boleto.</value>
        [JsonProperty(PropertyName = "descricaoTipoTitulo")]
        public string DescricaoTipoTitulo { get; set; }

        /// <summary>
        /// Código para identificação da autorização de pagamento parcial do boleto.  Domínios: S - SIM N - NÃO 
        /// </summary>
        /// <value>Código para identificação da autorização de pagamento parcial do boleto.  Domínios: S - SIM N - NÃO </value>
        [JsonProperty(PropertyName = "indicadorPermissaoRecebimentoParcial")]
        public string IndicadorPermissaoRecebimentoParcial { get; set; }

        /// <summary>
        /// Número de identificação do boleto (correspondente ao SEU NÚMERO), no formato STRING (Limitado a 15 caracteres - Letras Maiúsculas).
        /// </summary>
        /// <value>
        /// Número de identificação do boleto (correspondente ao SEU NÚMERO), no formato STRING (Limitado a 15 caracteres - Letras Maiúsculas).
        /// </value>
        [JsonProperty(PropertyName = "numeroTituloBeneficiario")]
        public string NumeroTituloBeneficiario { get; set; }

        /// <summary>
        /// Informações adicionais sobre o beneficiário.
        /// </summary>
        /// <value>Informações adicionais sobre o beneficiário.</value>
        [JsonProperty(PropertyName = "campoUtilizacaoBeneficiario")]
        public string CampoUtilizacaoBeneficiario { get; set; }

        /// <summary>
        /// Número de identificação do boleto (correspondente ao NOSSO NÚMERO), no formato STRING, com 20 dígitos, 
        /// que deverá ser formatado da seguinte forma:  “000” +  (número do convênio com 7 dígitos) + 
        /// (10 algarismos - se necessário, completar com zeros à esquerda).
        /// </summary>
        /// <value>
        /// Número de identificação do boleto (correspondente ao NOSSO NÚMERO), no formato STRING, com 20 dígitos, 
        /// que deverá ser formatado da seguinte forma:  “000” +  (número do convênio com 7 dígitos) + 
        /// (10 algarismos - se necessário, completar com zeros à esquerda).
        /// </value>
        [JsonProperty(PropertyName = "numeroTituloCliente")]
        public string NumeroTituloCliente { get; set; }

        /// <summary>
        /// Mensagem definida pelo beneficiário para ser impressa no boleto. (Limitado a 30 caracteres)
        /// </summary>
        /// <value>Mensagem definida pelo beneficiário para ser impressa no boleto. (Limitado a 30 caracteres)</value>
        [JsonProperty(PropertyName = "mensagemBloquetoOcorrencia")]
        public string MensagemBloquetoOcorrencia { get; set; }

        /// <summary>
        /// Gets or Sets Desconto
        /// </summary>
        [JsonProperty(PropertyName = "desconto")]
        public Desconto Desconto { get; set; }

        /// <summary>
        /// Gets or Sets SegundoDesconto
        /// </summary>
        [JsonProperty(PropertyName = "segundoDesconto")]
        public Desconto SegundoDesconto { get; set; }

        /// <summary>
        /// Gets or Sets TerceiroDesconto
        /// </summary>
        [JsonProperty(PropertyName = "terceiroDesconto")]
        public Desconto TerceiroDesconto { get; set; }

        /// <summary>
        /// Gets or Sets JurosMora
        /// </summary>
        [JsonProperty(PropertyName = "jurosMora")]
        public JurosMora JurosMora { get; set; }

        /// <summary>
        /// Gets or Sets Multa
        /// </summary>
        [JsonProperty(PropertyName = "multa")]
        public Multa Multa { get; set; }

        /// <summary>
        /// Gets or Sets Pagador
        /// </summary>
        [JsonProperty(PropertyName = "pagador")]
        public Pagador Pagador { get; set; }

        /// <summary>
        /// Gets or Sets BeneficiarioFinal
        /// </summary>
        [JsonProperty(PropertyName = "beneficiarioFinal")]
        public BeneficiarioFinal BeneficiarioFinal { get; set; }

        /// <summary>
        /// Código para informar se o boleto terá um QRCode Pix atrelado. 
        /// Se informado caracter inválido,  assumirá 'N'.  Domínios: 'S' - QRCODE DINAMICO; 'N' - SEM PIX; OUTRO - SEM PIX
        /// </summary>
        /// <value>
        /// Código para informar se o boleto terá um QRCode Pix atrelado. 
        /// Se informado caracter inválido,  assumirá 'N'.  Domínios: 'S' - QRCODE DINAMICO; 'N' - SEM PIX; OUTRO - SEM PIX
        /// </value>
        [JsonProperty(PropertyName = "indicadorPix")]
        public string IndicadorPix { get; set; }
        public string CdElement { get; set; }
        public int SqCobranca { get; set; }
        public int Cduop { get; set; }
        public int Sqmatric { get; set; }
        public string IdClasse { get; set; }
    }
}