using GUI_20212202_D6UWHX.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace GUI_20212202_D6UWHX.Model
{
    internal enum GameStatus
    {
        Starting,
        Started,
        Stopped,
        LevelCompleted,
        Exiting,
        Exited
    }

    internal class Level
    {
        private static readonly object locker = new object();

        private double invaderHorizontalStep;
        private double invaderVerticalStep;
        private double saucerHorizontalStep;
        private double playerHorizontalStep;
        private double missileVerticalStep;
        private double bombVerticalStep;

        private Task taskLoop;
        private List<Sprite> sprites;
        private Key keyPressed;
        private Dictionary<FormattedText, Point> middleTexts;
        private FormattedText displayedMiddleText;
        private Random randomNumber;

        public int Number { get; private set; }
        public int Score { get; private set; }
        public int Lives { get; private set; }
        private Size GameSize { get; set; }
        private Size PixelSize { get; set; }
        private Player Player { get; set; }
        private Saucer Saucer { get; set; }
        private Missile Missile { get; set; }
        private Explosion EnemyExplosion { get; set; }
        private Explosion PlayerExplosion { get; set; }
        public GameStatus Status { get; set; }

        #region Constructor
        public Level(Size gameSize, Size pixelSize)
        {
            invaderHorizontalStep = 0;
            invaderVerticalStep = 0;
            saucerHorizontalStep = 0;
            playerHorizontalStep = 0;
            missileVerticalStep = 0;
            bombVerticalStep = 0;

            taskLoop = null;
            middleTexts = new Dictionary<FormattedText, Point>()
            {
                { Game.TextLevelStart, Game.PosTextLevelStart },
                { Game.TextGameOver, Game.PosTextGameOver },
                { Game.TextLevelCompleted, Game.PosTextLevelCompleted },
                { Game.TextRestart, Game.PosTextRestart },
                { Game.TextStartNewLevel, Game.PosTextStartNewLevel },
            };
            displayedMiddleText = null;
            keyPressed = Key.None;
            randomNumber = new Random();
            sprites = new List<Sprite>();

            this.Status = GameStatus.Starting;
            this.Number = 0;
            this.Score = 0;
            this.Lives = 0;
            this.GameSize = gameSize;
            this.PixelSize = pixelSize;

            this.Reset();
        }

        #endregion

        #region Drawing objects
        public void Draw(DrawingContext dc)
        {
            lock(locker)
            {
                foreach (Sprite sprite in sprites)
                {
                    if ((sprite is Player) && (this.Status == GameStatus.Stopped))
                        sprite.Draw(dc, true);
                    else
                        sprite.Draw(dc);
                }

                if (this.Missile != null)
                {
                    this.Missile.Draw(dc);
                }

                if (this.EnemyExplosion != null)
                {
                    this.EnemyExplosion.Draw(dc);
                }

                if (this.PlayerExplosion != null)
                {
                    this.PlayerExplosion.Draw(dc);
                }

                if (displayedMiddleText != null)
                {
                    if (middleTexts.ContainsKey(displayedMiddleText))
                        dc.DrawText(displayedMiddleText, middleTexts[displayedMiddleText]);
                }
            }
        }

        #endregion

        #region Key Management

        public void PressKey(Key inputKey)
        {
            keyPressed = inputKey;
        }

        public void ReleaseKey()
        {
            keyPressed = Key.None;
        }

        private void HandleInputKey()
        {
            if (this.Status == GameStatus.Started)
            {
                if (keyPressed == Key.Left)
                {
                    if (this.Player != null)
                    {
                        Point position = this.Player.GetPosition();

                        if (position.X > 20)
                            this.Player.SetPosition(position.X - playerHorizontalStep, position.Y);
                    }
                }
                else if (keyPressed == Key.Right)
                {
                    if (this.Player != null)
                    {
                        Point position = this.Player.GetPosition();

                        if (position.X < 610)
                            this.Player.SetPosition(position.X + playerHorizontalStep, position.Y);
                    }
                }
                else if (keyPressed == Key.Space)
                {
                    if ((this.Missile == null) && (this.Player != null))
                    {
                        this.Missile = CreateMissile(this.Player.GetPosition());
                    }
                }
            }
        }

        #endregion

        #region Creating/Destroying

        private Shield CreateShield(Point position)
        {
            Shield sprite = new Shield(Game.PShield, Game.BShield, this.PixelSize, position);
            sprites.Add(sprite);

            return sprite;
        }
        private Invader CreateInvader(Point position, int type, int[] sequence)
        {
            Invader sprite;

            switch (type)
            {
                default:
                case 1: sprite = new Invader(Game.PInvader1, Game.BInvader1, this.PixelSize, position, sequence, 30); break;
                case 2: sprite = new Invader(Game.PInvader2, Game.BInvader2, this.PixelSize, position, sequence, 20); break;
                case 3: sprite = new Invader(Game.PInvader3, Game.BInvader3, this.PixelSize, position, sequence, 10); break;
            }

            sprites.Add(sprite);

            return sprite;
        }

        private Saucer CreateSaucer(Point position)
        {
            Saucer sprite = new Saucer(Game.PSaucer, Game.BSaucer, this.PixelSize, position, new int[] { 0, 1 }, (int)randomNumber.Next(5, 15) * 10);
            this.Saucer = sprite;
            sprites.Add(sprite);

            return sprite;
        }

        private Player CreatePlayer(Point position)
        {
            Player sprite = new Player(Game.PPlayer, Game.BPlayer, this.PixelSize, position);
            this.Player = sprite;
            sprites.Add(sprite);

            return sprite;
        }

        private Missile CreateMissile(Point position)
        {
            return new Missile(Game.PMissile, Game.BMissile, this.PixelSize, position);
        }

        private Bomb CreateBomb(Point position, int[] sequence)
        {
            Bomb sprite = new Bomb(Game.PBomb, Game.BBomb, this.PixelSize, position, sequence);
            sprites.Add(sprite);

            return sprite;
        }

        private void DestroyInvader(Invader invader)
        {
            sprites.Remove(invader);
        }

        private void DestroySaucer()
        {
            sprites.Remove(this.Saucer);
            this.Saucer = null;
        }

        private void DestroyEnemy(IEnemy enemy)
        {
            this.Score += enemy.Points;

            if (enemy is Invader)
            {
                Invader invader = enemy as Invader;

                DestroyInvader(invader);
                this.EnemyExplosion = new Explosion(Game.PExplosion, invader.Foreground, this.PixelSize, invader.GetPosition(), 20);
            }
            else if (enemy is Saucer)
            {
                Saucer saucer = enemy as Saucer;

                DestroySaucer();
                this.EnemyExplosion = new Explosion(saucer.Points, saucer.Foreground, saucer.GetPosition(), 80);
            }

        }
        private void DestroyPlayer()
        {
            sprites.Remove(this.Player);
            this.Player = null;
        }

        private void DestroyMissile()
        {
             this.Missile = null;
        }

        #endregion

        #region Middle Management
        private void SetDisplayedMiddleText(FormattedText formattedTextToDiplay)
        {
            displayedMiddleText = formattedTextToDiplay;
        }

        private void ResetDisplayedMiddleText()
        {
            displayedMiddleText = null;
        }

        private bool IsDisplayedMiddleText()
        {
            return displayedMiddleText != null;
        }

        #endregion

        #region Level actions

        public void Start(int lives)
        {
            taskLoop = Task.Factory.StartNew((Action)(() =>
            {
                int counter = 300;
                this.Status = GameStatus.Stopped;
                if (this.Player == null)
                {
                    CreatePlayer(new Point(320, 430));
                }
                this.Player.BecomeInvincible();

                while ((this.Status != GameStatus.Exiting) && (keyPressed != Key.Escape))
                {
                    GameStatus savedStatus = this.Status;

                    #region Game restart loop

                    while ((this.Status != GameStatus.Exiting) && (keyPressed != Key.Enter) && (keyPressed != Key.Escape))
                    {
                        lock (locker)
                        {
                            // Text
                            if (counter >= 300)
                            {
                                if (this.Status == GameStatus.LevelCompleted)
                                    SetDisplayedMiddleText(Game.TextStartNewLevel);
                                else
                                    SetDisplayedMiddleText(Game.TextRestart);
                            }

                            // Saucer
                            if (counter % 10 == 0)
                            {
                                MoveSaucer();
                                AnimateSaucer();
                            }

                            // Invaders
                            if (counter % 20 == 0)
                            {
                                AnimateInvaders();
                            }

                            // Bombs
                            if (counter % 3 == 0)
                            {
                                AnimateBombs();
                                MoveBombs();
                            }

                            // Missile
                            MoveMissile();

                            // Explosion of enemy
                            if (this.EnemyExplosion != null)
                            {
                                if (this.EnemyExplosion.VisibilityCounter > 0)
                                {
                                    this.EnemyExplosion.VisibilityCounter--;
                                }
                                else
                                {
                                    this.EnemyExplosion = null;
                                }
                            }

                            // Explosion of player
                            if (this.PlayerExplosion != null)
                            {
                                if (this.PlayerExplosion.VisibilityCounter > 0)
                                {
                                    this.PlayerExplosion.VisibilityCounter--;
                                }
                                else
                                {
                                    this.PlayerExplosion = null;
                                }
                            }

                            // Keyboard
                            HandleInputKey();
                            Thread.Sleep(10);
                            counter++;
                        }
                    }

                    #endregion

                    #region Game level loop

                    if (savedStatus == GameStatus.LevelCompleted)
                    {
                        this.Number++;
                    }
                    else
                    {
                        this.Number = 1;
                        this.Score = 0;
                    }

                    counter = 0;

                    this.Reset();
                    this.Lives = lives;
                    this.Status = GameStatus.Started;
                    this.Player.BecomeInvincible();
                    SetDisplayedMiddleText(Game.TextLevelStart);

                    int speedInvader = 21 - (int)this.Number;
                    int speedSaucer = 11 - (int)this.Number / 5;
                    int speedBomb = 4 - (int)this.Number / 10;

                    while ((this.Status != GameStatus.Exiting) && (this.Status == GameStatus.Started))
                    {
                        lock(locker)
                        {
                            // Text
                            if ((IsDisplayedMiddleText()) && (counter >= 300))
                            {
                                ResetDisplayedMiddleText();
                            }

                            // Saucer
                            if (counter % speedSaucer == 0)
                            {
                                AnimateSaucer();
                                MoveSaucer();
                            }

                            // Invaders
                            if (counter % speedInvader == 0)
                            {
                                AnimateInvaders();
                                MoveInvaders();
                            }

                            // Bombs
                            if (counter % speedBomb == 0)
                            {
                                AnimateBombs();
                                MoveBombs();
                            }

                            // Missile
                            MoveMissile();

                            // Explosion of enemy
                            if (this.EnemyExplosion != null)
                            {
                                if (this.EnemyExplosion.VisibilityCounter > 0)
                                {
                                    this.EnemyExplosion.VisibilityCounter--;
                                }
                                else
                                {
                                    this.EnemyExplosion = null;
                                }
                            }

                            // Invicibility of player
                            if ((this.Player != null) && this.Player.IsInvincible)
                            {
                                this.Player.DecreaseInvincibility();
                            }

                            // Explosion of player
                            if (this.PlayerExplosion != null)
                            {
                                if (this.PlayerExplosion.VisibilityCounter > 0)
                                {
                                    this.PlayerExplosion.VisibilityCounter--;
                                }
                                else
                                {
                                    this.PlayerExplosion = null;

                                    if (this.Lives == 0)
                                    {
                                        this.Status = GameStatus.Stopped;
                                    }
                                    else
                                    {
                                        CreatePlayer(new Point(320, 430));
                                        this.Player.BecomeInvincible();
                                    }
                                }
                            }

                            // Keyboard
                            HandleInputKey();
                            if (keyPressed == Key.Space)
                            {
                                keyPressed = Key.None;
                            }
                            else if (keyPressed == Key.Escape)
                            {
                                keyPressed = Key.None;
                                this.Status = GameStatus.Stopped;
                            }

                            Thread.Sleep(10);
                            counter++;
                        }
                    }

                    if (this.Status == GameStatus.LevelCompleted)
                        SetDisplayedMiddleText(Game.TextLevelCompleted);
                    else if (this.Status == GameStatus.Stopped)
                        SetDisplayedMiddleText(Game.TextGameOver);

                    if (this.Player != null)
                        this.Player.BecomeNormal();
                    counter = 0;

                    #endregion
                }

                this.Status = GameStatus.Exited;
            }));

        }
        public void Exit()
        {
            if (this.Status == GameStatus.Starting) return;
            if (this.Status == GameStatus.Exited) return;

            this.Status = GameStatus.Exiting;
            while (this.Status == GameStatus.Exiting) { Thread.Sleep(100); }
        }
        private void Reset()
        {
            invaderHorizontalStep = this.PixelSize.Width;
            invaderVerticalStep = this.PixelSize.Height * this.Number;
            saucerHorizontalStep = this.PixelSize.Width * 3;
            playerHorizontalStep = this.PixelSize.Width * 1;
            missileVerticalStep = this.PixelSize.Height * 3;
            bombVerticalStep = this.PixelSize.Height;

            lock (locker)
            {
                sprites.Clear();

                double x = 120;

                for (int i = 0; i < 11; i++)
                {
                    double y = 80;

                    CreateInvader(new Point(x + i * 40, y), 1, new int[] { 0, 1 });
                    y += 30;

                    CreateInvader(new Point(x + i * 40, y), 2, new int[] { 1, 0 });
                    y += 30;

                    CreateInvader(new Point(x + i * 40, y), 2, new int[] { 0, 1 });
                    y += 30;

                    CreateInvader(new Point(x + i * 40, y), 3, new int[] { 1, 0 });
                    y += 30;

                    CreateInvader(new Point(x + i * 40, y), 3, new int[] { 0, 1 });
                    y += 30;
                }

                CreateSaucer(new Point(-30, 50));

                for (int i = 0; i < 4; i++)
                {
                    CreateShield(new Point(135 + i * 120, 390));
                }

                CreatePlayer(new Point(320, 430));

                this.Missile = null;
                this.EnemyExplosion = null;
                this.PlayerExplosion = null;
            }
        }

        #endregion

        #region Animation
        private void AnimateInvaders()
        {
            foreach (Invader invader in sprites.Where(s => s is Invader))
            {
                invader.Animate();
            }
        }
        private void AnimateBombs()
        {
            foreach (Bomb bomb in sprites.Where(b => b is Bomb))
            {
                bomb.Animate();
            }
        }
        private void AnimateSaucer()
        {
            if (this.Saucer != null)
            {
                this.Saucer.Animate();
            }
        }

        #endregion

        #region Moving objects
        private void MoveSaucer()
        {
            if (this.Saucer != null)
            {
                Point position = this.Saucer.GetPosition();
                if (position.X < 670)
                {
                    this.Saucer.SetPosition(position.X + saucerHorizontalStep, position.Y);
                }
                else
                {
                    if (randomNumber.Next(150) == 4)
                    {
                        this.Saucer.ResetPosition();
                    }
                }
            }
        }
        private void MoveMissile()
        {
            if (this.Missile != null)
            {
                Point posMissile = this.Missile.GetPosition();
                this.Missile.SetPosition(posMissile.X, posMissile.Y - missileVerticalStep);
                posMissile = this.Missile.GetPosition();

                if (posMissile.Y <= 0)
                {
                    DestroyMissile();
                }
                else
                {
                    Bomb explodedBomb = null;

                    foreach (Bomb bomb in sprites.Where(s => s is Bomb))
                    {
                        if (this.Missile.CheckExplosion(bomb))
                        {
                            explodedBomb = bomb;
                            break;
                        }
                    }

                    if (explodedBomb != null)
                    {
                        sprites.Remove(explodedBomb);
                        DestroyMissile();
                    }
                    else
                    {
                        IEnemy reachedEnemy = null;

                        foreach (IEnemy enemy in sprites.Where(s => s is IEnemy))
                        {
                            if ((enemy as Sprite).Bounds.IntersectsWith(this.Missile.Bounds))
                            {
                                DestroyEnemy(enemy);
                                DestroyMissile();
                                reachedEnemy = enemy;
                                break;
                            }
                        }

                        if (reachedEnemy == null)
                        {
                            foreach (Shield shield in sprites.Where(s => s is Shield))
                            {
                                if (shield.CheckBurning(this.Missile))
                                {
                                    DestroyMissile();
                                    break;
                                }

                            }
                        }
                    }
                }
            }
        }
        private void MoveInvaders()
        {
            if (this.Status == GameStatus.Stopped)
            {
                return;
            }

            double verticalStep = 0;

            var invaders = sprites.Where(s => s is Invader).ToList();

            if (invaders.Count == 0)
            {
                this.Status = GameStatus.LevelCompleted;
                return;
            }

            invaderHorizontalStep = Math.Sign(invaderHorizontalStep) * this.PixelSize.Width * (1d + 3d * (1d - invaders.Count / 55d));

            foreach (Invader invader in invaders)
            {
                Point position = invader.GetPosition();
                if (((position.X > 615) && (invaderHorizontalStep > 0)) || ((position.X < 15) && (invaderHorizontalStep < 0)))
                {
                    invaderHorizontalStep = -invaderHorizontalStep;
                    verticalStep = invaderVerticalStep;
                    break;
                }
            }

            int bombCount = sprites.Count(s => s is Bomb);

            foreach (Invader invader in invaders)
            {
                Point position = invader.GetPosition();
                invader.SetPosition(position.X + invaderHorizontalStep, position.Y + verticalStep);

                if (position.Y > 400)
                {
                    this.Status = GameStatus.Stopped;

                    this.Lives = 0;

                    this.PlayerExplosion = new Explosion(Game.PExplosion, this.Player.Foreground, this.PixelSize, this.Player.GetPosition(), 80);
                    DestroyPlayer();

                    return;
                }

                if ((position.Y < 350) && (bombCount < 15 + this.Number * 2) && (randomNumber.Next(150) == 5))
                {
                    CreateBomb(position, new int[] { 0, 1 });
                }
            }

            var shields = sprites.Where(s => s is Shield).ToList();
            var bombs = sprites.Where(s => s is Bomb).ToList();

            foreach (Bomb bomb in bombs)
            {
                foreach (Shield shield in shields)
                {
                    if (shield.CheckExplosion(bomb))
                    {
                        // Remove bomb from sprite list
                        sprites.Remove(bomb);
                        break;
                    }
                }

                if ((this.Missile != null) && this.Missile.CheckExplosion(bomb))
                {
                    sprites.Remove(bomb);
                    DestroyMissile();
                    break;
                }

                if ((this.Player != null) && !this.Player.IsInvincible && this.Player.CheckExplosion(bomb))
                {
                    sprites.Remove(bomb);

                    this.Lives--;

                    this.PlayerExplosion = new Explosion(Game.PExplosion, this.Player.Foreground, this.PixelSize, this.Player.GetPosition(), 80);
                    DestroyPlayer();
                }
            }
        }
        private void MoveBombs()
        {
            List<Bomb> explodedBombs = null;

            foreach (Bomb bomb in sprites.Where(s => s is Bomb))
            {
                Point position = bomb.GetPosition();
                if (position.Y > Game.Screen.Height)
                {
                    if (explodedBombs == null)
                        explodedBombs = new List<Bomb>();

                    explodedBombs.Add(bomb);
                }
                else bomb.SetPosition(position.X, position.Y + bombVerticalStep);
            }

            if (explodedBombs != null)
            {
                foreach (Bomb bomb in explodedBombs)
                {
                    sprites.Remove(bomb);
                }
            }
        }

        #endregion
    }
}
