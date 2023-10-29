namespace graph_traversal;

public class Node
{
    public int id { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public bool visited { get; set; } = false;
}

public class Link
{
    public int id1 { get; set; }
    public int id2 { get; set; }
}


public abstract class BaseDrawable : IDrawable
{
    public abstract void Draw(ICanvas canvas, RectF dirtyRect);
}


public class GraphDrawable : BaseDrawable, IDrawable
{
    static int D = 50;

    public int? selectedID { get; set; }

    public Dictionary<int, Node> Nodes { get; set; } = new Dictionary<int, Node>() { };

    public List<Link> Links = new List<Link>() { };

    public override void Draw(ICanvas canvas, RectF dirtyRect)
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
}


public partial class MainPage : ContentPage
{
    int count = 1;
    int mode = 0;

    void switchMode(int num)
    {
        addLinksMode.IsEnabled = true;
        removeLinksMode.IsEnabled = true;
        removeNodesMode.IsEnabled = true;
        addNodesMode.IsEnabled = true;
        switch (num)
        {
            case 0:
                addNodesMode.IsEnabled = false;
                mode = 0;
                break;
            case 1:
                addLinksMode.IsEnabled = false;
                mode = 1;
                break;
            case 2:
                removeNodesMode.IsEnabled = false;
                mode = 2;
                break;
            case 3:
                removeLinksMode.IsEnabled = false;
                mode = 3;
                break;
        }
    }

    void addNodesMode_Clicked(System.Object sender, System.EventArgs e)
    {
        switchMode(0);
    }

    void addLinksMode_Clicked(System.Object sender, System.EventArgs e)
    {
        switchMode(1);
    }

    void removeNodesMode_Clicked(System.Object sender, System.EventArgs e)
    {
        switchMode(2);
    }

    void removeLinksMode_Clicked(System.Object sender, System.EventArgs e)
    {
        switchMode(3);
    }

    void clearCanvas_Clicked(System.Object sender, System.EventArgs e)
    {
        count = 1;
        var graphView = this.graphDrawableView;
        var graphDrawable = (GraphDrawable)graphView.Drawable;
        graphDrawable.Nodes.Clear();
        graphDrawable.Links.Clear();
        graphDrawable.selectedID = null;
        graphView.Invalidate();
        switchMode(0);
    }


    public MainPage()
    {
        InitializeComponent();
    }


    void TapGestureRecognizer_Tapped(System.Object sender, Microsoft.Maui.Controls.TappedEventArgs e)
    {
        // Position relative to the container view, that is the image, the origin point is at the top left of the image.
        Point? relativeToContainerPosition = e.GetPosition((View)sender);
        var graphView = this.graphDrawableView;
        var graphDrawable = (GraphDrawable)graphView.Drawable;

        float X = (float)relativeToContainerPosition.Value.X;
        float Y = (float)relativeToContainerPosition.Value.Y;

        switch (mode)
        {
            case 0:

                bool interSection = false;
                foreach (Node node in graphDrawable.Nodes.Values)
                {
                    if (Math.Sqrt(Math.Pow(node.X - X, 2) + Math.Pow(node.Y - Y, 2)) <= 52)
                    {
                        interSection = true;
                        break;
                    }
                }

                if (interSection)
                {
                    DisplayAlert("Ошибка", "Вершины пересекаются", "Ок");
                }
                else
                {
                    graphDrawable.Nodes[count] = (new Node() { id = count++, X = X, Y = Y });
                    graphView.Invalidate();
                }
                break;


            case 1:

                foreach (Node node in graphDrawable.Nodes.Values)
                {
                    if (Math.Sqrt(Math.Pow(node.X - X, 2) + Math.Pow(node.Y - Y, 2)) <= 25)
                    {
                        if (graphDrawable.selectedID == null) graphDrawable.selectedID = node.id;
                        else if (node.id == graphDrawable.selectedID) graphDrawable.selectedID = null;
                        else
                        {
                            foreach (Link link in graphDrawable.Links)
                            {
                                if ((link.id1 == node.id && link.id2 == graphDrawable.selectedID) || (link.id1 == graphDrawable.selectedID && link.id2 == node.id))
                                {
                                    DisplayAlert("Ошибка", "Вершины уже соеденины", "Ок");
                                    graphDrawable.selectedID = null;
                                    return;
                                }
                            }
                            graphDrawable.Links.Add(new Link() { id1 = (int)graphDrawable.selectedID, id2 = node.id });
                            graphDrawable.selectedID = null;
                        }
                        graphView.Invalidate();
                        break;
                    }
                }
                break;
            case 2:
                foreach (Node node in graphDrawable.Nodes.Values)
                {
                    if (Math.Sqrt(Math.Pow(node.X - X, 2) + Math.Pow(node.Y - Y, 2)) <= 25)
                    {
                        List<Link> linksToRemove = new List<Link>();
                        for (int i = 0; i < graphDrawable.Links.Count; i++)
                        {
                            Link link = graphDrawable.Links[i];
                            if (link.id1 == node.id || link.id2 == node.id)
                            {
                                linksToRemove.Add(link);
                            }
                        }

                        foreach (Link link in linksToRemove)
                        {
                            graphDrawable.Links.Remove(link);
                        }

                        graphDrawable.Nodes.Remove(node.id);
                        graphView.Invalidate();
                    }
                }
                break;
            case 3:
                foreach (Node node in graphDrawable.Nodes.Values)
                {
                    if (Math.Sqrt(Math.Pow(node.X - X, 2) + Math.Pow(node.Y - Y, 2)) <= 25)
                    {
                        if (graphDrawable.selectedID == null) graphDrawable.selectedID = node.id;
                        else if (node.id == graphDrawable.selectedID) graphDrawable.selectedID = null;
                        else
                        {
                            foreach (Link link in graphDrawable.Links)
                            {
                                if ((link.id1 == node.id && link.id2 == graphDrawable.selectedID) || (link.id1 == graphDrawable.selectedID && link.id2 == node.id))
                                {
                                    graphDrawable.selectedID = null;
                                    graphDrawable.Links.Remove(link);
                                    graphView.Invalidate();
                                    return;
                                }
                            }
                            DisplayAlert("Ошибка", "Вершины не соединены", "Ok");
                            graphDrawable.selectedID = null;
                            break;
                        }
                        graphView.Invalidate();
                        break;
                    }
                }
                break;
        }
    }
}
