using System;

namespace Lucene.Net.DocumentMapper.Attributes
{
    /// <summary>
    /// Search Attribute. The search attribute comes with three properties, Store, IsKey and Tokenized.
    /// Store is set to true by default and tokenized is false by default.
    /// If you set tokenzied to true for a string field it will store the string as a text field.
    /// </summary>
    public class SearchAttribute : Attribute
    {
        public SearchAttribute()
        {
            Store = true;
        }

        /// <summary>
        /// True会分词，创建索引时使用TextField。False不会分词，创建索引时使用StringField
        /// </summary>
        public bool Tokenized { get; set; }

        /// <summary>
        /// Lucene.Net.Documents.Field.Store.YES if the content should also be stored
        /// Field.Store.YES : Field.Store.NO
        /// </summary>
        public bool Store { get; set; }

        public bool IsKey { get; set; }
    }
}