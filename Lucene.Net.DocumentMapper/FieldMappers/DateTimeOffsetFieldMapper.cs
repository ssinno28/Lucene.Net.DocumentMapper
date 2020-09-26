using System;
using System.Reflection;
using Lucene.Net.DocumentMapper.Interfaces;
using Lucene.Net.Documents;

namespace Lucene.Net.DocumentMapper.FieldMappers
{
    public class DateTimeOffsetFieldMapper : AFieldMapper, IFieldMapper
    {
        public int Priority => 0;

        public bool IsMatch(PropertyInfo propertyInfo)
        {
            var type = GetPropertyType(propertyInfo);
            return type == typeof(DateTimeOffset);
        }

        public Field MapToField(PropertyInfo propertyInfo, object value, string name)
        {
            return new StringField(name, value.ToString(), GetStore(propertyInfo));
        }

        public object MapFromField(Field field)
        {
            return DateTimeOffset.Parse(field.GetStringValue());
        }
    }
}