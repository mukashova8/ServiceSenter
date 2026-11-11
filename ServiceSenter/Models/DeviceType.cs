using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceSenter.Models;

namespace ServiceSenter.Models
{
    public class DeviceType : BaseEntity
    {
        public string TypeName { get; set; }
        public ICollection<Device> Devices { get; set; } = new List<Device>();
    }
}
