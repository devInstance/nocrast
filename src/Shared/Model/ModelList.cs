using System;
using System.Collections.Generic;
using System.Text;

namespace NoCrast.Shared.Model
{
    public class ModelList<T>
    {
        public int TotalCount { get; set; }
        public int Count { get; set; }
        public T[] Items { get; set; }
        public int Page { get; set; }
    }
}
