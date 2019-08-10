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
        private lib.Aerial aerial = null;
        private readonly Timer timer = null;
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
                    Dispatcher.Invoke(() => Repaint());
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
        }
        private void Repaint()
        {
            for (int k = 0; k < cells.Children.Count; k++)
            {
                if (cells.Children[k] is Button)
                {
                    var btn = cells.Children[k] as Button;
                    var locator = btn.Tag as lib.Locator;
                    btn.Content = aerial.GetMarker(locator.X, locator.Y);
                }
            }
        }
        private void Repaint(int x, int y)
        {
            for (int k = 0; k < cells.Children.Count; k++)
            {
                if (cells.Children[k] is Button)
                {
                    var btn = cells.Children[k] as Button;
                    var locator = btn.Tag as lib.Locator;
                    if (locator.X <= x + 1 && locator.X >= x - 1 || locator.Y <= y + 1 && locator.Y >= y - 1)
                    {
                        btn.Content = aerial.GetMarker(locator.X, locator.Y);
                    }
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
                        Tag = new lib.Locator(i, j),
                    };
                    button.Click += Slave_Click;
                    cells.Children.Add(button);
                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, j);
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
