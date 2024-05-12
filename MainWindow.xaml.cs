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
        private const int SnakeSpeed = 10;  // A kígyó sebességének konstans értéke
        // A kígyó mozgatásának iránya vízszintesen és függőlegesen
        readonly int dx = SnakeSpeed;
        readonly int dy = 0;
        private DispatcherTimer gameTimer;  // A játék időzítője
        private Food food;  // Az étel
        public Snake snake;  // A kígyó
        private ScoreDisplay scoreDisplay;  // Pontszám kijelző
        private bool isPaused = false;  // Játék szüneteltetésének állapota
        private List<Point> foodPoints = new List<Point>();


        public MainWindow()  // A főablak konstruktora
        {
            InitializeComponent();
            SetupGame();
            StartGameLoop();
            scoreDisplay = new ScoreDisplay(this);
        }

        private void SetupGame() // A játék beállításához szolgáló metódus
        {
            // Kígyó létrehozása
            snake = new Snake(GameSpace);
            snake.SetDirection(SnakeSpeed, 0);
            this.KeyDown += MainWindow_KeyDown;

            // Étel létrehozása kis késleltetéssel
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

        // Billentyűleütések kezelése
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                // Kígyó irányának beállítása
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

                // Játék szüneteltetése
                case Key.Escape:
                    TogglePause();
                    break;
            }
        }

        private void UpdateSnakePosition()  // Kígyó pozíciójának frissítése
        {
            // Ütközés vizsgálat
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
                Console.WriteLine($"Type: {segment.GetType()}, Value: ({segment.X}, {segment.Y})");  // Debug üzenet
            }
        
        }

        // Metódus, mely ellenőrzi az étel elfogyasztását és új ételt generál
        private void CheckFoodConsumptionAndGenerateFood(Point snakeHeadPosition)
        {
            if (food.Eat(snakeHeadPosition))
            {
                snake.Grow(); 
                scoreDisplay.UpdateScoreDisplay(); // Pontszám frissítése
            }
        }

        // Játék szüneteltetésére szolgáló metódus
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

        // Játék fő ciklusának indítása
        private void StartGameLoop()
        {
            gameTimer = new DispatcherTimer();
            {
                gameTimer.Interval = TimeSpan.FromMilliseconds(100);
            };
            gameTimer.Tick += GameLoop;
            gameTimer.Start();
        }

        // Játék fő ciklusa
        private void GameLoop(object sender, EventArgs e)
        {
            if (!isPaused)
            {
                UpdateSnakePosition();
            }

        }

        // Játék vége
        private void EndGame()
        {
            gameTimer.Stop();
            MessageBoxResult result = MessageBox.Show("Vége a játéknak! Akarsz újra játszani?", "Vége a játéknak", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                RestartGame();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        // Új játékmenet indítása
        private void RestartGame()
        {
            // Játéktér törlése
            GameSpace.Children.Clear();
            // A kígyó az étel és a pontszámláló visszaállítása
            snake.Reset();
            food.Reset();
            scoreDisplay.UpdateScoreDisplay();
            // Játék fő ciklusának újraindítása
            gameTimer.Start();
        }

        private void Pause_Click(object sender, RoutedEventArgs e) // Szünet gombhoz tartozó eseménykezelő
        {
            TogglePause();
        }

        private void Exit_Click(object sender, RoutedEventArgs e) // Kilépés gombhoz tartozó eseménykezelő
        {
            MessageBoxResult result = MessageBox.Show("Biztosan ki akar lépni?", "Kilépés", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.Yes) Application.Current.Shutdown();
        }
    }
}
