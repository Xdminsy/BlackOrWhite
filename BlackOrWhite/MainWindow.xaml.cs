using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.IO;

namespace BlackOrWhite
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public int score { get; set; }
        double cellHeight = 0, cellWidth = 0;

        private void WinLoaded(object sender, RoutedEventArgs e)
        {
        }
        Random random = new Random();
        Cell[][] lines;
        private void InitializePlayground()
        {
            score = 0;
            Score.Content = score;
            lines = new Cell[4][];
            for (int i = 0; i < 4; i++)
            {
                lines[i] = newLine(i);
            }
        }

        public Cell[] newLine(int i)
        {
            Cell[] line = new Cell[4];
            int black = random.Next(4);
            for (int j = 0; j < 4; j++)
            {
                Color color;
                if (j == black) color = Colors.Black;
                else color = Colors.White;
                Cell c = line[j] = new Cell(color);
                c.Height = cellHeight;
                c.Width = cellWidth;
                c.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                c.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                c.Margin = new Thickness(j * cellWidth, i * cellHeight, 0, (1 - i) * cellHeight);
                c.MouseDown += mouseDown;
                Playground.Children.Add(c);
            }
            return line;
        }

        private void PlaygroundLoaded(object sender, RoutedEventArgs e)
        {
            cellHeight = Playground.Height / 4;
            cellWidth = Playground.Width / 4;
            InitializePlayground();
        }

        public void mouseDown(object sender, MouseButtonEventArgs e)
        {
            Cell c = sender as Cell;
            Color color = c.color;
            if (color == Colors.White)
            {
                ColorAnimation a = new ColorAnimation();
                a.From = Colors.White;
                a.To = Colors.Red;
                a.Duration = TimeSpan.FromSeconds(0.2);
                a.RepeatBehavior = new RepeatBehavior(3);
                a.Completed += (s, ev) => {
                    End.Visibility = System.Windows.Visibility.Visible;
                    EndScore.Content = score;
                };
                c.Background.BeginAnimation(SolidColorBrush.ColorProperty, a);
                Playground.IsEnabled = false;
                return;
            }
            else if (color == Colors.Black)
            {
                if (!lines[3].Contains(c))
                {
                    return;
                }
                ColorAnimation a = new ColorAnimation();
                a.From = color;
                a.To = c.color = Colors.Gray;
                a.Duration = TimeSpan.FromSeconds(0.2);
                c.Background.BeginAnimation(SolidColorBrush.ColorProperty, a);
                score++;
                Score.Content = score;
            }

            Cell[] line4 = lines[3];
            ThicknessAnimation animation = new ThicknessAnimation();
            for (int i = 4; i >= 0; i--)
            {
                Cell[] line;
                if (i >= 4) line = line4;
                else if (i > 0) line = lines[i] = lines[i - 1];
                else line = lines[0] = newLine(0);
                for (int j = 0; j < 4; j++)
                {
                    Cell cell = line[j];
                    animation.From = new Thickness(j * cellWidth, (i - 1) * cellHeight, 0, 0);
                    animation.To = new Thickness(j * cellWidth, i * cellHeight, 0, 0);
                    animation.Duration = TimeSpan.FromSeconds(0.2);
                    animation.Completed += (s, ev) => cell.Margin = new Thickness(j * cellWidth, (i + 1) * cellHeight, 0, 0);
                    if (i == 4)
                    {
                        animation.Completed += (s, ev) => Playground.Children.Remove(cell);
                    }
                    cell.BeginAnimation(Grid.MarginProperty, animation);
                }
            }
        }


        public class Cell : Border
        {
            public Color color;
            public Cell(Color color)
            {
                this.BorderBrush = new SolidColorBrush(Colors.Black);
                this.BorderThickness = new Thickness(0.8);
                this.color = color;
                this.Background = new SolidColorBrush(color);
                this.Visibility = Visibility.Visible;
            }
        }

        private void newGame()
        {
            End.Visibility = System.Windows.Visibility.Hidden;
            Playground.IsEnabled = true;
            InitializePlayground();
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            newGame();
        }


        private void win_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                newGame();
            }
        }

    }
}