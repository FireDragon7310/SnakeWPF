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

namespace SnakeWPF
{
    public class Food
    {
        private readonly Canvas gameCanvas;
        readonly Random random = new Random();
        private readonly int foodSize = 20;
        private Ellipse foodPiece;
        readonly List<Point> snakeSegments;
        private Point foodPosition;

        public Food(Canvas canvas, List<Point> snakeSegments)
        {
            gameCanvas = canvas;
            this.snakeSegments = snakeSegments;
            GenerateFood();
        }

        public void GenerateFood()
        {
            bool foodGenerated = false;
            while (!foodGenerated)
            {
                double left = random.Next(0, (int)(gameCanvas.ActualWidth / foodSize)) * foodSize;
                double top = random.Next(0, (int)(gameCanvas.ActualHeight / foodSize)) * foodSize;

                bool overlapsWithSnake = false;
                foreach (Point segment in snakeSegments)
                {
                    if (segment.X == left && segment.Y == top)
                    {
                        overlapsWithSnake = true;
                        break;
                    }
                }

                if (!overlapsWithSnake)
                {
                    foodPiece = new Ellipse
                    {
                        Width = foodSize,
                        Height = foodSize,
                        Fill = Brushes.Red
                    };

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

            if (snakeHeadPosition.X == foodLeft && snakeHeadPosition.Y == foodTop)
            {
                gameCanvas.Children.Remove(foodPiece);
                GenerateFood();

                // Az új szegment helyének meghatározása
                Point lastSegmentPosition = snakeSegments.Last();
                double deltaX = snakeHeadPosition.X - lastSegmentPosition.X;
                double deltaY = snakeHeadPosition.Y - lastSegmentPosition.Y;

                Point newSegmentPosition = new Point(snakeHeadPosition.X + deltaX, snakeHeadPosition.Y + deltaY);

                // Hozzáadjuk az új szegmentet a kígyóhoz
                snakeSegments.Add(newSegmentPosition);
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
