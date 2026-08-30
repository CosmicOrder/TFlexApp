using System;
using System.Collections.Generic;
using System.Text;

namespace TFlexApp
{
    /// <summary>
    /// Централизованное хранилище сообщений об ошбиках для пользователя.
    /// </summary>
    public static class Messages
    {
        // Ошибки валидации
        public const string ErrDimensionsRequired = "Заполните все поля габаритов детали!";
        public const string ErrDimensionsMustBePositive = "Габариты детали должны быть положительными числами!";
        public const string ErrHoleDiameterRequired = "Укажите диаметр отверстия!";
        public const string ErrHoleDiameterMustBePositive = "Диаметр отверстия должен быть положительным числом!";
        public const string ErrHoleDiameterExceedsLimit = "Диаметр ({0}) слишком велик! Максимум: {1:F2}";
        // Ошибки CAD
        public const string ErrDocumentCreation = "Не удалось создать документ T-Flex CAD.";
        // Успешные операции
        public const string SuccessBuildMessage = "Готово!\nОбозначение: {0}\nНаименование: {1}";
        // Заголовки диалоговых окон
        public const string TitleSuccess = "Успех!";
        public const string TitleError = "Ошибка!";
    }
}
