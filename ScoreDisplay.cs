using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SnakeWPF
{
    public class ScoreDisplay
    {
        private MainWindow mainWindow;
        private Snake snake;

        public ScoreDisplay(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            UpdateScoreDisplay();
        }

        public void UpdateScoreDisplay()
        {
            mainWindow.Score.Text = "Pontszám: " + mainWindow.snake.CountSegments().ToString();
        }

    }
}
