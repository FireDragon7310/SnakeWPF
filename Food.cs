using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace SnakeWPF
{
    public class Food
    {
        private readonly Canvas gameCanvas;
        readonly Random random = new Random();
        private readonly int foodSize = 20;
        private Ellipse foodPiece;
        readonly List<Point> snakeSegments;

        public Food(Canvas canvas, List<Point> snakeSegments)
        {
            gameCanvas = canvas;
            this.snakeSegments = snakeSegments;
            GenerateFood();
        }

        private void GenerateFood()
        {
            double left = random.Next(0, (int)(gameCanvas.ActualWidth / foodSize)) * foodSize;
            double top = random.Next(0, (int)(gameCanvas.ActualHeight / foodSize)) * foodSize;


            foodPiece = new Ellipse
            {
                Width = foodSize,
                Height = foodSize,
                Fill = Brushes.Red
            };

            gameCanvas.Children.Add(foodPiece);
            Canvas.SetLeft(foodPiece, left);
            Canvas.SetTop(foodPiece, top);
        }

        public bool Eat(Point snakeHeadPosition)
        {
            double foodLeft = Canvas.GetLeft(foodPiece);
            double foodTop = Canvas.GetTop(foodPiece);

            if (snakeHeadPosition.X == foodLeft && snakeHeadPosition.Y == foodTop)
            {
                gameCanvas.Children.Remove(foodPiece);
                GenerateFood();

                Point newSegmentPositon = new Point(snakeHeadPosition.X, snakeHeadPosition.Y);
                snakeSegments.Add(newSegmentPositon);
                return true;
            }
            return false;
        }

    }
}
