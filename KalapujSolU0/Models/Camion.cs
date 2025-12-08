using System;

namespace KalapujSolU0.Models {
    public class Camion : Vehiculo, IMantenimiento {
        private double capacidadDeCarga;

        public double CapacidadDeCarga {
            get => capacidadDeCarga;
            set {
                if (value <= 0) throw new ArgumentOutOfRangeException(nameof(capacidadDeCarga));
                capacidadDeCarga = value;
            }
        }

        // Constructor vacío requerido para deserialización
        public Camion() { }

        public Camion(string marca, string modelo, int patentamiento, decimal precio, int cilindrada, double carga)
            : base(marca, modelo, patentamiento, precio, cilindrada) {
            CapacidadDeCarga = carga;
        }

        public override string ToString() {
            return $"[Camion] {base.ToString()}, Capacidad de carga: {CapacidadDeCarga}";
        }

        // Implementación de IMantenimiento
        public string RealizarMantenimiento() {
            return $"Mantenimiento completado en Camion {Marca} {Modelo} ({Patentamiento})";
        }

        public decimal CalcularCostoMantenimiento() {
            // costo fijo + costo por cindrada * 20 * extra según capacidad de carga
            return 15000m + ((Cilindrada * 20m) * (decimal)CapacidadDeCarga);
        }
    }
}
