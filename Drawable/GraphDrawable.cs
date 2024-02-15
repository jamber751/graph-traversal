using graph_traversal.Graph;

namespace graph_traversal.Drawable
{
    public class GraphDrawable : IDrawable
    {
        static int D = 50;
        public int? selectedID { get; set; }

        public int dayCount = 1;

        public int componentCount = 1;
        public int currentLink = 0;


        public Dictionary<int, Node> Nodes { get; set; } = new Dictionary<int, Node>();
        public List<Link> Links { get; set; } = new List<Link>();

        public List<Color> ColorList = new List<Color>() { Colors.IndianRed, Colors.SteelBlue, Colors.Tomato, Colors.LightSalmon, Colors.Azure, Colors.SkyBlue, Colors.LightBlue };

        public HashSet<int> visitedIDs { get; set; } = new HashSet<int>();
        public HashSet<int> currentIDs { get; set; } = new HashSet<int>();

        public List<Link> currentLinks { get; set; } = new List<Link>();

        public int? currentID { get; set; }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            foreach (Link link in Links)
            {
                Color strokeColor = Colors.IndianRed;
                int strokeSize = 4;
                switch (link.marker)
                {
                    case 2:
                        strokeColor = Colors.Green;
                        strokeSize = 8;
                        break;
                    case 0:
                    case 1:
                    case -1:
                        strokeSize = 1;
                        break;
                }
                canvas.StrokeColor = strokeColor;
                canvas.StrokeSize = strokeSize;
                canvas.DrawLine(Nodes[link.id1].X, Nodes[link.id1].Y, Nodes[link.id2].X, Nodes[link.id2].Y);
                canvas.DrawString(link.weight.ToString(), (Nodes[link.id1].X + Nodes[link.id2].X - D) / 2, (Nodes[link.id1].Y + Nodes[link.id2].Y - D) / 2, D, D, HorizontalAlignment.Center, VerticalAlignment.Center);
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
                int component = node.componentID is null ? 0 : (int)node.componentID;
                canvas.FillColor = ColorList[component];

                PathF path = new PathF();
                path.AppendCircle(node.X, node.Y, D / 2);
                canvas.StrokeColor = Colors.IndianRed;
                canvas.StrokeSize = 4;
                canvas.FillPath(path);
                canvas.DrawPath(path);
                canvas.FontSize = 18;
                canvas.DrawString($"{node.id}", node.X - D / 2, node.Y - D / 2, D, D, HorizontalAlignment.Center, VerticalAlignment.Center);
            }
        }

        public void Kruskal()
        {
            bool linkFound = false;
            while (linkFound == false)
            {
                int? component1 = Nodes[Links[currentLink].id1].componentID;
                int? component2 = Nodes[Links[currentLink].id2].componentID;

                if (component1 is null && component2 is null)
                {
                    Nodes[Links[currentLink].id1].componentID = componentCount;
                    Nodes[Links[currentLink].id2].componentID = componentCount++;
                    Links[currentLink].marker = 2;

                    visitedIDs.Add(Links[currentLink].id1);
                    visitedIDs.Add(Links[currentLink].id2);
                    linkFound = true;
                }

                else if (component1 == component2)
                {
                    Links[currentLink].marker = -1;
                    currentLink++;
                    continue;
                }

                else if (component1 is null && component2 is not null)
                {
                    Links[currentLink].marker = 2;
                    Nodes[Links[currentLink].id1].componentID = component2;

                    visitedIDs.Add(Links[currentLink].id1);
                    linkFound = true;
                }

                else if (component2 is null && component1 is not null)
                {
                    Links[currentLink].marker = 2;
                    Nodes[Links[currentLink].id2].componentID = component1;

                    visitedIDs.Add(Links[currentLink].id2);
                    linkFound = true;
                }

                else if (component1 != component2)
                {

                    foreach (Node node in Nodes.Values)
                    {
                        if (node.componentID == component2)
                        {
                            node.componentID = component1;
                        }
                        Links[currentLink].marker = 2;
                        linkFound = true;
                    }
                }
            }
            currentLink++;
        }

        public bool isFinished()
        {
            int? component = Nodes[1].componentID;
            if (component is null) return false;

            foreach (Node node in Nodes.Values)
            {
                if (node.componentID != component)
                    return false;
            }
            return true;
        }

        public void QuickSortLinks(int start, int end)
        {
            if (end <= start) return;
            Link pivot = Links[end];
            int i = start - 1;
            for (int j = start; j <= end; j++)
            {
                if (Links[j].weight < pivot.weight)
                {
                    i++;
                    Link temp = Links[j];
                    Links[j] = Links[i];
                    Links[i] = temp;
                }
            }
            i++;
            Link temp1 = Links[i];
            Links[i] = Links[end];
            Links[end] = temp1;

            QuickSortLinks(start, i - 1);
            QuickSortLinks(i + 1, end);
        }


        public void resetNodes()
        {
            foreach (Node node in Nodes.Values)
            {
                node.dayVisited = null;
                node.visitedFrom = null;
                node.componentID = null;
                node.weightSum = null;
            }

            foreach (Link link in Links)
            {
                link.isPath = false;
                link.marker = 0;
            }

            currentIDs.Clear();
            visitedIDs.Clear();
            currentID = null;
            dayCount = 1;
            componentCount = 1;
            currentLink = 0;
        }

        public void Prim()
        {
            foreach (Link link in Links)
            {
                if (link.marker != 0) continue;
                if (link.id1 == currentID && !visitedIDs.Contains(link.id2)) currentLinks.Add(link);
                if (link.id2 == currentID && !visitedIDs.Contains(link.id1)) currentLinks.Add(link);
            }

            Link minLink = new Link() { weight = int.MaxValue };
            foreach (Link link in currentLinks)
            {
                if (link.weight < minLink.weight) minLink = link;
            }

            minLink.marker = 2;
            currentLinks.Remove(minLink);

            visitedIDs.Add((int)currentID);
            if (visitedIDs.Contains(minLink.id1) || minLink.id1 == currentID) currentID = minLink.id2;
            else currentID = minLink.id1;

            for (int i = 0; i < currentLinks.Count; i++)
            {
                Link link = currentLinks[i];
                if (link.marker == 2 || link.marker == -1) continue;
                if ((visitedIDs.Contains(link.id1) || link.id1 == currentID) && (visitedIDs.Contains(link.id2) || link.id2 == currentID))
                {
                    link.marker = -1;
                    currentLinks.Remove(link);
                    i--;
                }
            }
        }



        public void Dijkstra()
        {
            if (!visitedIDs.Contains((int)currentID))
            {
                foreach (Link link in Links)
                {
                    if (link.id1 == currentID && link.id2 != Nodes[(int)currentID].visitedFrom)
                    {
                        if (Nodes[link.id2].weightSum is null || Nodes[link.id2].weightSum > Nodes[(int)currentID].weightSum + link.weight)
                        {
                            Nodes[link.id2].weightSum = Nodes[(int)currentID].weightSum + link.weight;
                            Nodes[link.id2].visitedFrom = currentID;
                            currentIDs.Add(link.id2);
                        }
                    }
                    if (link.id2 == currentID && link.id1 != Nodes[(int)currentID].visitedFrom)
                    {
                        if (Nodes[link.id1].weightSum is null || Nodes[link.id1].weightSum > Nodes[(int)currentID].weightSum + link.weight)
                        {
                            Nodes[link.id1].weightSum = Nodes[(int)currentID].weightSum + link.weight;
                            Nodes[link.id1].visitedFrom = currentID;
                            currentIDs.Add(link.id1);
                        }
                    }
                }
            }

            currentIDs.Remove((int)currentID);
            visitedIDs.Add((int)currentID);

            int minValue = Int32.MaxValue;
            foreach (int id in currentIDs)
            {
                if (Nodes[id].weightSum < minValue)
                {
                    minValue = (int)Nodes[id].weightSum;
                    currentID = id;
                }
            }
        }

        public void ComponentSearch(Grid gridAnswer)
        {
            dayCount++;

            bool nodeFound = false;
            List<int> temp = new List<int>(currentIDs);

            visitedIDs.UnionWith(currentIDs);
            currentIDs.Clear();

            foreach (int id in temp)
            {
                foreach (Link link in Links)
                {
                    if (link.id1 == id && !visitedIDs.Contains(link.id2) && !currentIDs.Contains(link.id2))
                    {
                        currentIDs.Add(link.id2);
                        Nodes[link.id2].componentID = componentCount;
                        Nodes[link.id2].dayVisited = dayCount;
                        componentSearchPrint(gridAnswer, Nodes[link.id2]);
                        nodeFound = true;
                    }
                    else if (link.id2 == id && !visitedIDs.Contains(link.id1) && !currentIDs.Contains(link.id1))
                    {
                        currentIDs.Add(link.id1);
                        Nodes[link.id1].componentID = componentCount;
                        Nodes[link.id1].dayVisited = dayCount;
                        componentSearchPrint(gridAnswer, Nodes[link.id1]);
                        nodeFound = true;
                    }
                }
            }

            if (!nodeFound)
            {
                foreach (Node node in Nodes.Values)
                {
                    if (!visitedIDs.Contains(node.id))
                    {
                        currentIDs.Add(node.id);
                        node.componentID = ++componentCount;
                        node.dayVisited = dayCount;
                        componentSearchPrint(gridAnswer, node);
                        break;
                    }
                }
            }
        }

        public void componentSearchPrint(Grid gridAnswer, Node node)
        {
            gridAnswer.AddRowDefinition(new RowDefinition());
            Label label1 = new Label() { Text = node.id.ToString(), HorizontalOptions = LayoutOptions.Center, FontSize = 20, TextColor = ColorList[(int)node.componentID - 1] };
            Label label2 = new Label() { Text = node.dayVisited.ToString(), HorizontalOptions = LayoutOptions.Center, FontSize = 20, TextColor = ColorList[(int)node.componentID - 1] };
            Label label3 = new Label() { Text = node.componentID.ToString(), HorizontalOptions = LayoutOptions.Center, FontSize = 20, TextColor = ColorList[(int)node.componentID - 1] };

            gridAnswer.Add(label1, 1, gridAnswer.RowDefinitions.Count - 1);
            gridAnswer.Add(label2, 0, gridAnswer.RowDefinitions.Count - 1);
            gridAnswer.Add(label3, 2, gridAnswer.RowDefinitions.Count - 1);
        }


        public void clearCanvas()
        {
            Nodes.Clear();
            Links.Clear();
            currentIDs.Clear();
            visitedIDs.Clear();
            currentID = null;
            dayCount = 1;
            componentCount = 1;
        }

    }
}

