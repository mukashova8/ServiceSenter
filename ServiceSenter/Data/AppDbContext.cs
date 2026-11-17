using Microsoft.EntityFrameworkCore;
using ServiceSenter.Models;

namespace ServiceSenter.Data
{
    /// <summary>
    /// класс подключения приложения с бд
    /// </summary>
    public class AppDbContext : DbContext
    {
        //DbSet таблица в бд
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeRole> EmployeesRole { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<DeviceType> DeviceTypes { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<RequestStatus> RequestStatuses { get; set; }
        public DbSet<StatusHistory> StatusHistories { get; set; }

        //подключение к бд 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=localhost\SQLEXPRESS;Database=ServiceSenter;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        //настройка связей 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //связи между заявок и историей статусов (у одной заявки может быть много историй статусов, но каджая запись в истории  привязана к одной зявки)
            modelBuilder.Entity<Request>()
                .HasMany(r => r.StatusHistory)
                .WithOne(h => h.Request)
                //если удалить заявку в истории она не удалиться
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}