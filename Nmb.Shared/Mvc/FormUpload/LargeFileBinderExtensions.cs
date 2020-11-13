using Nmb.Shared.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Nmb.Shared.Mvc.FormUpload
{
    public static class LargeFileBinderExtensions
    {
        private static readonly FormOptions DefaultFormOptions = new FormOptions();

        public static async Task<(ICollection<TempFormFile>, KeyValueAccumulator)> ParseFormAsync(this HttpRequest request, ILogger logger)
        {
            if (!request.IsMultipartContentType())
            {
                throw new Exception($"Expected a multipart request, but got {request.ContentType}");
            }

            // Used to accumulate all the form url encoded key value pairs in the
            // request.
            var formAccumulator = new KeyValueAccumulator();
            var fileAccumulator = new List<TempFormFile>();

            var boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(request.ContentType),
                DefaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, request.Body);

            var section = await reader.ReadNextSectionAsync();
            while (section != null)
            {
                if (ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition))
                {
                    if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                    {
                        var targetFilePath = FileUtils.GetTempFilePath();
                        using (var targetStream = File.Create(targetFilePath))
                        {
                            await section.Body.CopyToAsync(targetStream);
                            logger.LogInformation($"Copied the uploaded file '{targetFilePath}'");
                        }

                        var headers = new HeaderDictionary(section.Headers);
                        fileAccumulator.Add(new TempFormFile(contentDisposition, targetFilePath, section.Body.Length)
                        {
                            ContentType = headers["Content-Type"],
                        });
                    }
                    else if (MultipartRequestHelper.HasFormDataContentDisposition(contentDisposition))
                    {
                        // Content-Disposition: form-data; name="key"
                        //
                        // value

                        // Do not limit the key name length here because the
                        // multipart headers length limit is already in effect.
                        var key = HeaderUtilities.RemoveQuotes(contentDisposition.Name);
                        var encoding = GetEncoding(section);
                        using (var streamReader = new StreamReader(
                            section.Body,
                            encoding,
                            true,
                            1024,
                            true))
                        {
                            // The value length limit is enforced by MultipartBodyLengthLimit
                            var value = await streamReader.ReadToEndAsync();
                            if (string.Equals(value, "undefined", StringComparison.OrdinalIgnoreCase))
                            {
                                value = string.Empty;
                            }
                            formAccumulator.Append(key.ToString(), value);

                            if (formAccumulator.ValueCount > DefaultFormOptions.ValueCountLimit)
                            {
                                throw new InvalidDataException($"Form key count limit {DefaultFormOptions.ValueCountLimit} exceeded.");
                            }
                        }
                    }
                }

                // Drains any remaining section body that has not been consumed and
                // reads the headers for the next section.
                section = request.Body.CanSeek && request.Body.Position == request.Body.Length ? null : await reader.ReadNextSectionAsync();
            }
            logger.LogInformation($"Number of file upload: '{fileAccumulator.Count}'");
            return (fileAccumulator, formAccumulator);
        }

        private static Encoding GetEncoding(MultipartSection section)
        {
            var hasMediaTypeHeader = MediaTypeHeaderValue.TryParse(section.ContentType, out var mediaType);
            // UTF-7 is insecure and should not be honored. UTF-8 will succeed in
            // most cases.
            if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
            {
                return Encoding.UTF8;
            }
            return mediaType.Encoding;
        }
    }
}