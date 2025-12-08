using KalapujSolU0.Exceptions;
using KalapujSolU0.Models;
using System.Text.Json;

namespace KalapujSolU0.Services
{

    public class Persistencia
    {
        private readonly string archivoConfig = "config.txt";
        private readonly string archivoDatos = "datos.json";
        private string directorioActual;

        private static readonly JsonSerializerOptions opcionesSerializacion = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            IncludeFields = true
        };

        private static readonly JsonSerializerOptions opcionesDeserializacion = new()
        {
            PropertyNameCaseInsensitive = true,
            IncludeFields = true,
            AllowTrailingCommas = true
        };

        public Persistencia()
        {
            //Directorio base
            directorioActual = CargarDirectorioConfig();
            AsegurarDirectorio(directorioActual);

            //Serializacion y deserializacion de Vehiculo
            opcionesSerializacion.Converters.Add(new VehiculoJsonConverter());
            opcionesDeserializacion.Converters.Add(new VehiculoJsonConverter());
        }

        public void Guardar(List<string> marcas, Dictionary<string, (Vehiculo Vehiculo, Propietario Propietario)> vehiculos)
        {
            try
            {
                var lista = new List<Registro>();

                foreach (var elemento in vehiculos)
                {
                    lista.Add(new Registro(elemento.Key, elemento.Value.Vehiculo, elemento.Value.Propietario));
                }

                DatosCompletosDTO datos = new() { Marcas = marcas, Registros = lista };

                string path = Path.Combine(directorioActual, archivoDatos);

                string json = JsonSerializer.Serialize(datos, opcionesSerializacion);
                File.WriteAllText(path, json);

                Console.WriteLine("\nDatos guardados correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar: {ex.Message}");
            }
        }

        public void Cargar(List<string> marcas, Dictionary<string, (Vehiculo Vehiculo, Propietario Propietario)> vehiculos)
        {
            try
            {
                string ruta = Path.Combine(directorioActual, archivoDatos);

                if (!File.Exists(ruta))
                {
                    Console.WriteLine($"No hay archivos para guardados. Se cargan los atributos por defecto.");
                    return;
                }

                string json = File.ReadAllText(ruta);

                DatosCompletosDTO? datos = JsonSerializer.Deserialize<DatosCompletosDTO>(json, opcionesDeserializacion);

                if (datos != null)
                {
                    marcas.Clear();
                    foreach (var marca in datos.Marcas)
                    {
                        marcas.Add(marca);
                    }

                    vehiculos.Clear();
                    foreach (var registro in datos.Registros)
                    {
                        vehiculos[registro.Patente] = (registro.Vehiculo, registro.Propietario);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar: {ex.Message}");
            }
        }


        public void CambiarDirectorio(string nuevaRuta)
        {
            try
            {
                if (!Directory.Exists(nuevaRuta))
                {
                    throw new ValorInvalidoException("La ruta inexistente o invalida.");
                }

                directorioActual = nuevaRuta;
                File.WriteAllText(archivoConfig, nuevaRuta);

                Console.WriteLine($"Directorio cambiado a: {nuevaRuta}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"No se pudo cambiar: {ex.Message}");
            }
        }


        public string ObtenerDirectorioActual() => directorioActual;


        private string CargarDirectorioConfig()
        {
            try
            {
                if (!File.Exists(archivoConfig))
                {
                    string defecto = Path.Combine(Environment.CurrentDirectory, "Datos");
                    File.WriteAllText(archivoConfig, defecto);
                    return defecto;
                }

                string ruta = File.ReadAllText(archivoConfig).Trim();
                if (string.IsNullOrWhiteSpace(ruta))
                {
                    string defecto = Path.Combine(Environment.CurrentDirectory, "Datos");
                    File.WriteAllText(archivoConfig, defecto);
                    return defecto;
                }

                return ruta;
            }
            catch
            {
                return Path.Combine(Environment.CurrentDirectory, "Datos");
            }
        }

        private static void AsegurarDirectorio(string ruta)
        {
            if (!Directory.Exists(ruta))
                Directory.CreateDirectory(ruta);
        }

        public void BorrarTodo()
        {
            try
            {
                if (Directory.Exists(directorioActual))
                {
                    Directory.Delete(directorioActual, true);
                }

                AsegurarDirectorio(directorioActual);

                Console.WriteLine("Datos eliminados exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"No se pudo borrar: {ex.Message}");
            }
        }
    }

    
    public class Registro(string patente, Vehiculo vehiculo, Propietario propietario)
    {
        public string Patente { get; set; } = patente;
        public Vehiculo Vehiculo { get; set; } = vehiculo;
        public Propietario Propietario { get; set; } = propietario;
    }

    // DTO (Data Transfer Object) para serializar correctamente 
    public class DatosCompletosDTO
    {
        public List<string> Marcas { get; set; } = [];
        public List<Registro> Registros { get; set; } = [];
    }
}
