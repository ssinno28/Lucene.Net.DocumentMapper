using System.Reflection;

namespace Lucene.Net.DocumentMapper.Interfaces
{
    public interface IPropertyMapperManager
    {
        IFieldMapper GetPropertyMapper(PropertyInfo propertyInfo);
    }
}