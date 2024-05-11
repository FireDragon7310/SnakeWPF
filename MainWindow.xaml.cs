using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Windows.Shapes;

namespace SnakeWPF
{
    public partial class MainWindow : Window
    {
        private const int SnakeSpeed = 10;
        readonly int dx = SnakeSpeed;
        readonly int dy = 0;
        private DispatcherTimer gameTimer;
        private Food food;
        public Snake snake;
        private ScoreDisplay scoreDisplay;
        private bool isPaused = false;
        private List<Point> foodPoints = new List<Point>();


        public MainWindow()
        {
            InitializeComponent();
            SetupGame();
            StartGameLoop();
            scoreDisplay = new ScoreDisplay(this);
        }

        private void SetupGame()
        {
            snake = new Snake(GameSpace);
            snake.SetDirection(SnakeSpeed, 0);
            this.KeyDown += MainWindow_KeyDown;

            // Meghívjuk a GenerateFood() metódust egy kis késleltetéssel
            DispatcherTimer delayTimer = new DispatcherTimer();
            delayTimer.Interval = TimeSpan.FromMilliseconds(100);
            delayTimer.Tick += (sender, e) =>
            {
                delayTimer.Stop();
                food = new Food(GameSpace, snake.GetSegments());
                foodPoints.Add(new Point(food.GetFoodPosition().X, food.GetFoodPosition().Y));
            };
            delayTimer.Start();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.A:
                    snake.SetDirection(-SnakeSpeed, 0);
                    break;

                case Key.D:
                    snake.SetDirection(SnakeSpeed, 0);
                    break;

                case Key.W:
                    snake.SetDirection(0, -SnakeSpeed);
                    break;

                case Key.S:
                    snake.SetDirection(0, SnakeSpeed);
                    break;

                case Key.Escape:
                    TogglePause();
                    break;
            }
        }

        private void UpdateSnakePosition()
        {
            if (snake.CheckCollision(GameSpace.ActualWidth, GameSpace.ActualHeight))
            {
                EndGame();
                return;
            }

            Point snakeHeadPosition = snake.Move();
            CheckFoodConsumptionAndGenerateFood(snakeHeadPosition);

            Console.WriteLine("Snake segments: ");
            foreach (Point segment in snake.GetSegments())
            {
                Console.WriteLine($"Type: {segment.GetType()}, Value: ({segment.X}, {segment.Y})");
            }
        
        }

        // Az új CheckFoodConsumptionAndGenerateFood metódus, amely ellenőrzi az étel elfogyasztását és generál egy új ételt
        private void CheckFoodConsumptionAndGenerateFood(Point snakeHeadPosition)
        {
            if (food.Eat(snakeHeadPosition))
            {
                snake.Grow();
                Console.WriteLine("The snake grew");
                scoreDisplay.UpdateScoreDisplay();
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
            MessageBoxResult result = MessageBox.Show("Game Over! Do you want to play again?", "Game Over", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                RestartGame();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        private void RestartGame()
        {
            // Clear the gamespace
            GameSpace.Children.Clear();
            // Reset snake and food
            snake.Reset();
            food.Reset();
            // Start the game loop again
            gameTimer.Start();
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
