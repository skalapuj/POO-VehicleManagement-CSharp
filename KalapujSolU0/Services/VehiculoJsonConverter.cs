using KalapujSolU0.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KalapujSolU0.Services
{
    public class VehiculoJsonConverter : JsonConverter<Vehiculo>
    {
        public override Vehiculo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var jsonDoc = JsonDocument.ParseValue(ref reader);
            var root = jsonDoc.RootElement;

            if (!root.TryGetProperty("TipoVehiculo", out var tipoProp))
                throw new JsonException("No se encontró la propiedad TipoVehiculo en el JSON.");

            string tipo = tipoProp.GetString() ?? "";

            return tipo switch
            {
                "Auto" => JsonSerializer.Deserialize<Auto>(root.GetRawText(), options)!,
                "Moto" => JsonSerializer.Deserialize<Moto>(root.GetRawText(), options)!,
                "Camion" => JsonSerializer.Deserialize<Camion>(root.GetRawText(), options)!,

                _ => throw new JsonException($"Tipo de vehículo desconocido: {tipo}")
            };
        }

        public override void Write(Utf8JsonWriter writer, Vehiculo value, JsonSerializerOptions options)
        {
            string tipoReal = value switch
            {
                Auto => "Auto",
                Moto => "Moto",
                Camion => "Camion",
                _ => throw new JsonException("Tipo de vehículo no soportado al serializar.")
            };

            var json = JsonSerializer.SerializeToElement(value, value.GetType(), options);

            writer.WriteStartObject();
            writer.WriteString("TipoVehiculo", tipoReal);

            foreach (var prop in json.EnumerateObject())
            {
                prop.WriteTo(writer);
            }

            writer.WriteEndObject();
        }
    }
}
