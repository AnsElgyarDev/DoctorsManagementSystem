using DoctorsManagementSystem.model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoctorsManagementSystem.Configurations;

class PatientConfigurations : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.HasKey(x => x.PatientId);
        
        builder.Property(x => x.PatientName).
        HasMaxLength(100).
        IsRequired(true);

        builder.Property(x => x.TotalBill).
        HasDefaultValue(0).
        HasPrecision(10, 2);
    
        builder.Property(p => p.RegisteredAt)
        .HasDefaultValueSql("GETUTCDATE()");
    }
}