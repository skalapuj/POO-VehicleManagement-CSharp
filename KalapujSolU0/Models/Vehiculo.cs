using KalapujSolU0.Exceptions;
using System;

namespace KalapujSolU0.Models{
    public abstract class Vehiculo {
        private string? marca;
        private string? modelo;
        private int patentamiento;
        private decimal precio;
        private int cilindrada;

        public string Marca {
            get => marca ?? throw new InvalidOperationException("Marca no puede ser null");
            set => marca = value ?? throw new ValorInvalidoException("Marca no puede ser null.");
        }

        public string Modelo {
            get => modelo ?? throw new InvalidOperationException("Modelo no puede ser null");
            set => modelo = value ?? throw new ValorInvalidoException("Modelo no puede ser null.");
        }

        public int Patentamiento {
            get => patentamiento;
            set {
                if (value < 1885 || value > DateTime.Today.Year) // primer auto fue patentado en 1886
                    throw new ValorInvalidoException("El año de patentamiento no puede ser menor al primer auto pantentado en 1885 ni mayor al corriente año.");
                patentamiento = value;
            }
        }

        public decimal Precio {
            get => precio;
            set {
                if (value <= 0) throw new ValorInvalidoException("El precio debe ser mayor a cero.");
                precio = value;
            }
        }

        public int Cilindrada {
            get => cilindrada;
            set {
                if (value <= 0) throw new ValorInvalidoException("La cilindrada debe ser mayor a cero.");
                cilindrada = value;
            }
        }

        // Constructor vacío requerido para deserialización
        public Vehiculo() { }

        protected Vehiculo(string marca, string modelo, int patentamiento, decimal precio, int cilindrada) {
            Marca = marca;
            Modelo = modelo;
            Patentamiento = patentamiento;
            Precio = precio;
            Cilindrada = cilindrada;
        }

        public override string ToString() {
            return $"Marca: {Marca}, Modelo: {Modelo}, Año de patentamiento: {Patentamiento}, Precio: ${Precio}, Cilindrada: {Cilindrada} cc ";
        }
    }
}
