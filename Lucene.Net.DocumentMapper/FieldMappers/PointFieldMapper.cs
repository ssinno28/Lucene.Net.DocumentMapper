using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.DocumentMapper.Interfaces;
using Lucene.Net.Documents;
using Lucene.Net.Spatial;
using Lucene.Net.Spatial.Vector;
using Spatial4n.Context;
using Spatial4n.Shapes;

namespace Lucene.Net.DocumentMapper.FieldMappers
{
    public class PointFieldMapper : IFieldsMapper
    {
        public bool IsMatch(Type type)
        {
            return type.GetInterfaces().ToList().Contains(typeof(ILocationIndex));
        }

        public IList<Field> MapToFields(object @object)
        {
            ILocationIndex locationIndex = (ILocationIndex) @object;

            SpatialContext ctx = SpatialContext.GEO;
            SpatialStrategy strategy = new PointVectorStrategy(ctx, FieldPrefixConstants.LocationPointPrefix);
            IShape shape = new Point(locationIndex.Longitude, locationIndex.Latitude, ctx);

            return strategy.CreateIndexableFields(shape);
        }
    }
}