namespace BuberDinner.Infrastructure.Persistence;

public class MssqlSettings
{
    public const string SectionName = "MssqlSettings";

    public string ConnectionString { get; init; } = null!;
}