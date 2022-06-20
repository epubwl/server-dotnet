using System;
using System.Collections.Generic;

namespace EpubWebLibraryServer.Areas.Library.Models
{
    public class EpubMetadata
    {
        public int EpubId { get; set; }

        public string? Owner { get; set; }

        public ICollection<string> Tags { get; set; } = new List<string>();

        public string? Contributors { get; set; }

        public string? Coverage { get; set; }

        public string? Creators { get; set; }

        public DateTime? Date { get; set; }

        public string? Description { get; set; }

        public string? Format { get; set; }

        public string? Identifier { get; set; }

        public string? Languages { get; set; }

        public string? Publisher { get; set; }
        
        public string? Relation { get; set; }

        public string? Rights { get; set; }

        public string? Source { get; set; }

        public string? Subject { get; set; }

        public string? Title { get; set; }

        public string? Type { get; set; }
    }
}