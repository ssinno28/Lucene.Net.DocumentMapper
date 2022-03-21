using System.Reflection;
using Lucene.Net.DocumentMapper.Attributes;
using Lucene.Net.DocumentMapper.Interfaces;
using Lucene.Net.Documents;

namespace Lucene.Net.DocumentMapper.FieldMappers
{
    /// <summary>
    /// A field that is indexed and tokenized, without term vectors.
    /// </summary>
    public class TextFieldMapper : AFieldMapper, IFieldMapper
    {
        public int Priority => 1;

        public bool IsMatch(PropertyInfo propertyInfo)
        {
            var type = GetPropertyType(propertyInfo);
            var searchAttribute = propertyInfo.GetCustomAttribute<SearchAttribute>();
            return searchAttribute != null && searchAttribute.Tokenized && type == typeof(string);
        }

        /// <summary>
        /// A field that is indexed and tokenized, without term vectors.
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="value">System.String value</param>
        /// <param name="name">field name</param>
        /// <returns></returns>
        public Field MapToField(PropertyInfo propertyInfo, object value, string name)
        {
            if (value.ToString().Length > 32766)
            {
                return new Field(name, value.ToString(), GetStore(propertyInfo), Field.Index.NO);
            }

            return new TextField(name, value.ToString(), GetStore(propertyInfo));
        }

        /// <summary>
        /// The value of the field as a <see cref="System.String"/>, or null.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public object? MapFromField(Field value)
        {
            return value.GetStringValue();
        }
    }
}