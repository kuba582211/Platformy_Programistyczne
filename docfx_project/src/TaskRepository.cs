using System;
using System.Collections.Generic;
using System.Linq;

namespace GoogleStyleCalendar
{
    /// <summary>
    /// Statyczna klasa odpowiedzialna za operacje na bazie danych związane z zadaniami (TaskItem)
    /// </summary>
    public static class TaskRepository
    {
        /// <summary>
        /// Inicjalizuje bazę danych, tworząc ją jeśli jeszcze nie istnieje
        /// </summary>
        public static void InitializeDatabase()
        {
            using var db = new AppDbContext();
            db.Database.EnsureCreated();
        }

        /// <summary>
        /// Dodaje nowe zadanie do bazy danych
        /// </summary>
        /// <param name="task">Obiekt zadania do dodania</param>
        public static void InsertTask(TaskItem task)
        {
            using var db = new AppDbContext();
            db.Tasks.Add(task);
            db.SaveChanges();
        }

        /// <summary>
        /// Aktualizuje istniejące zadanie w bazie danych
        /// </summary>
        /// <param name="task">Obiekt zadania do aktualizacji</param>
        public static void UpdateTask(TaskItem task)
        {
            using var db = new AppDbContext();
            db.Tasks.Update(task);
            db.SaveChanges();
        }

        /// <summary>
        /// Usuwa zadanie z bazy danych
        /// </summary>
        /// <param name="task">Obiekt zadania do usunięcia</param>
        public static void DeleteTask(TaskItem task)
        {
            using var db = new AppDbContext();
            db.Tasks.Remove(task);
            db.SaveChanges();
        }

        /// <summary>
        /// Ładuje listę zadań z bazy dla konkretnego tygodnia (od poniedziałku do niedzieli)
        /// </summary>
        /// <param name="weekStart">Data poniedziałku tygodnia</param>
        /// <returns>Lista zadań z tego tygodnia</returns>
        public static List<TaskItem> LoadTasksForWeek(DateTime weekStart)
        {
            var weekEnd = weekStart.AddDays(7);

            using var db = new AppDbContext();
            return db.Tasks
                .Where(t => t.Date >= weekStart && t.Date < weekEnd)
                .ToList();
        }
    }
}