using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ServiceSenter.Models;

namespace ServiceSenter.Models
{
    public class Request : BaseEntity
    {
        public DateTime DateReceived { get; set; }
        public string FaultDescription { get; set; }
        public decimal Cost { get; set; }
        public DateTime DueDate { get; set; }
        public RequestStatus Status { get; set; }
        public Device Device { get; set; }
        public Client Client { get; set; }
        public Employee Employee { get; set; }
        public ICollection<StatusHistory> StatusHistory { get; set; } = new List<StatusHistory>();

    }
}