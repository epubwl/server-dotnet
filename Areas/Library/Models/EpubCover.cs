namespace EpubWebLibraryServer.Areas.Library.Models
{
    public class EpubCover
    {
        public int EpubId { get; set; }

        public byte[] BinaryData { get; set; } = {};

        public string Mimetype { get; set; } = "";
    }
}