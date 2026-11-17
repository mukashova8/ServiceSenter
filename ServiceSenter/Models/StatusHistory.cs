using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ServiceSenter.Models;

namespace ServiceSenter.Models
{
    /// <summary>
    /// класс таблицы истории статусов, наследуется Id из BaseEntity 
    /// </summary>
    public class StatusHistory : BaseEntity
    {
        public DateTime ChangeDatetime { get; set; }
        public Request Request { get; set; }
        public RequestStatus Status { get; set; }
    }
}