using DoctorsManagementSystem.model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoctorsManagementSystem.Configurations;

class PrescriptionConfigurations : IEntityTypeConfiguration<Prescription>
{
    public void Configure(EntityTypeBuilder<Prescription> builder)
    {
        builder.HasKey(x => x.PrescriptionId);
        
        builder.HasOne(p => p.patient)        
        .WithMany(b => b.prescriptions)       
        .HasForeignKey(p => p.PatientId);

        builder.Property(x => x.SurgeryName)
       .HasMaxLength(150)
       .IsRequired(true);
    }
}