using System.Text.Json.Serialization;

namespace FlightViewer;

public class FlightData
{
  public string Icao24 { get; set; } = string.Empty;
  public string Callsign { get; set; } = string.Empty;   // e.g. UAL123
  public string Country { get; set; } = string.Empty;
  public double Longitude { get; set; }
  public double Latitude { get; set; }
  public double AltitudeFeet { get; set; }
  public double VelocityKnots { get; set; }
  public double Heading { get; set; }
  public double VerticalRate { get; set; }
  public bool OnGround { get; set; }
  public string IataFlightNumber { get; set; } = string.Empty;  // e.g. UA123
  public string AirlineCode => Callsign.Length >= 3 ? Callsign[..3].Trim() : "N/A";
}


///////////////////////////////////// AdsbLol API Response Classes////////////////////////////////////

public class AdsbLolResponse
{
  [JsonPropertyName("ac")]
  public List<AircraftData>? Aircraft { get; set; }

  [JsonPropertyName("ctime")]
  public long ServerTime { get; set; }

  [JsonPropertyName("total")]
  public int TotalCount { get; set; }
}

public class AircraftData
{
  [JsonPropertyName("hex")]
  public string Hex { get; set; } = string.Empty;

  [JsonPropertyName("flight")]
  public string? Flight { get; set; } // Callsign

  [JsonPropertyName("lat")]
  public double? Latitude { get; set; }

  [JsonPropertyName("lon")]
  public double? Longitude { get; set; }

  [JsonPropertyName("alt_baro")]
  public object? AltBaro { get; set; } // Can be integer altitude in feet or "ground"

  [JsonPropertyName("gs")]
  public double? GroundSpeed { get; set; } // Knots

  [JsonPropertyName("track")]
  public double? Track { get; set; } // Heading in degrees

  [JsonPropertyName("baro_rate")]
  public double? VerticalRate { get; set; } // Feet per minute

  [JsonPropertyName("squawk")]
  public string? Squawk { get; set; }

  public string Callsign => Flight?.Trim() ?? "N/A";

  public double AltitudeFeet
  {
    get
    {
      if (AltBaro is System.Text.Json.JsonElement element)
      {
        if (element.ValueKind == System.Text.Json.JsonValueKind.Number)
          return element.GetDouble();
      }
      return 0;
    }
  }
}
