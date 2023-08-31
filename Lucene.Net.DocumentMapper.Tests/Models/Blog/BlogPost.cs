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
        public string Name { get; set; } = string.Empty;
        [Search(Tokenized = true)]
        public string Body { get; set; } = string.Empty;
        public string SeoDescription { get; set; } = string.Empty;
        [Search(Store = false)]
        public string SeoTitle { get; set; } = string.Empty;
        public string Excerpt { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public IList<string> TagIds { get; set; } = new List<string>();
        public object? Category { get; set; }
        public Category? Category2 { get; set; }
        public IList<Tag> Tags { get; set; } = new List<Tag>();
        public string[] TagsArray { get; set; }
        public byte[] Thumbnail { get; set; }

        public EnumCategory Category3 { get; set; }
    }

    public enum EnumCategory
    {
        Developoment,
        Database
    }
}