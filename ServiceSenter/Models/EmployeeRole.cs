using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceSenter.Models;

namespace ServiceSenter.Models
{
    /// <summary>
    /// класс таблицы должностей сотрудников, наследуется Id из BaseEntity 
    /// </summary>
    public class EmployeeRole : BaseEntity
    {
        public string RoleName { get; set; }
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
