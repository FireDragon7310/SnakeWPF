﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
using System.Windows.Threading;
using System.Xml.Linq;

namespace SnakeWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int SnakeSpeed = 10;
        private int dx = SnakeSpeed;
        private int dy = 0;
        private DispatcherTimer gameTimer;
        private Food food;
        readonly List<Point> snakeSegments = new List<Point>();
        private bool isPaused = false;

        public MainWindow()
        {
            InitializeComponent();
            SetupGame();
            StartGameLoop();
        }

        private void SetupGame()
        {
            Rectangle snake = new Rectangle
            {
                Width = 20,
                Height = 20,
                Fill = Brushes.Green,
                            
            };
            GameSpace.Children.Add(snake);
            Canvas.SetLeft(snake, 0);
            Canvas.SetTop(snake, 0);
            snakeSegments.Add(new Point(0, 0));

            food = new Food(GameSpace, snakeSegments);

            this.KeyDown += MainWindow_KeyDown;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.A:
                    dx = -SnakeSpeed;
                    dy = 0;
                    break;

                case Key.D:
                    dx = SnakeSpeed;
                    dy = 0;
                    break;

                case Key.W:
                    dx = 0;
                    dy = -SnakeSpeed;
                    break;

                case Key.S:
                    dx = 0;
                    dy = SnakeSpeed;
                    break;

                case Key.Escape:
                    TogglePause();
                    break;
            }
        }

        private void UpdateSnakePosition()
        {
            Rectangle snakeHead = (Rectangle)GameSpace.Children[0];
            double headLeft = Canvas.GetLeft(snakeHead);
            double headTop = Canvas.GetTop(snakeHead);
            if (headLeft < 0 || headLeft >= GameSpace.ActualWidth || headTop < 0 || headTop >= GameSpace.ActualHeight)
            {
                EndGame();
                return;
            }

            foreach (var child in GameSpace.Children)
            {
                if (child is Rectangle snakeSegment && snakeSegment != snakeHead)
                {
                    double segmentLeft = Canvas.GetLeft(snakeSegment);
                    double segmentTop = Canvas.GetTop(snakeSegment);

                    if (headLeft == segmentLeft && headTop == segmentTop)
                    {
                        EndGame();
                        return;
                    }
                }
            }

            Point snakeHeadPosition = new Point(Canvas.GetLeft(snakeHead), Canvas.GetTop(snakeHead));
            if (food.Eat(snakeHeadPosition))
            {
                Point snakeTailPosition = snakeSegments.Last();
                snakeSegments.Add(new Point(snakeTailPosition.X, snakeTailPosition.Y));
            }


            foreach (var child in GameSpace.Children)
            {
                if (child is Rectangle snake)
                {
                    double left = Canvas.GetLeft(snake) + dx;
                    double top = Canvas.GetTop(snake) + dy;
                    Canvas.SetLeft(snake, left);
                    Canvas.SetTop(snake, top);
                }
            }
        }

        private void TogglePause()
        {
            if (isPaused)
            {
                gameTimer.Start();
                isPaused = false;
            }
            else
            {
                gameTimer.Stop();
                isPaused = true;
            }
        }

        private void StartGameLoop()
        {
            gameTimer = new DispatcherTimer();
            {
                gameTimer.Interval = TimeSpan.FromMilliseconds(100);
            };
            gameTimer.Tick += GameLoop;
            gameTimer.Start();
        }

        private void GameLoop(object sender, EventArgs e)
        {
            if (!isPaused)
            {
                UpdateSnakePosition();
            }

        }

        private void EndGame()
        {
            gameTimer.Stop();
            MessageBox.Show("Game Over!");
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            TogglePause();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Biztosan ki akar lépni?", "Megerősítés", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.Yes) Application.Current.Shutdown();
        }
    }
}
