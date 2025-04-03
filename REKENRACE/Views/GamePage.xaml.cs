using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Navigation;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace REKENRACE.Views
{
    public partial class GamePage : Page
    {
        private string playerName;
        private string difficulty;
        private int correctAnswer;
        private int score = 0;
        private int highScore;
        private int questionCount = 0;
        private const int totalQuestions = 10;
        private Random random = new Random();
        private string highscoreFile = "highscore.txt";

        private MediaPlayer player = new MediaPlayer(); // ✅ MediaPlayer voor geluid

        public GamePage(string playerName, string difficulty)
        {
            InitializeComponent();
            this.playerName = playerName;
            this.difficulty = difficulty;

            PlayerNameTextBlock.Text = $"Speler: {playerName}";
            DifficultyTextBlock.Text = $"Moeilijkheid: {difficulty}";

            highScore = FindHighScore(playerName);
            HighScoreTextBlock.Text = $"Hoogste score ooit: {highScore}";

            RestartButton.Visibility = Visibility.Collapsed;

            GenerateQuestion();
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            if (window != null)
            {
                DoubleAnimation fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
                fadeOut.Completed += (s, a) =>
                {
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Opacity = 0;
                    mainWindow.Show();

                    DoubleAnimation fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
                    mainWindow.BeginAnimation(Window.OpacityProperty, fadeIn);

                    window.Close();
                };

                window.BeginAnimation(Window.OpacityProperty, fadeOut);
            }
        }

        private void Sound_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Geluid instellingen worden later toegevoegd!");
        }

        private void GenerateQuestion()
        {
            if (questionCount >= totalQuestions)
            {
                SaveHighScore();
                HighScoreTextBlock.Text = $"Hoogste score ooit: {highScore}";

                RestartButton.Visibility = Visibility.Visible;
                AnswerInput.IsEnabled = false;
                CheckAnswerButton.IsEnabled = false;
                return;
            }

            questionCount++;
            QuestionCounterText.Text = $"Som {questionCount} van {totalQuestions}";

            int num1, num2;
            char operation;

            if (difficulty.ToLower() == "makkelijk")
            {
                num1 = random.Next(1, 20);
                num2 = random.Next(1, 10);
                operation = random.Next(2) == 0 ? '+' : '-';
                correctAnswer = operation == '+' ? num1 + num2 : num1 - num2;
            }
            else if (difficulty.ToLower() == "normaal")
            {
                num1 = random.Next(2, 15);
                num2 = random.Next(2, 10);
                operation = random.Next(2) == 0 ? '*' : '/';

                if (operation == '*')
                {
                    correctAnswer = num1 * num2;
                }
                else
                {
                    correctAnswer = num1;
                    num1 = num1 * num2;
                }
            }
            else // Moeilijk
            {
                num1 = random.Next(10, 100);
                num2 = random.Next(2, 40);
                operation = random.Next(2) == 0 ? '*' : '/';

                if (operation == '*')
                {
                    correctAnswer = num1 * num2;
                }
                else
                {
                    correctAnswer = num1;
                    num1 = num1 * num2;
                }
            }

            QuestionText.Text = $"{num1} {operation} {num2} = ?";
            AnswerInput.Text = "";
        }

        private void CheckAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AnswerInput.Text) || !int.TryParse(AnswerInput.Text, out int userAnswer))
            {
                FeedbackText.Text = "Voer een geldig getal in!";
                FeedbackText.Foreground = System.Windows.Media.Brushes.Orange;
                return;
            }

            if (userAnswer == correctAnswer)
            {
                FeedbackText.Text = "Correct!";
                FeedbackText.Foreground = System.Windows.Media.Brushes.Green;
                score++;

                ScoreTextBlock.Text = $"Score: {score}";

                if (score > highScore)
                {
                    highScore = score;
                    HighScoreTextBlock.Text = $"Hoogste score ooit: {highScore}";
                    SaveHighScore();
                }

                SpeelGeluid(true); // ✅ Correct geluid afspelen
            }
            else
            {
                FeedbackText.Text = "Fout!";
                FeedbackText.Foreground = System.Windows.Media.Brushes.Red;

                SpeelGeluid(false); // ✅ Fout geluid afspelen
            }

            GenerateQuestion();
        }

        private void SaveHighScore()
        {
            var lines = File.Exists(highscoreFile) ? File.ReadAllLines(highscoreFile).ToList() : new List<string>();
            lines.RemoveAll(l => l.StartsWith($"{playerName}:"));
            lines.Add($"{playerName}:{highScore}");
            File.WriteAllLines(highscoreFile, lines);
        }

        private int FindHighScore(string playerName)
        {
            if (!File.Exists(highscoreFile)) return 0;
            var lines = File.ReadAllLines(highscoreFile);
            return lines.Where(l => l.StartsWith($"{playerName}:"))
                        .Select(l => int.Parse(l.Split(':')[1]))
                        .FirstOrDefault();
        }

        private void RestartGame_Click(object sender, RoutedEventArgs e)
        {
            GamePage newGame = new GamePage(playerName, difficulty);

            NavigationService nav = NavigationService.GetNavigationService(this);
            if (nav != null)
            {
                nav.Navigate(newGame);
            }
            else
            {
                Window window = Window.GetWindow(this);
                if (window != null)
                {
                    window.Content = newGame;
                }
            }
        }

        // ✅ Methode om geluid af te spelen
        private void SpeelGeluid(bool correct)
        {
            string bestand = correct ? "correct.mp3" : "wrong.mp3";
            string pad = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds", bestand);

            if (File.Exists(pad))
            {
                player.Open(new Uri(pad, UriKind.Absolute));
                player.Play();
            }
            else
            {
                MessageBox.Show($"Geluidsbestand niet gevonden: {pad}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }       
        }
    }
}
