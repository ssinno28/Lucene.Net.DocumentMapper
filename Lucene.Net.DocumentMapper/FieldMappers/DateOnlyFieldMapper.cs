using System;
using System.Reflection;
using Lucene.Net.DocumentMapper.Interfaces;
using Lucene.Net.Documents;
using Lucene.Net.Documents.Extensions;

namespace Lucene.Net.DocumentMapper.FieldMappers
{
    public class DateOnlyFieldMapper : AFieldMapper, IFieldMapper
    {
        public int Priority => 0;

        public bool IsMatch(PropertyInfo propertyInfo)
        {
            var type = GetPropertyType(propertyInfo);
            return type == typeof(DateOnly);
        }

        public Field MapToField(PropertyInfo propertyInfo, object value, string name)
        {
            var date = value is DateOnly only ? only : default;
            return new Int64Field(name, date.ToDateTime(TimeOnly.MinValue).Ticks, GetStore(propertyInfo));
        }

        public object MapFromField(Field field)
        {
            long ticks = field.GetInt64ValueOrDefault();
            return DateOnly.FromDateTime(new DateTime(ticks));
        }
    }
}