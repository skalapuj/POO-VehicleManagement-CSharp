using System;
using System.Numerics;
using System.Runtime.Intrinsics.X86;

// === MODIFICACIONES Y NUEVOS ELEMENTOS - UNIDAD 3 ===
//
// En esta instancia se incorporaron los siguientes conceptos:
//
//  ARRAY: Se agregó un array fijo 'marcasDisponibles' con un catálogo de marcas predeterminadas.
//   Se implementaron los métodos ObtenerMarcaPorIndice(int) y ObtenerCantidadMarcas() para
//   permitir el acceso por índice, cumpliendo con el requisito de acceso directo.
//
//  DICCIONARIO: Se reemplazó la lista de vehículos por un Dictionary<Vehiculo, Propietario>
//   que permite agregar y eliminar pares vehículo–propietario en tiempo de ejecución.
//
//  ORDENAMIENTO: Se añadió el método OrdenarPorPrecio() para mostrar los vehículos
//   ordenados de manera ascendente según su precio.
//
//  BÚSQUEDA: Se implementó BuscarPorMarca(string) para localizar vehículos existentes
//   dentro del diccionario a través del nombre de la marca.
//
//  ELIMINACIÓN: Se mejoró la coherencia del modelo con EliminarVehiculo(),
//   que elimina un registro considerando ambos objetos relacionados.
//
//  MENÚ Y CONSOLA: Se actualizó la lógica en Program.cs para permitir la selección de
//   marca mediante el array, y la gestión dinámica del diccionario desde las opciones del menú.
//
// ====================================================
// === MODIFICACIONES Y NUEVOS ELEMENTOS - UNIDAD 4 (EXCEPCIONES Y REFACTORIZACIÓN) ===
//
// Manejo de Excepciones:
// 
//     Se implementó la clase de excepción personalizada **ValorInvalidoException** para manejar
//     errores específicos de la lógica de negocio (datos faltantes, formatos incorrectos,
//     valores fuera de rango, etc.).
//
//    Los métodos de entrada de datos (p. ej., PedirEntero, PedirDecimal, PedirCadena)
//     fueron actualizados para **lanzar ValorInvalidoException** al detectar input inválido,
//     centralizando la lógica de validación.
//
//     Se asumió la implementación de bloques **try/catch** en el menú principal (o punto de 
//     entrada) para capturar ValorInvalidoException y cualquier otra excepción, 
//     evitando el cierre inesperado del programa y mostrando mensajes claros al usuario.
//
// ====================================================


// === MODIFICACIONES Y NUEVOS ELEMENTOS - FINAL
//
// Refactorización y Modularidad (Funciones Auxiliares):
//
//   - Se crearon **funciones auxiliares** (PedirMarca, PedirEntero, PedirDecimal, PedirCadena,
//     GestionarActualizacion, etc.) para encapsular el patrón repetitivo de:
//     1. Solicitar datos al usuario.
//     2. Validar formato y rango (`TryParse` y comprobaciones).
//     3. Lanzar la excepción `ValorInvalidoException` si falla la validación.
//     4. Devolver el valor correcto.
//
//   - La lógica principal de creación y actualización de vehículos se hizo más **limpia y legible**
//     al reemplazar bloques grandes de código por llamadas a estas funciones modulares.
//
//   - Se utilizó una **función delegada** en GestionarActualizacion para manejar la lógica
//     de actualización de valores, permitiendo reutilizar el mismo patrón para diferentes campos
//     sin duplicar código.
//   
//   - Se agregó un nuevo vehìculo 'Camion'
//     con su respectiva clase, implementación de IMantenimiento y manejo en el menú.
//
//  - Se implementó lógica para poder actualizar un vehículo existente,
//    lo que conllevo a una restructuración general que permita identificar a lo vehículos por
//    su patente de manera unívoca.
//
// ====================================================

namespace KalapujSol {
    class Program {

        ///=============================  Funciones auxiliares ============================= 
        // Muestra un mensaje y lee una cadena de texto, validando que no esté vacía.
        static string PedirCadena(string prompt, string mensajeError)
        {
            Console.Write(prompt);
            string input = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(input.Trim()))
                throw new ValorInvalidoException(mensajeError);

            return input.Trim();
        }


        /// Muestra un mensaje y lee un entero, validando su formato y un rango opcional.
        static int PedirEntero(string prompt, string mensajeError, int min = int.MinValue, int max = int.MaxValue)
        {
            Console.Write(prompt);
            if (!int.TryParse(Console.ReadLine(), out int valor))
                throw new ValorInvalidoException(mensajeError);

            if (valor < min || valor > max)
                throw new ValorInvalidoException($"El valor debe estar entre {min} y {max}.");

            return valor;
        }


        /// Muestra un mensaje y lee un decimal, validando su formato.
        static decimal PedirDecimal(string prompt, string mensajeError)
        {
            Console.Write(prompt);
            if (!decimal.TryParse(Console.ReadLine(), out decimal valor))
                throw new ValorInvalidoException(mensajeError);

            return valor;
        }


        /// Muestra un mensaje y lee un double, validando su formato.
        static double PedirDouble(string prompt, string mensajeError)
        {
            Console.Write(prompt);
            if (!double.TryParse(Console.ReadLine(), out double valor))
                throw new ValorInvalidoException(mensajeError);

            return valor;
        }


        /// Solicita al usuario que seleccione una marca de la lista del gestor.
        static string PedirMarca(GestorVehiculos gestor) 
        {
            gestor.MostrarMarcasDisponibles();
            Console.Write("\nSeleccione una marca por número: ");

            if (int.TryParse(Console.ReadLine(), out int indice) &&
                indice >= 1 && indice <= gestor.ObtenerCantidadMarcas())
            {
                string marca = gestor.ObtenerMarcaPorIndice(indice - 1);
                Console.WriteLine($"Marca seleccionada: {marca}");
                return marca;
            }
            else
            {
                throw new ValorInvalidoException("La marca seleccionada no existe en el catálogo.");
            }
        }


        /// Solicita al usuario que seleccione un tipo de vehículo.
        static int PedirTipoVehiculo()
        {
            int option;
            
            while (true)
            {
                Console.WriteLine("\nElija el tipo de vehículo a instanciar:");
                Console.WriteLine("1 - Auto");
                Console.WriteLine("2 - Moto");
                Console.WriteLine("3 - Camion");
                Console.Write("Opción: ");

                if (int.TryParse(Console.ReadLine(), out option) && (option >= 1 && option <= 3))
                {
                    return option; 
                }
                else
                {
                    Console.WriteLine("Opción inválida. Por favor, ingrese un número entre 1 y 3.\n");
                }
            }
        }
        
        
        /// Solicita al usuario que ingrese y valide el tipo de manillar para una moto.
        static TipoManillar PedirTipoManillar()
        {
            int op;
            while (true)
            {
                Console.WriteLine("Selecciona el tipo de manillar:");
                Console.WriteLine("  1. Recto");
                Console.WriteLine("  2. Curvo");
                Console.WriteLine("  3. Deportivo");
                Console.Write("Ingrese una opción (1-3): ");

                if (int.TryParse(Console.ReadLine(), out op) &&
                    Enum.IsDefined(typeof(TipoManillar), op))
                {
                    return (TipoManillar)(op);
                }
                else
                {
                    Console.WriteLine("Opción inválida. Por favor, ingrese un número entre 1 y 3.\n");
                }
            }
        }


        /// Solicita la patente y valida su longitud.
        static string PedirPatente(string prompt = "Patente: ")
        {
            string patente = PedirCadena(prompt, "Debe ingresar una patente válida.");

            if (patente.Trim().Length < 6 || patente.Trim().Length > 7)
                throw new ValorInvalidoException("La patente debe tener entre 6 y 7 caracteres.");

            return patente;
        }


        /// Gestiona la actualización de un valor mediante una función delegada.
        static string GestionarActualizacion(string prompt, Func<string> obtenerNuevoValor)
        {
            string opt = "";
            string nuevoValor = "";

            while (true) 
            {
                opt = PedirCadena(prompt + " (Y o N): ", "Opción inválida");
                opt = opt.Trim().ToUpper();

                if (opt == "Y")
                {
                    // Ejecuto la función delegada para obtener el nuevo valor.
                    // Si la función interna lanza una excepción (ej: ValorInvalidoException),
                    // el bucle de arriba lo capturará, pero aquí solo se ejecuta.
                    nuevoValor = obtenerNuevoValor();
                    break;
                }
                else if (opt == "N")
                {
                    nuevoValor = ""; 
                    break;
                }
                else
                {
                    Console.WriteLine("Opción inválida. Por favor, ingrese 'Y' o 'N'.");
                }
            }
            return nuevoValor;
        }



        //============================= PUNTO DE ENTRADA ========================
        static void Main(string[] args) {
            var gestor = new GestorVehiculos();
            bool salir = false;

            Console.WriteLine("=== Proyecto POO de Sol Kalapuj: Sistema de Vehículos ===\n");

            while (!salir)
            {
                try
                {
                    Console.WriteLine("\n=== MENÚ PRINCIPAL ===");
                    Console.WriteLine("1 - Ver marcas disponibles");
                    Console.WriteLine("2 - Agregar Marca");
                    Console.WriteLine("3 - Agregar vehículo a la lista");
                    Console.WriteLine("4 - Mostrar todos los vehículos");
                    Console.WriteLine("5 - Ordenar vehículos por precio");
                    Console.WriteLine("6 - Buscar vehículo por marca");
                    Console.WriteLine("7 - Actualizar vehiculo creado");
                    Console.WriteLine("8 - Eliminar vehículo");
                    Console.WriteLine("0 - Salir");
                    Console.Write("Opción: ");
                    string opcion = Console.ReadLine() ?? "opcion ausente";

                    switch (opcion)
                    {
                        case "1":
                            gestor.MostrarMarcasDisponibles();
                            break;

                        case "2":
                            string marcaNueva = PedirCadena("Ingrese una nueva Marca: ", "La marca nueva debe contener un valor para poder ser agregada en el catálogo."); 
                            gestor.AgregarMarca(marcaNueva);
                            break;

                        case "3":
                            // Pido datos base del vehículo
                            string marca = PedirMarca(gestor);
                            string modelo = PedirCadena("Modelo: ", "Debe ingresar un modelo válido.");
                            int patentamiento = PedirEntero("Año de patentamiento: ", "El año de patentamiento debe ser un número entero."); 
                            decimal precio = PedirDecimal("Precio: ", "El precio debe ser numérico.");
                            int cilindrada = PedirEntero("Cilindrada (cc): ", "La cilindrada debe ser un número entero.");

                            // Creo el vehículo vacio                          
                            Vehiculo vehiculoCreado;
                            int option = PedirTipoVehiculo();

                            if (option == 1) 
                            {
                                int puertas = PedirEntero("Cantidad de puertas: ", "La cantidad de puertas debe ser numérica.");
                                vehiculoCreado = gestor.CrearVehiculo(option, marca, modelo, patentamiento, precio, cilindrada, puertas);
                            }
                            else if (option == 2) 
                            {
                                TipoManillar tipoManillar = PedirTipoManillar(); 
                                vehiculoCreado = gestor.CrearVehiculo(option, marca, modelo, patentamiento, precio, cilindrada, 0, tipoManillar);
                            }
                            else if (option == 3) 
                            {
                                double carga = PedirDouble("Capacidad de carga: ", "La capacidad de carga debe ser numérica.");
                                vehiculoCreado = gestor.CrearVehiculo(option, marca, modelo, patentamiento, precio, cilindrada, 0, 0, carga);
                            }
                            else
                            {
                                throw new Exception("Error inesperado en la selección de tipo de vehículo."); 
                            }

                            // Pido datos base del propietario y lo creo
                            string nombreProp = PedirCadena("Nombre del propietario: ", "Debe ingresar un nombre válido.");
                            string dni = PedirCadena("DNI: ", "Debe ingresar un DNI válido.");
                            Propietario propietario = new Propietario(nombreProp, dni);



                            // Muestro la info usando ToString sobrescrito
                            Console.WriteLine("\nObjeto creado:");
                            Console.WriteLine(vehiculoCreado.ToString());
                            Console.WriteLine(propietario.ToString());


                            // Ejecuto implementacion de IMantenimiento
                            if (vehiculoCreado is IMantenimiento m)
                            {
                                Console.WriteLine("\nEjecutando mantenimiento (interfaz):");
                                Console.WriteLine(m.RealizarMantenimiento());

                                Console.WriteLine($"Costo estimado: ${m.CalcularCostoMantenimiento()}");
                            }


                            // Menú para usar los delegados
                            Console.WriteLine("\nElija cómo quiere ver la información:");
                            Console.WriteLine("1 - Normal (Action)");
                            Console.WriteLine("2 - Con descuento (Action)");
                            Console.WriteLine("3 - Descripción breve (Func)");
                            string modo = PedirCadena("Opción: ", "Opción inválida.");

                            if (modo == "1")
                                gestor.MostrarVehiculo(vehiculoCreado, gestor.MostrarNormal);
                            else
                            if (modo == "2")
                                gestor.MostrarVehiculo(vehiculoCreado, gestor.MostrarConDescuento);
                            else
                            if (modo == "3")
                                gestor.MostrarVehiculoTexto(vehiculoCreado, gestor.DescripcionBreve);
                            else
                                Console.WriteLine("Opción inválida.");

                            string patente = PedirPatente();
                            Console.WriteLine($"Patente ingresada: {patente}");

                            gestor.AgregarVehiculo(patente, vehiculoCreado, propietario);
                            break;

                        case "4":
                            gestor.MostrarVehiculos();
                            break;

                        case "5":
                            gestor.OrdenarPorPrecio();
                            break;

                        case "6":
                            Console.Write("Ingrese marca a buscar: ");
                            string buscada = Console.ReadLine() ?? "marca a buscar ausente"; 
                            gestor.BuscarVehiculoPorMarca(buscada);
                            break;

                        case "7":
                            int newAnioPatentamiento, newCilindrada, newTipoVehiculo, newPuertas = 0;
                            double newCarga = 0;
                            TipoManillar newtipoManillar = 0;
                            decimal newPrecio;
                            string newMarca, newModelo, stringAux;
                            string patenteAct = PedirPatente();

                            newMarca = GestionarActualizacion(
                                "¿Desea actualizar la marca?",
                                () => PedirMarca(gestor) 
                            );

                            newModelo = GestionarActualizacion(
                                "¿Desea actualizar el modelo?",
                                () => PedirCadena("Modelo: ", "Debe ingresar un modelo válido.")
                            );

                            stringAux = GestionarActualizacion(
                                "¿Desea actualizar el año de patentamiento?",
                                () => PedirEntero("Año de patentamiento: ", "El año debe ser entero.").ToString()
                            );
                            newAnioPatentamiento = string.IsNullOrEmpty(stringAux) ? 0 : int.Parse(stringAux);

                            stringAux = GestionarActualizacion(
                                 "¿Desea actualizar el precio?",
                                 () => PedirDecimal("Precio: ", "El precio debe ser numérico.").ToString()
                             );
                            newPrecio = string.IsNullOrEmpty(stringAux) ? 0m : decimal.Parse(stringAux);

                            stringAux = GestionarActualizacion(
                                "¿Desea actualizar la cilindrada?",
                                () => PedirEntero("Cilindrada (cc): ", "La cilindrada debe ser un número entero.").ToString()
                            );
                            newCilindrada = string.IsNullOrEmpty(stringAux) ? 0 : int.Parse(stringAux);

                            stringAux = GestionarActualizacion(
                                "¿Desea actualizar el tipo de vehiculo?",
                                () => PedirTipoVehiculo().ToString()
                            );
                            newTipoVehiculo = string.IsNullOrEmpty(stringAux) ? 0 : int.Parse(stringAux);

                            if (newTipoVehiculo == 1)
                                newPuertas = PedirEntero("Cantidad de puertas: ", "La cantidad de puertas debe ser numérica.");
                            else if (newTipoVehiculo == 2)
                                newtipoManillar = PedirTipoManillar();
                            else if (newTipoVehiculo == 3)
                                newCarga = PedirDouble("Capacidad de carga: ", "La capacidad de carga debe ser numérica.");
                            else
                            {
                                stringAux = gestor.ObtenerTipoVehiculo(patenteAct);

                                if (stringAux == "Auto")
                                {
                                    stringAux = GestionarActualizacion(
                                        "¿Desea actualizar la cantidad de puertas?",
                                        () => PedirEntero("Cantidad de puertas: ", "La cantidad de puertas debe ser numérica.").ToString()
                                    );
                                    newPuertas = string.IsNullOrEmpty(stringAux) ? 0 : int.Parse(stringAux);
                                }
                                else if (stringAux == "Moto")
                                {
                                    stringAux = GestionarActualizacion(
                                        "¿Desea actualizar el tipo de manillar?",
                                        () => PedirTipoManillar().ToString()
                                    );
                                    newtipoManillar = (TipoManillar)(string.IsNullOrEmpty(stringAux) ? 0 : int.Parse(stringAux));
                                }
                                else if (stringAux == "Camion")
                                {
                                    stringAux = GestionarActualizacion(
                                        "¿Desea actualizar la capacidad de carga?",
                                        () => PedirDouble("Capacidad de carga: ", "La capacidad de carga debe ser numérica.").ToString()
                                    );
                                    newCarga = string.IsNullOrEmpty(stringAux) ? 0d : double.Parse(stringAux);
                                }
                            }

                            Vehiculo vehiculoAct = gestor.ActualizarVehiculo(patenteAct, newMarca, newModelo, newAnioPatentamiento, newPrecio, newCilindrada, newTipoVehiculo, newPuertas, newtipoManillar, newCarga);
                            
                            // Muestro la info usando ToString sobrescrito
                            Console.WriteLine("\nObjeto actualizado:");
                            Console.WriteLine(vehiculoAct.ToString());

                            break;

                        case "8":
                            string patenteDel = PedirPatente();

                            gestor.EliminarVehiculo(patenteDel);
                            break;

                        case "0":
                            salir = true;
                            break;

                        default:
                            Console.WriteLine("Opción inválida.");
                            break;

                    }
                }
                catch (ValorInvalidoException ex)
                {
                    Console.WriteLine($"\n[ERROR DE VALIDACIÓN]: {ex.Message}\n");
                }
                catch (FormatException)
                {
                    Console.WriteLine("\n[ERROR]: Ingresaste un valor con formato inválido.\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n[ERROR NO CONTROLADO]: {ex.Message}\n");
                }
            }

            Console.WriteLine("\nPrograma finalizado. ¡Gracias por usar el sistema!");
        }
    }
}