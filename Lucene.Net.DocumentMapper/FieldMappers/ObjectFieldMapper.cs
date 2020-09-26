using System.Reflection;
using Lucene.Net.DocumentMapper.Helpers;
using Lucene.Net.DocumentMapper.Interfaces;
using Lucene.Net.Documents;
using Newtonsoft.Json;

namespace Lucene.Net.DocumentMapper.FieldMappers
{
    public class ObjectFieldMapper : AFieldMapper, IFieldMapper
    {
        public int Priority => 0;

        public bool IsMatch(PropertyInfo propertyInfo)
        {
            var type = GetPropertyType(propertyInfo);
            return !type.IsPrimitiveType() && !type.IsACollection() && type == typeof(object);
        }

        public Field MapToField(PropertyInfo propertyInfo, object value, string name)
        {
            return new StringField(name, JsonConvert.SerializeObject(value), GetStore(propertyInfo));
        }

        public object MapFromField(Field field)
        {
            return JsonConvert.DeserializeObject(field.GetStringValue());
        }
    }
}