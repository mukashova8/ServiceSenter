using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceSenter.Models;

namespace ServiceSenter.Patterns.State
{
    /// <summary>
    /// состояние Ожидает запчасти
    /// </summary>
    public class WaitingPartsState : RequestState
    {
        public override bool CanTransitionTo(RequestStatusEnum newStatus)
        {
            //из ОжидаетЗапчасти можно обратно в ВРаботе или сразу в Готова
            return newStatus == RequestStatusEnum.ВРаботе || newStatus == RequestStatusEnum.Готова;
        }
    }
}
