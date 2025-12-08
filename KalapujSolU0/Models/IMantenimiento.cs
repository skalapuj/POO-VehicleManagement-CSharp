using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalapujSolU0.Models {

    public interface IMantenimiento {
        string RealizarMantenimiento();
        decimal CalcularCostoMantenimiento();
    }
}
