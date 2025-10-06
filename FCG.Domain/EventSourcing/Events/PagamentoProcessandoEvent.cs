using System;
using FCG.Domain.EventSourcing;

namespace FCG.Domain.EventSourcing.Events
{
    public class PagamentoProcessandoEvent : Event
    {
        public PagamentoProcessandoEvent(
            Guid correlationId,
            int pagamentoId,
            string etapa,
            string destinoNotificacao,
            decimal valor)
            : base(correlationId, pagamentoId)
        {
            Etapa = etapa;
            DestinoNotificacao = destinoNotificacao;
            Valor = valor;
        }

        public string Etapa { get; }
        public string DestinoNotificacao { get; }
        public decimal Valor { get; }
    }
}
