using System;
using FCG.Domain.EventSourcing;

namespace FCG.Domain.EventSourcing.Events
{
    public class PagamentoIniciadoEvent : Event
    {
        public PagamentoIniciadoEvent(
            Guid correlationId,
            int pagamentoId,
            int usuarioId,
            int jogoId,
            int formaPagamentoId,
            decimal valor,
            int quantidade)
            : base(correlationId, pagamentoId)
        {
            UsuarioId = usuarioId;
            JogoId = jogoId;
            FormaPagamentoId = formaPagamentoId;
            Valor = valor;
            Quantidade = quantidade;
        }

        public int UsuarioId { get; }
        public int JogoId { get; }
        public int FormaPagamentoId { get; }
        public decimal Valor { get; }
        public int Quantidade { get; }
    }
}
