using Microsoft.Win32;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Web.Script.Serialization;
using System.Text;

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
            aerial = new lib.Aerial(lib.Config.xDimension, lib.Config.yDimension);
            CreateEnvironment(aerial);
            aerial.Sow(0.4);
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
        private void CreateEnvironment(lib.Aerial aerial)
        {
            buttonArray = new Button[aerial.XDimension, aerial.YDimension];

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
            Stream myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog
            {
                Filter = "life files (*.life)|*.life|All files (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (saveFileDialog1.ShowDialog() == true)
            {
                if ((myStream = saveFileDialog1.OpenFile()) != null)
                {
                    var save = aerial.GetState();
                    var json = new JavaScriptSerializer().Serialize(save);
                    var bytes = Encoding.UTF8.GetBytes(json);
                    myStream.Write(bytes, 0, bytes.Length);
                    myStream.Close();
                }
            }
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "life",
                Filter = "life files (*.life)|*.life|All files (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == true)
            {
                var text = File.ReadAllText(openFileDialog1.FileName);
                var json = new JavaScriptSerializer().Deserialize<lib.AerialStateObject>(text);
                aerial.LoadState(json);
                CreateEnvironment(aerial);
                Repaint();
            }

        }
    }
}
