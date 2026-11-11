using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ServiceSenter.Models;

namespace ServiceSenter.Models
{
    public class Client : BaseEntity
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime FirstContactDate { get; set; }
        public ICollection<Request> Requests { get; set; } = new List<Request>();
    }
}