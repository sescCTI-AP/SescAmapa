namespace PagamentoApi.Models.BB
{
    public class Sacado
    {
        public int codigoTipoInscricaoSacado { get; set; }
        public string numeroInscricaoSacadoCobranca { get; set; }
        public string nomeSacadoCobranca { get; set; }
        public string textoEnderecoSacadoCobranca { get; set; }
        public string nomeBairroSacadoCobranca { get; set; }
        public string nomeMunicipioSacadoCobranca { get; set; }
        public string siglaUnidadeFederacaoSacadoCobranca { get; set; }
        public int numeroCepSacadoCobranca { get; set; }
        public double valorPagoSacado { get; set; }
        public string numeroIdentidadeSacadoTituloCobranca { get; set; }

    }
}