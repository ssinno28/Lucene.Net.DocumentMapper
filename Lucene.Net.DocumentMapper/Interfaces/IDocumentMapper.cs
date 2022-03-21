using System;
using System.Reflection;
using Lucene.Net.Documents;

namespace Lucene.Net.DocumentMapper.Interfaces
{
    /// <summary>
    /// Map .NET Types to Lucene Documents and back.
    /// </summary>
    public interface IDocumentMapper
    {
        T Map<T>(Document source);
        object Map(Document source, Type contentType);
        Document Map(object source);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyInfo">a new instance of the <see cref="System.Reflection.PropertyInfo"/> class, or null.</param>
        /// <returns></returns>
        IFieldMapper? GetFieldMapper(PropertyInfo? propertyInfo);
    }
}