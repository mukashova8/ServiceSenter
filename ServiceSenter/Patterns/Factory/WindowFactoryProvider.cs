using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ServiceSenter.Patterns.Factory
{
    /// <summary>
    /// класс-провайдер, который возвращает нужную фабрику в зависимости от роли пользователя
    /// </summary>
    public static class WindowFactoryProvider
    {
        public static IWindowFactory GetFactory(string role)
        {
            switch (role)
            {
                case "Администратор":
                    return new AdminWindowFactory();
                case "Оператор":
                    return new OperatorWindowFactory();
                case "Мастер":
                    return new MasterWindowFactory();
                default:
                    return null;
            }
        }
    }
}