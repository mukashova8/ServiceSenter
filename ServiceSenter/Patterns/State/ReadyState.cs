using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceSenter.Models;

namespace ServiceSenter.Patterns.State
{
    /// <summary>
    /// состояние Готова
    /// </summary>
    public class ReadyState : RequestState
    {
        public override bool CanTransitionTo(RequestStatusEnum newStatus)
        {
            //из Готова можно только Выдана
            return newStatus == RequestStatusEnum.Выдана;
        }
    }
}