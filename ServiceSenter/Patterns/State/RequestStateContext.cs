using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceSenter.Models;

namespace ServiceSenter.Patterns.State
{
    /// <summary>
    /// класс-контекст, который управляет текущим состоянием
    /// </summary>
    public class RequestStateContext
    {
        private RequestState _currentState;

        //передача текущего статуса из перечисления
        public RequestStateContext(RequestStatusEnum currentStatusEnum)
        {
            switch (currentStatusEnum)
            {
                case RequestStatusEnum.Принята:
                    _currentState = new AcceptedState();
                    break;
                case RequestStatusEnum.ВРаботе:
                    _currentState = new InProgressState();
                    break;
                case RequestStatusEnum.ОжидаетЗапчасти:
                    _currentState = new WaitingPartsState();
                    break;
                case RequestStatusEnum.Готова:
                    _currentState = new ReadyState();
                    break;
                case RequestStatusEnum.Выдана:
                    _currentState = new IssuedState();
                    break;
                default:
                    _currentState = new AcceptedState();
                    break;
            }
        }

        //метод проверки перехода
        public bool CanTransitionTo(RequestStatusEnum newStatus)
        {
            return _currentState.CanTransitionTo(newStatus);
        }
    }
}