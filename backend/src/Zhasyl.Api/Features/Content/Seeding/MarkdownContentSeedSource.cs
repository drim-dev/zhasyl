using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Zhasyl.Api.Features.Content.Seeding;

public sealed class MarkdownContentSeedSource(IOptions<ContentOptions> options) : IContentSeedSource
{
    private static readonly IDeserializer YamlDeserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();

    public async Task<IReadOnlyList<ContentSeedDocument>> LoadAsync(
        CancellationToken cancellationToken)
    {
        var root = string.IsNullOrWhiteSpace(options.Value.Root)
            ? Path.Combine(AppContext.BaseDirectory, "Content")
            : Path.GetFullPath(options.Value.Root);

        if (!Directory.Exists(root))
        {
            throw new DirectoryNotFoundException($"Content root '{root}' does not exist.");
        }

        var documents = new List<ContentSeedDocument>();
        foreach (var file in Directory.EnumerateFiles(root, "*.mdx", SearchOption.AllDirectories)
                     .Order(StringComparer.Ordinal))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var source = await File.ReadAllTextAsync(file, cancellationToken);
            documents.Add(Parse(source, Path.GetRelativePath(root, file).Replace('\\', '/')));
        }

        return documents;
    }

    internal static ContentSeedDocument Parse(string source, string location)
    {
        var normalized = source.Replace("\r\n", "\n", StringComparison.Ordinal);
        var (frontmatter, body) = SplitFrontmatter(normalized, location);
        var metadata = YamlDeserializer.Deserialize<ContentFrontmatter>(frontmatter)
            ?? throw new InvalidDataException($"Content file '{location}' has empty frontmatter.");

        if (!string.Equals(metadata.Schema, "zhasyl.content/v1", StringComparison.Ordinal))
        {
            throw new InvalidDataException(
                $"Content file '{location}' must declare schema 'zhasyl.content/v1'.");
        }

        if (!Enum.TryParse<ContentKind>(metadata.Kind, true, out var kind))
        {
            throw new InvalidDataException($"Content file '{location}' has an unknown kind.");
        }

        Require(metadata.Slug, "slug", location);
        Require(metadata.Locale, "locale", location);
        Require(metadata.Title, "title", location);

        switch (kind)
        {
            case ContentKind.Station:
                Require(metadata.Location, "location", location);
                Require(metadata.Briefing, "briefing", location);
                break;
            case ContentKind.Laboratory:
                Require(metadata.Station, "station", location);
                Require(metadata.Purpose, "purpose", location);
                Require(metadata.Specialist, "specialist", location);
                RequirePositiveOrder(metadata.Order, location);
                break;
            case ContentKind.Mission:
                Require(metadata.Laboratory, "laboratory", location);
                Require(metadata.Problem, "problem", location);
                Require(metadata.Status, "status", location);
                RequirePositiveOrder(metadata.Order, location);
                if (string.IsNullOrWhiteSpace(body))
                {
                    throw new InvalidDataException($"Mission content file '{location}' has an empty MDX body.");
                }
                break;
            default:
                throw new InvalidDataException($"Content file '{location}' has an unsupported kind.");
        }

        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(normalized)))
            .ToLowerInvariant();

        return new ContentSeedDocument(
            kind,
            metadata.Slug!.Trim(),
            metadata.Locale!.Trim().ToLowerInvariant(),
            metadata.Station?.Trim(),
            metadata.Laboratory?.Trim(),
            metadata.Order,
            metadata.Title!.Trim(),
            metadata.Location?.Trim(),
            metadata.Briefing?.Trim(),
            metadata.Purpose?.Trim(),
            metadata.Specialist?.Trim(),
            metadata.Problem?.Trim(),
            metadata.Status?.Trim(),
            metadata.IsPublished,
            body.Trim(),
            hash,
            location);
    }

    private static (string Frontmatter, string Body) SplitFrontmatter(
        string source,
        string location)
    {
        const string openingFence = "---\n";
        const string closingFence = "\n---\n";

        if (!source.StartsWith(openingFence, StringComparison.Ordinal))
        {
            throw new InvalidDataException($"Content file '{location}' has no YAML frontmatter.");
        }

        var closingFenceIndex = source.IndexOf(closingFence, openingFence.Length, StringComparison.Ordinal);
        if (closingFenceIndex < 0)
        {
            throw new InvalidDataException($"Content file '{location}' has unclosed YAML frontmatter.");
        }

        var frontmatter = source[openingFence.Length..closingFenceIndex];
        var body = source[(closingFenceIndex + closingFence.Length)..];
        return (frontmatter, body);
    }

    private static void Require(string? value, string field, string location)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidDataException($"Content file '{location}' is missing '{field}'.");
        }
    }

    private static void RequirePositiveOrder(int order, string location)
    {
        if (order <= 0)
        {
            throw new InvalidDataException($"Content file '{location}' must have a positive order.");
        }
    }

    private sealed class ContentFrontmatter
    {
        public string? Schema { get; set; }
        public string? Kind { get; set; }
        public string? Slug { get; set; }
        public string? Locale { get; set; }
        public string? Station { get; set; }
        public string? Laboratory { get; set; }
        public int Order { get; set; }
        public string? Title { get; set; }
        public string? Location { get; set; }
        public string? Briefing { get; set; }
        public string? Purpose { get; set; }
        public string? Specialist { get; set; }
        public string? Problem { get; set; }
        public string? Status { get; set; }
        public bool IsPublished { get; set; } = true;
    }
}
