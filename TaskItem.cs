using System;

namespace GoogleStyleCalendar
{
    public class TaskItem
    {
        public int Id { get; set; }  // Klucz główny

        public string Title { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}