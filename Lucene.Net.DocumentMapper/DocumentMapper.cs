using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lucene.Net.DocumentMapper.Helpers;
using Lucene.Net.DocumentMapper.Interfaces;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Util;

namespace Lucene.Net.DocumentMapper
{
    /// <summary>
    /// Map .NET Types to Lucene Documents and back.
    /// </summary>
    public class DocumentMapper : IDocumentMapper
    {
        private readonly IEnumerable<IFieldMapper> _propertyMappers;
        private readonly IEnumerable<IFieldsMapper> _fieldsMappers;

        public DocumentMapper(IEnumerable<IFieldMapper> propertyMappers, IEnumerable<IFieldsMapper> fieldsMappers)
        {
            _propertyMappers = propertyMappers;
            _fieldsMappers = fieldsMappers;
        }

        public T Map<T>(Document source)
        {
            return (T)Map(source, typeof(T));
        }

        public object Map(Document source, Type contentType)
        {
            var contentItem = Activator.CreateInstance(contentType);
            return GetValue(source.Fields, contentItem, 0);
        }

        private object GetValue(IList<IIndexableField> fields, object parent, int level)
        {
            var properties = parent.GetType().GetProperties();
            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.GetSetMethod() == null)
                {
                    continue;
                }

                if (propertyInfo.IsPropertyACollection())
                {
                    var genericType = propertyInfo.GetPropertyType().GetGenericArguments().Single();
                    var listType = typeof(List<>).MakeGenericType(genericType);

                    object nestedCollection = Activator.CreateInstance(listType);
                    var listFields =
                        fields.Where(x =>
                            {
                                var nameSplit = x.Name.Split('.');
                                if (nameSplit.Length > 1)
                                {
                                    return nameSplit[level].Equals(propertyInfo.Name);
                                }

                                return x.Name.Equals(propertyInfo.Name);
                            })
                            .ToList();

                    if (genericType.IsPrimitiveType())
                    {
                        foreach (var indexableField in listFields)
                        {
                            var v = GetValueFromField(propertyInfo, (Field)indexableField);
                            if (v == null) continue;
                            ((IList)nestedCollection).Add(v);
                        }
                    }
                    else if(listFields.Any())
                    {
                        var firstField = listFields.First();
                        var firstFieldList =
                            listFields.Where(x => x.Name.Equals(firstField.Name))
                                .ToList();

                        foreach (var indexableField in firstFieldList)
                        {
                            IList<IIndexableField> groupedFields;
                            if (indexableField.Equals(firstFieldList.Last()))
                            {
                                var skip = listFields.IndexOf(indexableField);
                                var take = listFields.Count - skip;


                                groupedFields = listFields.Skip(skip).Take(take).ToList();
                            }
                            else
                            {
                                var firstFieldIdx = firstFieldList.IndexOf(indexableField);
                                var nextFirstField = firstFieldList[firstFieldIdx + 1];

                                var skip = listFields.IndexOf(indexableField);
                                var take = listFields.IndexOf(nextFirstField) - skip;


                                groupedFields = listFields.Skip(skip).Take(take).ToList();
                            }

                            object nestedComplexType = Activator.CreateInstance(genericType);
                            ((IList)nestedCollection).Add(GetValue(groupedFields, nestedComplexType, level + 1));
                        }
                    }

                    propertyInfo.SetValue(parent, nestedCollection);
                    continue;
                }

                if (!propertyInfo.IsPrimitiveType() && propertyInfo.GetPropertyType() != typeof(object))
                {
                    object nestedComplexType = Activator.CreateInstance(propertyInfo.GetPropertyType());
                    var listFields =
                        fields.Where(x => x.Name.Split('.')[level].Equals(propertyInfo.Name))
                            .ToList();

                    propertyInfo.SetValue(parent, GetValue(listFields, nestedComplexType, level + 1));
                    continue;
                }

                var field =
                    fields.FirstOrDefault(x => x.Name.Split('.')[level].Equals(propertyInfo.Name));

                if (field == null)
                {
                    continue;
                }

                propertyInfo.SetValue(parent, GetValueFromField(propertyInfo, (Field)field));
            }

            return parent;
        }

        public IFieldMapper? GetFieldMapper(PropertyInfo? propertyInfo)
        {
            if (propertyInfo == null) return null;

            return _propertyMappers
                .OrderByDescending(x => x.Priority)
                .FirstOrDefault(x => x.IsMatch(propertyInfo));
        }

        /// <summary>
        /// The value of the field as a object, or null.
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        private object? GetValueFromField(PropertyInfo propertyInfo, Field field)
        {
            var propertyMapper = GetFieldMapper(propertyInfo);
            return propertyMapper != null
                    ? propertyMapper.MapFromField(field)
                    : Parse(field.GetStringValue(), propertyInfo.GetPropertyType());
        }

        protected virtual object? Parse(string value, Type type)
        {
            if(value == null) return null;

            // Optimization - don't bother calling 
            if (type == typeof(string))
            {
                return value;
            }

            // string -> DateTime
            if (type == typeof(DateTime))
            {
                // Attempt to use the Lucene.Net date conversion if it fails to parse
                // Support the Lucene.Net date formatting (yyyyMMddHHmmssfff or any subset starting on the left) for convenience.
                return DateTools.StringToDate(value);
            }
                
            return Convert.ChangeType(value, type);
        }

        /// <summary>
        /// Map .NET object to Lucene Document
        /// </summary>
        /// <param name="source">.NET object</param>
        /// <returns>Lucene Document</returns>
        public Document Map(object source)
        {
            var document = new Document();
            var fields = GetFields(source, new List<Field>(), string.Empty);
            foreach (var field in fields)
            {
                if (field == null) continue;

                document.Add(field);
            }

            return document;
        }

        private Field? GetFieldFromValue(PropertyInfo propertyInfo, object value, string name)
        {
            var propertyMapper = GetFieldMapper(propertyInfo);
            return propertyMapper?.MapToField(propertyInfo, value, name);
        }

        private IList<Field> GetFields(object source, IList<Field> fields, string prefix)
        {
            foreach (var fieldsMapper in _fieldsMappers)
            {
                if (fieldsMapper.IsMatch(source.GetType()))
                {
                    fields.AddRange(fieldsMapper.MapToFields(source));
                }
            }

            foreach (var propertyInfo in source.GetType().GetProperties())
            {
                var propertyValue = propertyInfo.GetValue(source);
                if (propertyValue == null) continue;

                string name = !string.IsNullOrEmpty(prefix)
                    ? $"{prefix}.{propertyInfo.Name}"
                    : propertyInfo.Name;

                if (propertyInfo.IsPropertyACollection())
                {
                    IList propertyValueList = (IList)propertyValue;
                    if (propertyValueList.Count == 0)
                    {
                        continue;
                    }

                    var genericType = propertyInfo.GetPropertyType().GetGenericArguments().Single();
                    if (genericType.IsPrimitiveType())
                    {
                        foreach (var itemValue in propertyValueList)
                        {
                            var f = GetFieldFromValue(propertyInfo, itemValue, name);
                            if(f == null) continue;
                            fields.Add(f);
                        }
                    }
                    else
                    {
                        foreach (var itemValue in propertyValueList)
                        {
                            GetFields(itemValue, fields, name);
                        }
                    }

                    continue;
                }

                if (!propertyInfo.IsPrimitiveType() && propertyInfo.GetPropertyType() != typeof(object))
                {
                    GetFields(propertyValue, fields, name);
                    continue;
                }

                var fieldFromValue = GetFieldFromValue(propertyInfo, propertyValue, name);
                if(fieldFromValue != null)
                    fields.Add(fieldFromValue);
            }

            return fields;
        }
    }
}