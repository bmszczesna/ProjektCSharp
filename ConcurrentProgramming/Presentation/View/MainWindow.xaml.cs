using System.Windows;
using ViewModel;

namespace Presentation.View
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel ViewModel => DataContext as MainWindowViewModel;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(BallCountTextBox.Text, out int numberOfBalls))
            {
                if (numberOfBalls <= 0)
                {
                    MessageBox.Show("Liczba kulek musi być większa niż 0.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (numberOfBalls > 100)
                {
                    MessageBox.Show("Maksymalna liczba kulek to 100.", "Ograniczenie", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                ViewModel.Start(numberOfBalls);
            }
            else
            {
                MessageBox.Show("Wprowadź poprawną liczbę całkowitą.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
