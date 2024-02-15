using System;
namespace graph_traversal.Graph
{
    public class Link
    {
        public int id1 { get; set; }
        public int id2 { get; set; }
        public int? weight { get; set; }
        public int marker { get; set; } = 0;
        public bool isPath { get; set; } = false;
    }
}

