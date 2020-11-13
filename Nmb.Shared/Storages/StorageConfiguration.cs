namespace Nmb.Shared.Storages
{
    public class StorageConfiguration
    {
        public string Url { get; set; }
        public string Region { get; set; }
        public string BucketName { get; set; }
        public string ImagesPath { get; set; }
        public string PublicAssetsPath { get; set; }
        public string PrivateAssetsPath { get; set; }
    }
}
