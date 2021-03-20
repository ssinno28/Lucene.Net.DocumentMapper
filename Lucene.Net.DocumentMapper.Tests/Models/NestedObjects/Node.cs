using System.Collections.Generic;

namespace Lucene.Net.DocumentMapper.Tests.Models.NestedObjects
{
    public class Node
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IList<LevelOne> LevelOnes { get; set; }
    }
}