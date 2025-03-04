using System;
using System.Windows;
using System.Windows.Controls;

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
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Application.Current.MainWindow.Close();
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            string playerName = NameInput.Text.Trim();
            ComboBoxItem selectedDifficulty = DifficultySelector.SelectedItem as ComboBoxItem;

            if (string.IsNullOrEmpty(playerName))
            {
                MessageBox.Show("Vul je naam in om te beginnen!", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (selectedDifficulty == null)
            {
                MessageBox.Show("Kies een moeilijkheidsgraad!", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string difficulty = selectedDifficulty.Content.ToString();

            GamePage gamePage = new GamePage(playerName, difficulty);
            this.Content = gamePage;
        }

        private void Sound_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Knop werkt!");
        }

        private void NameInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            PlaceholderText.Visibility = string.IsNullOrWhiteSpace(NameInput.Text) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
