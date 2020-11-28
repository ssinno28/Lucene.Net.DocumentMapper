using System;
using System.Collections.Generic;
using Lucene.Net.Documents;

namespace Lucene.Net.DocumentMapper.Interfaces
{
    public interface IFieldsMapper
    {
        bool IsMatch(Type @type);
        IList<Field> MapToFields(object @object);
    }
}