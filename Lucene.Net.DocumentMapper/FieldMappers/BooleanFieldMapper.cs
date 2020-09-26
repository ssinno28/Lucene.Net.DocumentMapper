using System;
using System.Reflection;
using Lucene.Net.DocumentMapper.Interfaces;
using Lucene.Net.Documents;

namespace Lucene.Net.DocumentMapper.FieldMappers
{
    public class BooleanFieldMapper : AFieldMapper, IFieldMapper
    {
        public int Priority => 0;

        public bool IsMatch(PropertyInfo propertyInfo)
        {
            var type = GetPropertyType(propertyInfo);
            return type == typeof(bool);
        }

        public Field MapToField(PropertyInfo propertyInfo, object value, string name)
        {
            bool convertedValue = (bool)value;
            return new StringField(name,
                convertedValue
                    ? Boolean.TrueString
                    : Boolean.FalseString, GetStore(propertyInfo));
        }

        public object MapFromField(Field field)
        {
            return Boolean.Parse(field.GetStringValue());
        }
    }
}