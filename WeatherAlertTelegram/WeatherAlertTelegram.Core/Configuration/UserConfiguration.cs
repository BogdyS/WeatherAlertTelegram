using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeatherAlertTelegram.Core.Entities;

namespace WeatherAlertTelegram.Core.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public const int CityMaxLength = 300;
    public const int UserNameMaxLength = 256;

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired(true)
            .HasMaxLength(UserNameMaxLength);

        builder.Property(x => x.City)
            .IsRequired(true)
            .HasMaxLength(CityMaxLength);

        builder.Property(x => x.ChatId)
            .IsRequired();
    }
}