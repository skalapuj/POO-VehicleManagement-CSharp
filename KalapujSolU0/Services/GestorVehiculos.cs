using KalapujSolU0.Exceptions;
using KalapujSolU0.Models;

namespace KalapujSolU0.Services
{
    internal class GestorVehiculos
    {
        //============== Campos privados =============//
        // Lista de marcas disponibles 
        private readonly List<string> marcasDisponibles = ["Toyota", "Ford", "Chevrolet", "Honda", "Renault"];

        // Registro de patente con su vehículos y su propietario asociado
        private readonly Dictionary<string, (Vehiculo Vehiculo, Propietario Propietario)> registroVehiculos = [];

        // Instancia de la clase Persistencia para manejar el almacenamiento de datos
        private readonly Persistencia persistencia = new ();


        //============== Gestion de persistencia =============//
        public void GuardarTodo() {
            persistencia.Guardar(marcasDisponibles, registroVehiculos);
        }

        public void CargarTodo() {
            persistencia.Cargar(marcasDisponibles, registroVehiculos);
        }

        public string ObtenerDirectorioAct(){
            return persistencia.ObtenerDirectorioActual();
        }
        public void CambiarDirectorio(string nuevaRuta) {
            persistencia.CambiarDirectorio(nuevaRuta);
            GuardarTodo();
        }

        public void BorrarTodo() {
            persistencia.BorrarTodo();
            registroVehiculos.Clear();
        }

        //============== Delegados =============//
        public Action<Vehiculo> MostrarNormal = v => Console.WriteLine("\n[Normal]\n" + v.ToString());


        public Action<Vehiculo> MostrarConDescuento = v =>
        {
            decimal descuento = v.Precio * 0.10m;
            decimal precioDesc = v.Precio - descuento;
            Console.WriteLine($"\n[Con descuento]\nMarca: {v.Marca}, Modelo: {v.Modelo}, Año de patentamiento: {v.Patentamiento}, Precio con descuento: ${precioDesc}");
        };


        public Func<Vehiculo, string> DescripcionBreve = v => $"[Breve] {v.Marca} {v.Modelo} ({v.Patentamiento}) - ${v.Precio}";



        //==============Gestion de Marcas =============//
        public void AgregarMarca(string marcaNueva)
        {
            if (marcasDisponibles.Any(m => m.Equals(marcaNueva, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ValorInvalidoException($"La marca '{marcaNueva.Trim()}' ya se encuentra en el catálogo.");
            }

            marcasDisponibles.Add(marcaNueva);
            GuardarTodo();
            Console.WriteLine("Marca agregada correctamente");
        }



        // =============Consultar Marcas =============//
        public void MostrarMarcasDisponibles()
        {
            Console.WriteLine("\nMarcas disponibles:");
            for (int i = 0; i < marcasDisponibles.Count; i++)
                Console.WriteLine($"{i + 1}. {marcasDisponibles[i]}");
        }


        public int ObtenerCantidadMarcas() => marcasDisponibles.Count;


        public string ObtenerMarcaPorIndice(int index)
        {
            if (index >= 0 && index < marcasDisponibles.Count)
                return marcasDisponibles[index];
            else
                return "Marca inválida";
        }



        //============== Gestion de vehículos =============//
        public static Vehiculo CrearVehiculo(int tipo, string marca, string modelo, int patentamiento, decimal precio, int cilindrada, int puertas = 0, TipoManillar tipoManillar = 0, double capacidadCarga = 0)
        {
            return tipo switch
            {
                1 => new Auto(marca, modelo, patentamiento, precio, cilindrada, puertas),
                2 => new Moto(marca, modelo, patentamiento, precio, cilindrada, tipoManillar),
                3 => new Camion(marca, modelo, patentamiento, precio, cilindrada, capacidadCarga),
                _ => throw new ValorInvalidoException($"Tipo de vehículo inválido: {tipo}"),
            };
        }


        public void AgregarVehiculo(string patente, Vehiculo v, Propietario p)
        {
            if (registroVehiculos.ContainsKey(patente.Trim().ToUpper()))
            {
                Console.WriteLine("\nEl vehículo ya está registrado.");
                return;
            }

            registroVehiculos.Add(patente.Trim().ToUpper(), (v, p));
            GuardarTodo();
            Console.WriteLine("\nVehículo y propietario agregados correctamente.");
        }


        public void EliminarVehiculo(string patente)
        {
            string patenteLimpia = patente.Trim().ToUpper();

            // La eliminación es directa y eficiente por clave
            if (registroVehiculos.Remove(patenteLimpia))
            {
                GuardarTodo();
                Console.WriteLine($"\nSe eliminó el vehículo con patente {patenteLimpia}.");
            }
            else
            {
                Console.WriteLine($"\nNo se encontró un vehículo con patente {patenteLimpia}.");
            }
        }


        public Vehiculo ActualizarVehiculo(string Patente, string marca, string modelo, int nuevoPatentamiento, decimal precio, int cilindrada, int tipo, int puertas = 0, TipoManillar tipoManillar = 0, double capacidadCarga = 0)
        {
            string patenteLimpia = Patente.Trim().ToUpper();
            Vehiculo vehiculoActualizado;

            if (registroVehiculos.TryGetValue(patenteLimpia, out var registro))
            {
                Vehiculo vehiculoOriginal = registro.Vehiculo;

                if ((tipo == 0 && vehiculoOriginal is Auto) || tipo == 1)
                {
                    vehiculoActualizado = new Auto(
                        (marca == "") ? vehiculoOriginal.Marca : marca,
                        (modelo == "") ? vehiculoOriginal.Modelo : modelo,
                        (nuevoPatentamiento == 0) ? vehiculoOriginal.Patentamiento : nuevoPatentamiento,
                        (precio == 0) ? vehiculoOriginal.Precio : precio,
                        (cilindrada == 0) ? vehiculoOriginal.Cilindrada : cilindrada,
                        (puertas == 0 && vehiculoOriginal is Auto a) ? a.CantidadPuertas : puertas
                    );
                }
                else if ((tipo == 0 && vehiculoOriginal is Moto) || tipo == 2)
                {
                    vehiculoActualizado = new Moto(
                        (marca == "") ? vehiculoOriginal.Marca : marca,
                        (modelo == "") ? vehiculoOriginal.Modelo : modelo,
                        (nuevoPatentamiento == 0) ? vehiculoOriginal.Patentamiento : nuevoPatentamiento,
                        (precio == 0) ? vehiculoOriginal.Precio : precio,
                        (cilindrada == 0) ? vehiculoOriginal.Cilindrada : cilindrada,
                        (tipoManillar == 0 && vehiculoOriginal is Moto m) ? m.TipoManillar : tipoManillar
                    );
                }
                else if ((tipo == 0 && vehiculoOriginal is Camion) || tipo == 3)
                {
                    vehiculoActualizado = new Camion(
                        (marca == "") ? vehiculoOriginal.Marca : marca,
                        (modelo == "") ? vehiculoOriginal.Modelo : modelo,
                        (nuevoPatentamiento == 0) ? vehiculoOriginal.Patentamiento : nuevoPatentamiento,
                        (precio == 0) ? vehiculoOriginal.Precio : precio,
                        (cilindrada == 0) ? vehiculoOriginal.Cilindrada : cilindrada,
                        (capacidadCarga == 0 && vehiculoOriginal is Camion c) ? c.CapacidadDeCarga : capacidadCarga
                    );
                }
                else
                {
                    throw new Exception($"\nTipo de vehículo desconocido: {vehiculoOriginal.GetType().Name}. No se pudo actualizar.");
                }

                registroVehiculos[patenteLimpia] = (vehiculoActualizado!, registro.Propietario);
                GuardarTodo();
                Console.WriteLine($"\nSe actualizó el vehículo con patente {patenteLimpia}.");
                return vehiculoActualizado!;
            }

            throw new ValorInvalidoException($"\nNo se encontró un vehículo con patente {patenteLimpia}.");
        }



        //============== Consulta Vehiculos =============//
        public void MostrarVehiculos()
        {
            if (registroVehiculos.Count == 0)
            {
                Console.WriteLine("\nNo hay vehículos registrados.");
                return;
            }

            Console.WriteLine("\nListado de vehículos con propietarios:");
            int i = 1;
            foreach (var kvp in registroVehiculos)
            {
                Console.WriteLine($"\n{i}. Patente: {kvp.Key}");
                Console.WriteLine(kvp.Value.Vehiculo.ToString());
                Console.WriteLine($"   Propietario: {kvp.Value.Propietario.Nombre} ({kvp.Value.Propietario.Dni})");
                i++;
            }
        }


        public string ObtenerTipoVehiculo(string patente)
        {
            string patenteLimpia = patente.Trim().ToUpper();
            if (registroVehiculos.TryGetValue(patenteLimpia, out var registro))
            {
                Vehiculo vehiculo = registro.Vehiculo;
                return vehiculo.GetType().Name;
            }
            else
            {
                throw new ValorInvalidoException($"\nNo se encontró un vehículo con patente {patenteLimpia}.");
            }
        }


        public void OrdenarPorPrecio()
        {

            var ordenados = registroVehiculos.OrderBy(x => x.Value.Vehiculo.Precio);
            Console.WriteLine("\nVehículos ordenados por precio:");
            foreach (var kvp in ordenados)
            {
                var v = kvp.Value.Vehiculo;
                var p = kvp.Value.Propietario;

                Console.WriteLine($"{v.Marca} {v.Modelo} - ${v.Precio}");
                Console.WriteLine($"   Propietario: {p.Nombre} ({p.Dni})");
            }
        }


        public void BuscarVehiculoPorMarca(string marca)
        {
            var encontrados = registroVehiculos
                .Where(x => x.Value.Vehiculo.Marca.Equals(marca.Trim(), StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (encontrados.Count > 0)
            {
                Console.WriteLine($"\nVehículos encontrados de la marca '{marca}':");
                foreach (var kvp in encontrados)
                {
                    var v = kvp.Value.Vehiculo;
                    var p = kvp.Value.Propietario;

                    Console.WriteLine(v);
                    Console.WriteLine($"   Propietario: {p.Nombre} ({p.Dni})");
                }
            }
            else
                Console.WriteLine($"No se encontraron vehículos de la marca '{marca}'.");
        }


        public static void MostrarVehiculo(Vehiculo v, Action<Vehiculo> estrategia)
        {
            estrategia(v);
        }


        public static void MostrarVehiculoTexto(Vehiculo v, Func<Vehiculo, string> estrategia)
        {
            string resultado = estrategia(v);
            Console.WriteLine(resultado);
        }
    
    

    }
}
