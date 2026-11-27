using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceSenter.Models;

namespace ServiceSenter.Patterns.State
{
    /// <summary>
    /// базовый класс для всех состояний заявки
    /// </summary>
    public abstract class RequestState
    {
        //метод, который проверяет, можно ли перейти в новое состояние
        public abstract bool CanTransitionTo(RequestStatusEnum newStatus);
    }
}
