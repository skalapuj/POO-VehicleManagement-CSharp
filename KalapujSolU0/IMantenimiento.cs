using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalapujSol {

    public interface IMantenimiento {
        string RealizarMantenimiento();
        decimal CalcularCostoMantenimiento();
    }
}
