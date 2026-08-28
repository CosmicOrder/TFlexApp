using System;
using System.Collections.Generic;
using System.Text;

namespace TFlexApp
{
    public interface IView
    {
        // Свойства для получения данных из формы
        string PartLength { get; }
        string PartHeight { get; }
        string PartThickness { get; }
        string HoleDiameter { get; }
        bool HasHole { get; }
        
        // Методы для вывода сообщений пользователю
        void ShowSuccess(string message, string title);
        void ShowError(string errorMessage, string title);

        // События 
        event EventHandler RunRequested;
    }
}
