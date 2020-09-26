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
                DateTools.DateToString(convertedValue, DateTools.Resolution.MILLISECOND), 
                GetStore(propertyInfo));
        }

        public object MapFromField(Field field)
        {
            return DateTools.StringToDate(field.GetStringValue());
        }
    }
}