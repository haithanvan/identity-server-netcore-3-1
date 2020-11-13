using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace Nmb.Shared.Utils
{
    public static class FileUtils
    {
        private const string TempFolderName = "temp";
        public static List<T> ReadCsv<T>(string path, ILogger logger, string[] headers, Func<string[], string[], T> mapper) where T : class
        {
            if (!File.Exists(path))
            {
                logger.LogWarning($"Data file not found at {path}");
                return new List<T>();
            }

            try
            {
                headers = ValidateHeaders(path, headers);
            }
            catch (Exception e)
            {
                logger.LogWarning($"Data file at {path} is not valid", e);
            }

            var items = File.ReadAllLines(path)
                .Skip(1) // skip header column
                .Select(row => Regex.Split(row, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)")
                    .Select(t => t.Trim('"')).ToArray()) 
                .Select(column => mapper(column, headers))
                .Where(x => x != null)
                .ToList();

            return items;
        }

        private static string[] ValidateHeaders(string csvFile, string[] requiredHeaders)
        {
            var csvheaders = File.ReadLines(csvFile).First().ToLowerInvariant().Split(',');

            foreach (var requiredHeader in requiredHeaders)
            {
                if (!csvheaders.Contains(requiredHeader))
                {
                    throw new Exception($"does not contain required header '{requiredHeader}'");
                }
            }

            return csvheaders;
        }

        public static string GetFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        public static string SearchForFile(string fileName, string folderPath)
        {
            var dirInfo = new DirectoryInfo(folderPath);
            var files = dirInfo.GetFiles(fileName, SearchOption.AllDirectories);
            var fileInfo = files.FirstOrDefault();
            if (fileInfo == null)
            {
                return null;
            }
            var uri1 = new Uri(dirInfo.FullName);
            var uri2 = new Uri(fileInfo.FullName);
            var relativeUri = uri1.MakeRelativeUri(uri2);
            return relativeUri.OriginalString;
        }

        public static bool TryRead(string path, out string text)
        {
            text = string.Empty;
            if (!File.Exists(path)) return false;
            text = File.ReadAllText(path);
            return true;
        }

        public static string GetTempFolderPath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), TempFolderName);
        }

        public static string GetTempFilePath()
        {
            return Path.Combine(GetTempFolderPath(), Guid.NewGuid().ToString("N"));
        }
    }
}
