namespace FCG.Domain.Entities
{
    public class Pagamento
    {
        public int Id { get; private set; }
        public int UsuarioId { get; private set; }
        public int JogoId { get; private set; }
        public int FormaPagamentoId { get; private set; }
        public decimal Valor { get; private set; }
        public int Quantidade { get; private set; }

        public Pagamento() { }

        public Pagamento(int usuarioId, int jogoId, int formaPagamentoId, decimal valor, int quantidade)
        {
            UsuarioId = usuarioId;
            JogoId = jogoId;
            FormaPagamentoId = formaPagamentoId;
            Valor = valor;
            Quantidade = quantidade;
        }
    }
}
