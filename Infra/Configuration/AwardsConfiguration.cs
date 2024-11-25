using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Configuration
{
    public class AwardsConfiguration : IEntityTypeConfiguration<Award>
    {
        public void Configure(EntityTypeBuilder<Award> builder)
        {
            builder.ToTable("Awards");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Year).IsRequired();
            builder.Property(a => a.Title).IsRequired();
            builder.Property(a => a.Studios).IsRequired();
            builder.Property(a => a.Producers).IsRequired();
            builder.Property(a => a.IsWinner).IsRequired();
        }
    }
}
