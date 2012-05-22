using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameEngine.Core.VisualTests
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Node StartingPoint { get; set; }
        Node EndingPoint { get; set; }

        private List<Node> _obstructions = new List<Node>();
        List<Node> Obstructions
        {
            get { return _obstructions; }
            set { _obstructions = value; }
        }

        public MainWindow()
        {
            InitializeComponent();
            MainGrid.MouseUp += MainGrid_MouseUp;
            DrawGrid();
        }

        void MainGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var mousePos = e.GetPosition(MainGrid);

            //MessageBox.Show(string.Format("You clicked X:{0} Y:{1}", mousePos.X, mousePos.Y), "Mouse Position", MessageBoxButton.OKCancel,
            //    MessageBoxImage.Exclamation);

            var startingX = (mousePos.X - mousePos.X % 20) + 1;
            var startingY = (mousePos.Y - mousePos.Y % 20) + 1;

            var poly = MainGrid.Children.OfType<Polygon>().Where(x => x.Points.Where(y => y.X == startingX && y.Y == startingY).Any()).FirstOrDefault();

            if (poly != null)
                switch (MainGrid.Children.OfType<RadioButton>().Where(x => x.IsChecked ?? false).First().Content.ToString())
                {
                    case "Pick Start":
                        (MainGrid.Children.OfType<Polygon>().Where(x => x.Fill == Brushes.Red).FirstOrDefault() ?? new Polygon()).Fill = Brushes.White;
                        poly.Fill = Brushes.Red;

                        StartingPoint = new Node((int)((startingX - 1) / 20), (int)((startingY - 1) / 20));

                        break;
                    case "Pick End":
                        (MainGrid.Children.OfType<Polygon>().Where(x => x.Fill == Brushes.DarkRed).FirstOrDefault() ?? new Polygon()).Fill = Brushes.White;
                        poly.Fill = Brushes.DarkRed;

                        EndingPoint = new Node((int)((startingX - 1) / 20), (int)((startingY - 1) / 20));

                        break;
                    case "Pick Wall":
                        if (poly.Fill == Brushes.Blue)
                        {
                            Obstructions.Remove(Obstructions.Where(o => o.X == (int)((startingX - 1) / 20) && o.Y == (int)((startingY - 1) / 20)).First());
                            poly.Fill = Brushes.White;
                        }
                        else
                        {
                            Obstructions.Add(new Node((int)((startingX - 1) / 20), (int)((startingY - 1) / 20)));
                            poly.Fill = Brushes.Blue;
                        }
                        break;
                    default:
                        poly.Fill = Brushes.White;
                        break;
                }
        }

        public void DrawGrid()
        {
            int Width = 32;
            int Height = 32;

            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                {
                    var unit = new Polygon()
                    {
                        Points = new PointCollection() {
                            new Point() {
                                X = (x * 20) + 1,
                                Y = (y * 20) + 1,
                            },
                            new Point() {
                                X = (x * 20) + 1,
                                Y = (y * 20) + 20,
                            },
                            new Point() {
                                X = (x * 20) + 20,
                                Y = (y * 20) + 20,
                            },
                            new Point() {
                                X = (x * 20) + 20,
                                Y = (y * 20) + 1,
                            }
                        },
                        Stroke = Brushes.Black,
                        StrokeThickness = -1,
                        Fill = Brushes.White

                    };

                    MainGrid.Children.Add(unit);
                }
        }

        private void FindPath_Click(object sender, RoutedEventArgs e)
        {
            StartingPoint.State = NodeState.Open;

            foreach (var node in MainGrid.Children.OfType<Polygon>().Where(x => x.Fill == Brushes.LightGreen))
            {
                node.Fill = Brushes.White;
            }

            var path = new Pathing(Obstructions, 0, 0, 31, 31, 1).CreatePath(StartingPoint, EndingPoint);

            path.Remove(path.Where(x => x.X == StartingPoint.X && x.Y == StartingPoint.Y).FirstOrDefault());
            path.Remove(path.Where(x => x.X == EndingPoint.X && x.Y == EndingPoint.Y).FirstOrDefault());

            if (!path.Any())
                MessageBox.Show("Path Not Found");

            foreach (var item in path)
            {
                var currentX = (item.X * 20) + 1;
                var currentY = (item.Y * 20) + 1;

                MainGrid.Children.OfType<Polygon>().Where(x => x.Points.Where(y => y.X == currentX && y.Y == currentY).Any()).First().Fill = Brushes.LightGreen;
            }
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var node in MainGrid.Children.OfType<Polygon>())
            {
                node.Fill = Brushes.White;
            }

            StartingPoint = null;
            EndingPoint = null;
            Obstructions = new List<Node>();
        }
    }
}
