using System.Net;
using System.Text.Json;

namespace FlightViewer;

/// <summary>
/// This class accesses the adsb.lol web service to retrieve flight-tracking data.
/// Adsb.lol is an open-source, community-driven platform and API that aggregates raw ADS-B, Mode S, and 
/// MLAT (Multilateration) data provided by volunteer feeders worldwide. Data is collected directly from
/// Raspberry Pi receivers and software-defined radios (SDRs) running lightweight feeder containers.
/// </summary>
public class AdsbService
{
  readonly HttpClient _httpClient;

  public AdsbService()
  {
    _httpClient = new HttpClient();
    _httpClient.BaseAddress = new Uri("https://api.adsb.lol/v2/");
  }

  public async Task<List<FlightData>> FetchFlightsAsync(double latitude, double longitude, int radius)
  {
    var flights = new List<FlightData>();

    try
    {
      _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("FlightViewer/1.0");
      var request = $"lat/{latitude}/lon/{longitude}/dist/{radius}";

      var response = await _httpClient.GetAsync(request);

      if (response.StatusCode == HttpStatusCode.TooManyRequests)  // the web service throttles requests
      {
        System.Diagnostics.Debug.WriteLine($"Rate limited (429).");
        throw new HttpRequestException($"Rate limit exceeded (HTTP 429)", null, HttpStatusCode.TooManyRequests);
      }

      response.EnsureSuccessStatusCode();

      using var stream = await response.Content.ReadAsStreamAsync();
      var data = await JsonSerializer.DeserializeAsync<AdsbLolResponse>(stream);
      if (data?.Aircraft == null) return flights;

      foreach (var aircraft in data!.Aircraft)
      {
        FlightData? flight = GetFlight(aircraft);
        if (flight == null) continue;
        flights.Add(flight);
      }
    }
    catch (HttpRequestException)
    {
      throw;
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"ADSB API Fetch Error: {ex.Message}");
      throw;
    }

    return flights;
  }

  FlightData? GetFlight(AircraftData? aircraft)
  {
    if (aircraft == null) return null;

    return new FlightData
    {
      Icao24 = aircraft.Hex,
      Callsign = aircraft.Callsign,
      Country = "",
      Longitude = aircraft.Longitude ?? 0,
      Latitude = aircraft.Latitude ?? 0,
      AltitudeFeet = aircraft.AltitudeFeet,
      VelocityKnots = aircraft.GroundSpeed ?? 0,
      Heading = aircraft.Track ?? 0,
      VerticalRate = aircraft.VerticalRate ?? 0,
      OnGround = false,
      IataFlightNumber = AirlineCodes.GetIataFlightNumber(aircraft.Callsign)
    };
  }
}