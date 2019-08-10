using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace Life
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int xDimension = 12;
        private const int yDimension = 12;
        private const int TimerInterval = 2000;
        private lib.Aerial aerial = null;
        private readonly Timer timer = null;
        public MainWindow()
        {
            InitializeComponent();
            timer = new Timer(TimerInterval);
            timer.Elapsed += OnTimer;
            CreateEnvironment(xDimension, yDimension);
            Repaint();
        }
        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            if (aerial.Step())
            {
                Dispatcher.Invoke(() => console.Text = $"iteration {aerial.Iteration()}");
                //console.Text = $"iteration {aerial.Iteration()}";
                Dispatcher.Invoke(() => Repaint());
                //Repaint();
            }
            else
            {
                Dispatcher.Invoke(() => console.Text = $"no survivors at iteration {aerial.Iteration()}");
                //console.Text = $"no survivors at iteration {aerial.Iteration()}";
                Stop_Click(sender, null);
            }
        }
        private void Repaint()
        {
            for (int k = 0; k < cells.Children.Count; k++)
            {
                if (cells.Children[k] is Button)
                {
                    var btn = cells.Children[k] as Button;
                    var locator = btn.Tag as lib.Locator;
                    btn.Content = aerial.Get(locator.X, locator.Y) ? "X" : "";
                }
            }

        }
        private void CreateEnvironment(int x, int y)
        {
            aerial = new lib.Aerial(x, y);
            aerial.Sow(0.4);
            cells.Children.Clear();
            cells.RowDefinitions.Clear();
            cells.ColumnDefinitions.Clear();

            for (int i = 0; i < aerial.XDimension(); i++)
            {
                cells.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < aerial.YDimension(); i++)
            {
                cells.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < aerial.XDimension(); i++)
            {
                for (int j = 0; j < aerial.YDimension(); j++)
                {
                    var button = new Button
                    {
                        Name = "btn_" + i + "_" + j,
                        Tag = new lib.Locator(i, j),
                    };
                    cells.Children.Add(button);
                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, j);
                }
            }
        }
        private void Start_Click(object sender, RoutedEventArgs e) => timer.Start();
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
    }
}
