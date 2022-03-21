using System.Collections.Generic;

namespace Lucene.Net.DocumentMapper.Tests.Models.NestedObjects
{
    public class Node
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public IList<LevelOne> LevelOnes { get; set; } = new List<LevelOne>();
    }
}