using System;
namespace graph_traversal.Graph
{
	public class Node
	{
        public int id { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public int? dayVisited { get; set; } = null;
        public int? visitedFrom { get; set; } = null;
    }
}

