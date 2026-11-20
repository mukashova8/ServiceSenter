using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceSenter.Models
{
    /// <summary>
    /// класс перечислений (статусов, должностей сотрудников и видов устройств)
    /// </summary>
    public enum RequestStatusEnum
    {
        Принята,
        ВРаботе,  
        ОжидаетЗапчасти,
        Готова,
        Выдана
    }

    public enum EmployeeRoleEnum
    {
        Администратор,
        Оператор,
        Мастер
    }

    public enum DeviceTypeEnum
    {
        Ноутбук,
        Смартфон,
        Телевизор,
        Планшет,
        Принтер
    }
}
