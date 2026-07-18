using DoctorsManagementSystem.model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoctorsManagementSystem.Configurations;

class AppointmentConfigurations : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.HasKey(x => x.AppointmentId);

        builder.Property(x => x.ReasonForVisit)
               .HasMaxLength(300)
               .IsRequired(true);

        builder.Property(x => x.Status)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired(true);

        builder.HasOne(a => a.patient)
               .WithMany()
               .HasForeignKey(a => a.PatientId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}