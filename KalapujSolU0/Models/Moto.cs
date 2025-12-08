using KalapujSolU0.Services;
using System;

namespace KalapujSolU0.Models{
    public class Moto : Vehiculo, IMantenimiento {
        public TipoManillar TipoManillar { get; set; }

        // Constructor vacío requerido para deserialización
        public Moto() { }

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
