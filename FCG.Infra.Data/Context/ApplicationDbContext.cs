using FCG.Domain.Entities;
using FCG.Infra.Data.EntitiesConfiguration;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace FCG.Infra.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }
        public DbSet<Pagamento> Pagamento { get; set; }
        public DbSet<PagamentoDetalhe> PagamentoDetalhe { get; set; }
        public DbSet<StoredEvent> StoredEvent { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            _ = new PagamentoConfiguration(builder);
            builder.Entity<PagamentoDetalhe>().HasNoKey().ToView(null);
        }    }
}
