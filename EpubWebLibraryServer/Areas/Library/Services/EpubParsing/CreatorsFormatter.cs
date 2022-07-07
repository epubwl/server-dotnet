using System;
using System.Collections.Generic;
using System.Linq;

namespace EpubWebLibraryServer.Areas.Library.Services.EpubParsing
{
    public class CreatorsFormatter
    {
        public string Format(IDictionary<string, ICollection<string>> creatorRoles)
        {
            return String.Join(", ",
                creatorRoles.Select(kvp => {
                    return kvp.Value.Count > 0
                        ? $"{kvp.Key} ({String.Join(",", kvp.Value)})"
                        : kvp.Key;}));
        }
    }
}