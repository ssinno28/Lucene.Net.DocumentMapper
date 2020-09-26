using System.Reflection;
using Lucene.Net.Documents;

namespace Lucene.Net.DocumentMapper.Interfaces
{
    public interface IFieldMapper
    {
        int Priority { get; }
        bool IsMatch(PropertyInfo propertyInfo);
        Field MapToField(PropertyInfo propertyInfo, object value, string name);
        object MapFromField(Field field);
    }
}