using System;
using System.Collections.Generic;
using Lucene.Net.DocumentMapper.Attributes;

namespace Lucene.Net.DocumentMapper.Tests.Models
{
    public class BlogPost
    {
        public DateTime PublishedDate { get; set; }
        public DateTimeOffset PublishedDateOffset { get; set; }
        public bool IsPublished { get; set; }
        public string Name { get; set; }
        [Search(Tokenized = true)]
        public string Body { get; set; }
        public string SeoDescription { get; set; }
        [Search(Store = false)]
        public string SeoTitle { get; set; }
        public string Excerpt { get; set; }
        public string ThumbnailUrl { get; set; }
        public IList<string> TagIds { get; set; }
        public object Category { get; set; }
        public Category Category2 { get; set; }
        public IList<Tag> Tags { get; set; }

        public EnumCategory Category3 { get; set; }
    }

    public enum EnumCategory
    {
        Developoment,
        Database
    }
}