namespace Zhasyl.Api.Domain.Content;

public sealed class Station
{
    public Guid Id { get; set; }
    public required string Slug { get; set; }
    public ICollection<StationTranslation> Translations { get; set; } = [];
    public ICollection<Laboratory> Laboratories { get; set; } = [];
}

public sealed class StationTranslation
{
    public Guid StationId { get; set; }
    public required string Locale { get; set; }
    public required string Name { get; set; }
    public required string Location { get; set; }
    public required string Briefing { get; set; }
    public Station Station { get; set; } = null!;
}
