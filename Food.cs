using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Xml.Linq;
using System.Windows.Threading;

namespace SnakeWPF
{
    public class Food
    {
        private bool isFood = true;  // Jelzi, hogy az étel még jelen van-e a játéktéren
        private readonly Canvas gameCanvas;  // Játéktér eleme
        readonly Random random = new Random();  // Random szám generátor
        private readonly int foodSize = 20;  // Az étel mérete
        private readonly Ellipse foodPiece;  // Az étel grafikus reprezentációja
        readonly List<Point> snakeSegments;  // A kígyó szegmenseinek listája
        private Point foodPosition;  // Az étel pozíciója

        public Food(Canvas canvas, List<Point> snakeSegments)
        {
            gameCanvas = canvas;
            this.snakeSegments = snakeSegments;
            foodPiece = new Ellipse
            {
                Width = foodSize,
                Height = foodSize,
                Fill = Brushes.Red
            };
            GenerateFood(); // Az első étel legenerálása.
        }

        // Az étel generálására szolgáló metódus
        public void GenerateFood()
        {
            bool foodGenerated = false;
            while (!foodGenerated)
            {
                // Az étel helyének véletlenszerű megadása
                double left = random.Next(0, (int)(gameCanvas.ActualWidth / foodSize)) * foodSize;
                double top = random.Next(0, (int)(gameCanvas.ActualHeight / foodSize)) * foodSize;

                // Ellenőrizzük, hogy az új étel helye nem esik-e egybe a kígyó bármely szegmensének helyével
                bool overlapsWithSnake = false;
                foreach (var segment in snakeSegments)
                {
                    if (segment.X == left && segment.Y == top)
                    {
                        overlapsWithSnake = true;
                        break;
                    }
                }

                // Ellenőrizzük, hogy az új étel helye nem esik-e egybe az előző étel helyével
                if (foodPosition.X == left && foodPosition.Y == top)
                {
                    overlapsWithSnake = true;
                }

                if (!overlapsWithSnake)
                {

                    // Az étel hozzáadása a játéktérhez és pozíciójának beállítása
                    gameCanvas.Children.Add(foodPiece);
                    Canvas.SetLeft(foodPiece, left);
                    Canvas.SetTop(foodPiece, top);

                    foodPosition = new Point(left, top);  // Az étel új pozíciójának mentése

                    Console.WriteLine($"Generated food at position ({foodPosition.X}, {foodPosition.Y})");  // Debug üzenet

                    foodGenerated = true;
                }
            }
        }

        // Eat metódus az étel elfogyasztásának logikájával
        public bool Eat(Point snakeHeadPosition)
        {
            double foodLeft = Canvas.GetLeft(foodPiece);
            double foodTop = Canvas.GetTop(foodPiece);

            if (snakeHeadPosition.X == foodLeft && snakeHeadPosition.Y == foodTop && isFood)
            {
                gameCanvas.Children.Remove(foodPiece);
                // Új étel generálása kis késleltetéssel
                DispatcherTimer delayTimer = new DispatcherTimer();
                delayTimer.Interval = TimeSpan.FromMilliseconds(50);
                delayTimer.Tick += (sender, e) =>
                {
                    delayTimer.Stop();
                    GenerateFood();
                };
                delayTimer.Start();


                Console.WriteLine($"Food eaten at: ({foodLeft}, {foodTop})"); // Debug üzenet
                return true;
            }
            return false;
        }

        // Az étel pozíciójának lekérése
        public Point GetFoodPosition()
        {
            return foodPosition;
        }

        // Étel újragenerálása
        public void Reset()
        {
            gameCanvas.Children.Remove(foodPiece);
            GenerateFood();
        }

    }

}
