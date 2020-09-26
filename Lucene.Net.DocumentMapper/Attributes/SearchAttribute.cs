using System;

namespace Lucene.Net.DocumentMapper.Attributes
{
    public class SearchAttribute : Attribute
    {
        public SearchAttribute()
        {
            Store = true;
        }
        public bool Tokenized { get; set; }
        public bool Store { get; set; }
    }
}