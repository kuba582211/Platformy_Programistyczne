using System;
using System.Globalization;
using System.Windows;

namespace GoogleStyleCalendar
{
    public partial class EditTaskWindow : Window
    {
        private TaskItem _task;

        public EditTaskWindow(TaskItem task)
        {
            InitializeComponent();
            _task = task;

            // Wypełniamy pola formularza danymi zadania
            TitleTextBox.Text = _task.Title;
            StartTimeBox.Text = _task.StartTime.ToString(@"hh\:mm");
            EndTimeBox.Text = _task.EndTime.ToString(@"hh\:mm");
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Walidacja tytułu
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                MessageBox.Show("Wprowadź tytuł zadania.");
                return;
            }

            // Walidacja czasu - format, zakres, logiczne godziny
            if (!TimeSpan.TryParseExact(StartTimeBox.Text, @"hh\:mm", CultureInfo.InvariantCulture, out TimeSpan start) ||
                !TimeSpan.TryParseExact(EndTimeBox.Text, @"hh\:mm", CultureInfo.InvariantCulture, out TimeSpan end) ||
                end <= start || start < TimeSpan.FromHours(6) || end > TimeSpan.FromHours(23))
            {
                MessageBox.Show("Wprowadź poprawny czas (HH:mm), start przed końcem, w zakresie 06:00 - 23:00.");
                return;
            }

            // Aktualizacja obiektu zadania
            _task.Title = TitleTextBox.Text.Trim();
            _task.StartTime = start;
            _task.EndTime = end;

            try
            {
                // Zapis do repozytorium (bazy danych)
                TaskRepository.UpdateTask(_task);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas zapisywania: " + ex.Message);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Czy na pewno chcesz usunąć to zadanie?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    TaskRepository.DeleteTask(_task);
                    DialogResult = true;
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd podczas usuwania: " + ex.Message);
                }
            }
        }
    }
}