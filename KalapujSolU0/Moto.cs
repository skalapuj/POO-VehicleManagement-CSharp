using System;

namespace KalapujSol {
    public class Moto : Vehiculo, IMantenimiento {
        public TipoManillar TipoManillar { get; set; }

        public Moto(string marca, string modelo, int patentamiento, decimal precio, int cilindrada,TipoManillar tipoManillar)
            : base(marca, modelo, patentamiento, precio, cilindrada) {
            TipoManillar = tipoManillar;
        }

        public override string ToString() {
            return $"[Moto] {base.ToString()}, Tipo de Manillar: {TipoManillar}";
        }

        public string RealizarMantenimiento() {
            return $"Mantenimiento completado en Moto {Marca} {Modelo} ({Patentamiento})";
        }

        public decimal CalcularCostoMantenimiento() {
            // Costo fijo + costo por cilindrada + costo por tipo de manillar
            return 5000m + (Cilindrada * 2m) + CostoManillar.ObtenerCosto(TipoManillar);
        }
    }
}
