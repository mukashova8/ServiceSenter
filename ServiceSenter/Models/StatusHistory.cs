using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ServiceSenter.Models;

namespace ServiceSenter.Models
{
    public class StatusHistory : BaseEntity
    {
        public DateTime ChangeDatetime { get; set; }
        public Request Request { get; set; }
        public RequestStatus Status { get; set; }
    }
}