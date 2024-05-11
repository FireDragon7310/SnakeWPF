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
        private bool isFood = true;
        private readonly Canvas gameCanvas;
        readonly Random random = new Random();
        private readonly int foodSize = 20;
        private readonly Ellipse foodPiece;
        readonly List<Point> snakeSegments;
        private Point foodPosition;

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

        public void GenerateFood()
        {
            bool foodGenerated = false;
            while (!foodGenerated)
            {
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
                    

                    gameCanvas.Children.Add(foodPiece);
                    Canvas.SetLeft(foodPiece, left);
                    Canvas.SetTop(foodPiece, top);

                    foodPosition = new Point(left, top);

                    Console.WriteLine($"Generated food at position ({foodPosition.X}, {foodPosition.Y})");

                    foodGenerated = true;
                }
            }
        }


        public bool Eat(Point snakeHeadPosition)
        {
            double foodLeft = Canvas.GetLeft(foodPiece);
            double foodTop = Canvas.GetTop(foodPiece);

            if (snakeHeadPosition.X == foodLeft && snakeHeadPosition.Y == foodTop && isFood)
            {
                gameCanvas.Children.Remove(foodPiece);
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

        public Point GetFoodPosition()
        {
            return foodPosition;
        }

        public void Reset()
        {
            gameCanvas.Children.Remove(foodPiece);
            GenerateFood();
        }

    }

}
