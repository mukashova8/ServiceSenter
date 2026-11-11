using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceSenter.Models;

namespace ServiceSenter.Models
{
    public class EmployeeRole : BaseEntity
    {
        public string RoleName { get; set; }
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
