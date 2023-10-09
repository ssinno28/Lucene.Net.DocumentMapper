using System;
using System.Globalization;
using System.Reflection;
using J2N.Text;
using Lucene.Net.DocumentMapper.Interfaces;
using Lucene.Net.Documents;

namespace Lucene.Net.DocumentMapper.FieldMappers
{
    public class DateTimeOffsetFieldMapper : AFieldMapper, IFieldMapper
    {
        private string _format = "yyyyMMddHHmmss.fffzzzzz";
        public int Priority => 0;

        public bool IsMatch(PropertyInfo propertyInfo)
        {
            var type = GetPropertyType(propertyInfo);
            return type == typeof(DateTimeOffset);
        }

        public Field MapToField(PropertyInfo propertyInfo, object value, string name)
        {
            DateTimeOffset convertedValue = (DateTimeOffset)value;
            var dateString = convertedValue.ToString(_format,  CultureInfo.InvariantCulture);
            return new StringField(name,
                dateString,
                GetStore(propertyInfo));
        }

        /// <summary>
        /// The value of the field as a <see cref="System.DateTimeOffset"/>, or null.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public object? MapFromField(Field field)
        {
            var dateString = field.GetStringValue();
            if (!DateTimeOffset.TryParseExact(dateString, _format, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeUniversal, out DateTimeOffset dateOffset))
                throw new ParseException($"Input is not valid date string: '{dateString}'.", 0);

            return dateOffset;
        }
    }
}