namespace Zhasyl.Api.Features.Content.Seeding;

public enum ContentKind
{
    Station,
    Laboratory,
    Mission
}

public sealed record ContentSeedDocument(
    ContentKind Kind,
    string Slug,
    string Locale,
    string? Station,
    string? Laboratory,
    int Order,
    string Title,
    string? Location,
    string? Briefing,
    string? Purpose,
    string? Specialist,
    string? Problem,
    string? Status,
    bool IsPublished,
    string BodyMdx,
    string ContentHash,
    string SourceLocation);

public interface IContentSeedSource
{
    Task<IReadOnlyList<ContentSeedDocument>> LoadAsync(CancellationToken cancellationToken);
}
