namespace Zhasyl.Api.Domain.Content;

public sealed class Laboratory
{
    public Guid Id { get; set; }
    public Guid StationId { get; set; }
    public required string Slug { get; set; }
    public int Order { get; set; }
    public bool IsPublished { get; set; }
    public Station Station { get; set; } = null!;
    public ICollection<LaboratoryTranslation> Translations { get; set; } = [];
    public ICollection<Mission> Missions { get; set; } = [];
}

public sealed class LaboratoryTranslation
{
    public Guid LaboratoryId { get; set; }
    public required string Locale { get; set; }
    public required string Name { get; set; }
    public required string Purpose { get; set; }
    public required string Specialist { get; set; }
    public Laboratory Laboratory { get; set; } = null!;
}
