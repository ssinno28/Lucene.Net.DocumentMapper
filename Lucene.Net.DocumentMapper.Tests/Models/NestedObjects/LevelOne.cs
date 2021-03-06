﻿using System.Collections.Generic;

namespace Lucene.Net.DocumentMapper.Tests.Models.NestedObjects
{
    public class LevelOne
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IList<LevelTwo> LevelTwos { get; set; }
    }
}