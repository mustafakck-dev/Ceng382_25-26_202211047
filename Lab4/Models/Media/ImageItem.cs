namespace Lab4.Models.Media
{
    public class ImageItem
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long Size { get; set; }
        public byte[] Data { get; set; } = Array.Empty<byte>();
    }
}