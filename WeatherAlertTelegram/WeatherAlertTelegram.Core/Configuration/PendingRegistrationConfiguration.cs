using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeatherAlertTelegram.Core.Entities;

namespace WeatherAlertTelegram.Core.Configuration;

public class PendingRegistrationConfiguration : IEntityTypeConfiguration<PendingRegistrations>
{
    public void Configure(EntityTypeBuilder<PendingRegistrations> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ChatId)
            .IsRequired(true);

        builder.Property(x => x.Step)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(x => x.City)
            .IsRequired(false);

        builder.Property(x => x.UserName)
            .IsRequired(false);
    }
}