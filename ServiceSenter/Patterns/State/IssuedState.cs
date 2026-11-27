using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceSenter.Models;

namespace ServiceSenter.Patterns.State
{
    /// <summary>
    /// состояние Выдана
    /// </summary>
    public class IssuedState : RequestState
    {
        public override bool CanTransitionTo(RequestStatusEnum newStatus)
        {
            //из Выдана переходов нет 
            return false;
        }
    }
}
