using Microsoft.EntityFrameworkCore;

namespace GoogleStyleCalendar
{
    // DbContext obsługujący bazę danych SQLite dla aplikacji kalendarza
    public class AppDbContext : DbContext
    {
        // DbSet przechowujący zadania
        public DbSet<TaskItem> Tasks { get; set; }

        private const string DbFileName = "tasks_ef.db"; // Nazwa pliku bazy danych

        // Konfiguracja połączenia do SQLite
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={DbFileName}");
        }

        // Konfiguracja modelu EF - definiowanie kluczy i wymagań dla pól TaskItem
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Title).IsRequired();
                entity.Property(t => t.Date).IsRequired();
                entity.Property(t => t.StartTime).IsRequired();
                entity.Property(t => t.EndTime).IsRequired();
            });
        }
    }
}