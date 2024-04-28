using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SnakeWPF
{
    public class Snake
    {
        private const int InitialLength = 1;
        private const int SegmentSize = 20;
        private readonly List<Point> segments = new List<Point>();
        private readonly Canvas gameSpace;
        private int dx;
        private int dy;

        public Snake(Canvas gameSpace)
        {
            this.gameSpace = gameSpace;
            CreateSnake();
        }

        private void CreateSnake()
        {
            for (int i = 0; i < InitialLength; i++)
            {
                AddSegment(0, 0);
            }
        }

        public List<Point> GetSegments()
        {
            return segments;
        }

        private void AddSegment(double x, double y)
        {
            Rectangle segment = new Rectangle
            {
                Width = SegmentSize,
                Height = SegmentSize,
                Fill = Brushes.Green,
            };
            gameSpace.Children.Add(segment);
            Canvas.SetLeft(segment, x);
            Canvas.SetTop(segment, y);
            segments.Add(new Point(x, y));
        }

        public void SetDirection(int dx, int dy)
        {
            this.dx = dx;
            this.dy = dy;
        }

        public Point Move()
        {
            double headX = segments[0].X + dx;
            double headY = segments[0].Y + dy;
            for (int i = segments.Count - 1; i > 0; i--)
            {
                segments[i] = segments[i - 1];
            }
            segments[0] = new Point(headX, headY);
            UpdateSegmentsPosition();
            return segments[0];
        }

        public void Grow()
        {
            Point tail = segments[segments.Count - 1];
            AddSegment(tail.X, tail.Y);
        }

        private void UpdateSegmentsPosition()
        {
            for (int i = 0; i < segments.Count; i++)
            {
                Canvas.SetLeft(gameSpace.Children[i], segments[i].X);
                Canvas.SetTop(gameSpace.Children[i], segments[i].Y);
            }
        }

        public bool CheckCollision(double maxWidth, double maxHeight)
        {
            double headX = segments[0].X;
            double headY = segments[0].Y;
            return headX < 0 || headX >= maxWidth || headY < 0 || headY >= maxHeight || CheckSelfCollision();
        }

        private bool CheckSelfCollision()
        {
            for (int i = 1; i < segments.Count; i++)
            {
                if (segments[0] == segments[i])
                {
                    return true;
                }
            }
            return false;
        }
    }
}