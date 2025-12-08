using System;

namespace KalapujSolU0.Models {
    public class Auto : Vehiculo, IMantenimiento {
        private int cantidadPuertas;

        public int CantidadPuertas {
            get => cantidadPuertas;
            set {
                if (value <= 0) throw new ArgumentOutOfRangeException(nameof(CantidadPuertas));
                cantidadPuertas = value;
            }
        }

        // Constructor vacío requerido para deserialización
        public Auto() { }

        public Auto(string marca, string modelo, int patentamiento, decimal precio, int cilindrada, int puertas)
            : base(marca, modelo, patentamiento, precio, cilindrada) {
            CantidadPuertas = puertas;
        }

        public override string ToString() {
            return $"[Auto] {base.ToString()}, Puertas: {CantidadPuertas}";
        }

        // Implementación de IMantenimiento
        public string RealizarMantenimiento() {
            return $"Mantenimiento completado en Auto {Marca} {Modelo} ({Patentamiento})";
        }

        public decimal CalcularCostoMantenimiento() {
            // costo fijo + costo por cindrada + extra según cantidad de puertas
            return 5000m + (Cilindrada * 2m) + (CantidadPuertas * 500m);
        }
    }
}
