using System;
using System.Reflection;
using Lucene.Net.DocumentMapper.Interfaces;
using Lucene.Net.Documents;

namespace Lucene.Net.DocumentMapper.FieldMappers
{
    public class StringFieldMapper : AFieldMapper, IFieldMapper
    {
        public int Priority => 0;

        public bool IsMatch(PropertyInfo propertyInfo)
        {
            var type = GetPropertyType(propertyInfo);
            return type == typeof(string) || type == typeof(object) || type == typeof(String) || type == typeof(Object);
        }

        public Field MapToField(PropertyInfo propertyInfo, object value, string name)
        {
            if (value.ToString().Length > 32766)
            {
                return new Field(name, value.ToString(), GetStore(propertyInfo), Field.Index.NO);
            }

            return new StringField(name, value.ToString(), GetStore(propertyInfo));
        }

        public object MapFromField(Field value)
        {
            return value.GetStringValue();
        }
    }
}