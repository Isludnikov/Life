using Microsoft.Win32;
using System;
using System.IO;
using System.Timers;
using System.Web.Script.Serialization;
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
        private readonly lib.Aerial Aerial = null;
        private readonly Timer timer = null;
        private Button[,] buttonArray = null;
        public MainWindow()
        {
            InitializeComponent();
            timer = new Timer(lib.Config.TimerInterval);
            timer.Elapsed += OnTimer;
            Aerial = new lib.Aerial(lib.Config.xDimension, lib.Config.yDimension);
            CreateEnvironment();
            Aerial.Sow(0.4);
            Repaint();
            InnerSave(".\\autosave" + DateTimeOffset.UtcNow.ToUnixTimeSeconds() + ".life");
        }
        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            switch (Aerial.Step())
            {
                case lib.StepResult.NORMAL:
                    Dispatcher.Invoke(() => console.Text = $"iteration {Aerial.Iteration()}");
                    break;
                case lib.StepResult.DEAD:
                    Dispatcher.Invoke(() => console.Text = $"no survivors at iteration {Aerial.Iteration()}");
                    Stop_Click(sender, null);
                    break;
                case lib.StepResult.CYCLING:
                    Dispatcher.Invoke(() => console.Text = $"deadlock at iteration {Aerial.Iteration()}");
                    Stop_Click(sender, null);
                    break;

            }
            Dispatcher.Invoke(() => Repaint());
        }
        private void Repaint()
        {
            for (int k = 0; k < Aerial.XDimension; k++)
            {
                for (int j = 0; j < Aerial.YDimension; j++)
                {
                    //buttonArray[k, j].Content = aerial.GetMarker(k, j);
                    buttonArray[k, j].Background = Aerial.Get(k, j) ? Brushes.Black : Brushes.White;
                }
            }
        }
        private void Repaint(int x, int y)
        {
            for (int k = 0; k < Aerial.XDimension; k++)
            {
                for (int j = 0; j < Aerial.YDimension; j++)
                {
                    if (k <= x + 1 && k >= x - 1 || j <= y + 1 && j >= y - 1)
                    {
                        buttonArray[k, j].Background = Aerial.Get(k, j) ? Brushes.Black : Brushes.White;
                    }
                }
            }
        }
        private void CreateEnvironment()
        {
            buttonArray = new Button[Aerial.XDimension, Aerial.YDimension];

            cells.Children.Clear();
            cells.RowDefinitions.Clear();
            cells.ColumnDefinitions.Clear();

            for (int i = 0; i < Aerial.XDimension; i++)
            {
                cells.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < Aerial.YDimension; i++)
            {
                cells.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < Aerial.XDimension; i++)
            {
                for (int j = 0; j < Aerial.YDimension; j++)
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
                Aerial.Set(locator.X, locator.Y, !Aerial.Get(locator.X, locator.Y));
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
            Aerial.Clear();
            Repaint();
        }
        private void Random_Click(object sender, RoutedEventArgs e)
        {
            Aerial.Sow();
            Repaint();
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog
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
            var save = Aerial.GetState();
            var json = new JavaScriptSerializer().Serialize(save);
            myStream.Write(json);
            myStream.Close();
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
                Aerial.LoadState(json);
                CreateEnvironment();
                Repaint();
            }

        }
    }
}
