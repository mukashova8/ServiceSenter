using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceSenter.Models;

namespace ServiceSenter.Models
{
    /// <summary>
    /// класс таблицы видов устройств, наследуется Id из BaseEntity
    /// </summary>
    public class DeviceType : BaseEntity
    {
        public string TypeName { get; set; }
        public ICollection<Device> Devices { get; set; } = new List<Device>();
    }
}
