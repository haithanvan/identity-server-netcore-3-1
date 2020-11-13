using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Nmb.Shared.Storages
{
    public interface IStorageProvider
    {
        Task UploadAsync(string objectKey, Stream data, CancellationToken cancellationToken = default(CancellationToken));
        Task UploadAsync(string objectKey, string path, CancellationToken cancellationToken = default(CancellationToken));
        Task<MultipartUploadDelegation> CreateMultipartUploadAsync(string key, long fileSize, long filePartSize, string contentType = null, CancellationToken cancellationToken = default);
        Task CompleteMultiparUploadTaskAsync(string key, string uploadId, IList<MultipartUploadPartChecksum> parts, CancellationToken cancellationToken = default);
        Task AbortMultipartUploadTaskAsync(string key, string uploadId);
        string CreateUploadUrl(string key, string contentType = null);
        Task<FileInfo> GetObjectInfoAsync(string key);
        string GetSignedDownloadUrl(string key, string fileName = null);
    }
}