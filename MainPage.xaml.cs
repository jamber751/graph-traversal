namespace graph_traversal;
using graph_traversal.Graph;
using graph_traversal.Drawable;

public partial class MainPage : ContentPage
{
    int count = 1;
    int mode = 0;
    int startID = 4;
    int dayCount = 1;
    int componentCount = 1;
    int algorithmID = 0;

    void algorithmSwitcher(System.Object sender, Microsoft.Maui.Controls.CheckedChangedEventArgs e)
    {
        //RadioButton radiobutton = sender as RadioButton;
        //if(radiobutton.IsChecked)
        //{
        //    if (radiobutton == ButtonBFS) algorithmID = 0;
        //    if (radiobutton == ButtonDFS) algorithmID = 1;
        //}
    }

    void nextStep_Clicked(System.Object sender, System.EventArgs e)
    {
        switch (algorithmID)
        {
            case 0:
                BFS();
                break;
            case 1:
                DFS();
                break;
        }
    }


    void addToGrid(Node node)
    {
        gridAnswer.AddRowDefinition(new RowDefinition());

        Label label1 = new Label() { Text = node.id.ToString(), HorizontalOptions = LayoutOptions.Center, FontSize = 20 };
        Label label2 = new Label() { Text = node.dayVisited.ToString(), HorizontalOptions = LayoutOptions.Center, FontSize = 20 };
        Label label3 = new Label() { Text = node.componentID.ToString(), HorizontalOptions = LayoutOptions.Center, FontSize = 20 };

        gridAnswer.Add(label1, 1, gridAnswer.RowDefinitions.Count - 1);
        gridAnswer.Add(label2, 0, gridAnswer.RowDefinitions.Count - 1);
        gridAnswer.Add(label3, 2, gridAnswer.RowDefinitions.Count - 1);
    }


    void startAlgo_Clicked(System.Object sender, System.EventArgs e)
    {
        var graphView = this.graphDrawableView;
        var graphDrawable = (GraphDrawable)graphView.Drawable;

        int start = 0;
        bool inputIsValid = int.TryParse(beginningEntry.Text, out start);

        if (inputIsValid && graphDrawable.Nodes.Keys.Contains(start))
        {
            graphDrawable.resetNodes();
            dayCount = 1;
            componentCount = 1;
            startID = start;
            switchMode(5);

            switch (algorithmID)
            {
                case 0:
                    graphDrawable.currentIDs.Add(startID);
                    graphDrawable.Nodes[startID].dayVisited = dayCount;
                    graphDrawable.Nodes[startID].componentID = componentCount;
                    break;
                case 1:
                    graphDrawable.currentID = startID;
                    graphDrawable.visitedIDs.Add(startID);
                    graphDrawable.Nodes[startID].dayVisited = dayCount;
                    graphDrawable.Nodes[startID].visitedFrom = -1;
                    break;
            }
            addToGrid(graphDrawable.Nodes[startID]);
            graphView.Invalidate();
        }
        else
        {
            DisplayAlert("Ошибка", "Неверное значение вершины", "Ok");
        }
    }


    void resetAlgo_Clicked(System.Object sender, System.EventArgs e)
    {
        dayCount = 1;
        var graphView = this.graphDrawableView;
        var graphDrawable = (GraphDrawable)graphView.Drawable;
        graphDrawable.resetNodes();
        graphView.Invalidate();

        gridAnswer.Children.Clear();
        gridAnswer.RowDefinitions.Clear();

        switchMode(4);
    }


    void BFS()
    {
        dayCount++;

        bool nodeFound = false;

        var graphView = this.graphDrawableView;
        var graphDrawable = (GraphDrawable)graphView.Drawable;

        List<int> temp = new List<int>(graphDrawable.currentIDs);

        graphDrawable.visitedIDs.UnionWith(graphDrawable.currentIDs);

        graphDrawable.currentIDs.Clear();

        foreach (int id in temp)
        {
            foreach (Link link in graphDrawable.Links)
            {
                if (link.id1 == id && !graphDrawable.visitedIDs.Contains(link.id2) && !graphDrawable.currentIDs.Contains(link.id2))
                {
                    graphDrawable.currentIDs.Add(link.id2);
                    graphDrawable.Nodes[link.id2].componentID = componentCount;
                    graphDrawable.Nodes[link.id2].dayVisited = dayCount;
                    addToGrid(graphDrawable.Nodes[link.id2]);
                    nodeFound = true;
                }
                else if (link.id2 == id && !graphDrawable.visitedIDs.Contains(link.id1) && !graphDrawable.currentIDs.Contains(link.id1))
                {
                    graphDrawable.currentIDs.Add(link.id1);
                    graphDrawable.Nodes[link.id1].componentID = componentCount;
                    graphDrawable.Nodes[link.id1].dayVisited = dayCount;
                    addToGrid(graphDrawable.Nodes[link.id1]);
                    nodeFound = true;
                }
            }
        }

        if (!nodeFound)
        {
            foreach (Node node in graphDrawable.Nodes.Values)
            {
                if (!graphDrawable.visitedIDs.Contains(node.id))
                {
                    graphDrawable.currentIDs.Add(node.id);
                    node.componentID = ++componentCount;
                    node.dayVisited = dayCount;
                    addToGrid(node);
                    break;
                }
            }
        }

        graphView.Invalidate();

        if (graphDrawable.visitedIDs.Count + graphDrawable.currentIDs.Count == graphDrawable.Nodes.Count)
        {
            DisplayAlert("Успех", "Обход графа завершен", "Ok");
            switchMode(6);
        }
    }


    void DFS()
    {
        var graphView = this.graphDrawableView;
        var graphDrawable = (GraphDrawable)graphView.Drawable;

        bool newNodeFound = false;

        foreach (Link link in graphDrawable.Links)
        {
            if (link.id1 == graphDrawable.currentID && !graphDrawable.visitedIDs.Contains(link.id2))
            {
                graphDrawable.currentID = link.id2;
                graphDrawable.visitedIDs.Add(link.id2);
                graphDrawable.Nodes[link.id2].visitedFrom = link.id1;
                graphDrawable.Nodes[link.id2].dayVisited = ++dayCount;
                newNodeFound = true;
                addToGrid(graphDrawable.Nodes[link.id2]);
                break;
            }
            else if (link.id2 == graphDrawable.currentID && !graphDrawable.visitedIDs.Contains(link.id1))
            {
                graphDrawable.currentID = link.id1;
                graphDrawable.visitedIDs.Add(link.id1);
                graphDrawable.Nodes[link.id1].visitedFrom = link.id2;
                graphDrawable.Nodes[link.id1].dayVisited = ++dayCount;
                newNodeFound = true;
                addToGrid(graphDrawable.Nodes[link.id1]);
                break;
            }
        }

        if (!newNodeFound)
        {
            graphDrawable.currentID = graphDrawable.Nodes[(int)graphDrawable.currentID].visitedFrom;
        }

        graphView.Invalidate();

        if (graphDrawable.visitedIDs.Count == graphDrawable.Nodes.Count)
        {
            DisplayAlert("Успех", "Обход графа завершен", "Ok");
            switchMode(6);
        }
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


    void TapGestureRecognizer_Tapped(System.Object sender, Microsoft.Maui.Controls.TappedEventArgs e)
    {
        if (mode > 3) return;

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

                        foreach (Link link in linksToRemove) graphDrawable.Links.Remove(link);

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


    void switchMode(int num)
    {
        foreach (Button button in canvasButtons.Children) button.IsEnabled = true;
        nextStep.IsEnabled = startAlgo.IsEnabled = resetAlgo.IsEnabled = editGraph.IsEnabled = false;

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
            case 4:
                foreach (Button button in canvasButtons.Children) button.IsEnabled = false;
                startAlgo.IsEnabled = true;
                editGraph.IsEnabled = true;
                mode = 4;
                break;
            case 5:
                foreach (Button button in canvasButtons.Children) button.IsEnabled = false;
                nextStep.IsEnabled = true;
                resetAlgo.IsEnabled = true;
                break;
            case 6:
                foreach (Button button in canvasButtons.Children) button.IsEnabled = false;
                resetAlgo.IsEnabled = true;
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


    void acceptGraph_Clicked(System.Object sender, System.EventArgs e)
    {
        switchMode(4);
    }


    void editGraph_Clicked(System.Object sender, System.EventArgs e)
    {
        switchMode(0);
    }

    public MainPage()
    {
        InitializeComponent();
    }
}