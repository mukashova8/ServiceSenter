using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ServiceSenter.Models;

namespace ServiceSenter.Models
{
    public class Device : BaseEntity
    {
        public DeviceType DeviceType { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public string Manufacturer { get; set; }
        public ICollection<Request> Requests { get; set; } = new List<Request>();

        [NotMapped]
        public string DisplayName => $"{Manufacturer} {Model} ({SerialNumber})";
    }
}