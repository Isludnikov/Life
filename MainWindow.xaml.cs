using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Life
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private lib.Aerial aerial = null;
        private readonly Timer timer = null;
        private Button[,] buttonArray = null;
        public MainWindow()
        {
            InitializeComponent();
            timer = new Timer(lib.Config.TimerInterval);
            timer.Elapsed += OnTimer;
            CreateEnvironment(lib.Config.xDimension, lib.Config.yDimension);
            Repaint();
        }
        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            switch (aerial.Step())
            {
                case lib.StepResult.NORMAL:
                    Dispatcher.Invoke(() => console.Text = $"iteration {aerial.Iteration()}");
                    break;
                case lib.StepResult.DEAD:
                    Dispatcher.Invoke(() => console.Text = $"no survivors at iteration {aerial.Iteration()}");
                    Stop_Click(sender, null);
                    break;
                case lib.StepResult.CYCLING:
                    Dispatcher.Invoke(() => console.Text = $"deadlock at iteration {aerial.Iteration()}");
                    Stop_Click(sender, null);
                    break;

            }
            Dispatcher.Invoke(() => Repaint());
        }
        private void Repaint()
        {
            for (int k = 0; k < aerial.XDimension; k++)
            {
                for (int j = 0; j < aerial.YDimension; j++)
                {
                    //buttonArray[k, j].Content = aerial.GetMarker(k, j);
                    buttonArray[k, j].Background = aerial.Get(k, j) ? Brushes.Black : Brushes.White;
                }
            }
        }
        private void Repaint(int x, int y)
        {
            for (int k = 0; k < aerial.XDimension; k++)
            {
                for (int j = 0; j < aerial.YDimension; j++)
                {
                    if (k <= x + 1 && k >= x - 1 || j <= y + 1 && j >= y - 1)
                    {
                        //buttonArray[k, j].Content = aerial.GetMarker(k, j);
                        buttonArray[k, j].Background = aerial.Get(k, j) ? Brushes.Black : Brushes.White;
                    }
                }
            }
        }
        private void CreateEnvironment(int x, int y)
        {
            aerial = new lib.Aerial(x, y);
            aerial.Sow(0.4);

            buttonArray = new Button[x, y];

            cells.Children.Clear();
            cells.RowDefinitions.Clear();
            cells.ColumnDefinitions.Clear();

            for (int i = 0; i < aerial.XDimension; i++)
            {
                cells.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < aerial.YDimension; i++)
            {
                cells.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < aerial.XDimension; i++)
            {
                for (int j = 0; j < aerial.YDimension; j++)
                {
                    var button = new Button
                    {
                        Tag = new lib.Locator(i, j),
                    };
                    button.Click += Slave_Click;
                    cells.Children.Add(button);
                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, j);
                    buttonArray[i, j] = button;

                }
            }
        }
        private void Slave_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                var btn = sender as Button;
                var locator = btn.Tag as lib.Locator;
                aerial.Set(locator.X, locator.Y, !aerial.Get(locator.X, locator.Y));
                Repaint(locator.X, locator.Y);
            }
        }
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            timer.Start();
            OnTimer(sender, null);
        }
        private void Stop_Click(object sender, RoutedEventArgs e) => timer.Stop();
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            aerial.Clear();
            Repaint();
        }
        private void Random_Click(object sender, RoutedEventArgs e)
        {
            aerial.Sow();
            Repaint();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            console.Text = "not implemented";
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            console.Text = "not implemented";
        }
    }
}
