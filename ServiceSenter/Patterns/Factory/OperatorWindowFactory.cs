using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ServiceSenter.Views;

namespace ServiceSenter.Patterns.Factory
{
    /// <summary>
    /// фабрика для создания окна Оператора, реализует интерфейс IWindowFactory
    /// </summary>
    public class OperatorWindowFactory : IWindowFactory
    {
        //фабричный метод, который создает и возвращает окно
        public Window CreateWindow()
        {
            return App.ServiceProvider.GetRequiredService<OperatorWindow>();
        }
    }
}