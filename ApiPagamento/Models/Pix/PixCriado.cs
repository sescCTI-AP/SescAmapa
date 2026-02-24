using System.Runtime.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace PagamentoApi.Models.Pix
{
    [DataContract]
    public class PixCriado
    {
        /// <summary>
        /// Os campos aninhados sob o identificador calendário organizam informações a respeito de controle de tempo da cobrança.
        /// </summary>
        /// <value></value>
        [DataMember(Name = "calendario")]
        public PixCalendario Calendario { get; set; }

        /// <summary>
        /// Status da Cobrança
        /// </summary>
        /// <value></value>
        [DataMember(Name = "status")]
        public string Status { get; set; }

        /// <summary>
        /// Id da Transação
        /// pattern: [A-Z0-9-]{1,35}
        /// </summary>
        /// <value></value>
        [DataMember(Name = "txid")]
        public string Txid { get; set; }


        [DataMember(Name = "PixCopiaECola")]
        public string PixCopiaECola { get; set; }

        /// <summary>
        /// Texto para exibir qrcode
        /// </summary>
        /// <value></value>
        [DataMember(Name = "textoImagemQRcode")]
        public string TextoImagemQRcode { get; set; }

        /// <summary>
        /// Revisão da cobrança. Sempre começa em zero. Sempre varia em acréscimos de 1.
        /// </summary>
        /// <value></value>
        [DataMember(Name = "revisao")]
        public string Revisao { get; set; }

        /// <summary>
        /// Localização do Payload a ser informada na criação da cobrança.
        /// example: https://pix.example.com/qr/2353c790-eefb-11ea-adc1-0242ac120002
        /// </summary>
        /// <value></value>
        [DataMember(Name = "location")]
        public string Location { get; set; }

        /// <summary>
        /// Os campos aninhados sob o objeto devedor são opcionais e identificam o devedor, ou seja, a pessoa ou a instituição a quem a cobrança está endereçada. Não identifica, necessariamente, quem irá efetivamente realizar o pagamento. Um CPF pode ser o devedor de uma cobrança, mas pode acontecer de outro CPF realizar, efetivamente, o pagamento do documento. Não é permitido que o campo pagador.cpf e campo pagador.cnpj estejam preenchidos ao mesmo tempo. Se o campo pagador.cnpj está preenchido, então o campo pagador.cpf não pode estar preenchido, e vice-versa. Se o campo pagador.nome está preenchido, então deve existir ou um pagador.cpf ou um campo pagador.cnpj preenchido.
        /// </summary>
        /// <value>Os campos aninhados sob o objeto devedor são opcionais e identificam o devedor, ou seja, a pessoa ou a instituição a quem a cobrança está endereçada. Não identifica, necessariamente, quem irá efetivamente realizar o pagamento. Um CPF pode ser o devedor de uma cobrança, mas pode acontecer de outro CPF realizar, efetivamente, o pagamento do documento. Não é permitido que o campo pagador.cpf e campo pagador.cnpj estejam preenchidos ao mesmo tempo. Se o campo pagador.cnpj está preenchido, então o campo pagador.cpf não pode estar preenchido, e vice-versa. Se o campo pagador.nome está preenchido, então deve existir ou um pagador.cpf ou um campo pagador.cnpj preenchido.</value>

        [DataMember(Name = "devedor")]
        public PixDevedor Devedor { get; set; }

        /// <summary>
        /// Gets or Sets Valor
        /// </summary>
        [Required]

        [DataMember(Name = "valor")]
        public PixValor Valor { get; set; }

        /// <summary>
        /// O campo chave, obrigatório, determina a chave Pix registrada no DICT que será utilizada para a cobrança. Essa chave será lida pelo aplicativo do PSP do pagador para consulta ao DICT, que retornará a informação que identificará o recebedor da cobrança.
        /// </summary>
        /// <value>O campo chave, obrigatório, determina a chave Pix registrada no DICT que será utilizada para a cobrança. Essa chave será lida pelo aplicativo do PSP do pagador para consulta ao DICT, que retornará a informação que identificará o recebedor da cobrança.</value>
        [Required]

        [MaxLength(77)]
        [DataMember(Name = "chave")]
        public string Chave { get; set; }

        /// <summary>
        /// O campo solicitacaoPagador, opcional, determina um texto a ser apresentado ao pagador para que ele possa digitar uma informação correlata, em formato livre, a ser enviada ao recebedor. Esse texto será preenchido, na pacs.008, pelo PSP do pagador, no campo RemittanceInformation &lt;RmtInf&gt;. O tamanho do campo &lt;RmtInf&gt; na pacs.008 está limitado a 140 caracteres.
        /// </summary>
        /// <value>O campo solicitacaoPagador, opcional, determina um texto a ser apresentado ao pagador para que ele possa digitar uma informação correlata, em formato livre, a ser enviada ao recebedor. Esse texto será preenchido, na pacs.008, pelo PSP do pagador, no campo RemittanceInformation &lt;RmtInf&gt;. O tamanho do campo &lt;RmtInf&gt; na pacs.008 está limitado a 140 caracteres.</value>

        [MaxLength(140)]
        [DataMember(Name = "solicitacaoPagador")]
        public string SolicitacaoPagador { get; set; }

        /// <summary>
        /// Cada respectiva informação adicional contida na lista (nome e valor) deve ser apresentada ao pagador.
        /// </summary>
        /// <value>Cada respectiva informação adicional contida na lista (nome e valor) deve ser apresentada ao pagador.</value>

        [DataMember(Name = "infoAdicionais")]
        public List<PixInfoAdicionais> InfoAdicionais { get; set; }

        /// <summary>
        /// Pix recebidos
        /// </summary>
        /// <value></value>
        [DataMember(Name = "pix")]
        public Pix[] Pix { get; set; }
    }
}