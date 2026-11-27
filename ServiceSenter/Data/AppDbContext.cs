using Microsoft.EntityFrameworkCore;
using ServiceSenter.Models;
using System;   
using System.Linq; 

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

        //метод автозаполнения справочников (статусы, роли, типы устройств) из enum
        public void SeedData()
        {
            //заполнение таблицы статусов заявок из RequestStatusEnum
            if (!RequestStatuses.Any())
            {
                foreach (RequestStatusEnum status in Enum.GetValues(typeof(RequestStatusEnum)))
                {
                    RequestStatuses.Add(new RequestStatus { StatusName = status.ToString() });
                }
                SaveChanges();
            }

            //заполнение таблицы ролей сотрудников из EmployeeRoleEnum
            if (!EmployeesRole.Any())
            {
                foreach (EmployeeRoleEnum role in Enum.GetValues(typeof(EmployeeRoleEnum)))
                {
                    EmployeesRole.Add(new EmployeeRole { RoleName = role.ToString() });
                }
                SaveChanges();
            }

            //заполнение таблицы типов устройств из DeviceTypeEnum
            if (!DeviceTypes.Any())
            {
                foreach (DeviceTypeEnum type in Enum.GetValues(typeof(DeviceTypeEnum)))
                {
                    DeviceTypes.Add(new DeviceType { TypeName = type.ToString() });
                }
                SaveChanges();
            }
        }
    }
}