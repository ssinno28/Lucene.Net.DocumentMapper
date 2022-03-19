using System;
using System.Reflection;
using Lucene.Net.DocumentMapper.Interfaces;
using Lucene.Net.Documents;

namespace Lucene.Net.DocumentMapper.FieldMappers
{
    /// <summary>
    /// Field Mapper
    /// </summary>
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

        /// <summary>
        /// The value of the field as a <see cref="System.Boolean"/>, or null.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public object? MapFromField(Field field)
        {
            var v = field.GetStringValue();
            return v == null ? null : bool.Parse(v);
        }
    }
}