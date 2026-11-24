using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CronogramaCapilar.Infrastructure.Persistence.Converters;

public sealed class UtcDateTimeConverter() : ValueConverter<DateTime, DateTime>(
    toDatabase => toDatabase.Kind == DateTimeKind.Utc ? toDatabase : toDatabase.ToUniversalTime(),
    fromDatabase => DateTime.SpecifyKind(fromDatabase, DateTimeKind.Utc));
