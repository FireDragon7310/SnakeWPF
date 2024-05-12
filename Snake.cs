using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace SnakeWPF
{
    public class Snake
    {
        private const int InitialLength = 1;  // Alap kígyóhossz beállítása
        private const int SegmentSize = 20;  // Az egyes kígyó szegmensek mérete
        public  List<Point> segments = new List<Point>();  // A kígyó szegmenseit tartalmazó lista
        private readonly Canvas gameSpace;  // Játéktér elem
        private int dx;  // A kígyó vízszintes mozgási iránya
        private int dy;  // A kígyó függőleges mozgási iránya

        public Snake(Canvas gameSpace)  // Kígyó konstruktor
        {
            this.gameSpace = gameSpace;
            CreateSnake();
        }

        private void CreateSnake()  // Kígyó létrehozása
        {
            for (int i = 0; i < InitialLength; i++)
            {
                AddSegment(0, 0);
            }
        }

        public Point GetHeadPosition()  // A kígyó fejének pozíciójának lekérése
        {
            return segments[0];
        }

        public List<Point> GetSegments()  // A kígyó szegmenseinek lekérése
        {
            return segments;
        }

        private void AddSegment(double x, double y)  // Új szegmens hozzáadása a kígyóhoz
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

        public void SetDirection(int dx, int dy)  // Kígyó mozgásának irányának beállítása
        {
            this.dx = dx;
            this.dy = dy;
        }

        public Point Move()  // Kígyó mozgatása
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

        public void Grow()  // Kígyó növekedése
        {
            Point tail = segments[segments.Count - 1];
            double newX = tail.X - dx; // Az új szegmens X koordinátája az utolsó szegmens X koordinátájából és a kígyó mozgásának különbségéből
            double newY = tail.Y - dy; // Az új szegmens Y koordinátája az utolsó szegmens Y koordinátájából és a kígyó mozgásának különbségéből
            AddSegment(newX, newY);


            Console.WriteLine("Tail: "+ tail);
            Console.WriteLine("Segments: "+ segments.Count);
        }

        public int CountSegments() // Kígyószegmensek számának lekérése (pontszám)
        {
            return segments.Count -1;
        }

        private void UpdateSegmentsPosition()  // Kígyószegmensek pozíciójának frissítése a képernyőn
        {
            for (int i = 0; i < segments.Count; i++)
            {
                Canvas.SetLeft(gameSpace.Children[i], segments[i].X);
                Canvas.SetTop(gameSpace.Children[i], segments[i].Y);
            }
        }

        // Ütközés ellenőrzése a játéktér határain és a kígyó önmagával
        public bool CheckCollision(double maxWidth, double maxHeight)
        {
            double headX = segments[0].X;
            double headY = segments[0].Y;
            return headX < 0 || headX >= maxWidth || headY < 0 || headY >= maxHeight||  CheckSelfCollision();
        }

        private bool CheckSelfCollision()  // Ütközés ellenőrzése a kígyó önmagával
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

        public void Reset()  // Kígyó újraindítása
        {
            // Kígyószegmensek törlése
            segments.Clear();

            // Kígyó kezdeti hosszának és pozíciójának visszaállítása
            for (int i = 0; i < InitialLength; i++)
            {
                AddSegment(0, 0);
            }

            // Kezdeti irány visszaállítása
            dx = 10;
            dy = 0;

            
        }
    }
}