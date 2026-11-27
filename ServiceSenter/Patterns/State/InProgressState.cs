using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceSenter.Models;

namespace ServiceSenter.Patterns.State
{
    /// <summary>
    /// состояние В работе
    /// </summary>
    public class InProgressState : RequestState
    {
        public override bool CanTransitionTo(RequestStatusEnum newStatus)
        {
            //из ВРаботе можно в ОжидаетЗапчасти или Готова
            return newStatus == RequestStatusEnum.ОжидаетЗапчасти || newStatus == RequestStatusEnum.Готова;
        }
    }
}