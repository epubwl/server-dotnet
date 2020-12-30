using System;

namespace EpubWebLibraryServer.Areas.Library.Data
{
    public class EpubMetadata
    {
        public int EpubId { get; set; }

        public string Owner { get; set; }

        public string Identifier { get; set; }

        public string Title { get; set; }

        public string Creators { get; set; }

        public string Contributors { get; set; }

        public string Publisher { get; set; }

        public DateTime? Date { get; set; }

        public string Languages { get; set; }

        public string Description { get; set; }
    }
}