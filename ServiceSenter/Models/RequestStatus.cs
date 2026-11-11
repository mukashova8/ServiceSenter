using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ServiceSenter.Models;

namespace ServiceSenter.Models
{
    public class RequestStatus : BaseEntity
    {
        public string StatusName { get; set; }
        public ICollection<Request> Requests { get; set; } = new List<Request>();
        public ICollection<StatusHistory> StatusHistory { get; set; } = new List<StatusHistory>();
    }
}