# Lucene.Net.DocumentMapper

![CI](https://github.com/ssinno28/Lucene.Net.DocumentMapper/workflows/CI/badge.svg)

`Install-Package Lucene.Net.DocumentMapper`

This is a simple service that helps with mapping C# Types to Lucene Documents and back. In order to wire it up with DI just call `ServiceCollection.AddLuceneDocumentMapper`. 

In order to get started you will just have to inject `IDocumentMapper` into your class and use one of the Map methods:

```
T Map<T>(Document source);
object Map(Document source, Type contentType);
Document Map(object source);
```

It comes with a default set of Field Mappers, but you can easily add your own and override how any property is mapped by creating a class that implements `IFieldMapper` as shown here:

```c#
   public class BooleanFieldMapper : AFieldMapper, IFieldMapper
    {
        public int Priority => 0;

        public bool IsMatch(PropertyInfo propertyInfo)
        {
            var type = GetPropertyType(propertyInfo);
            return type == typeof(bool);
        }

        public Field MapToField(PropertyInfo propertyInfo, object value, string name)
        {
            bool convertedValue = (bool)value;
            return new StringField(name,
                convertedValue
                    ? Boolean.TrueString
                    : Boolean.FalseString, GetStore(propertyInfo));
        }

        public object MapFromField(Field field)
        {
            return Boolean.Parse(field.GetStringValue());
        }
    }
```

If you want to override a default one, you make sure that the IsMatch method returns true for a specific property and increment the priority so it takes precedence over the default mappers. 

### Object Mapping

Any property that is of type `object` gets mapped to a JSON string in the field value, but if it has an actual type declared, that types properties will be mapped individually with dot notation. 

For example if you have a BlogPost type setup like this: 

```
    public class BlogPost
    {
        public DateTime PublishedDate { get; set; }
        public DateTimeOffset PublishedDateOffset { get; set; }
        public bool IsPublished { get; set; }
        public string Name { get; set; }
        [Search(Tokenized = true)]
        public string Body { get; set; }
        public string SeoDescription { get; set; }
        [Search(Store = false)]
        public string SeoTitle { get; set; }
        public string Excerpt { get; set; }
        public string ThumbnailUrl { get; set; }
        public IList<string> TagIds { get; set; }
        public object Category { get; set; }
        public Category Category2 { get; set; }
        public IList<Tag> Tags { get; set; }
    }
```

Category will be stored as json and Category2 will be stored as multiple fields:

1. Category2.Name:{value}
2. Category2.Id:{value}
3. Category2.Description:{value}



### Collection Mapping

For collection mapping, if the generic type is a primitive type in the instance of TagIds in the BlogPost type above, the tag ids will each be stored as an individual field all with the same name (in this case TagIds). If the collection is a complex type, then it will be similar to how complex types are stored above, instead in this case where there could be two tags, you would have these fields:

1. Tags.Id:{value}
2. Tags.Name:{value}
3. Tags.Id:{value}
4. Tags.Name:{value}

### Search Attribute

The search attribute comes with three properties, Store, IsKey and Tokenized. Store is set to true by default and tokenized is false by default. If you set tokenzied to true for a string field it will store the string as a text field. 
