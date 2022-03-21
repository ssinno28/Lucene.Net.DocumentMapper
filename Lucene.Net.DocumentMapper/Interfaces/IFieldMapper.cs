using System.Reflection;
using Lucene.Net.Documents;

namespace Lucene.Net.DocumentMapper.Interfaces
{
    public interface IFieldMapper
    {
        int Priority { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        bool IsMatch(PropertyInfo propertyInfo);

        Field MapToField(PropertyInfo propertyInfo, object value, string name);

        /// <summary>
        /// The value of the field as a <see cref="System.Object"/>, or null.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        object? MapFromField(Field field);
    }
}