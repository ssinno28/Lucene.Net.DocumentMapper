using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Lucene.Net.DocumentMapper.FieldMappers;
using Lucene.Net.DocumentMapper.Helpers;
using Lucene.Net.DocumentMapper.Interfaces;
using Lucene.Net.DocumentMapper.Tests.Models;
using Lucene.Net.DocumentMapper.Tests.Models.NestedObjects;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

namespace Lucene.Net.DocumentMapper.Tests
{
    public class DocumentMapperTests
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly Fixture _fixture;

        public DocumentMapperTests()
        {
            _fixture = new Fixture();
            _serviceProvider = new ServiceCollection()
                .AddLuceneDocumentMapper()
                .BuildServiceProvider();
        }

        [Fact]
        public void Test_Is_Tokenized()
        {
            var documentMapper = _serviceProvider.GetRequiredService<IDocumentMapper>();
            // public PropertyInfo? GetProperty(string name);
            var propertyMapper = documentMapper.GetFieldMapper(typeof(BlogPost).GetProperty("Body"));

            Assert.Equal(typeof(TextFieldMapper), propertyMapper?.GetType());
        }

        [Fact]
        public void Test_Is_String()
        {
            var documentMapper = _serviceProvider.GetRequiredService<IDocumentMapper>();
            // public PropertyInfo? GetProperty(string name);
            //var p = typeof(BlogPost).GetProperties().FirstOrDefault(x => x.Name == "Name");
            var propertyMapper = documentMapper.GetFieldMapper(typeof(BlogPost).GetProperty("Name"));

            Assert.Equal(typeof(StringFieldMapper), propertyMapper?.GetType());
        }

        [Fact]
        public void Test_Is_Enum()
        {
            var documentMapper = _serviceProvider.GetRequiredService<IDocumentMapper>();
            // public PropertyInfo? GetProperty(string name);
            var propertyMapper = documentMapper.GetFieldMapper(typeof(BlogPost).GetProperty("Category3"));

            Assert.Equal(typeof(EnumFieldMapper), propertyMapper?.GetType());
        }

        [Fact]
        public void Test_Is_DateTime()
        {
            var documentMapper = _serviceProvider.GetRequiredService<IDocumentMapper>();
            // public PropertyInfo? GetProperty(string name);
            var propertyMapper = documentMapper.GetFieldMapper(typeof(BlogPost).GetProperty("PublishedDate"));

            Assert.Equal(typeof(DateTimeFieldMapper), propertyMapper?.GetType());
        }

        [Fact]
        public void Test_Is_DateTimeOffset()
        {
            var documentMapper = _serviceProvider.GetRequiredService<IDocumentMapper>();
            // public PropertyInfo? GetProperty(string name);
            var propertyMapper = documentMapper.GetFieldMapper(typeof(BlogPost).GetProperty("PublishedDateOffset"));

            Assert.Equal(typeof(DateTimeOffsetFieldMapper), propertyMapper?.GetType());
        }

        [Fact]
        public void Test_Is_Object()
        {
            var documentMapper = _serviceProvider.GetRequiredService<IDocumentMapper>();
            // public PropertyInfo? GetProperty(string name);
            var propertyMapper = documentMapper.GetFieldMapper(typeof(BlogPost).GetProperty("Category"));

            Assert.Equal(typeof(ObjectFieldMapper), propertyMapper?.GetType());
        }

        [Fact]
        public void Test_Is_Boolean()
        {
            var documentMapper = _serviceProvider.GetRequiredService<IDocumentMapper>();
            // public PropertyInfo? GetProperty(string name);
            var propertyMapper = documentMapper.GetFieldMapper(typeof(BlogPost).GetProperty("IsPublished"));

            Assert.Equal(typeof(BooleanFieldMapper), propertyMapper?.GetType());
        }

        [Fact]
        public void Test_Not_Indexed_If_Too_Large_String()
        {
            var documentMapper = _serviceProvider.GetRequiredService<IDocumentMapper>();
            // public PropertyInfo? GetProperty(string name);
            var propertyMapper = documentMapper.GetFieldMapper(typeof(BlogPost).GetProperty("SeoDescription"));

            Assert.Equal(typeof(StringFieldMapper), propertyMapper?.GetType());

            var blogPost = new BlogPost { SeoDescription = new string('*', 32767) };

            var document = documentMapper.Map(blogPost);
            var isIndexed = document.GetField("SeoDescription").IndexableFieldType.IsIndexed;

            Assert.False(isIndexed);
        }

        [Fact]
        public void Test_Field_Not_Stored()
        {
            var documentMapper = _serviceProvider.GetRequiredService<IDocumentMapper>();
            // public PropertyInfo? GetProperty(string name);
            var propertyMapper = documentMapper.GetFieldMapper(typeof(BlogPost).GetProperty("SeoTitle"));

            Assert.Equal(typeof(StringFieldMapper), propertyMapper?.GetType());

            var blogPost = new BlogPost { SeoTitle = "My Test Seo Title" };

            var document = documentMapper.Map(blogPost);
            var isStored = document.GetField("SeoTitle").IndexableFieldType.IsStored;

            Assert.False(isStored);
        }

        [Fact]
        public void Test_Object_Stored_As_Json()
        {
            var documentMapper = _serviceProvider.GetRequiredService<IDocumentMapper>();
            // public PropertyInfo? GetProperty(string name);
            var propertyMapper = documentMapper.GetFieldMapper(typeof(BlogPost).GetProperty("Category"));

            Assert.Equal(typeof(ObjectFieldMapper), propertyMapper?.GetType());

            var blogPost = new BlogPost
            {
                Category = new Category
                {
                    Description = "My Test Category",
                    Name = "TestCategory"
                }
            };

            var document = documentMapper.Map(blogPost);
            var category = JsonConvert.DeserializeObject<Category>(document.GetField("Category").GetStringValue());

            Assert.Equal("TestCategory", category?.Name);
        }

        [Fact]
        public void Test_ComplexType_Stored_As_Individual_Fields()
        {
            var documentMapper = _serviceProvider.GetRequiredService<IDocumentMapper>();
            // public PropertyInfo? GetProperty(string name);
            var propertyMapper = documentMapper.GetFieldMapper(typeof(BlogPost).GetProperty("Category2"));

            Assert.Null(propertyMapper);

            var blogPost = new BlogPost
            {
                Category2 = new Category
                {
                    Description = "My Test Category",
                    Name = "TestCategory"
                }
            };

            var document = documentMapper.Map(blogPost);
            var fields = document.Fields.Where(x => x.Name.StartsWith("Category2"));

            Assert.Equal(2, fields.Count());
        }

        [Fact]
        public void Test_Collection_Primitive_Type_Fields()
        {
            var documentMapper = _serviceProvider.GetRequiredService<IDocumentMapper>();
            var blogPost = new BlogPost
            {
                TagIds = new List<string>()
                {
                    "1",
                    "2",
                    "3"
                }
            };

            var document = documentMapper.Map(blogPost);
            var fields = document.Fields.Where(x => x.Name.StartsWith("TagIds"));

            Assert.Equal(3, fields.Count());
            Assert.All(fields, field => field.Name.Equals("TagIds"));
        }

        [Fact]
        public void Test_Enum_Properly_Mapped()
        {
            var documentMapper = _serviceProvider.GetRequiredService<IDocumentMapper>();
            var blogPost = new BlogPost
            {
                Category3 = EnumCategory.Database
            };

            var document = documentMapper.Map(blogPost);
            var field = document.Fields.FirstOrDefault(x => x.Name.StartsWith("Category3"));

            Assert.NotNull(field);

            blogPost = documentMapper.Map<BlogPost>(document);
            Assert.Equal(EnumCategory.Database, blogPost.Category3);
        }

        [Fact]
        public void Test_Collection_Complex_Type_Fields()
        {
            var documentMapper = _serviceProvider.GetRequiredService<IDocumentMapper>();
            var blogPost = new BlogPost
            {
                PublishedDate = DateTime.Now,
                PublishedDateOffset = DateTimeOffset.Now,
                Tags = new List<Tag>
                {
                    new Tag
                    {
                        Id = "1",
                        Name = "test"
                    },
                    new Tag
                    {
                        Id = "2",
                        Name = "test2"
                    },
                    new Tag
                    {
                        Id = "3",
                        Name = "test3"
                    }
                }
            };

            var document = documentMapper.Map(blogPost);
            var fields = document.Fields.Where(x => x.Name.StartsWith("Tags"));

            Assert.Equal(6, fields.Count());
            Assert.All(fields, field => field.Name.Equals("Tags"));

            var mappedBlogPost = documentMapper.Map<BlogPost>(document);
            for (var i = 0; i < blogPost.Tags.Count; i++)
            {
                Assert.Equal(mappedBlogPost.Tags[i].Id, blogPost.Tags[i].Id);
                Assert.Equal(mappedBlogPost.Tags[i].Name, blogPost.Tags[i].Name);
            }
        }

        [Fact]
        public void Test_Collection_Nested_Complex_types()
        {
            var documentMapper = _serviceProvider.GetRequiredService<IDocumentMapper>();
            var node = _fixture.Create<Node>();

            var document = documentMapper.Map(node);
            var expectedNode = documentMapper.Map<Node>(document);

            var obj1Str = JsonConvert.SerializeObject(node);
            var obj2Str = JsonConvert.SerializeObject(expectedNode);
            Assert.Equal(obj1Str, obj2Str);
        }

        [Fact]
        public void Test_Not_Indexed_If_Too_Large_Text()
        {
            var documentMapper = _serviceProvider.GetRequiredService<IDocumentMapper>();
            // public PropertyInfo? GetProperty(string name);
            var propertyMapper = documentMapper?.GetFieldMapper(typeof(BlogPost).GetProperty("Body"));

            Assert.Equal(typeof(TextFieldMapper), propertyMapper?.GetType());

            var blogPost = new BlogPost { Body = new string('*', 32767) };

            var document = documentMapper?.Map(blogPost);
            var isIndexed = document?.GetField("Body").IndexableFieldType.IsIndexed;

            Assert.False(isIndexed);
        }
    }
}
