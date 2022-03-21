using System.Collections.Generic;

namespace Lucene.Net.DocumentMapper.Tests.Models.NestedObjects
{
    public class LevelOne
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public IList<LevelTwo> LevelTwos { get; set; } = new List<LevelTwo>();
    }
}