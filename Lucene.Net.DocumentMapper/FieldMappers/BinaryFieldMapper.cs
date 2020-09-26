using System.Reflection;
using Lucene.Net.DocumentMapper.Helpers;
using Lucene.Net.DocumentMapper.Interfaces;
using Lucene.Net.Documents;
using Lucene.Net.Util;

namespace Lucene.Net.DocumentMapper.FieldMappers
{
    public class BinaryFieldMapper : AFieldMapper, IFieldMapper
    {
        public int Priority => 0;

        public bool IsMatch(PropertyInfo propertyInfo)
        {
            return propertyInfo.GetPropertyType() == typeof(byte[]);
        }

        public Field MapToField(PropertyInfo propertyInfo, object value, string name)
        {
            return new StoredField(name, new BytesRef((byte[]) value));
        }

        public object MapFromField(Field field)
        {
            var binaryValue = field.GetBinaryValue();
            return binaryValue.Bytes;
        }
    }
}