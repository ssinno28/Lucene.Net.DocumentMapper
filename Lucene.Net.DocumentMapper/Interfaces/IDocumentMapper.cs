using System;
using System.Reflection;
using Lucene.Net.Documents;

namespace Lucene.Net.DocumentMapper.Interfaces
{
    public interface IDocumentMapper
    {
        T Map<T>(Document source);
        object Map(Document source, Type contentType);
        Document Map(object source);
        IFieldMapper GetFieldMapper(PropertyInfo propertyInfo);
    }
}