using System;
using System.Collections.Generic;
using System.Text;

namespace TFlexApp
{
    public interface ICadOperations
    {
        /// <summary>
        /// Создает 3D-модель в T-Flex по параметрам из модели.
        /// </summary>
        public void Create3DPart(Model model);       

    }
}
