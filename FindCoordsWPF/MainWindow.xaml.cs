using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FindCoordsWPF
{
    public partial class MainWindow : Window
    {
        private readonly int minres = 10;
        private readonly int maxres = 360;
        private int midres => (minres + maxres) / 2;

        private readonly int[] xpoints = new int[8];
        private readonly int[] ypoints = new int[8];
        private Random rand = new Random();

        private void GenerateUniqueRandomPoints()
        {
            var pointsSet = new HashSet<(int x, int y)>();

            int count = 0;
            while (count < 8)
            {
                int x = rand.Next(-6, 7); // от -6 до 6 включительно
                int y = rand.Next(-6, 7);

                if (!pointsSet.Contains((x, y)))
                {
                    pointsSet.Add((x, y));
                    xpoints[count] = x;
                    ypoints[count] = y;
                    count++;
                }
                // Если точка уже есть, генерируем заново
            }
        }

        private readonly List<TextBox> xTextBoxes = new List<TextBox>();
        private readonly List<TextBox> yTextBoxes = new List<TextBox>();

        public MainWindow()
        {
            GenerateUniqueRandomPoints();
            InitializeComponent();
            InitializePointsInput();
            DrawGridAndPoints();
        }

        private int XMath(int num) => midres + 25 * num;
        private int YMath(int num) => midres - 25 * num;

        private void InitializePointsInput()
        {
            for (int i = 0; i < xpoints.Length; i++)
            {
                var panel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 5) };

                var label = new Label
                {
                    Content = $"{i + 1} точка",
                    Width = 50,
                    VerticalAlignment = VerticalAlignment.Center
                };

                var xBox = new TextBox
                {
                    Width = 30,
                    Margin = new Thickness(5, 0, 5, 0),
                    Text = ""
                };

                var yBox = new TextBox
                {
                    Width = 30,
                    Text = ""
                };

                panel.Children.Add(label);
                panel.Children.Add(xBox);
                panel.Children.Add(yBox);

                PointsInputPanel.Items.Add(panel);

                xTextBoxes.Add(xBox);
                yTextBoxes.Add(yBox);
            }
        }

        private void DrawGridAndPoints()
        {
            DrawingCanvas.Children.Clear();

            // Рисуем сетку
            for (int i = minres; i <= maxres; i += 25)
            {
                // Горизонтальные линии
                var hLine = new Line
                {
                    X1 = minres,
                    Y1 = i,
                    X2 = maxres,
                    Y2 = i,
                    Stroke = Brushes.LightBlue,
                    StrokeThickness = 1
                };
                DrawingCanvas.Children.Add(hLine);

                // Вертикальные линии
                var vLine = new Line
                {
                    X1 = i,
                    Y1 = minres,
                    X2 = i,
                    Y2 = maxres,
                    Stroke = Brushes.LightBlue,
                    StrokeThickness = 1
                };
                DrawingCanvas.Children.Add(vLine);
            }

            // Оси
            var xAxis = new Line
            {
                X1 = minres,
                Y1 = midres,
                X2 = maxres,
                Y2 = midres,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            DrawingCanvas.Children.Add(xAxis);

            var yAxis = new Line
            {
                X1 = midres,
                Y1 = minres,
                X2 = midres,
                Y2 = maxres,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            DrawingCanvas.Children.Add(yAxis);

            // Рисуем точки
            for (int i = 0; i < xpoints.Length; i++)
            {
                int x = XMath(xpoints[i]);
                int y = YMath(ypoints[i]);

                // Точка
                var ellipse = new Ellipse
                {
                    Width = 10,
                    Height = 10,
                    Fill = Brushes.Blue
                };
                Canvas.SetLeft(ellipse, x - 5);
                Canvas.SetTop(ellipse, y - 5);
                DrawingCanvas.Children.Add(ellipse);

                // Метка с номером точки
                var label = new Label
                {
                    Content = (i + 1).ToString(),
                    FontSize = 12,
                    Foreground = Brushes.Black,
                    Background = Brushes.Transparent
                };
                Canvas.SetLeft(label, x + 5);
                Canvas.SetTop(label, y - 15);
                DrawingCanvas.Children.Add(label);
            }
        }

        private void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            int incorrectPoints = 0;

            for (int i = 0; i < xpoints.Length; i++)
            {
                if (int.TryParse(xTextBoxes[i].Text, out int x) && int.TryParse(yTextBoxes[i].Text, out int y))
                {
                    if (x != xpoints[i] || y != ypoints[i])
                    {
                        incorrectPoints++;
                    }
                }
                else
                {
                    incorrectPoints++;
                }
            }

            if (incorrectPoints == 0)
            {
                MessageBox.Show("Все точки указаны верно!", "Результат", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"Неверно указаны {incorrectPoints} точек.", "Результат", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
