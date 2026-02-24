using System.Text;
using Newtonsoft.Json;

namespace PagamentoApi.Models.BB
{
    /// <summary>
    /// Representação dos campos de resposta de uma solicitação de detalhamento de boletos bancários.
    /// </summary>
    public class DetalhamentoBoleto
    {

        /// <summary>
        /// Campo correpondente à linha digitável do boleto.
        /// </summary>
        /// <value>Campo correpondente à linha digitável do boleto.</value>
        [JsonProperty(PropertyName = "codigoLinhaDigitavel")]
        public string CodigoLinhaDigitavel { get; set; }

        /// <summary>
        /// E-mail do Pagador.
        /// </summary>
        /// <value>E-mail do Pagador.</value>
        [JsonProperty(PropertyName = "textoEmailPagador")]
        public string TextoEmailPagador { get; set; }

        /// <summary>
        /// Mensagem para ser impressa no boleto.
        /// </summary>
        /// <value>Mensagem para ser impressa no boleto.</value>
        [JsonProperty(PropertyName = "textoMensagemBloquetoTitulo")]
        public string TextoMensagemBloquetoTitulo { get; set; }

        /// <summary>
        /// Código para identificação do tipo de multa para o Título de Cobrança.  Domínios: 0 - Sem multa 1 - Valor da Multa 2 - Percentual da Multa.  
        /// </summary>
        /// <value>Código para identificação do tipo de multa para o Título de Cobrança.  Domínios: 0 - Sem multa 1 - Valor da Multa 2 - Percentual da Multa.  </value>
        [JsonProperty(PropertyName = "codigoTipoMulta")]
        public int? CodigoTipoMulta { get; set; }

        /// <summary>
        /// Código para identificação da forma de pagamento e do canal onde foi pago o boleto. Composto por 3 dígitos.  O primeiro dígito é: 1    Espécie 2    Débito em conta 3    Cartão de crédito 4    Cheque  Os dois últimos: 01 Agencias - Postos tradicionais 02 Terminal de Auto-atendimento 03 Internet (home / office banking) 05 Correspondente bancário 06 Central de atendimento(Call Center) 07 Arquivo Eletrônico 08 DDA 61 Pix
        /// </summary>
        /// <value>Código para identificação da forma de pagamento e do canal onde foi pago o boleto. Composto por 3 dígitos.  O primeiro dígito é: 1    Espécie 2    Débito em conta 3    Cartão de crédito 4    Cheque  Os dois últimos: 01 Agencias - Postos tradicionais 02 Terminal de Auto-atendimento 03 Internet (home / office banking) 05 Correspondente bancário 06 Central de atendimento(Call Center) 07 Arquivo Eletrônico 08 DDA 61 Pix</value>
        [JsonProperty(PropertyName = "codigoCanalPagamento")]
        public int? CodigoCanalPagamento { get; set; }

        /// <summary>
        /// Código adotado pelo Banco para identificar o Contrato entre este e a Empresa Cliente.
        /// </summary>
        /// <value>Código adotado pelo Banco para identificar o Contrato entre este e a Empresa Cliente.</value>
        [JsonProperty(PropertyName = "numeroContratoCobranca")]
        public int? NumeroContratoCobranca { get; set; }

        /// <summary>
        /// Código que identifica o tipo de inscrição da Empresa ou Pessoa Física perante uma Instituição governamental.  Domínios:  1 - CPF  2 - CNPJ
        /// </summary>
        /// <value>Código que identifica o tipo de inscrição da Empresa ou Pessoa Física perante uma Instituição governamental.  Domínios:  1 - CPF  2 - CNPJ</value>
        [JsonProperty(PropertyName = "codigoTipoInscricaoSacado")]
        public int? CodigoTipoInscricaoSacado { get; set; }

        /// <summary>
        /// Número de inscrição da Empresa ou Pessoa Física perante uma Instituição governamental.
        /// </summary>
        /// <value>Número de inscrição da Empresa ou Pessoa Física perante uma Instituição governamental.</value>
        [JsonProperty(PropertyName = "numeroInscricaoSacadoCobranca")]
        public long? NumeroInscricaoSacadoCobranca { get; set; }

        /// <summary>
        /// Código da situação atual do boleto.  Domínios:  1 - NORMAL 2 - MOVIMENTO CARTORIO 3 - EM CARTORIO 4 - TITULO COM OCORRENCIA DE CARTORIO 5 - PROTESTADO ELETRONICO 6 - LIQUIDADO 7 - BAIXADO 8 - TITULO COM PENDENCIA DE CARTORIO 9 - TITULO PROTESTADO MANUAL 10 - TITULO BAIXADO/PAGO EM CARTORIO 11 - TITULO LIQUIDADO/PROTESTADO 12 - TITULO LIQUID/PGCRTO 13 - TITULO PROTESTADO AGUARDANDO BAIXA 14 - TITULO EM LIQUIDACAO 15 - TITULO AGENDADO 16 - TITULO CREDITADO 17 - PAGO EM CHEQUE - AGUARD.LIQUIDACAO 18 - PAGO PARCIALMENTE 19 - PAGO PARCIALMENTE CREDITADO  21 - TITULO AGENDADO COMPE 80 - EM PROCESSAMENTO (ESTADO TRANSITÓRIO)
        /// </summary>
        /// <value>Código da situação atual do boleto.  Domínios:  1 - NORMAL 2 - MOVIMENTO CARTORIO 3 - EM CARTORIO 4 - TITULO COM OCORRENCIA DE CARTORIO 5 - PROTESTADO ELETRONICO 6 - LIQUIDADO 7 - BAIXADO 8 - TITULO COM PENDENCIA DE CARTORIO 9 - TITULO PROTESTADO MANUAL 10 - TITULO BAIXADO/PAGO EM CARTORIO 11 - TITULO LIQUIDADO/PROTESTADO 12 - TITULO LIQUID/PGCRTO 13 - TITULO PROTESTADO AGUARDANDO BAIXA 14 - TITULO EM LIQUIDACAO 15 - TITULO AGENDADO 16 - TITULO CREDITADO 17 - PAGO EM CHEQUE - AGUARD.LIQUIDACAO 18 - PAGO PARCIALMENTE 19 - PAGO PARCIALMENTE CREDITADO  21 - TITULO AGENDADO COMPE 80 - EM PROCESSAMENTO (ESTADO TRANSITÓRIO)</value>
        [JsonProperty(PropertyName = "codigoEstadoTituloCobranca")]
        public int? CodigoEstadoTituloCobranca { get; set; }

        /// <summary>
        /// Código para identificar o tipo de boleto de cobrança.  Domínios:  1 - CHEQUE 2 - DUPLICATA MERCANTIL 3 - DUPLICATA MTIL POR INDICACAO 4 - DUPLICATA DE SERVICO 5 - DUPLICATA DE SRVC P/INDICACAO 6 - DUPLICATA RURAL 7 - LETRA DE CAMBIO 8 - NOTA DE CREDITO COMERCIAL 9 - NOTA DE CREDITO A EXPORTACAO 10 - NOTA DE CREDITO INDULTRIAL 11 - NOTA DE CREDITO RURAL 12 - NOTA PROMISSORIA 13 - NOTA PROMISSORIA RURAL 14 - TRIPLICATA MERCANTIL 15 - TRIPLICATA DE SERVICO 16 - NOTA DE SEGURO 17 - RECIBO 18 - FATURA 19 - NOTA DE DEBITO 20 - APOLICE DE SEGURO 21 - MENSALIDADE ESCOLAR 22 - PARCELA DE CONSORCIO 23 - DIVIDA ATIVA DA UNIAO 24 - DIVIDA ATIVA DE ESTADO 25 - DIVIDA ATIVA DE MUNICIPIO 31 - CARTAO DE CREDITO 32  - BOLETO PROPOSTA 99 - OUTROS
        /// </summary>
        /// <value>Código para identificar o tipo de boleto de cobrança.  Domínios:  1 - CHEQUE 2 - DUPLICATA MERCANTIL 3 - DUPLICATA MTIL POR INDICACAO 4 - DUPLICATA DE SERVICO 5 - DUPLICATA DE SRVC P/INDICACAO 6 - DUPLICATA RURAL 7 - LETRA DE CAMBIO 8 - NOTA DE CREDITO COMERCIAL 9 - NOTA DE CREDITO A EXPORTACAO 10 - NOTA DE CREDITO INDULTRIAL 11 - NOTA DE CREDITO RURAL 12 - NOTA PROMISSORIA 13 - NOTA PROMISSORIA RURAL 14 - TRIPLICATA MERCANTIL 15 - TRIPLICATA DE SERVICO 16 - NOTA DE SEGURO 17 - RECIBO 18 - FATURA 19 - NOTA DE DEBITO 20 - APOLICE DE SEGURO 21 - MENSALIDADE ESCOLAR 22 - PARCELA DE CONSORCIO 23 - DIVIDA ATIVA DA UNIAO 24 - DIVIDA ATIVA DE ESTADO 25 - DIVIDA ATIVA DE MUNICIPIO 31 - CARTAO DE CREDITO 32  - BOLETO PROPOSTA 99 - OUTROS</value>
        [JsonProperty(PropertyName = "codigoTipoTituloCobranca")]
        public int? CodigoTipoTituloCobranca { get; set; }

        /// <summary>
        /// Código para identificar a característica dos boletos dentro das modalidades de cobrança existentes no banco.  Domínios:  1 - SIMPLES 4 - VINCULADA
        /// </summary>
        /// <value>Código para identificar a característica dos boletos dentro das modalidades de cobrança existentes no banco.  Domínios:  1 - SIMPLES 4 - VINCULADA</value>
        [JsonProperty(PropertyName = "codigoModalidadeTitulo")]
        public int? CodigoModalidadeTitulo { get; set; }

        /// <summary>
        /// Código para  identificar se o boleto de cobrança foi aceito (reconhecimento da dívida pelo Pagador).  Domínios:  A - ACEITE N - NAO ACEITE
        /// </summary>
        /// <value>Código para  identificar se o boleto de cobrança foi aceito (reconhecimento da dívida pelo Pagador).  Domínios:  A - ACEITE N - NAO ACEITE</value>
        [JsonProperty(PropertyName = "codigoAceiteTituloCobranca")]
        public string CodigoAceiteTituloCobranca { get; set; }

        /// <summary>
        /// Código agência da praça do pagador (endereço).
        /// </summary>
        /// <value>Código agência da praça do pagador (endereço).</value>
        [JsonProperty(PropertyName = "codigoPrefixoDependenciaCobrador")]
        public int? CodigoPrefixoDependenciaCobrador { get; set; }

        /// <summary>
        /// Código para identificar a moeda referenciada no boleto.  Domínios: 0 - NENHUM 1 - FAJTR 2 - DOLAR 3 - EURO 4 - IENE 5 - MARCO ALEMAO 6 - FTR 7 - IDTR 8 - UFIR 9 - REAL 10 - SELIC 11 - IGP-M 12 - INPC 13 - TR (BESC)
        /// </summary>
        /// <value>Código para identificar a moeda referenciada no boleto.  Domínios: 0 - NENHUM 1 - FAJTR 2 - DOLAR 3 - EURO 4 - IENE 5 - MARCO ALEMAO 6 - FTR 7 - IDTR 8 - UFIR 9 - REAL 10 - SELIC 11 - IGP-M 12 - INPC 13 - TR (BESC)</value>
        [JsonProperty(PropertyName = "codigoIndicadorEconomico")]
        public int? CodigoIndicadorEconomico { get; set; }

        /// <summary>
        /// Campo destinado para uso da Empresa Beneficiário para identificação do boleto. Equivalente ao SEU NÚMERO ou ao numeroTituloBeneficiario do request do registro
        /// </summary>
        /// <value>Campo destinado para uso da Empresa Beneficiário para identificação do boleto. Equivalente ao SEU NÚMERO ou ao numeroTituloBeneficiario do request do registro</value>
        [JsonProperty(PropertyName = "numeroTituloCedenteCobranca")]
        public string NumeroTituloCedenteCobranca { get; set; }

        /// <summary>
        /// Código utilizado pela FEBRABAN para identificar o tipo de taxa de juros.   Domínios: 0 - DISPENSAR 1 - VALOR DIA ATRASO 2 - TAXA MENSAL 3 - ISENTO 
        /// </summary>
        /// <value>Código utilizado pela FEBRABAN para identificar o tipo de taxa de juros.   Domínios: 0 - DISPENSAR 1 - VALOR DIA ATRASO 2 - TAXA MENSAL 3 - ISENTO </value>
        [JsonProperty(PropertyName = "codigoTipoJuroMora")]
        public int? CodigoTipoJuroMora { get; set; }

        /// <summary>
        /// Data de emissão do boleto.
        /// </summary>
        /// <value>Data de emissão do boleto.</value>
        [JsonProperty(PropertyName = "dataEmissaoTituloCobranca")]
        public string DataEmissaoTituloCobranca { get; set; }

        /// <summary>
        /// Data de registro do boleto.
        /// </summary>
        /// <value>Data de registro do boleto.</value>
        [JsonProperty(PropertyName = "dataRegistroTituloCobranca")]
        public string DataRegistroTituloCobranca { get; set; }

        /// <summary>
        /// Data de vencimento do boleto.
        /// </summary>
        /// <value>Data de vencimento do boleto.</value>
        [JsonProperty(PropertyName = "dataVencimentoTituloCobranca")]
        public string DataVencimentoTituloCobranca { get; set; }

        /// <summary>
        /// Valor original do boleto indicado quando do registro.
        /// </summary>
        /// <value>Valor original do boleto indicado quando do registro.</value>
        [JsonProperty(PropertyName = "valorOriginalTituloCobranca")]
        public double? ValorOriginalTituloCobranca { get; set; }

        /// <summary>
        /// Valor atualizado do boleto, considerando possíveis multa, juros, mora, descontos, etc., que incidiram sob o valor original
        /// </summary>
        /// <value>Valor atualizado do boleto, considerando possíveis multa, juros, mora, descontos, etc., que incidiram sob o valor original</value>
        [JsonProperty(PropertyName = "valorAtualTituloCobranca")]
        public double? ValorAtualTituloCobranca { get; set; }

        /// <summary>
        /// Valores já recebidos em pagamentos parciais.
        /// </summary>
        /// <value>Valores já recebidos em pagamentos parciais.</value>
        [JsonProperty(PropertyName = "valorPagamentoParcialTitulo")]
        public double? ValorPagamentoParcialTitulo { get; set; }

        /// <summary>
        /// Valor do abatimento (redução do valor do documento, devido a algum problema), expresso em moeda corrente.
        /// </summary>
        /// <value>Valor do abatimento (redução do valor do documento, devido a algum problema), expresso em moeda corrente.</value>
        [JsonProperty(PropertyName = "valorAbatimentoTituloCobranca")]
        public double? ValorAbatimentoTituloCobranca { get; set; }

        /// <summary>
        /// Percentual do IOF - Imposto sobre Operações Financeiras - de um boleto prêmio de seguro na sua data de emissão, expresso de acordo com o tipo de moeda.
        /// </summary>
        /// <value>Percentual do IOF - Imposto sobre Operações Financeiras - de um boleto prêmio de seguro na sua data de emissão, expresso de acordo com o tipo de moeda.</value>
        [JsonProperty(PropertyName = "percentualImpostoSobreOprFinanceirasTituloCobranca")]
        public double? PercentualImpostoSobreOprFinanceirasTituloCobranca { get; set; }

        /// <summary>
        /// Valor do IOF - Imposto sobre Operações Financeiras - de um boleto prêmio de seguro na sua data de emissão, expresso de acordo com o tipo de moeda.
        /// </summary>
        /// <value>Valor do IOF - Imposto sobre Operações Financeiras - de um boleto prêmio de seguro na sua data de emissão, expresso de acordo com o tipo de moeda.</value>
        [JsonProperty(PropertyName = "valorImpostoSobreOprFinanceirasTituloCobranca")]
        public double? ValorImpostoSobreOprFinanceirasTituloCobranca { get; set; }

        /// <summary>
        /// Valor do boleto expresso em moeda variável.
        /// </summary>
        /// <value>Valor do boleto expresso em moeda variável.</value>
        [JsonProperty(PropertyName = "valorMoedaTituloCobranca")]
        public double? ValorMoedaTituloCobranca { get; set; }

        /// <summary>
        /// Porcentagem sobre o valor do boleto a ser cobrada de juros de mora.
        /// </summary>
        /// <value>Porcentagem sobre o valor do boleto a ser cobrada de juros de mora.</value>
        [JsonProperty(PropertyName = "percentualJuroMoraTitulo")]
        public double? PercentualJuroMoraTitulo { get; set; }

        /// <summary>
        /// Valor  sobre o valor do boleto a ser cobrado de juros de mora.
        /// </summary>
        /// <value>Valor  sobre o valor do boleto a ser cobrado de juros de mora.</value>
        [JsonProperty(PropertyName = "valorJuroMoraTitulo")]
        public double? ValorJuroMoraTitulo { get; set; }

        /// <summary>
        /// Porcentagem sobre o valor do boleto a ser cobrada de multa.
        /// </summary>
        /// <value>Porcentagem sobre o valor do boleto a ser cobrada de multa.</value>
        [JsonProperty(PropertyName = "percentualMultaTitulo")]
        public double? PercentualMultaTitulo { get; set; }

        /// <summary>
        /// Valor  sobre o valor do boleto a ser cobrado de multa.
        /// </summary>
        /// <value>Valor  sobre o valor do boleto a ser cobrado de multa.</value>
        [JsonProperty(PropertyName = "valorMultaTituloCobranca")]
        public double? ValorMultaTituloCobranca { get; set; }

        /// <summary>
        /// Quantidade de parcela do boleto.
        /// </summary>
        /// <value>Quantidade de parcela do boleto.</value>
        [JsonProperty(PropertyName = "quantidadeParcelaTituloCobranca")]
        public int? QuantidadeParcelaTituloCobranca { get; set; }

        /// <summary>
        /// Data da baixa automática do boleto, conforme cadastrado no convênio.
        /// </summary>
        /// <value>Data da baixa automática do boleto, conforme cadastrado no convênio.</value>
        [JsonProperty(PropertyName = "dataBaixaAutomaticoTitulo")]
        public string DataBaixaAutomaticoTitulo { get; set; }

        /// <summary>
        /// Texto de observações destinado ao envio de mensagens livres, a serem impressas no campo de instruções da ficha de compensação do Boleto de Pagamento.
        /// </summary>
        /// <value>Texto de observações destinado ao envio de mensagens livres, a serem impressas no campo de instruções da ficha de compensação do Boleto de Pagamento.</value>
        [JsonProperty(PropertyName = "textoCampoUtilizacaoCedente")]
        public string TextoCampoUtilizacaoCedente { get; set; }

        /// <summary>
        /// Código para identificação de Rateio de Crédito (partilhamento).  Domínios:  S - SIM  N - NÃO
        /// </summary>
        /// <value>Código para identificação de Rateio de Crédito (partilhamento).  Domínios:  S - SIM  N - NÃO</value>
        [JsonProperty(PropertyName = "indicadorCobrancaPartilhadoTitulo")]
        public string IndicadorCobrancaPartilhadoTitulo { get; set; }

        /// <summary>
        /// Nome que identifica a pessoa, física ou jurídica, a qual se quer fazer referência.
        /// </summary>
        /// <value>Nome que identifica a pessoa, física ou jurídica, a qual se quer fazer referência.</value>
        [JsonProperty(PropertyName = "nomeSacadoCobranca")]
        public string NomeSacadoCobranca { get; set; }

        /// <summary>
        /// Texto referente a localização da rua/avenida, número, complemento para entrega de correspondência.
        /// </summary>
        /// <value>Texto referente a localização da rua/avenida, número, complemento para entrega de correspondência.</value>
        [JsonProperty(PropertyName = "textoEnderecoSacadoCobranca")]
        public string TextoEnderecoSacadoCobranca { get; set; }

        /// <summary>
        /// Texto referente ao bairro para entrega de correspondência.
        /// </summary>
        /// <value>Texto referente ao bairro para entrega de correspondência.</value>
        [JsonProperty(PropertyName = "nomeBairroSacadoCobranca")]
        public string NomeBairroSacadoCobranca { get; set; }

        /// <summary>
        /// Texto referente ao nome do município componente do endereço utilizado para entrega de correspondência.
        /// </summary>
        /// <value>Texto referente ao nome do município componente do endereço utilizado para entrega de correspondência.</value>
        [JsonProperty(PropertyName = "nomeMunicipioSacadoCobranca")]
        public string NomeMunicipioSacadoCobranca { get; set; }

        /// <summary>
        /// Código do estado, unidade da federação componente do endereço utilizado para entrega de correspondência.
        /// </summary>
        /// <value>Código do estado, unidade da federação componente do endereço utilizado para entrega de correspondência.</value>
        [JsonProperty(PropertyName = "siglaUnidadeFederacaoSacadoCobranca")]
        public string SiglaUnidadeFederacaoSacadoCobranca { get; set; }

        /// <summary>
        /// Código adotado pelos Correios, para identificação de logradouros.
        /// </summary>
        /// <value>Código adotado pelos Correios, para identificação de logradouros.</value>
        [JsonProperty(PropertyName = "numeroCepSacadoCobranca")]
        public int? NumeroCepSacadoCobranca { get; set; }

        /// <summary>
        /// Valor da moeda do abatimento.
        /// </summary>
        /// <value>Valor da moeda do abatimento.</value>
        [JsonProperty(PropertyName = "valorMoedaAbatimentoTitulo")]
        public double? ValorMoedaAbatimentoTitulo { get; set; }

        /// <summary>
        /// Data para inicialização do processo de cobrança via protesto.
        /// </summary>
        /// <value>Data para inicialização do processo de cobrança via protesto.</value>
        [JsonProperty(PropertyName = "dataProtestoTituloCobranca")]
        public string DataProtestoTituloCobranca { get; set; }

        /// <summary>
        /// Código que identifica o tipo de inscrição do Beneficiário original do boleto de cobrança.  Domínios:  1 - CPF  2 - CNPJ
        /// </summary>
        /// <value>Código que identifica o tipo de inscrição do Beneficiário original do boleto de cobrança.  Domínios:  1 - CPF  2 - CNPJ</value>
        [JsonProperty(PropertyName = "codigoTipoInscricaoSacador")]
        public int? CodigoTipoInscricaoSacador { get; set; }

        /// <summary>
        /// Número de inscrição do Beneficiário original do boleto de cobrança.
        /// </summary>
        /// <value>Número de inscrição do Beneficiário original do boleto de cobrança.</value>
        [JsonProperty(PropertyName = "numeroInscricaoSacadorAvalista")]
        public long? NumeroInscricaoSacadorAvalista { get; set; }

        /// <summary>
        /// Nome que identifica a entidade, pessoa física ou jurídica, Beneficiário original do boleto de cobrança.
        /// </summary>
        /// <value>Nome que identifica a entidade, pessoa física ou jurídica, Beneficiário original do boleto de cobrança.</value>
        [JsonProperty(PropertyName = "nomeSacadorAvalistaTitulo")]
        public string NomeSacadorAvalistaTitulo { get; set; }

        /// <summary>
        /// Percentual de desconto a ser concedido sobre o boleto de cobrança.
        /// </summary>
        /// <value>Percentual de desconto a ser concedido sobre o boleto de cobrança.</value>
        [JsonProperty(PropertyName = "percentualDescontoTitulo")]
        public double? PercentualDescontoTitulo { get; set; }

        /// <summary>
        /// Data limite do desconto do boleto de cobrança.
        /// </summary>
        /// <value>Data limite do desconto do boleto de cobrança.</value>
        [JsonProperty(PropertyName = "dataDescontoTitulo")]
        public string DataDescontoTitulo { get; set; }

        /// <summary>
        /// Valor de desconto a ser concedido sobre o boleto de cobrança.
        /// </summary>
        /// <value>Valor de desconto a ser concedido sobre o boleto de cobrança.</value>
        [JsonProperty(PropertyName = "valorDescontoTitulo")]
        public double? ValorDescontoTitulo { get; set; }

        /// <summary>
        /// Código para identificação do tipo de desconto que deverá ser concedido.  Domínios:  0 - SEM DESCONTO  1 - VLR FIXO ATE A DATA INFORMADA  2 - PERCENTUAL ATE A DATA INFORMADA  3 - DESCONTO POR DIA DE ANTECIPACAO
        /// </summary>
        /// <value>Código para identificação do tipo de desconto que deverá ser concedido.  Domínios:  0 - SEM DESCONTO  1 - VLR FIXO ATE A DATA INFORMADA  2 - PERCENTUAL ATE A DATA INFORMADA  3 - DESCONTO POR DIA DE ANTECIPACAO</value>
        [JsonProperty(PropertyName = "codigoDescontoTitulo")]
        public int? CodigoDescontoTitulo { get; set; }

        /// <summary>
        /// Percentual do segundo desconto a ser concedido sobre o boleto de cobrança.
        /// </summary>
        /// <value>Percentual do segundo desconto a ser concedido sobre o boleto de cobrança.</value>
        [JsonProperty(PropertyName = "percentualSegundoDescontoTitulo")]
        public double? PercentualSegundoDescontoTitulo { get; set; }

        /// <summary>
        /// Data limite do segundo desconto do boleto de cobrança.
        /// </summary>
        /// <value>Data limite do segundo desconto do boleto de cobrança.</value>
        [JsonProperty(PropertyName = "dataSegundoDescontoTitulo")]
        public string DataSegundoDescontoTitulo { get; set; }

        /// <summary>
        /// Valor do segundo desconto a ser concedido sobre o boleto de cobrança.
        /// </summary>
        /// <value>Valor do segundo desconto a ser concedido sobre o boleto de cobrança.</value>
        [JsonProperty(PropertyName = "valorSegundoDescontoTitulo")]
        public double? ValorSegundoDescontoTitulo { get; set; }

        /// <summary>
        /// Código para identificação do tipo do segundo desconto que deverá ser concedido.  Domínios:  0 - SEM DESCONTO  1 - VLR FIXO ATE A DATA INFORMADA  2 - PERCENTUAL ATE A DATA INFORMADA  3 - DESCONTO POR DIA DE ANTECIPACAO
        /// </summary>
        /// <value>Código para identificação do tipo do segundo desconto que deverá ser concedido.  Domínios:  0 - SEM DESCONTO  1 - VLR FIXO ATE A DATA INFORMADA  2 - PERCENTUAL ATE A DATA INFORMADA  3 - DESCONTO POR DIA DE ANTECIPACAO</value>
        [JsonProperty(PropertyName = "codigoSegundoDescontoTitulo")]
        public int? CodigoSegundoDescontoTitulo { get; set; }

        /// <summary>
        /// Percentual do terceiro desconto a ser concedido sobre o boleto de cobrança.
        /// </summary>
        /// <value>Percentual do terceiro desconto a ser concedido sobre o boleto de cobrança.</value>
        [JsonProperty(PropertyName = "percentualTerceiroDescontoTitulo")]
        public double? PercentualTerceiroDescontoTitulo { get; set; }

        /// <summary>
        /// Data limite do terceiro desconto do boleto de cobrança.
        /// </summary>
        /// <value>Data limite do terceiro desconto do boleto de cobrança.</value>
        [JsonProperty(PropertyName = "dataTerceiroDescontoTitulo")]
        public string DataTerceiroDescontoTitulo { get; set; }

        /// <summary>
        /// Valor do terceiro desconto a ser concedido sobre o boleto de cobrança.
        /// </summary>
        /// <value>Valor do terceiro desconto a ser concedido sobre o boleto de cobrança.</value>
        [JsonProperty(PropertyName = "valorTerceiroDescontoTitulo")]
        public double? ValorTerceiroDescontoTitulo { get; set; }

        /// <summary>
        /// Código para identificação do tipo do terceiro desconto que deverá ser concedido.  Domínios:  0 - SEM DESCONTO  1 - VLR FIXO ATE A DATA INFORMADA  2 - PERCENTUAL ATE A DATA INFORMADA  3 - DESCONTO POR DIA DE ANTECIPACAO
        /// </summary>
        /// <value>Código para identificação do tipo do terceiro desconto que deverá ser concedido.  Domínios:  0 - SEM DESCONTO  1 - VLR FIXO ATE A DATA INFORMADA  2 - PERCENTUAL ATE A DATA INFORMADA  3 - DESCONTO POR DIA DE ANTECIPACAO</value>
        [JsonProperty(PropertyName = "codigoTerceiroDescontoTitulo")]
        public int? CodigoTerceiroDescontoTitulo { get; set; }

        /// <summary>
        /// Data para início da cobrança da multa.
        /// </summary>
        /// <value>Data para início da cobrança da multa.</value>
        [JsonProperty(PropertyName = "dataMultaTitulo")]
        public string DataMultaTitulo { get; set; }

        /// <summary>
        /// Número da carteira do convênio de cobrança.
        /// </summary>
        /// <value>Número da carteira do convênio de cobrança.</value>
        [JsonProperty(PropertyName = "numeroCarteiraCobranca")]
        public int? NumeroCarteiraCobranca { get; set; }

        /// <summary>
        /// Número da variação da carteira do convênio de cobrança.
        /// </summary>
        /// <value>Número da variação da carteira do convênio de cobrança.</value>
        [JsonProperty(PropertyName = "numeroVariacaoCarteiraCobranca")]
        public int? NumeroVariacaoCarteiraCobranca { get; set; }

        /// <summary>
        /// Número de dias decorrentes após a data de vencimento para inicialização do processo de cobrança via protesto.
        /// </summary>
        /// <value>Número de dias decorrentes após a data de vencimento para inicialização do processo de cobrança via protesto.</value>
        [JsonProperty(PropertyName = "quantidadeDiaProtesto")]
        public int? QuantidadeDiaProtesto { get; set; }

        /// <summary>
        /// Número de dias corridos para recebimento do boleto após a data de vencimento.
        /// </summary>
        /// <value>Número de dias corridos para recebimento do boleto após a data de vencimento.</value>
        [JsonProperty(PropertyName = "quantidadeDiaPrazoLimiteRecebimento")]
        public int? QuantidadeDiaPrazoLimiteRecebimento { get; set; }

        /// <summary>
        /// Data limite para recebimento do boleto após a data de vencimento.
        /// </summary>
        /// <value>Data limite para recebimento do boleto após a data de vencimento.</value>
        [JsonProperty(PropertyName = "dataLimiteRecebimentoTitulo")]
        public string DataLimiteRecebimentoTitulo { get; set; }

        /// <summary>
        /// Código para identificação da autorização de pagamento parcial do boleto.  Domínios:  S - SIM  N - NAO
        /// </summary>
        /// <value>Código para identificação da autorização de pagamento parcial do boleto.  Domínios:  S - SIM  N - NAO</value>
        [JsonProperty(PropertyName = "indicadorPermissaoRecebimentoParcial")]
        public string IndicadorPermissaoRecebimentoParcial { get; set; }

        /// <summary>
        /// Código de barras do boleto.
        /// </summary>
        /// <value>Código de barras do boleto.</value>
        [JsonProperty(PropertyName = "textoCodigoBarrasTituloCobranca")]
        public string TextoCodigoBarrasTituloCobranca { get; set; }

        /// <summary>
        /// Código para identificação das ocorrências de retorno do cartório.  Domínios:  0 - TITULO PROTOCOLADO - ANTIGO \"TEC\"  1 - TITULO PAGO EM CARTORIO  2 - TITULO PROTESTADO - ANTIGO \"DDP\"  3 - TITULO RETIRADO CARTORIO - ANT. DDS  4 - TITULO SUSTADO JUDICIALMENTE  5 - TITULO RECUSADO SEM CUSTAS  6 - TITULO RECUSADO COM CUSTAS  7 - TITULO PAGO LIQUIDACAO CONDICIONAL  8 - TITULO ACEITO  9 - CUSTAS DE EDITAL  20 - LQ. CARTORIO AG. SEMI-AUTOM.  21 - CHQ DEVOLV. TIT. ENC. PROT.  22 - TITULO SUSTADO DEFINITIVO  23 - RETIRADA APÓS SUSTAÇÃO JUDICIAL  59 - PAGTO CONDICIONAL VIA SELTEC  60 - TITULO PAGO EM CARTORIO-SELTEC
        /// </summary>
        /// <value>Código para identificação das ocorrências de retorno do cartório.  Domínios:  0 - TITULO PROTOCOLADO - ANTIGO \"TEC\"  1 - TITULO PAGO EM CARTORIO  2 - TITULO PROTESTADO - ANTIGO \"DDP\"  3 - TITULO RETIRADO CARTORIO - ANT. DDS  4 - TITULO SUSTADO JUDICIALMENTE  5 - TITULO RECUSADO SEM CUSTAS  6 - TITULO RECUSADO COM CUSTAS  7 - TITULO PAGO LIQUIDACAO CONDICIONAL  8 - TITULO ACEITO  9 - CUSTAS DE EDITAL  20 - LQ. CARTORIO AG. SEMI-AUTOM.  21 - CHQ DEVOLV. TIT. ENC. PROT.  22 - TITULO SUSTADO DEFINITIVO  23 - RETIRADA APÓS SUSTAÇÃO JUDICIAL  59 - PAGTO CONDICIONAL VIA SELTEC  60 - TITULO PAGO EM CARTORIO-SELTEC</value>
        [JsonProperty(PropertyName = "codigoOcorrenciaCartorio")]
        public int? CodigoOcorrenciaCartorio { get; set; }

        /// <summary>
        /// Valor do IOF recebido.
        /// </summary>
        /// <value>Valor do IOF recebido.</value>
        [JsonProperty(PropertyName = "valorImpostoSobreOprFinanceirasRecebidoTitulo")]
        public double? ValorImpostoSobreOprFinanceirasRecebidoTitulo { get; set; }

        /// <summary>
        /// Valor do abatimento concedido.
        /// </summary>
        /// <value>Valor do abatimento concedido.</value>
        [JsonProperty(PropertyName = "valorAbatimentoTotal")]
        public double? ValorAbatimentoTotal { get; set; }

        /// <summary>
        /// Valor dos juros recebidos.
        /// </summary>
        /// <value>Valor dos juros recebidos.</value>
        [JsonProperty(PropertyName = "valorJuroMoraRecebido")]
        public double? ValorJuroMoraRecebido { get; set; }

        /// <summary>
        /// Valor de desconto utilizado pelo pagador.
        /// </summary>
        /// <value>Valor de desconto utilizado pelo pagador.</value>
        [JsonProperty(PropertyName = "valorDescontoUtilizado")]
        public double? ValorDescontoUtilizado { get; set; }

        /// <summary>
        /// Valor pago.
        /// </summary>
        /// <value>Valor pago.</value>
        [JsonProperty(PropertyName = "valorPagoSacado")]
        public double? ValorPagoSacado { get; set; }

        /// <summary>
        /// Valor líquido creditado ao beneficiário.
        /// </summary>
        /// <value>Valor líquido creditado ao beneficiário.</value>
        [JsonProperty(PropertyName = "valorCreditoCedente")]
        public double? ValorCreditoCedente { get; set; }

        /// <summary>
        /// Código para identificação do tipo de liquidação.  Domínios:  1 CAIXA 2 VIA COMPE 3 EM CARTORIO 4 EM CARTORIO - SEM EXISTENCIA 17 POS 5 TITULO EM LIQUIDACAO - ORIGEM AGE 6 TITULO EM LIQUIDACAO - PGT 7 BANCO POSTAL 8 TITULO LIQUIDADO VIA COMPE/STR
        /// </summary>
        /// <value>Código para identificação do tipo de liquidação.  Domínios:  1 CAIXA 2 VIA COMPE 3 EM CARTORIO 4 EM CARTORIO - SEM EXISTENCIA 17 POS 5 TITULO EM LIQUIDACAO - ORIGEM AGE 6 TITULO EM LIQUIDACAO - PGT 7 BANCO POSTAL 8 TITULO LIQUIDADO VIA COMPE/STR</value>
        [JsonProperty(PropertyName = "codigoTipoLiquidacao")]
        public int? CodigoTipoLiquidacao { get; set; }

        /// <summary>
        /// Data a qual será creditado o valor inerente ao título (este campo só será preenchido após a liquidação, ou seja, após codigoEstadoTituloCobranca = 6).
        /// </summary>
        /// <value>Data a qual será creditado o valor inerente ao título (este campo só será preenchido após a liquidação, ou seja, após codigoEstadoTituloCobranca = 6).</value>
        [JsonProperty(PropertyName = "dataCreditoLiquidacao")]
        public string DataCreditoLiquidacao { get; set; }

        /// <summary>
        /// Data para a qual foi agendado o recebimento/pagamento do título.
        /// </summary>
        /// <value>Data para a qual foi agendado o recebimento/pagamento do título.</value>
        [JsonProperty(PropertyName = "dataRecebimentoTitulo")]
        public string DataRecebimentoTitulo { get; set; }

        /// <summary>
        /// Código agência da praça do recebimento do boleto.
        /// </summary>
        /// <value>Código agência da praça do recebimento do boleto.</value>
        [JsonProperty(PropertyName = "codigoPrefixoDependenciaRecebedor")]
        public int? CodigoPrefixoDependenciaRecebedor { get; set; }

        /// <summary>
        /// Código para identificar as ocorrências (rejeições, tarifas, custas, liquidação e baixas) do boleto.  Domínios:  1 - NORMAL 2 - POR CONTA 3 - POR SALDO 4 - CHEQUE A COMPENSAR 7 - LIQUIDADO NA APRESENTACAO 8 - POR CONTA EM CARTORIO 9 - EM CARTORIO
        /// </summary>
        /// <value>Código para identificar as ocorrências (rejeições, tarifas, custas, liquidação e baixas) do boleto.  Domínios:  1 - NORMAL 2 - POR CONTA 3 - POR SALDO 4 - CHEQUE A COMPENSAR 7 - LIQUIDADO NA APRESENTACAO 8 - POR CONTA EM CARTORIO 9 - EM CARTORIO</value>
        [JsonProperty(PropertyName = "codigoNaturezaRecebimento")]
        public int? CodigoNaturezaRecebimento { get; set; }

        /// <summary>
        /// Número de identidade do sacado do título.
        /// </summary>
        /// <value>Número de identidade do sacado do título.</value>
        [JsonProperty(PropertyName = "numeroIdentidadeSacadoTituloCobranca")]
        public string NumeroIdentidadeSacadoTituloCobranca { get; set; }

        /// <summary>
        /// Código para identificação do sistema/usuário responsável pela atualização do boleto.
        /// </summary>
        /// <value>Código para identificação do sistema/usuário responsável pela atualização do boleto.</value>
        [JsonProperty(PropertyName = "codigoResponsavelAtualizacao")]
        public string CodigoResponsavelAtualizacao { get; set; }

        /// <summary>
        /// Código para identificação do tipo de baixa do boleto.  Domínios:  1 - BAIXADO POR SOLICITACAO 2 - ENTREGA FRANCO PAGAMENTO 9 - COMANDADA BANCO 10 - COMANDADA CLIENTE - ARQUIVO 11 - COMANDADA CLIENTE - ON-LINE 12 - DECURSO PRAZO - CLIENTE 13 - DECURSO PRAZO - BANCO 15 - PROTESTADO 31 - LIQUIDADO ANTERIORMENTE 32 - HABILITADO EM PROCESSO 35 - TRANSFERIDO PARA PERDAS 51 - REGISTRADO INDEVIDAMENTE 90 - BAIXA AUTOMATICA
        /// </summary>
        /// <value>Código para identificação do tipo de baixa do boleto.  Domínios:  1 - BAIXADO POR SOLICITACAO 2 - ENTREGA FRANCO PAGAMENTO 9 - COMANDADA BANCO 10 - COMANDADA CLIENTE - ARQUIVO 11 - COMANDADA CLIENTE - ON-LINE 12 - DECURSO PRAZO - CLIENTE 13 - DECURSO PRAZO - BANCO 15 - PROTESTADO 31 - LIQUIDADO ANTERIORMENTE 32 - HABILITADO EM PROCESSO 35 - TRANSFERIDO PARA PERDAS 51 - REGISTRADO INDEVIDAMENTE 90 - BAIXA AUTOMATICA</value>
        [JsonProperty(PropertyName = "codigoTipoBaixaTitulo")]
        public int? CodigoTipoBaixaTitulo { get; set; }

        /// <summary>
        /// Valor de multa recebido.
        /// </summary>
        /// <value>Valor de multa recebido.</value>
        [JsonProperty(PropertyName = "valorMultaRecebido")]
        public double? ValorMultaRecebido { get; set; }

        /// <summary>
        /// Valor do reajuste (índice econômico).
        /// </summary>
        /// <value>Valor do reajuste (índice econômico).</value>
        [JsonProperty(PropertyName = "valorReajuste")]
        public double? ValorReajuste { get; set; }

        /// <summary>
        /// Outros valores recebidos.
        /// </summary>
        /// <value>Outros valores recebidos.</value>
        [JsonProperty(PropertyName = "valorOutroRecebido")]
        public double? ValorOutroRecebido { get; set; }

        /// <summary>
        /// Código do índice econômico utilizado para o cálculo de juros/multa.
        /// </summary>
        /// <value>Código do índice econômico utilizado para o cálculo de juros/multa.</value>
        [JsonProperty(PropertyName = "codigoIndicadorEconomicoUtilizadoInadimplencia")]
        public double? CodigoIndicadorEconomicoUtilizadoInadimplencia { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class RespostaDetalhamentoBoleto {\n");
            sb.Append("  CodigoLinhaDigitavel: ").Append(CodigoLinhaDigitavel).Append("\n");
            sb.Append("  TextoEmailPagador: ").Append(TextoEmailPagador).Append("\n");
            sb.Append("  TextoMensagemBloquetoTitulo: ").Append(TextoMensagemBloquetoTitulo).Append("\n");
            sb.Append("  CodigoTipoMulta: ").Append(CodigoTipoMulta).Append("\n");
            sb.Append("  CodigoCanalPagamento: ").Append(CodigoCanalPagamento).Append("\n");
            sb.Append("  NumeroContratoCobranca: ").Append(NumeroContratoCobranca).Append("\n");
            sb.Append("  CodigoTipoInscricaoSacado: ").Append(CodigoTipoInscricaoSacado).Append("\n");
            sb.Append("  NumeroInscricaoSacadoCobranca: ").Append(NumeroInscricaoSacadoCobranca).Append("\n");
            sb.Append("  CodigoEstadoTituloCobranca: ").Append(CodigoEstadoTituloCobranca).Append("\n");
            sb.Append("  CodigoTipoTituloCobranca: ").Append(CodigoTipoTituloCobranca).Append("\n");
            sb.Append("  CodigoModalidadeTitulo: ").Append(CodigoModalidadeTitulo).Append("\n");
            sb.Append("  CodigoAceiteTituloCobranca: ").Append(CodigoAceiteTituloCobranca).Append("\n");
            sb.Append("  CodigoPrefixoDependenciaCobrador: ").Append(CodigoPrefixoDependenciaCobrador).Append("\n");
            sb.Append("  CodigoIndicadorEconomico: ").Append(CodigoIndicadorEconomico).Append("\n");
            sb.Append("  NumeroTituloCedenteCobranca: ").Append(NumeroTituloCedenteCobranca).Append("\n");
            sb.Append("  CodigoTipoJuroMora: ").Append(CodigoTipoJuroMora).Append("\n");
            sb.Append("  DataEmissaoTituloCobranca: ").Append(DataEmissaoTituloCobranca).Append("\n");
            sb.Append("  DataRegistroTituloCobranca: ").Append(DataRegistroTituloCobranca).Append("\n");
            sb.Append("  DataVencimentoTituloCobranca: ").Append(DataVencimentoTituloCobranca).Append("\n");
            sb.Append("  ValorOriginalTituloCobranca: ").Append(ValorOriginalTituloCobranca).Append("\n");
            sb.Append("  ValorAtualTituloCobranca: ").Append(ValorAtualTituloCobranca).Append("\n");
            sb.Append("  ValorPagamentoParcialTitulo: ").Append(ValorPagamentoParcialTitulo).Append("\n");
            sb.Append("  ValorAbatimentoTituloCobranca: ").Append(ValorAbatimentoTituloCobranca).Append("\n");
            sb.Append("  PercentualImpostoSobreOprFinanceirasTituloCobranca: ").Append(PercentualImpostoSobreOprFinanceirasTituloCobranca).Append("\n");
            sb.Append("  ValorImpostoSobreOprFinanceirasTituloCobranca: ").Append(ValorImpostoSobreOprFinanceirasTituloCobranca).Append("\n");
            sb.Append("  ValorMoedaTituloCobranca: ").Append(ValorMoedaTituloCobranca).Append("\n");
            sb.Append("  PercentualJuroMoraTitulo: ").Append(PercentualJuroMoraTitulo).Append("\n");
            sb.Append("  ValorJuroMoraTitulo: ").Append(ValorJuroMoraTitulo).Append("\n");
            sb.Append("  PercentualMultaTitulo: ").Append(PercentualMultaTitulo).Append("\n");
            sb.Append("  ValorMultaTituloCobranca: ").Append(ValorMultaTituloCobranca).Append("\n");
            sb.Append("  QuantidadeParcelaTituloCobranca: ").Append(QuantidadeParcelaTituloCobranca).Append("\n");
            sb.Append("  DataBaixaAutomaticoTitulo: ").Append(DataBaixaAutomaticoTitulo).Append("\n");
            sb.Append("  TextoCampoUtilizacaoCedente: ").Append(TextoCampoUtilizacaoCedente).Append("\n");
            sb.Append("  IndicadorCobrancaPartilhadoTitulo: ").Append(IndicadorCobrancaPartilhadoTitulo).Append("\n");
            sb.Append("  NomeSacadoCobranca: ").Append(NomeSacadoCobranca).Append("\n");
            sb.Append("  TextoEnderecoSacadoCobranca: ").Append(TextoEnderecoSacadoCobranca).Append("\n");
            sb.Append("  NomeBairroSacadoCobranca: ").Append(NomeBairroSacadoCobranca).Append("\n");
            sb.Append("  NomeMunicipioSacadoCobranca: ").Append(NomeMunicipioSacadoCobranca).Append("\n");
            sb.Append("  SiglaUnidadeFederacaoSacadoCobranca: ").Append(SiglaUnidadeFederacaoSacadoCobranca).Append("\n");
            sb.Append("  NumeroCepSacadoCobranca: ").Append(NumeroCepSacadoCobranca).Append("\n");
            sb.Append("  ValorMoedaAbatimentoTitulo: ").Append(ValorMoedaAbatimentoTitulo).Append("\n");
            sb.Append("  DataProtestoTituloCobranca: ").Append(DataProtestoTituloCobranca).Append("\n");
            sb.Append("  CodigoTipoInscricaoSacador: ").Append(CodigoTipoInscricaoSacador).Append("\n");
            sb.Append("  NumeroInscricaoSacadorAvalista: ").Append(NumeroInscricaoSacadorAvalista).Append("\n");
            sb.Append("  NomeSacadorAvalistaTitulo: ").Append(NomeSacadorAvalistaTitulo).Append("\n");
            sb.Append("  PercentualDescontoTitulo: ").Append(PercentualDescontoTitulo).Append("\n");
            sb.Append("  DataDescontoTitulo: ").Append(DataDescontoTitulo).Append("\n");
            sb.Append("  ValorDescontoTitulo: ").Append(ValorDescontoTitulo).Append("\n");
            sb.Append("  CodigoDescontoTitulo: ").Append(CodigoDescontoTitulo).Append("\n");
            sb.Append("  PercentualSegundoDescontoTitulo: ").Append(PercentualSegundoDescontoTitulo).Append("\n");
            sb.Append("  DataSegundoDescontoTitulo: ").Append(DataSegundoDescontoTitulo).Append("\n");
            sb.Append("  ValorSegundoDescontoTitulo: ").Append(ValorSegundoDescontoTitulo).Append("\n");
            sb.Append("  CodigoSegundoDescontoTitulo: ").Append(CodigoSegundoDescontoTitulo).Append("\n");
            sb.Append("  PercentualTerceiroDescontoTitulo: ").Append(PercentualTerceiroDescontoTitulo).Append("\n");
            sb.Append("  DataTerceiroDescontoTitulo: ").Append(DataTerceiroDescontoTitulo).Append("\n");
            sb.Append("  ValorTerceiroDescontoTitulo: ").Append(ValorTerceiroDescontoTitulo).Append("\n");
            sb.Append("  CodigoTerceiroDescontoTitulo: ").Append(CodigoTerceiroDescontoTitulo).Append("\n");
            sb.Append("  DataMultaTitulo: ").Append(DataMultaTitulo).Append("\n");
            sb.Append("  NumeroCarteiraCobranca: ").Append(NumeroCarteiraCobranca).Append("\n");
            sb.Append("  NumeroVariacaoCarteiraCobranca: ").Append(NumeroVariacaoCarteiraCobranca).Append("\n");
            sb.Append("  QuantidadeDiaProtesto: ").Append(QuantidadeDiaProtesto).Append("\n");
            sb.Append("  QuantidadeDiaPrazoLimiteRecebimento: ").Append(QuantidadeDiaPrazoLimiteRecebimento).Append("\n");
            sb.Append("  DataLimiteRecebimentoTitulo: ").Append(DataLimiteRecebimentoTitulo).Append("\n");
            sb.Append("  IndicadorPermissaoRecebimentoParcial: ").Append(IndicadorPermissaoRecebimentoParcial).Append("\n");
            sb.Append("  TextoCodigoBarrasTituloCobranca: ").Append(TextoCodigoBarrasTituloCobranca).Append("\n");
            sb.Append("  CodigoOcorrenciaCartorio: ").Append(CodigoOcorrenciaCartorio).Append("\n");
            sb.Append("  ValorImpostoSobreOprFinanceirasRecebidoTitulo: ").Append(ValorImpostoSobreOprFinanceirasRecebidoTitulo).Append("\n");
            sb.Append("  ValorAbatimentoTotal: ").Append(ValorAbatimentoTotal).Append("\n");
            sb.Append("  ValorJuroMoraRecebido: ").Append(ValorJuroMoraRecebido).Append("\n");
            sb.Append("  ValorDescontoUtilizado: ").Append(ValorDescontoUtilizado).Append("\n");
            sb.Append("  ValorPagoSacado: ").Append(ValorPagoSacado).Append("\n");
            sb.Append("  ValorCreditoCedente: ").Append(ValorCreditoCedente).Append("\n");
            sb.Append("  CodigoTipoLiquidacao: ").Append(CodigoTipoLiquidacao).Append("\n");
            sb.Append("  DataCreditoLiquidacao: ").Append(DataCreditoLiquidacao).Append("\n");
            sb.Append("  DataRecebimentoTitulo: ").Append(DataRecebimentoTitulo).Append("\n");
            sb.Append("  CodigoPrefixoDependenciaRecebedor: ").Append(CodigoPrefixoDependenciaRecebedor).Append("\n");
            sb.Append("  CodigoNaturezaRecebimento: ").Append(CodigoNaturezaRecebimento).Append("\n");
            sb.Append("  NumeroIdentidadeSacadoTituloCobranca: ").Append(NumeroIdentidadeSacadoTituloCobranca).Append("\n");
            sb.Append("  CodigoResponsavelAtualizacao: ").Append(CodigoResponsavelAtualizacao).Append("\n");
            sb.Append("  CodigoTipoBaixaTitulo: ").Append(CodigoTipoBaixaTitulo).Append("\n");
            sb.Append("  ValorMultaRecebido: ").Append(ValorMultaRecebido).Append("\n");
            sb.Append("  ValorReajuste: ").Append(ValorReajuste).Append("\n");
            sb.Append("  ValorOutroRecebido: ").Append(ValorOutroRecebido).Append("\n");
            sb.Append("  CodigoIndicadorEconomicoUtilizadoInadimplencia: ").Append(CodigoIndicadorEconomicoUtilizadoInadimplencia).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Get the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}