using System;

namespace KalapujSolU0.Models {
    public class Propietario {
        public string Nombre { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;

        public Propietario() { }
        public Propietario(string nombre, string dni) {
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            Dni = dni ?? throw new ArgumentNullException(nameof(dni));
        }

        public override string ToString() {
            return $"Propietario: {Nombre} - DNI: {Dni}";
        }
    }
}
