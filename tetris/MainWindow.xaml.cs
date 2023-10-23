using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace tetris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ImageSource[] tileImages = new ImageSource[]
        {
            new BitmapImage(new Uri("assets/TileEmpty.png", UriKind.Relative)),
            new BitmapImage(new Uri("assets/TileCyan.png", UriKind.Relative)),
            new BitmapImage(new Uri("assets/TileBlue.png", UriKind.Relative)),
            new BitmapImage(new Uri("assets/TileOrange.png", UriKind.Relative)),
            new BitmapImage(new Uri("assets/TileYellow.png", UriKind.Relative)),
            new BitmapImage(new Uri("assets/TileGreen.png", UriKind.Relative)),
            new BitmapImage(new Uri("assets/TilePurple.png", UriKind.Relative)),
            new BitmapImage(new Uri("assets/TileRed.png", UriKind.Relative))
        };

        private readonly ImageSource[] blockImages = new ImageSource[]
        {
            new BitmapImage(new Uri("assets/Block-Empty.png", UriKind.Relative)),
            new BitmapImage(new Uri("assets/Block-I.png", UriKind.Relative)),
            new BitmapImage(new Uri("assets/Block-J.png", UriKind.Relative)),
            new BitmapImage(new Uri("assets/Block-L.png", UriKind.Relative)),
            new BitmapImage(new Uri("assets/Block-O.png", UriKind.Relative)),
            new BitmapImage(new Uri("assets/Block-S.png", UriKind.Relative)),
            new BitmapImage(new Uri("assets/Block-T.png", UriKind.Relative)),
            new BitmapImage(new Uri("assets/Block-Z.png", UriKind.Relative)),
        };

        private readonly Image[,] imageControls;
        private readonly int maxDelay = 1000;
        private readonly int minDelay = 75;
        private readonly double delayDecrease = 0.25;
        private GameState gameState= new GameState();
        private int Y_axis;
        private static bool flag = true;
        private string nickName;
        private Leaderboard db;

        public MainWindow()
        {
            db = new Leaderboard();
            InitializeComponent();
            imageControls = SetupGameCanvas(gameState.GameGrid);
        }

        private Image[,] SetupGameCanvas(GameGrid grid)
        {
            Image[,] imageControls = new Image[grid.Rows, grid.Columns];
            int cellSize = 25;
            for(int r = 0; r < grid.Rows; r++)
            {
                for(int c = 0; c < grid.Columns; c++)
                {
                    Image imageControl = new Image
                    {
                        Width = cellSize,
                        Height = cellSize,
                    };

                    Canvas.SetTop(imageControl, (r-2) * cellSize + 10);
                    Canvas.SetLeft(imageControl, c * cellSize);
                    GameCanvas.Children.Add(imageControl);
                    imageControls[r, c] = imageControl;
                }
            }
            return imageControls;
        }

        private void DrawNextBlock(BlockQueue blockQueue)
        {
            Block Next = blockQueue.NextBlock;
            NextImage.Source = blockImages[Next.Id];
        }

        private void DrawGrid(GameGrid grid)
        {
            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
                {
                    int id = grid[r, c];
                    imageControls[r, c].Opacity = 1;
                    imageControls[r, c].Source = tileImages[id];
                }
            }
        }

        private void DrawBlock(Block block)
        {
            foreach (Position p in block.TilePositions())
            {
                imageControls[p.Row, p.Column].Opacity = 1;
                imageControls[p.Row, p.Column].Source = tileImages[block.Id];
            }
        }

        private void DrawGhostBlock(Block block)
        {
            int dropDistance = gameState.BlockDropDistance();
            foreach(Position p in block.TilePositions())
            {
                imageControls[p.Row + dropDistance, p.Column].Opacity = 0.25;
                imageControls[p.Row + dropDistance, p.Column].Source = tileImages[block.Id];
            }
        }

        private void Draw(GameState gameState)
        {
            DrawGrid(gameState.GameGrid);
            DrawGhostBlock(gameState.CurrentBlock);
            DrawBlock(gameState.CurrentBlock);
            DrawNextBlock(gameState.BlockQueue);
            ScoreText.Text = $"Score: {gameState.Score}";
            CurrentLevel.Text = $"Level: {gameState.Level}";
        }
        private async Task GameLoop()
        {
            flag = true;
            Y_axis = 1;
            Draw(gameState);
            while(!gameState.GameOver && flag == true)
            {
                int delay = (int)Math.Max(minDelay, maxDelay - (gameState.Score * delayDecrease));  
                await Task.Delay(delay);
                gameState.MoveBlockDown(Y_axis);
                Draw(gameState);
            }
            if (gameState.GameOver) 
            {
                db.AddToDb(gameState.Score, nickName);
                GameOverMenu.Visibility = Visibility.Visible;
                FinalScoreText.Text = $"Score: {gameState.Score}";
            }
            
        }

        private async void Start_Click(object sender, RoutedEventArgs e) 
        {
            gameState = new GameState();
            MainMenu.Visibility = Visibility.Hidden;
            await GameLoop();
        }

        private void LeaderBoard_Click(object sender, RoutedEventArgs e) { }

        private void NickReg_Click(object sender, RoutedEventArgs e)
        {
            nickName = nickBox.Text.Trim();

            if (nickName.Length < 3 )
            {
                nickBox.ToolTip = "Your nickname must be at least 3 characters long";
                nickBox.Background = Brushes.LightPink;
            }
            else
            {
                RegField.Visibility = Visibility.Hidden;
            }
            
        }
        private void Window_KeyDown(object sender, KeyEventArgs e) 
        {
            if(gameState.GameOver)
            {
                return;
            }
            if (Y_axis == 1)
            {
                switch (e.Key)
                {
                    case Key.Left:
                        gameState.MoveBlockLeft();
                        break;
                    case Key.Right:
                        gameState.MoveBlockRight();
                        break;
                    case Key.Down:
                        gameState.MoveBlockDown(Y_axis);
                        break;
                    case Key.A:
                        gameState.RotateBlockCCW();
                        break;
                    case Key.D:
                        gameState.RotateBlockCW();
                        break;
                    case Key.Space:
                        gameState.DropBlock();
                        break;
                    case Key.P:
                        PauseMenu.Visibility = Visibility.Visible;
                        Y_axis = 0;
                        break;
                    default:
                        return;
                }
            }
            Draw(gameState);    
        }
        
        private void MainMenu_Click(object sender, RoutedEventArgs e) 
        {
            GameOverMenu.Visibility = Visibility.Hidden;   
            PauseMenu.Visibility = Visibility.Hidden;
            MainMenu.Visibility = Visibility.Visible;
            flag = false; 
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            Y_axis = 1;
            PauseMenu.Visibility = Visibility.Hidden;
        }

        private async void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            gameState = new GameState();
            GameOverMenu.Visibility = Visibility.Hidden;
            await GameLoop();
        }
    }
}
