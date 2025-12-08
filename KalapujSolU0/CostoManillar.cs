using System;

namespace KalapujSol
{
    public static class CostoManillar {
        public static decimal ObtenerCosto(TipoManillar tipo) {
            switch (tipo) {
                case TipoManillar.Recto:
                    return 100m;

                case TipoManillar.Curvo:
                    return 150m;

                case TipoManillar.Deportivo:
                    return 200m;

                default:
                    return 0m;

            }
        }
    }
}
