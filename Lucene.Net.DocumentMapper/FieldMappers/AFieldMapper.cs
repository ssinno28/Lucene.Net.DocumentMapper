using System;
using System.Linq;
using System.Reflection;
using Lucene.Net.DocumentMapper.Attributes;
using Lucene.Net.DocumentMapper.Helpers;
using Lucene.Net.Documents;

namespace Lucene.Net.DocumentMapper.FieldMappers
{
    public abstract class AFieldMapper
    {
        protected Type GetPropertyType(PropertyInfo propertyInfo)
        {
            if (propertyInfo.IsPropertyACollection())
            {
                return propertyInfo.GetPropertyType().GetGenericArguments().Single();
            }

            return propertyInfo.GetPropertyType();
        }

        protected Field.Store GetStore(PropertyInfo propertyInfo)
        {
            var searchAttribute = propertyInfo.GetCustomAttribute<SearchAttribute>();
            if (searchAttribute == null)
            {
                return Field.Store.YES;
            }

            return searchAttribute.Store ? Field.Store.YES : Field.Store.NO;
        }
    }
}