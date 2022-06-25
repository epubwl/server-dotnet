namespace EpubWebLibraryServer.Areas.Library.Models
{
    public class EpubFile
    {
        public int EpubId { get; set; }

        public byte[] BinaryData { get; set; } = {};
    }
}