using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace GoogleStyleCalendar
{
    public partial class MainWindow : Window
    {
        // Lista świąt pobranych z serwisu
        private List<Holiday> _holidays = new();

        // Zakres godzin pokazywanych w kalendarzu
        private const int StartHour = 6;
        private const int EndHour = 22;

        // Wysokość jednego wiersza (godziny) w kalendarzu
        private const double RowHeight = 40;

        // Data pierwszego dnia aktualnie wyświetlanego tygodnia (poniedziałek)
        private DateTime _currentWeekStart = GetStartOfWeek(DateTime.Today);

        public MainWindow()
        {
            InitializeComponent();

            // Ustaw nagłówki dni w kalendarzu (dni tygodnia + numery dni)
            UpdateHeaderDates();

            // Inicjalizuj bazę danych (jeśli jest)
            TaskRepository.InitializeDatabase();

            // Załaduj zadania i święta dla aktualnego tygodnia
            LoadTasksFromDatabase();
            LoadHolidays();
        }

        /// <summary>
        /// Dodaje do kalendarza wizualne oznaczenie święta całodniowego
        /// </summary>
        private void AddHolidayToCalendar(Holiday holiday)
        {
            // Oblicz kolumnę (dzień tygodnia) - poniedziałek = 0
            int dayColumn = ((int)holiday.Date.DayOfWeek + 6) % 7;

            // Czas trwania święta to cały zakres godzin (StartHour do EndHour)
            double durationHours = EndHour - StartHour + 1;

            // Szerokość kontrolki canvas z zadaniami
            double canvasWidth = TasksCanvas.ActualWidth;
            if (canvasWidth == 0) canvasWidth = TasksCanvas.Width;

            double columnWidth = canvasWidth / 7;

            // Tworzymy prostokąt z czerwonym tłem i tekstem nazwy święta
            var border = new Border
            {
                Background = Brushes.Red,
                CornerRadius = new CornerRadius(6),
                Margin = new Thickness(2),
                Height = durationHours * RowHeight - 4,
                Width = columnWidth - 6,
                Child = new TextBlock
                {
                    Text = holiday.LocalName,
                    Foreground = Brushes.White,
                    FontWeight = FontWeights.Bold,
                    TextWrapping = TextWrapping.Wrap,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(4)
                },
                ToolTip = $"Święto: {holiday.LocalName} ({holiday.Date.ToShortDateString()})"
            };

            // Pozycjonujemy prostokąt na odpowiednim dniu i na górze (cały dzień)
            Canvas.SetTop(border, 2);
            Canvas.SetLeft(border, dayColumn * columnWidth + 2);

            TasksCanvas.Children.Add(border);
        }

        /// <summary>
        /// Asynchroniczne pobieranie świąt z serwisu zewnętrznego i aktualizacja listy w UI
        /// </summary>
        private async void LoadHolidays()
        {
            try
            {
                _holidays = await HolidayService.GetHolidaysAsync(2025, "PL");
                HolidaysListBox.ItemsSource = _holidays;
                HolidaysListBox.DisplayMemberPath = "LocalName";

                HighlightHolidaysInHeaders();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nie udało się pobrać świąt: " + ex.Message);
            }
        }

        /// <summary>
        /// Podświetla dni w nagłówku kalendarza jeśli są świętami
        /// </summary>
        private void HighlightHolidaysInHeaders()
        {
            for (int i = 0; i <= 6; i++)
            {
                DateTime date = _currentWeekStart.AddDays(i);
                bool isHoliday = _holidays.Any(h => h.Date.Date == date.Date);

                // Wybieramy odpowiedni TextBlock z nagłówka wg indeksu dnia
                TextBlock dayTextBlock = i switch
                {
                    0 => Day0Date,
                    1 => Day1Date,
                    2 => Day2Date,
                    3 => Day3Date,
                    4 => Day4Date,
                    5 => Day5Date,
                    6 => Day6Date,
                    _ => null
                };

                if (dayTextBlock != null)
                {
                    // Jeśli dzień jest świętem, ustawiamy kolor czerwony, inaczej czarny
                    dayTextBlock.Foreground = isHoliday ? Brushes.Red : Brushes.Black;
                }
            }
        }

        /// <summary>
        /// Aktualizuje daty wyświetlane w nagłówku kalendarza oraz podświetla święta
        /// </summary>
        private void UpdateHeaderDates()
        {
            // Wyświetla nazwę miesiąca i rok (np. "czerwiec 2025")
            MonthLabel.Text = _currentWeekStart.ToString("MMMM yyyy", new CultureInfo("pl-PL"));

            // Ustawiamy numery dni od poniedziałku do niedzieli
            Day0Date.Text = _currentWeekStart.AddDays(0).Day.ToString();
            Day1Date.Text = _currentWeekStart.AddDays(1).Day.ToString();
            Day2Date.Text = _currentWeekStart.AddDays(2).Day.ToString();
            Day3Date.Text = _currentWeekStart.AddDays(3).Day.ToString();
            Day4Date.Text = _currentWeekStart.AddDays(4).Day.ToString();
            Day5Date.Text = _currentWeekStart.AddDays(5).Day.ToString();
            Day6Date.Text = _currentWeekStart.AddDays(6).Day.ToString();

            HighlightHolidaysInHeaders();
        }

        /// <summary>
        /// Zwraca datę poniedziałku danego tygodnia dla podanej daty
        /// </summary>
        private static DateTime GetStartOfWeek(DateTime date)
        {
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-diff).Date;
        }

        /// <summary>
        /// Ładuje zadania z bazy danych dla aktualnego tygodnia i wyświetla je w kalendarzu
        /// </summary>
        private void LoadTasksFromDatabase()
        {
            // Czyścimy wszystkie istniejące wizualne elementy z kalendarza
            TasksCanvas.Children.Clear();

            // Pobieramy zadania z bazy na wybrany tydzień
            var tasks = TaskRepository.LoadTasksForWeek(_currentWeekStart);

            // Dodajemy każde zadanie do kalendarza
            foreach (var task in tasks)
            {
                AddTaskToCalendar(task);
            }

            // Dodajemy święta z aktualnego tygodnia jako czerwone bloki
            var holidaysThisWeek = _holidays.Where(h => h.Date >= _currentWeekStart && h.Date < _currentWeekStart.AddDays(7));
            foreach (var holiday in holidaysThisWeek)
            {
                AddHolidayToCalendar(holiday);
            }
        }

        /// <summary>
        /// Event wywoływany po załadowaniu kontrolki TasksCanvas - wtedy ładujemy zadania
        /// </summary>
        private void TasksCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTasksFromDatabase();
        }

        /// <summary>
        /// Obsługa kliknięcia przycisku dodania zadania
        /// </summary>
        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            // Sprawdź, czy wybrano dzień z ComboBox
            if (!(DayComboBox.SelectedItem is ComboBoxItem selectedDay))
            {
                MessageBox.Show("Wybierz dzień.");
                return;
            }

            // Parsujemy czas rozpoczęcia i zakończenia zadania, walidujemy zakres
            if (!TimeSpan.TryParseExact(StartTimeBox.Text, "hh\\:mm", CultureInfo.InvariantCulture, out TimeSpan startTime) ||
                !TimeSpan.TryParseExact(EndTimeBox.Text, "hh\\:mm", CultureInfo.InvariantCulture, out TimeSpan endTime) ||
                endTime <= startTime || startTime < TimeSpan.FromHours(StartHour) || endTime > TimeSpan.FromHours(EndHour + 1))
            {
                MessageBox.Show("Wprowadź poprawny czas (HH:mm), od 06:00 do 22:59.");
                return;
            }

            // Sprawdź czy wprowadzono tytuł
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                MessageBox.Show("Wprowadź tytuł zadania.");
                return;
            }

            // Obliczamy datę zadania na podstawie wybranego dnia tygodnia i aktualnego tygodnia
            DayOfWeek selected = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), selectedDay.Tag.ToString());
            DateTime taskDate = _currentWeekStart.AddDays(((int)selected + 6) % 7);

            // Tworzymy nowy obiekt zadania
            var task = new TaskItem
            {
                Title = TitleTextBox.Text.Trim(),
                Date = taskDate,
                StartTime = startTime,
                EndTime = endTime
            };

            // Wstawiamy zadanie do bazy i odświeżamy widok
            TaskRepository.InsertTask(task);
            LoadTasksFromDatabase();
        }

        /// <summary>
        /// Dodaje wizualne przedstawienie zadania do kalendarza (na kanwie)
        /// </summary>
        private void AddTaskToCalendar(TaskItem task)
        {
            int dayColumn = ((int)task.Date.DayOfWeek + 6) % 7;

            double startOffsetHours = (task.StartTime - TimeSpan.FromHours(StartHour)).TotalHours;
            double durationHours = (task.EndTime - task.StartTime).TotalHours;

            double canvasWidth = TasksCanvas.ActualWidth;
            if (canvasWidth == 0) canvasWidth = TasksCanvas.Width;

            double columnWidth = canvasWidth / 7;

            // Tworzymy prostokąt reprezentujący zadanie z tytułem
            var border = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(100, 149, 237)), // Kolor cornflowerblue
                CornerRadius = new CornerRadius(6),
                Margin = new Thickness(2),
                Height = durationHours * RowHeight - 4,
                Width = columnWidth - 6,
                Child = new TextBlock
                {
                    Text = task.Title,
                    Foreground = Brushes.White,
                    FontWeight = FontWeights.SemiBold,
                    TextWrapping = TextWrapping.Wrap,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(4)
                }
            };

            // Obsługa kliknięcia na zadanie - otwarcie okna edycji
            border.MouseLeftButtonUp += (s, e) =>
            {
                var editWindow = new EditTaskWindow(task);
                if (editWindow.ShowDialog() == true)
                {
                    // Po edycji odświeżamy kalendarz
                    LoadTasksFromDatabase();
                }
            };

            // Ustawiamy pozycję prostokąta wg godziny rozpoczęcia i dnia tygodnia
            Canvas.SetTop(border, startOffsetHours * RowHeight + 2);
            Canvas.SetLeft(border, dayColumn * columnWidth + 2);

            TasksCanvas.Children.Add(border);
        }

        /// <summary>
        /// Obsługa zmiany wybranego tygodnia w kalendarzu - aktualizacja nagłówka i danych
        /// </summary>
        private void WeekCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WeekCalendar.SelectedDate is DateTime selectedDate)
            {
                // Ustaw nowy tydzień na poniedziałek wybranego dnia
                _currentWeekStart = GetStartOfWeek(selectedDate);

                // Odśwież nagłówki i kalendarz
                UpdateHeaderDates();
                LoadTasksFromDatabase();
            }
        }
    }
}