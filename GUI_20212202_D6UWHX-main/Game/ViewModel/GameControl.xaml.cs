using GUI_20212202_D6UWHX.Model;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace GUI_20212202_D6UWHX.ViewModel
{
    public partial class GameControl : UserControl
    {
        private readonly object locker = new object();
        private readonly string hiScoreDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).TrimEnd('\\') + @"\.sig\";
        private readonly string hiScoreFileName = ".his";
        private DispatcherTimer timerGame;
        private Key keyPressed;
        private Game game;
        private int hiScore;
        public GameControl()
        {
            InitializeComponent();
            game = new Game(hiScore);

            timerGame = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(20)
            };
            timerGame.Tick += OnTimerGameTick;
        }

        private void OnLoaded(object sender, RoutedEventArgs pe)
        {
            hiScore = 0;

            try
            {
                if (!Directory.Exists(hiScoreDirectory))
                    Directory.CreateDirectory(hiScoreDirectory);

                if (File.Exists(hiScoreDirectory + hiScoreFileName))
                {
                    hiScore = (int) BitConverter.ToUInt32(File.ReadAllBytes(hiScoreDirectory + hiScoreFileName), 0);
                }
            }
            catch { /* Nothing */ }

            game.Reset();
            timerGame.Start();

            var window = Window.GetWindow(this);
            if (window != null)
            {
                window.KeyDown += OnKeyDown;
                window.KeyUp += OnKeyUp;
            }
        }
        private void OnUnloaded(object sender, RoutedEventArgs pe)
        {
            var window = Window.GetWindow(this);
            if (window != null)
            {
                window.KeyDown -= OnKeyDown;
                window.KeyUp -= OnKeyUp;
            }

            timerGame.Stop();
            game.Exit();
            try
            {
                File.WriteAllBytes(hiScoreDirectory + hiScoreFileName, BitConverter.GetBytes(game.HiScore));
            }
            catch { /*/* Nothing */ }
        }
        private void OnTimerGameTick(object sender, EventArgs pe)
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (game != null)
                game.Draw(dc);
        }
        private void OnKeyDown(object sender, KeyEventArgs pe)
        {
            if ((pe.Key == Key.Space) && (keyPressed == Key.Space))
                return;

            if (game != null)
            {
                game.Level.PressKey(pe.Key);
                keyPressed = pe.Key;
            }
        }
        private void OnKeyUp(object sender, KeyEventArgs pe)
        {
            if (game != null)
            {
                game.Level.ReleaseKey();
                keyPressed = Key.None;
            }
        }
    }
}
