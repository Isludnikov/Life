using System;
using System.IO;
using System.Timers;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Life.lib;
using Microsoft.Win32;

namespace Life
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Aerial _aerial;
        private readonly Timer _timer;
        private Button[,] _buttonArray;
        public MainWindow()
        {
            InitializeComponent();
            _timer = new Timer(Config.TimerInterval);
            _timer.Elapsed += OnTimer;
            _aerial = new Aerial(Config.XDimension, Config.YDimension);
            CreateEnvironment();
            _aerial.Sow(0.4);
            Repaint();
            InnerSave(".\\autosave" + DateTimeOffset.UtcNow.ToUnixTimeSeconds() + ".life");
        }
        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            switch (_aerial.Step())
            {
                case StepResult.NORMAL:
                    Dispatcher.Invoke(() => console.Text = $"iteration {_aerial.Iteration}");
                    break;
                case StepResult.DEAD:
                    Dispatcher.Invoke(() => console.Text = $"no survivors at iteration {_aerial.Iteration}");
                    Stop_Click(sender, null);
                    break;
                case StepResult.CYCLING:
                    Dispatcher.Invoke(() => console.Text = $"deadlock at iteration {_aerial.Iteration}");
                    Stop_Click(sender, null);
                    break;

            }
            Dispatcher.Invoke(Repaint);
        }
        private void Repaint()
        {
            for (var k = 0; k < _aerial.XDimension; k++)
            {
                for (var j = 0; j < _aerial.YDimension; j++)
                {
                    _buttonArray[k, j].Background = _aerial.Get(k, j) ? Brushes.Black : Brushes.White;
                }
            }
        }
        private void Repaint(int x, int y)
        {
            for (var k = 0; k < _aerial.XDimension; k++)
            {
                for (var j = 0; j < _aerial.YDimension; j++)
                {
                    if (k <= x + 1 && k >= x - 1 || j <= y + 1 && j >= y - 1)
                    {
                        _buttonArray[k, j].Background = _aerial.Get(k, j) ? Brushes.Black : Brushes.White;
                    }
                }
            }
        }
        private void CreateEnvironment()
        {
            _buttonArray = new Button[_aerial.XDimension, _aerial.YDimension];

            cells.Children.Clear();
            cells.RowDefinitions.Clear();
            cells.ColumnDefinitions.Clear();

            for (var i = 0; i < _aerial.XDimension; i++)
            {
                cells.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (var i = 0; i < _aerial.YDimension; i++)
            {
                cells.RowDefinitions.Add(new RowDefinition());
            }

            for (var i = 0; i < _aerial.XDimension; i++)
            {
                for (var j = 0; j < _aerial.YDimension; j++)
                {
                    var button = new Button
                    {
                        Tag = new Locator(i, j)
                    };
                    button.Click += Slave_Click;
                    cells.Children.Add(button);
                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, j);
                    _buttonArray[i, j] = button;

                }
            }
        }

        private void Slave_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button btn)) return;

            var locator = btn.Tag as Locator;
            _aerial.Set(locator.X, locator.Y, !_aerial.Get(locator.X, locator.Y));
            Repaint(locator.X, locator.Y);
        }
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            _timer.Start();
            OnTimer(sender, null);
        }
        private void Stop_Click(object sender, RoutedEventArgs e) => _timer.Stop();
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            _aerial.Clear();
            Repaint();
        }
        private void Random_Click(object sender, RoutedEventArgs e)
        {
            _aerial.Sow();
            Repaint();
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog1 = new SaveFileDialog
            {
                Filter = "life files (*.life)|*.life|All files (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (saveFileDialog1.ShowDialog() == true)
            {
                InnerSave(saveFileDialog1.FileName);
            }
        }
        private void InnerSave(string filename)
        {
            var myStream = new StreamWriter(filename);
            var saveState = _aerial.GetState();
            var json = new JavaScriptSerializer().Serialize(saveState);
            myStream.Write(json);
            myStream.Close();
        }
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog1 = new OpenFileDialog
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
                var json = new JavaScriptSerializer().Deserialize<AerialStateObject>(text);
                _aerial.LoadState(json);
                CreateEnvironment();
                Repaint();
            }

        }
    }
}
