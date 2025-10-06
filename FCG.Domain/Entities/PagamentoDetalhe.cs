namespace FCG.Domain.Entities
{
    public class PagamentoDetalhe
    {
        public int PagamentoId { get; set; }
        public decimal TotalPagamento { get; set; }
        public string FormaPagamento { get; set; }
        public string Nome { get; set; }
        public string NomeJogo { get; set; }
        public string Email { get; set; }
    }
}