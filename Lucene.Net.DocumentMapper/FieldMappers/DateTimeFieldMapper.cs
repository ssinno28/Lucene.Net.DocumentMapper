using System;
using System.Reflection;
using Lucene.Net.DocumentMapper.Interfaces;
using Lucene.Net.Documents;

namespace Lucene.Net.DocumentMapper.FieldMappers
{
    public class DateTimeFieldMapper : AFieldMapper, IFieldMapper
    {
        public int Priority => 0;

        public bool IsMatch(PropertyInfo propertyInfo)
        {
            var type = GetPropertyType(propertyInfo);
            return type == typeof(DateTime);
        }

        public Field MapToField(PropertyInfo propertyInfo, object value, string name)
        {
            DateTime convertedValue = (DateTime)value;
            return new StringField(name, 
                DateTools.DateToString(convertedValue, DateResolution.MILLISECOND), // DateTools.Resolution.MILLISECOND 
                GetStore(propertyInfo));
        }

        /// <summary>
        /// The value of the field as a <see cref="System.DateTime"/>, or null.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public object? MapFromField(Field field)
        {
            var v = field.GetStringValue();
            // Attempt to use the Lucene.Net date conversion
            return v == null ? null : DateTools.StringToDate(v); // return DateTime.MinValue or null?
        }
    }
}