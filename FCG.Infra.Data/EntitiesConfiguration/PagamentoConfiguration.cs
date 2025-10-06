using FCG.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FCG.Infra.Data.EntitiesConfiguration
{
    public class PagamentoConfiguration
    {
        private static ModelBuilder _builder;
        public PagamentoConfiguration(ModelBuilder builder)
        {
            _builder = builder;
            Config();
        }

        private static void Config()
        {
            _builder.Entity<Pagamento>()
                .HasKey(x => x.Id);

            _builder.Entity<Pagamento>()
                .Property(x => x.UsuarioId)
                .IsRequired();

            _builder.Entity<Pagamento>()
                .Property(x => x.JogoId)
                .IsRequired();

            _builder.Entity<Pagamento>()
                .Property(x => x.FormaPagamentoId)
                .IsRequired();

            _builder.Entity<Pagamento>()
                .Property(x => x.Valor)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            _builder.Entity<Pagamento>()
                .Property(x => x.Quantidade)
                .IsRequired();
        }
    }
}