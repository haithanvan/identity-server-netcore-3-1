using System;
using System.Collections.Generic;

namespace Nmb.Shared.Storages
{
    public class MultipartUploadDelegation
    {
        public string UploadId { get; private set; }
        public string Key { get; private set; }
        public long FilePartSize { get; private set; }
        public long FileSize { get; private set; }
        public IList<MultipartPartialUploadDelegation> Parts { get; private set; }

        public MultipartUploadDelegation(string key, string uploadId, long fileSize, long filePartSize)
        {
            UploadId = uploadId;
            Key = key;
            FileSize = fileSize;
            FilePartSize = filePartSize;
            Parts = new List<MultipartPartialUploadDelegation>();
        }

        public void AddPart(int partNumber, string url)
        {
            var start = (partNumber - 1) * FilePartSize;
            var end = Math.Min(start + FilePartSize, FileSize);
            AddPart(new MultipartPartialUploadDelegation
            {
                PartNumber = partNumber,
                SignedUrl = url,
                Start = start,
                End = end,
            });
        }

        public void AddPart(MultipartPartialUploadDelegation part)
        {
            Parts.Add(part);
        }
    }

    public class MultipartPartialUploadDelegation
    {
        public int PartNumber { get; set; }
        public string SignedUrl { get; set; }
        public long Start { get; set; }
        public long End { get; set; }
    }
}
