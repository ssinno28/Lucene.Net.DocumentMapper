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

        /// <summary>
        /// The value of the field as a <see cref="System.DateTimeOffset"/>, or null.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public object? MapFromField(Field field)
        {
            var v = field.GetStringValue();
            return v == null ? null : DateTimeOffset.Parse(v);
        }
    }
}