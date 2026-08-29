using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TFlexApp
{
    public class Presenter
    {
        private readonly IView _view;
        private readonly ICadFacade _cadFacade;

        public Presenter(IView view, ICadFacade cadFacade)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _cadFacade = cadFacade ?? throw new ArgumentNullException(nameof(cadFacade));
            // Подписка на событие RunRequested
            _view.RunRequested += OnRun;
        }

        // Обработчик события RunRequested
        private void OnRun(object? sender, EventArgs e)
        {
            if (CreateModel(out Model? model))
            {
                if (model != null)
                {
                    try
                    {
                        _cadFacade.CreatePartDrawing(model);
                        _view.ShowSuccess(string.Format(Messages.SuccessBuildMessage, model.Designation, model.PartName), Messages.TitleSuccess);
                    }
                    catch (Exception ex)
                    {
                        _view.ShowError("Ошибка при создании детали: " + ex.Message, Messages.TitleError);
                    }
                }
            }
        }

        // Метод для создания модели детали на основе введенных данных
        private bool CreateModel(out Model? model)
        {
            model = null;

            if (!ValidateAndParseInputs(out double length, out double height, out double thickness, out double holeDiameter))
            {
                return false;
            }
            model = BuildModel(length, height, thickness, holeDiameter);
            return true;
        }

        // Метод для проверки и парсинга входных данных
        private bool ValidateAndParseInputs(out double length, out double height, out double thickness, out double holeDiameter)
        {
            length = height = thickness = holeDiameter = 0;

            // 1. Проверка на пустоту основных полей
            if (string.IsNullOrWhiteSpace(_view.PartLength) ||
                string.IsNullOrWhiteSpace(_view.PartHeight) ||
                string.IsNullOrWhiteSpace(_view.PartThickness))
            {
                _view.ShowError(Messages.ErrDimensionsRequired, Messages.TitleError);
                return false;
            }

            // 2. Парсинг и проверка на положительность основных полей
            if (!double.TryParse(_view.PartLength, out length) || length <= 0 ||
                !double.TryParse(_view.PartHeight, out height) || height <= 0 ||
                !double.TryParse(_view.PartThickness, out thickness) || thickness <= 0)
            {
                _view.ShowError(Messages.ErrDimensionsMustBePositive, Messages.TitleError);
                return false;
            }

            if (_view.HasHole)
            {
                if (string.IsNullOrWhiteSpace(_view.HoleDiameter))
                {
                    _view.ShowError(Messages.ErrHoleDiameterRequired, Messages.TitleError);
                    return false;
                }

                if (!double.TryParse(_view.HoleDiameter, out holeDiameter) || holeDiameter <= 0)
                {
                    _view.ShowError(Messages.ErrHoleDiameterMustBePositive, Messages.TitleError);
                    return false;
                }

                double maxAllowedDiameter = 0.7 * Math.Min(length, height);
                if (holeDiameter > maxAllowedDiameter)
                {
                    _view.ShowError(string.Format(Messages.ErrHoleDiameterExceedsLimit, holeDiameter, maxAllowedDiameter), Messages.TitleError);
                    return false;
                }
            }
            return true;
        }

        // Метод для создания модели детали
        private Model BuildModel(double length, double height, double thickness, double holeDiameter)
        {
            var model = new Model
            {
                Length = length,
                Height = height,
                Thickness = thickness,
                HasHole = _view.HasHole,
                HoleDiameter = _view.HasHole ? holeDiameter : 0
            };
            return model;
        }
    }
}
