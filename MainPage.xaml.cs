namespace graph_traversal;
using graph_traversal.Graph;
using graph_traversal.Drawable;
using System.Text.Json;

public partial class MainPage : ContentPage
{
    int mode = 0;
    int startID = 0;
    int endID = 0;
    int algorithmID = 0;

    int[,] matrix;

    void nextStep_Clicked(System.Object sender, System.EventArgs e)
    {
        var graphView = this.graphDrawableView;
        var graphDrawable = (GraphDrawable)graphView.Drawable;
        graphDrawable.Kruskal();
        graphView.Invalidate();

        int[,] matrix1 = new int[matrix.GetLength(0), matrix.GetLength(1) + 1];

        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                matrix1[i, j] = matrix[i, j];
            }
        }

        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            matrix1[i, matrix.GetLength(1)] = graphDrawable.Links[i].marker;
        }
        matrix = matrix1;


        if (graphDrawable.isFinished())
        {
            DisplayAlert("Успех", "Обход графа завершен", "Ok");
            switchMode(6);


            using (StreamWriter writer = new StreamWriter("/Users/jrrrrr/Desktop/table.csv"))
            {
                int rows = matrix.GetLength(0);
                int columns = matrix.GetLength(1);

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        writer.Write(matrix[i, j]);

                        if (j < columns - 1)
                        {
                            writer.Write(",");
                        }
                    }
                    writer.WriteLine();
                }
            }
        }
    }


    void startAlgo_Clicked(System.Object sender, System.EventArgs e)
    {
        var graphView = this.graphDrawableView;
        var graphDrawable = (GraphDrawable)graphView.Drawable;

        bool beginningIsValid = Int32.TryParse("1", out startID);

        if (beginningIsValid && graphDrawable.Nodes.Keys.Contains(startID))
        {
            graphDrawable.resetNodes();
            graphDrawable.QuickSortLinks(0, graphDrawable.Links.Count - 1);

            matrix = new int[graphDrawable.Links.Count, 4];
            for (int i = 0; i < graphDrawable.Links.Count; i++)
            {
                matrix[i, 0] = graphDrawable.Links[i].id1;
                matrix[i, 1] = graphDrawable.Links[i].id2;
                matrix[i, 2] = (int)graphDrawable.Links[i].weight;
                matrix[i, 3] = (int)graphDrawable.Links[i].marker;
            }

            switchMode(5);
            switch (algorithmID)
            {
                case 0:
                    graphDrawable.currentID = startID;
                    graphDrawable.Nodes[startID].weightSum = 0;
                    graphDrawable.Nodes[startID].visitedFrom = 0;
                    break;
                case 1:
                    graphDrawable.currentID = startID;
                    graphDrawable.visitedIDs.Add(startID);
                    graphDrawable.Nodes[startID].dayVisited = graphDrawable.dayCount;
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


    void Dijkstra()
    {
        var graphView = this.graphDrawableView;
        var graphDrawable = (GraphDrawable)graphView.Drawable;
        graphDrawable.Dijkstra();

        graphView.Invalidate();

        //List<Node> nodesList = graphDrawable.Nodes.Values.ToList();
        //resultListView.ItemsSource = graphDrawable.Nodes.Values.ToList();

        if (graphDrawable.currentID == endID)
        {

            Node currentNode = graphDrawable.Nodes[endID];

            while (currentNode != graphDrawable.Nodes[startID])
            {
                foreach (Link link in graphDrawable.Links)
                {
                    if ((link.id1 == currentNode.id && link.id2 == currentNode.visitedFrom) || (link.id1 == currentNode.visitedFrom && link.id2 == currentNode.id))
                    {
                        link.isPath = true;
                    }
                }
                currentNode = graphDrawable.Nodes[(int)currentNode.visitedFrom];
            }

            graphView.Invalidate();
            DisplayAlert("Успех", "Обход графа завершен", "Ok");
            switchMode(6);
        }

    }

    void algorithmSwitcher(System.Object sender, Microsoft.Maui.Controls.CheckedChangedEventArgs e)
    {
        //RadioButton radiobutton = sender as RadioButton;
        //if(radiobutton.IsChecked)
        //{
        //    if (radiobutton == ButtonBFS) algorithmID = 0;
        //    if (radiobutton == ButtonDFS) algorithmID = 1;
        //}
    }

    void addToGrid(Node node)
    {
        var graphView = this.graphDrawableView;
        var graphDrawable = (GraphDrawable)graphView.Drawable;
    }



    void clearCanvas_Clicked(System.Object sender, System.EventArgs e)
    {
        var graphView = this.graphDrawableView;
        var graphDrawable = (GraphDrawable)graphView.Drawable;
        graphDrawable.clearCanvas();
        graphView.Invalidate();
        switchMode(0);
    }

    async void TapGestureRecognizer_Tapped(System.Object sender, Microsoft.Maui.Controls.TappedEventArgs e)
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
                    await DisplayAlert("Ошибка", "Вершины пересекаются", "Ок");
                }
                else
                {
                    graphDrawable.Nodes[graphDrawable.Nodes.Count + 1] = (new Node() { id = graphDrawable.Nodes.Count + 1, X = X, Y = Y });
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
                                    await DisplayAlert("Ошибка", "Вершины уже соеденины", "Ок");
                                    graphDrawable.selectedID = null;
                                    return;
                                }
                            }

                            string result = await DisplayPromptAsync("Вес", "Введите вес дуги");
                            int weight;
                            if (Int32.TryParse(result, out weight) && weight > 0)
                            {
                                graphDrawable.Links.Add(new Link() { id1 = (int)graphDrawable.selectedID, id2 = node.id, weight = weight });
                                graphDrawable.selectedID = null;
                            }
                            else
                            {
                                await DisplayAlert("Ошибка", "Неверноеь значение веса дуги", "Ok");
                            }

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
                            await DisplayAlert("Ошибка", "Вершины не соединены", "Ok");
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
        nextStep.IsEnabled = resetAlgo.IsEnabled = false;
        startAlgo.IsEnabled = saveGraph.IsEnabled = loadGraph.IsEnabled = true;

        switch (num)
        {
            // Добавить вершины
            case 0:
                addNodesMode.IsEnabled = false;
                mode = 0;
                break;
            // Соединить вершины
            case 1:
                addLinksMode.IsEnabled = false;
                mode = 1;
                break;
            // Удалить вершины
            case 2:
                removeNodesMode.IsEnabled = false;
                mode = 2;
                break;
            // Отсоеденить вершины
            case 3:
                removeLinksMode.IsEnabled = false;
                mode = 3;
                break;
            // Начать обход
            case 5:
                foreach (Button button in canvasButtons.Children) button.IsEnabled = false;
                saveGraph.IsEnabled = loadGraph.IsEnabled = false;
                nextStep.IsEnabled = true;
                resetAlgo.IsEnabled = true;
                break;
            // Обзод завершен
            case 6:
                foreach (Button button in canvasButtons.Children) button.IsEnabled = false;
                saveGraph.IsEnabled = loadGraph.IsEnabled = false;
                resetAlgo.IsEnabled = true;
                break;
        }
    }





    void resetAlgo_Clicked(System.Object sender, System.EventArgs e)
    {
        var graphView = this.graphDrawableView;
        var graphDrawable = (GraphDrawable)graphView.Drawable;
        graphDrawable.resetNodes();
        graphView.Invalidate();

        //gridAnswer.Children.Clear();
        //gridAnswer.RowDefinitions.Clear();
        switchMode(0);
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


    public MainPage()
    {
        InitializeComponent();
    }

    async void loadGraph_Clicked(System.Object sender, System.EventArgs e)
    {
        var graphView = this.graphDrawableView;
        var graphDrawable = (GraphDrawable)graphView.Drawable;
        try
        {
            var result = await FilePicker.Default.PickAsync();
            if (result != null)
            {
                graphDrawable.resetNodes();

                GraphJsonObject jsonobject;

                using (StreamReader r = new StreamReader(result.FullPath))
                {
                    string json = r.ReadToEnd();
                    jsonobject = JsonSerializer.Deserialize<GraphJsonObject>(json);
                }

                graphDrawable.Nodes = jsonobject.Nodes;
                graphDrawable.Links = jsonobject.Links;
                graphView.Invalidate();
            }
        }
        catch (Exception ex)
        {
        }
    }


    void saveGraph_Clicked(System.Object sender, System.EventArgs e)
    {
        var graphView = this.graphDrawableView;
        var graphDrawable = (GraphDrawable)graphView.Drawable;

        GraphJsonObject jsonObject = new GraphJsonObject() { Nodes = graphDrawable.Nodes, Links = graphDrawable.Links };

        string fileName = "/Users/jrrrrr/Desktop/graph.json";
        string jsonString = JsonSerializer.Serialize(jsonObject);
        File.WriteAllText(fileName, jsonString);
    }

    async void test_Clicked(System.Object sender, System.EventArgs e)
    {
        string result = await DisplayPromptAsync("Вес", "Введите вес дуги");
    }

}