namespace FCG.API.Models
{
    public class PagamentoViewModel
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int JogoId { get; set; }
        public int FormaPagamentoId { get; set; }
        public decimal Valor { get; set; }
        public int Quantidade { get; set; }
        public string EmailDestino { get; set; } = "vbredadj@gmail.com";
    }
}
