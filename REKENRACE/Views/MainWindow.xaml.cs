using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace REKENRACE.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            string playerName = NameInput.Text.Trim();
            ComboBoxItem selectedDifficulty = DifficultySelector.SelectedItem as ComboBoxItem;

            // ✅ Eerst de foutmelding verbergen
            ErrorText.Visibility = Visibility.Collapsed;

            if (string.IsNullOrEmpty(playerName))
            {
                ErrorText.Text = "Vul je naam in om te beginnen!";
                ErrorText.Visibility = Visibility.Visible;
                return;
            }

            if (selectedDifficulty == null)
            {
                ErrorText1.Text = "Kies een moeilijkheidsgraad!";
                ErrorText1.Visibility = Visibility.Visible;
                return;
            }

            string difficulty = selectedDifficulty.Content.ToString();

            GamePage gamePage = new GamePage(playerName, difficulty);
            this.Content = gamePage;
        }

        private void NameInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            PlaceholderText.Visibility = string.IsNullOrWhiteSpace(NameInput.Text) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
