namespace FlightViewer;

internal class AirlineCodes
{
  /// <summary>
  /// Looks up IATA codes from ICAO codes.
  /// ICAO is the International Civil Aviation Organization.
  /// IATA is the International Air Transport Association. IATA code are the commercial
  /// flight designations most air travelers are familiar with.
  /// Dictionary definition:
  ///   Key is ICAO code (also called CallSign).
  //    Value is a tuple with the IATA code and the airline name.
  /// </summary>

  static readonly Dictionary<string, (string iataCode, string airlineName)> IcaoToIataMap = 
      new Dictionary<string, (string iataCode, string airlineName)>()
  {
      { "AEE", ("A3", "Aegean Airlines") },
      { "AAL", ("AA", "American Airlines") },
      { "AAR", ("OZ", "Asiana Airlines") },
      { "ACA", ("AC", "Air Canada") },
      { "AFL", ("SU", "Aeroflot") },
      { "AFR", ("AF", "Air France") },
      { "AIC", ("AI", "Air India") },
      { "AMF", ("A2", "Ameriflight") },
      { "AMX", ("AM", "Aeroméxico") },
      { "ANA", ("NH", "All Nippon Airways") },
      { "APA", ("5Y", "Atlas Air") },
      { "ASL", ("AS", "Alaska Airlines") },
      { "ATN", ("8C", "Air Transport International") },
      { "AUA", ("OS", "Austrian Airlines") },
      { "BAW", ("BA", "British Airways") },
      { "CAL", ("CI", "China Airlines") },
      { "CCA", ("CA", "Air China") },
      { "CES", ("MU", "China Eastern Airlines") },
      { "CMT", ("3C", "Corporate Air") },
      { "CPA", ("CX", "Cathay Pacific") },
      { "CSN", ("CZ", "China Southern Airlines") },
      { "DAL", ("DL", "Delta Air Lines") },
      { "DLH", ("LH", "Lufthansa") },
      { "ELY", ("LY", "El Al") },
      { "ETD", ("EY", "Etihad Airways") },
      { "EVA", ("BR", "EVA Air") },
      { "EZY", ("U2", "easyJet") },
      { "FDX", ("FX", "FedEx Express") },
      { "FFT", ("F9", "Frontier Airlines") },
      { "GIA", ("GA", "Garuda Indonesia") },
      { "GTI", ("5Y", "Atlas Air Worldwide") },
      { "IBE", ("IB", "Iberia") },
      { "IND", ("6E", "IndiGo") },
      { "JAL", ("JL", "Japan Airlines") },
      { "JBU", ("B6", "JetBlue Airways") },
      { "KAL", ("KE", "Korean Air") },
      { "KLM", ("KL", "KLM Royal Dutch Airlines") },
      { "MAS", ("MH", "Malaysia Airlines") },
      { "MRA", ("M6", "Amerijet International") },
      { "NLA", ("5Y", "Polar Air Cargo") },
      { "NKS", ("NK", "Spirit Airlines") },
      { "NPT", ("N5", "National Air Cargo") },
      { "PAC", ("PO", "Polar Air Cargo") },
      { "QFA", ("QF", "Qantas") },
      { "QTR", ("QR", "Qatar Airways") },
      { "RSL", ("M6", "Amerijet Cargo") },
      { "RYR", ("FR", "Ryanair") },
      { "SAS", ("SK", "Scandinavian Airlines") },
      { "SIA", ("SQ", "Singapore Airlines") },
      { "SOO", ("9S", "Southern Air") },
      { "SUD", ("SV", "Saudia") },
      { "SWA", ("WN", "Southwest Airlines") },
      { "SWR", ("LX", "Swiss International Air Lines") },
      { "TAP", ("TP", "TAP Air Portugal") },
      { "THY", ("TK", "Turkish Airlines") },
      { "UAE", ("EK", "Emirates") },
      { "UAL", ("UA", "United Airlines") },
      { "UPS", ("5X", "UPS Airlines") },
      { "USA", ("1U", "USA Jet Airlines") },
      { "VLG", ("VY", "Vueling") },
      { "VOI", ("Y4", "Volaris") },
      { "VOZ", ("VA", "Virgin Australia") },
      { "WGA", ("KD", "Western Global Airlines") },
      { "WJA", ("WS", "WestJet") },
      { "WZZ", ("W6", "Wizz Air") }
  };

  /// <summary>
  /// The callsign is a three-letter airline ICAO code, followed by the 3 or 4-digit flight number.
  /// The returned IATA code is the 2-letter airline code, e.g. UA for United Airlines, AA for 
  /// American Airlines, etc.
  /// </summary>
  public static (string iataCode, string airlineName, string flightNumber) GetDetailsForCallsign(string callsign)
  {
    if (string.IsNullOrWhiteSpace(callsign)) return ("", "", "");

    callsign = callsign.Trim();

    if (callsign.Length >= 6)
    {
      string icaoCode = callsign.Substring(0, 3);
      string flightNumber = callsign.Substring(3);

      if (!IcaoToIataMap.ContainsKey(icaoCode)) return (string.Empty, string.Empty, string.Empty);
      (string iataCode, string airlineName) = IcaoToIataMap[icaoCode];
      return (iataCode, airlineName, flightNumber);
    }

    return (string.Empty, string.Empty, string.Empty);
  }

  /// <summary>
  /// Looks up the IATA code for an ICAO Callsign
  /// </summary>
  public static string GetIataFlightNumber(string callsign)
  {
    (string iataCode, string airlineName, string flightNumber) = GetDetailsForCallsign(callsign);
    if (iataCode == string.Empty) return string.Empty;

    return $"{iataCode}{flightNumber}";
  }
}
