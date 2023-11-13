using System;
namespace graph_traversal.Graph
{
	public class GraphJsonObject
	{
        public Dictionary<int, Node> Nodes { get; set; } = new Dictionary<int, Node>();
        public List<Link> Links { get; set; } = new List<Link>();
    }
}