using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Data.EntityConfigurations
{
    public class ClinicalBreakpointConfiguration : IEntityTypeConfiguration<ClinicalBreakpoint>
    {
        public void Configure(EntityTypeBuilder<ClinicalBreakpoint> builder)
        {
            builder.Ignore(b => b.Title);
            builder.Ignore(b => b.NotAvailable);
            
            builder.Property(b => b.Version).IsRequired();
            builder.Property(b => b.AntifungalAgentDetails).IsRequired();

            builder.HasIndex(b => new { b.Standard, b.Version, b.Species, b.AntifungalAgentDetails}).IsUnique();
        }
    }
}