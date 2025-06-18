using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FindCoordsWPF
{
    public partial class MainWindow : Window
    {
        private readonly int minres = 10;
        private readonly int maxres = 360;
        private int midres => (minres + maxres) / 2;
        private List<string> objectData = new List<string>
{
    ("Палатка"),
    ("Флаг"),
    ("Дом"),
    ("Озеро"),
    ("Колодец"),
    ("Дуб"),
    ("Куст"),
    ("Вышка")
};

        private List<Point> objectCoordinates = new List<Point>();
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
                    Content = objectData[i],
                    Width = 60,
                    VerticalAlignment = VerticalAlignment.Center
                };

                var xBox = new TextBox
                {
                    Width = 30,
                    Margin = new Thickness(5, 0, 5, 0),
                    Text = ""
                };
                xBox.PreviewTextInput += (s, e) =>
                {
                    char c = e.Text[0];
                    string currentText = xBox.Text.Insert(xBox.CaretIndex, e.Text);
                    if (!(char.IsDigit(c) || c == '-' && (xBox.Text.Length == 0 || xBox.CaretIndex == 0)) || currentText.StartsWith("--"))
                    {
                        e.Handled = true;
                    }
                };
                xBox.PreviewKeyDown += (s, e) =>
                {
                    if (e.Key == Key.Space)
                    {
                        e.Handled = true;
                    }
                };

                var yBox = new TextBox
                {
                    Width = 30,
                    Text = ""
                };
                yBox.PreviewTextInput += (s, e) =>
                {
                    char c = e.Text[0];
                    string currentText = yBox.Text.Insert(yBox.CaretIndex, e.Text);
                    if (!(char.IsDigit(c) || c == '-' && (yBox.Text.Length == 0 || yBox.CaretIndex == 0)) || currentText.StartsWith("--"))
                    {
                        e.Handled = true;
                    }
                };
                yBox.PreviewKeyDown += (s, e) =>
                {
                    if (e.Key == Key.Space)
                    {
                        e.Handled = true;
                    }
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

            // Инициализируем objectCoordinates
            objectCoordinates.Clear();
            for (int i = 0; i < xpoints.Length; i++)
            {
                int x = XMath(xpoints[i]);
                int y = YMath(ypoints[i]);
                objectCoordinates.Add(new Point(x, y));
            }

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
        

                // Рисуем объект
                DrawSingleObject(i);
            }
        }
        private void ButtonRestart_Click(object sender, RoutedEventArgs e)
        {
            // Сброс координат
            GenerateUniqueRandomPoints();

            // Очистка и переинициализация текстовых полей
            PointsInputPanel.Items.Clear();
            xTextBoxes.Clear();
            yTextBoxes.Clear();
            InitializePointsInput();

            // Перерисовка сетки и точек
            DrawGridAndPoints();
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
        private void DrawSingleObject(int index)
        {
            if (index < 0 || index >= objectCoordinates.Count) return;

            Point point = objectCoordinates[index];
            double x = point.X + 10;  // правее
            double y = point.Y - 20;  // выше




            switch (index)
            {
                case 0: // Палатка
                    var shape = new Polygon
                    {
                        Fill = Brushes.Black,
                        Stroke = Brushes.Black,
                        Points = new PointCollection
                            {
                                new Point(0, -5),     // верх
                                new Point(-5, 5),    // низ слева
                                new Point(5, 5)      // низ справа
                            }
                    };
                    Canvas.SetLeft(shape, x);  // Center horizontally
                    Canvas.SetTop(shape, y + 10);   // Center vertically
                    var shape1 = new Polygon
                    {
                        Fill = Brushes.DarkGreen,
                        Stroke = Brushes.Black,
                        Points = new PointCollection
                            {
                                new Point(0, -10),     // верх
                                new Point(-10, 10),    // низ слева
                                new Point(10, 10)      // низ справа
                            }
                    };
                    Canvas.SetLeft(shape1, x);  // Center horizontally
                    Canvas.SetTop(shape1, y + 5);   // Center vertically

                    DrawingCanvas.Children.Add(shape1);
                    DrawingCanvas.Children.Add(shape);
                    break;


                case 1: // Пираты (флаг)
                    var flagpole = new Rectangle
                    {
                        Width = 3,
                        Height = 20,
                        Fill = Brushes.Black
                    };
                    Canvas.SetLeft(flagpole, x-5);
                    Canvas.SetTop(flagpole, y);
                    var flag = new Polygon
                    {
                        Fill = Brushes.Black,
                        Points = new PointCollection
                    {
                        new Point(x -2, y),
                        new Point(x + 10, y + 5),
                        new Point(x -2, y + 10)
                    }
                    };
                    DrawingCanvas.Children.Add(flagpole);
                    DrawingCanvas.Children.Add(flag);
                    return;

                case 2: // Дом
                    var houseBase = new Rectangle
                    {
                        Width = 20,
                        Height = 20,
                        Fill = Brushes.LightGreen,
                        Stroke = Brushes.Black
                    };
                    Canvas.SetLeft(houseBase, x-5);
                    Canvas.SetTop(houseBase, y);
                    var housewindow = new Rectangle
                    {
                        Width = 10,
                        Height = 10,
                        Fill = Brushes.LightBlue,
                        Stroke = Brushes.Black
                    };
                    var window1 = new Line
                    {
                        X1 = x ,
                        Y1 = y + 10,
                        X2 = x + 10,
                        Y2 = y + 10,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1
                    };
                    var window2 = new Line
                    {
                        X1 = x + 5,
                        Y1 = y + 5,
                        X2 = x + 5,
                        Y2 = y + 15,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1
                    };
                    Canvas.SetLeft(housewindow, x );
                    Canvas.SetTop(housewindow, y + 5);
                    var roof = new Polygon
                    {
                        Fill = Brushes.Green,
                        Stroke = Brushes.Black,
                        Points = new PointCollection
                    {
                        new Point(x-5, y),
                        new Point(x + 5, y - 10),
                        new Point(x + 15, y)
                    }
                    };
                    DrawingCanvas.Children.Add(houseBase);
                    DrawingCanvas.Children.Add(housewindow);
                    DrawingCanvas.Children.Add(window1);
                    DrawingCanvas.Children.Add(window2);
                    DrawingCanvas.Children.Add(roof);
                    return;

                case 3: // Родник (крестик-капля)
                    var spring = new Ellipse
                    {
                        Width = 15,
                        Height = 10,
                        Fill = Brushes.LightBlue,
                        Stroke = Brushes.Blue
                    };

                    Canvas.SetLeft(spring, x-10);
                    Canvas.SetTop(spring, y+5);
                    DrawingCanvas.Children.Add(spring);
                    return;

                case 4: // Колодец (цилиндр с ручкой)
                        // Перекладина (горизонтальная ручка)
                    var handle = new Line
                    {
                        X1 = x - 6,
                        Y1 = y +3,
                        X2 = x + 6,
                        Y2 = y +3,
                        Stroke = Brushes.SaddleBrown,
                        StrokeThickness = 1
                    };
                    var lefthandle = new Line
                    {
                        X1 = x - 4,
                        Y1 = y +3,
                        X2 = x - 4,
                        Y2 = y + 7,
                        Stroke = Brushes.SaddleBrown,
                        StrokeThickness = 1
                    };
                    var righthandle = new Line
                    {
                        X1 = x + 4,
                        Y1 = y +3,
                        X2 = x + 4,
                        Y2 = y + 7,
                        Stroke = Brushes.SaddleBrown,
                        StrokeThickness = 1
                    };
                    // Барабан ручки (сбоку)
                    var handleKnob = new Line
                    {
                        X1 = x + 6,
                        Y1 = y +2,
                        X2 = x + 6,
                        Y2 = y + 6,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1
                    };
                    // Верх колодца
                    var topOval = new Ellipse
                    {
                        Width = 10,
                        Height = 4,
                        Fill = Brushes.Gray,
                        Stroke = Brushes.Black
                    };
                    Canvas.SetLeft(topOval, x - 5);
                    Canvas.SetTop(topOval, y+5);

                    // Тело цилиндра
                    var body = new Rectangle
                    {
                        Width = 10,
                        Height = 10,
                        Fill = Brushes.DarkGray,
                        Stroke = Brushes.Black
                    };
                    Canvas.SetLeft(body, x - 5);
                    Canvas.SetTop(body, y + 7);

                    // Нижний овал
                    var bottomOval = new Ellipse
                    {
                        Width = 10,
                        Height = 4,
                        Fill = Brushes.Gray,
                        Stroke = Brushes.Black
                    };
                    Canvas.SetLeft(bottomOval, x - 5);
                    Canvas.SetTop(bottomOval, y + 15);

                    DrawingCanvas.Children.Add(handle);
                    DrawingCanvas.Children.Add(handleKnob);
                    DrawingCanvas.Children.Add(lefthandle); DrawingCanvas.Children.Add(righthandle);
                    DrawingCanvas.Children.Add(body);
                    DrawingCanvas.Children.Add(topOval);
                    DrawingCanvas.Children.Add(bottomOval);
                    return;

                case 5: // Дуб
                    var crown = new Ellipse
                    {
                        Width = 20,
                        Height = 15,
                        Fill = Brushes.Green,
                        Stroke = Brushes.Black
                    };
                    Canvas.SetLeft(crown, x-6);
                    Canvas.SetTop(crown, y);
                    var trunk = new Rectangle
                    {
                        Width = 4,
                        Height = 10,
                        Fill = Brushes.Brown
                    };
                    Canvas.SetLeft(trunk, x + 2);
                    Canvas.SetTop(trunk, y + 15);
                    DrawingCanvas.Children.Add(crown);
                    DrawingCanvas.Children.Add(trunk);
                    return;

                case 6: // Куст
                    {
                        // Центральный эллипс
                        var center = new Rectangle
                        {
                            Width = 11,
                            Height = 10,
                            Fill = Brushes.LimeGreen
                        };
                        Canvas.SetLeft(center, x-2);
                        Canvas.SetTop(center, y + 4);

                        // Левый эллипс
                        var left = new Ellipse
                        {
                            Width = 8,
                            Height = 7,
                            Fill = Brushes.LimeGreen
                        };
                        Canvas.SetLeft(left, x - 7);
                        Canvas.SetTop(left, y + 7);

                        // Правый эллипс
                        var right = new Ellipse
                        {
                            Width = 8,
                            Height = 7,
                            Fill = Brushes.LimeGreen
                        };
                        Canvas.SetLeft(right, x + 5);
                        Canvas.SetTop(right, y + 7);

                        // Верхний левый эллипс
                        var topLeft = new Ellipse
                        {
                            Width = 6,
                            Height = 5,
                            Fill = Brushes.LimeGreen
                        };
                        Canvas.SetLeft(topLeft, x - 3);
                        Canvas.SetTop(topLeft, y+2);
                        var topup = new Ellipse
                        {
                            Width = 6,
                            Height = 5,
                            Fill = Brushes.LimeGreen
                        };
                        Canvas.SetLeft(topup, x + 1);
                        Canvas.SetTop(topup, y+2);
                        var toplup = new Ellipse
                        {
                            Width = 8,
                            Height = 6,
                            Fill = Brushes.LimeGreen
                        };
                        Canvas.SetLeft(toplup, x - 6);
                        Canvas.SetTop(toplup, y + 4);
                        var toprup = new Ellipse
                        {
                            Width = 8,
                            Height = 6,
                            Fill = Brushes.LimeGreen
                        };
                        Canvas.SetLeft(toprup, x + 5);
                        Canvas.SetTop(toprup, y + 4);
                        // Верхний правый эллипс
                        var topRight = new Ellipse
                        {
                            Width = 6,
                            Height = 5,
                            Fill = Brushes.LimeGreen
                        };
                        Canvas.SetLeft(topRight, x + 6);
                        Canvas.SetTop(topRight, y);

                        // Добавляем все эллипсы на Canvas
                        DrawingCanvas.Children.Add(center);
                        DrawingCanvas.Children.Add(left);
                        DrawingCanvas.Children.Add(topLeft);
                        DrawingCanvas.Children.Add(topup);
                        DrawingCanvas.Children.Add(toplup);
                        DrawingCanvas.Children.Add(toprup);
                        DrawingCanvas.Children.Add(topRight);
                        DrawingCanvas.Children.Add(right);
                        return;
                    }


                case 7: // Вышка
                    var towerBase = new Rectangle
                    {
                        Width = 7,
                        Height = 5,
                        Fill = Brushes.Green,
                        Stroke = Brushes.Black
                    };
                    Canvas.SetLeft(towerBase, x - 4);
                    Canvas.SetTop(towerBase, y + 5);
                    var towerBase2 = new Polygon
                    {
                        Fill = Brushes.DarkGreen,
                        Stroke = Brushes.Black,
                        Points = new PointCollection
                {
                    new Point(x + 2, y + 5),
                    new Point(x + 2, y + 10),
                    new Point(x + 7, y + 8),
                    new Point(x + 7, y +4)
                }
                    };
                    var roofTop = new Polygon
                    {
                        Fill = Brushes.DarkGreen,
                        Stroke = Brushes.Black,
                        Points = new PointCollection
                {
                    new Point(x - 4, y+3),
                    new Point(x + 2, y+3),
                    new Point(x, y - 2)
                }
                    };
                    var roofTop2 = new Polygon
                    {
                        Fill = Brushes.DarkGreen,
                        Stroke = Brushes.Black,
                        Points = new PointCollection
                {
                    new Point(x + 2, y+3),
                    new Point(x, y - 2),
                    new Point(x + 7, y+2)
                }
                    };
                    var rightUpLeg = new Line
                    {
                        X1 = x + 7,
                        Y1 = y+2,
                        X2 = x + 7,
                        Y2 = y + 14,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1
                    };
                    var rightDownLeg = new Line
                    {
                        X1 = x - 4,
                        Y1 = y+2,
                        X2 = x - 4,
                        Y2 = y + 16,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1
                    };
                    var leftDownLeg = new Line
                    {
                        X1 = x + 3,
                        Y1 = y + 3,
                        X2 = x + 3,
                        Y2 = y + 16,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1
                    };
                    DrawingCanvas.Children.Add(towerBase);
                    DrawingCanvas.Children.Add(towerBase2);
                    DrawingCanvas.Children.Add(roofTop2);
                    DrawingCanvas.Children.Add(roofTop);
                    DrawingCanvas.Children.Add(rightUpLeg);
                    DrawingCanvas.Children.Add(rightDownLeg);
                    DrawingCanvas.Children.Add(leftDownLeg);
                    return;

                default:
                    return;
            }

            // Общее добавление для простых фигур
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}