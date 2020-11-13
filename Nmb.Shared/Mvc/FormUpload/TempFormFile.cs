using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace Nmb.Shared.Mvc.FormUpload
{
    public class TempFormFile
    {
        public TempFormFile()
        {

        }
        public TempFormFile(ContentDispositionHeaderValue contentDisposition, string tempPath, long length)
        {
            Name = HeaderUtilities.RemoveQuotes(contentDisposition.Name).ToString();
            var fileNameStar = contentDisposition.FileNameStar;
            FileName = HeaderUtilities.RemoveQuotes(fileNameStar.HasValue
                ? contentDisposition.FileNameStar
                : contentDisposition.FileName).ToString();
            TempPath = tempPath;
            Length = length;
            ContentDisposition = contentDisposition.ToString();
        }

        public string Name { get; set; }
        public string FileName { get; set; }

        public string ContentDisposition { get; set; }

        public string ContentType { get; set; }

        public string TempPath { get; set; }
        public long Length { get; set; }


        public Stream OpenReadStream()
        {
            return File.OpenRead(TempPath);
        }

        public IEnumerable<(string, StringValues)> GetValues()
        {
            yield return ($"{Name}.{nameof(Name)}", Name);
            yield return ($"{Name}.{nameof(FileName)}", FileName);
            yield return ($"{Name}.{nameof(TempPath)}", TempPath);
            yield return ($"{Name}.{nameof(Length)}", Length.ToString());
            yield return ($"{Name}.{nameof(ContentDisposition)}", ContentDisposition);
            yield return ($"{Name}.{nameof(ContentType)}", ContentType);
        }
    }
}
