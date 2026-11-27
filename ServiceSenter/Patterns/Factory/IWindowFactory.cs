using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ServiceSenter.Patterns.Factory
{
    /// <summary>
    /// интерфейс фабрики окон, позволяет отделить логику создания окон от логики их использования
    /// </summary>
    public interface IWindowFactory
    {
        Window CreateWindow();
    }
}
