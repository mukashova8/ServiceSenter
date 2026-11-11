using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ServiceSenter.Models;

namespace ServiceSenter.Models
{
    public class Employee : BaseEntity
    {
        public string FullName { get; set; }
        public EmployeeRole Role { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public ICollection<Request> Requests { get; set; } = new List<Request>();
    }


}