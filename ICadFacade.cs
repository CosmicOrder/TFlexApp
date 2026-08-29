using System;
using System.Collections.Generic;
using System.Text;

namespace TFlexApp
{
    public interface ICadFacade
    {
        /// <summary>
        /// Создаёт документ T-Flex: 3D-модель, штамп, стандартные проекции.
        /// </summary>
        void CreatePartDrawing(Model model);
    }
}
