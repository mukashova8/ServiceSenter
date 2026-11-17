using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceSenter.Models
{
    /// <summary>
    /// cтатический класс для хранения данных текущего пользователя после входа в систему
    /// </summary>
    public static class UserSession
    {
        public static int Id { get; set; }
        public static string FullName { get; set; }
        public static string Role { get; set; }
    }
}
