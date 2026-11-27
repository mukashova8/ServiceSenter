using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceSenter.Models;

namespace ServiceSenter.Patterns.State
{
    /// <summary>
    /// cостояние Принята
    /// </summary>
    public class AcceptedState : RequestState
    {
        public override bool CanTransitionTo(RequestStatusEnum newStatus)
        {
            //из Принята можно только в ВРаботе или ОжидаетЗапчасти
            return newStatus == RequestStatusEnum.ВРаботе || newStatus == RequestStatusEnum.ОжидаетЗапчасти;
        }
    }
}
