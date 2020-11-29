using System;
using System.Reflection;
using Lucene.Net.DocumentMapper.Interfaces;
using Lucene.Net.Documents;

namespace Lucene.Net.DocumentMapper.FieldMappers
{
    public class EnumFieldMapper : AFieldMapper, IFieldMapper
    {
        public int Priority => 0;

        public bool IsMatch(PropertyInfo propertyInfo)
        {
            var type = GetPropertyType(propertyInfo);
            return type.IsEnum;
        }

        public Field MapToField(PropertyInfo propertyInfo, object value, string name)
        {
            Int32 convertedValue = (Int32)value;
            return new Int32Field(name, convertedValue, GetStore(propertyInfo));
        }

        public object MapFromField(Field value)
        {
            return value.GetInt32Value();
        }
    }
}