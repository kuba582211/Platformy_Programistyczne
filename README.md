# 📅 SmartWeekPlanner

**SmartWeekPlanner** to aplikacja desktopowa w stylu kalendarza Google, umożliwiająca planowanie tygodnia, dodawanie zadań oraz wyświetlanie świąt.

## 🛠️ Funkcje

- 📌 Dodawanie zadań do konkretnego dnia i godziny
- 🗑️ Edycja i usuwanie zadań
- 📅 Automatyczne pobieranie świąt dla Polski (dzięki `HolidayService`)
- 🟥 Podświetlanie świąt w kalendarzu
- 💾 Przechowywanie zadań w lokalnej bazie danych SQLite
- 🖱️ Interfejs w technologii WPF (Windows Presentation Foundation)

## 🚀 Jak uruchomić

1. **Wymagania**:
   - .NET 6 lub nowszy
   - Visual Studio 2022+ (lub VS Code z obsługą .NET)

2. **Kroki**:
   ```bash
   git clone https://github.com/TWOJA-NAZWA-UZYTKOWNIKA/SmartWeekPlanner.git
   cd SmartWeekPlanner
   dotnet build
   dotnet run

3. Struktura projektu
SmartWeekPlanner/
├── MainWindow.xaml / .cs       # Główne UI aplikacji
├── TaskRepository.cs           # Logika bazy danych
├── HolidayService.cs           # Pobieranie świąt z API
├── Models/TaskItem.cs          # Model zadania
├── Models/Holiday.cs           # Model święta
├── README.md                   # Ten plik

Autor:
Jakub Golec
