using System;
using graph_traversal.Graph;

namespace graph_traversal.Drawable
{
    public class GraphDrawable : IDrawable
    {
        static int D = 50;
        public int? selectedID { get; set; }

        public Dictionary<int, Node> Nodes { get; set; } = new Dictionary<int, Node>();
        public List<Link> Links { get; set; } = new List<Link>();

        public HashSet<int> visitedIDs { get; set; } = new HashSet<int>();
        public HashSet<int> currentIDs { get; set; } = new HashSet<int>();

        public int? currentID { get; set; }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            foreach (Link link in Links)
            {
                canvas.StrokeColor = Colors.Red;
                canvas.StrokeSize = 4;
                canvas.DrawLine(Nodes[link.id1].X, Nodes[link.id1].Y, Nodes[link.id2].X, Nodes[link.id2].Y);
            }

            foreach (Node node in Nodes.Values)
            {
                if (node.id == selectedID)
                {
                    canvas.FillColor = Color.FromArgb("#A8B5E0");
                }
                else
                {
                    canvas.FillColor = Color.FromArgb("EAEEFA");
                }

                if (visitedIDs.Contains(node.id)) canvas.FillColor = Colors.Red;
                if (currentIDs.Contains(node.id)) canvas.FillColor = Colors.Green;
                if (currentID == node.id) canvas.FillColor = Colors.Green;


                PathF path = new PathF();
                path.AppendCircle(node.X, node.Y, D / 2);
                canvas.StrokeColor = Colors.Red;
                canvas.StrokeSize = 4;
                canvas.FillPath(path);
                canvas.DrawPath(path);
                canvas.FontSize = 18;
                canvas.DrawString($"{node.id}", node.X - D / 2, node.Y - D / 2, D, D, HorizontalAlignment.Center, VerticalAlignment.Center);
            }
        }


        public void resetNodes()
        {
            foreach (Node node in Nodes.Values)
            {
                node.dayVisited = null;
                node.visitedFrom = null;
            }
            currentIDs.Clear();
            visitedIDs.Clear();
            currentID = null;
        }


    }
}

