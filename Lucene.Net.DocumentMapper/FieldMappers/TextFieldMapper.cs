using System.Reflection;
using Lucene.Net.DocumentMapper.Attributes;
using Lucene.Net.DocumentMapper.Interfaces;
using Lucene.Net.Documents;

namespace Lucene.Net.DocumentMapper.FieldMappers
{
    public class TextFieldMapper : AFieldMapper, IFieldMapper
    {
        public int Priority => 1;

        public bool IsMatch(PropertyInfo propertyInfo)
        {
            var type = GetPropertyType(propertyInfo);
            var searchAttribute = propertyInfo.GetCustomAttribute<SearchAttribute>();
            return searchAttribute != null && searchAttribute.Tokenized && type == typeof(string);
        }

        public Field MapToField(PropertyInfo propertyInfo, object value, string name)
        {
            if (value.ToString().Length > 32766)
            {
                return new Field(name, value.ToString(), GetStore(propertyInfo), Field.Index.NO);
            }

            return new TextField(name, value.ToString(), GetStore(propertyInfo));
        }

        public object MapFromField(Field value)
        {
            return value.GetStringValue();
        }
    }
}