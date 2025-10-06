using FCG.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG.Infra.Data.EntitiesConfiguration
{
    public class StoredEventConfiguration : IEntityTypeConfiguration<StoredEvent>
    {
        public void Configure(EntityTypeBuilder<StoredEvent> builder)
        {
            builder.ToTable("StoredEvent");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.EventType)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(e => e.AggregateId)
                .IsRequired();

            builder.Property(e => e.Data)
                .IsRequired();

            builder.Property(e => e.Timestamp)
                .IsRequired();

            builder.Property(e => e.Version)
                .IsRequired();
        }
    }
}
