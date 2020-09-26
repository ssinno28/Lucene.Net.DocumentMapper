using System.Reflection;
using Lucene.Net.DocumentMapper.Interfaces;
using Lucene.Net.Documents;

namespace Lucene.Net.DocumentMapper.FieldMappers
{
    public class DoubleFieldMapper : AFieldMapper, IFieldMapper
    {
        public int Priority => 0;

        public bool IsMatch(PropertyInfo propertyInfo)
        {
            var type = GetPropertyType(propertyInfo);
            return type == typeof(double);
        }

        public Field MapToField(PropertyInfo propertyInfo, object value, string name)
        {
            double convertedValue = (double)value;
            return new DoubleField(name, convertedValue, GetStore(propertyInfo));
        }

        public object MapFromField(Field field)
        {
            return field.GetDoubleValue();
        }
    }
}