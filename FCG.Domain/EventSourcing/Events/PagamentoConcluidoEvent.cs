using System;
using FCG.Domain.EventSourcing;

namespace FCG.Domain.EventSourcing.Events
{
    public class PagamentoConcluidoEvent : Event
    {
        public PagamentoConcluidoEvent(
            Guid correlationId,
            int pagamentoId,
            bool sucesso,
            string mensagem,
            decimal valor)
            : base(correlationId, pagamentoId)
        {
            Sucesso = sucesso;
            Mensagem = mensagem ?? string.Empty;
            Valor = valor;
        }

        public bool Sucesso { get; }
        public string Mensagem { get; }
        public decimal Valor { get; }
    }
}
