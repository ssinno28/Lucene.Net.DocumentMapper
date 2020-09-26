using System.Linq;
using System.Reflection;
using Lucene.Net.DocumentMapper.FieldMappers;
using Lucene.Net.DocumentMapper.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Lucene.Net.DocumentMapper.Helpers
{
    public static class ServiceCollectionHelpers
    {
        public static IServiceCollection AddLuceneDocumentMapper(this IServiceCollection services)
        {
            services.AddScoped<IDocumentMapper, DocumentMapper>();

            var assembly = Assembly.GetAssembly(typeof(BooleanFieldMapper));
            foreach (var exportedType in assembly.DefinedTypes)
            {
                if (exportedType.ImplementedInterfaces.Contains(typeof(IFieldMapper)))
                {
                    services.AddScoped(typeof(IFieldMapper), exportedType);
                }
            }

            return services;
        }
    }
}