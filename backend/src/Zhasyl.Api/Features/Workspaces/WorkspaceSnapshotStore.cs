using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Zhasyl.Api.Features.Workspaces;

public interface IWorkspaceSnapshotStore
{
    Task WriteAsync(string blobName, string content, CancellationToken cancellationToken);
    Task<string> ReadAsync(string blobName, CancellationToken cancellationToken);
}

public sealed class AzureBlobWorkspaceSnapshotStore(BlobServiceClient blobServiceClient)
    : IWorkspaceSnapshotStore
{
    private const string ContainerName = "workspace-snapshots";

    public async Task WriteAsync(string blobName, string content, CancellationToken cancellationToken)
    {
        var container = blobServiceClient.GetBlobContainerClient(ContainerName);
        await container.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);
        var blob = container.GetBlobClient(blobName);
        await blob.UploadAsync(
            BinaryData.FromString(content),
            new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = "text/x-python; charset=utf-8",
                },
            },
            cancellationToken);
    }

    public async Task<string> ReadAsync(string blobName, CancellationToken cancellationToken)
    {
        var blob = blobServiceClient
            .GetBlobContainerClient(ContainerName)
            .GetBlobClient(blobName);
        var response = await blob.DownloadContentAsync(cancellationToken);
        return response.Value.Content.ToString();
    }
}
