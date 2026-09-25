using System.Data;
using System.Diagnostics;

using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;

namespace FlightViewer;

public partial class FlightViewer : GMap.NET.WindowsForms.GMapControl
{
  readonly AdsbService flightService = new();

  const double UsMinLat = 24.5;   // Southern border
  const double UsMaxLat = 53.0;   // Northern border
  const double UsMinLng = -125.0; // West Coast
  const double UsMaxLng = -66.5;  // East Coast

  string flightNumberFilter = "";
  string executablePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath)!;

  RectLatLng continentalUsArea;
  List<FlightData> allFlights = new();
  GMapOverlay flightsOverlay = new GMapOverlay("flights"); // the map overlay that contains the aircraft markers

  public FlightViewer()
  {
    InitializeComponent();

    base.MouseWheel += GMapControl_MouseWheel;

    base.MinZoom = 4;
    base.MaxZoom = 8;  // values appropriate for the continental US map

    continentalUsArea = RectLatLng.FromLTRB(UsMinLng, UsMaxLat, UsMaxLng, UsMinLat);

    base.Overlays.Add(flightsOverlay);

    InitializeMap();
  }

  // Copy the US map tiles included in the project to the GMap cache.
  // The cache is only created on the first run of this app.
  public void CreateTileCache()
  {
    string tileCache = System.IO.Path.Combine(executablePath, "TileDBv5", "en", "Data.gmdb");
    if (File.Exists(tileCache)) return;

    string? tileCacheFolder = System.IO.Path.GetDirectoryName(tileCache);
    Directory.CreateDirectory(tileCacheFolder!);
    string usMapTiles = System.IO.Path.Combine(executablePath, "US Map Tiles.gmdb");
    File.Copy(usMapTiles, tileCache);
  }

  void InitializeMap()
  {
    // basic properties
    base.DragButton = MouseButtons.Left;
    base.CanDragMap = true;
    base.MapProvider = OpenStreetMapProvider.Instance;
    base.Zoom = 4; // to show the entire US at startup
    base.ShowCenter = false; // hide the red crosshair in map's center

    // configure missing tiles
    base.EmptyTileText = string.Empty;
    base.FillEmptyTiles = false;
    base.EmptyTileColor = Color.Black;
    base.ShowTileGridLines = false;

    // load tiles from the local cache
    base.CacheLocation = executablePath;
    GMaps.Instance.Mode = AccessMode.CacheOnly;

    // center the map
    double midLat = (UsMinLat + UsMaxLat) / 2.0;
    double midLng = (UsMinLng + UsMaxLng) / 2.0;
    base.Position = new PointLatLng(midLat, midLng);

    // Constrain the map to US. Blank tiles along the map borders will still be visible
    base.BoundsOfMap = continentalUsArea;
  }

  public async Task<(int flightsFound, int flightsFiltered)> ShowFlightsAsync()
  {
    double usCenterLat = UsMinLat + (UsMaxLat - UsMinLat) / 2;
    double usCenterLon = UsMinLng + (UsMaxLng - UsMinLng) / 2;

    allFlights = await flightService.FetchFlightsAsync(usCenterLat, usCenterLon, radius: 1600); // covers continental US
    if (allFlights == null) return (0, 0);

    return AddFlightMarkers();
  }

  public (int flightsFound, int flightsFiltered) FilterByFlightNumber(string filter) // e.g. UA123
  {
    flightNumberFilter = filter.Trim().ToUpper();
    return AddFlightMarkers();
  }

  public (int flightsFound, int flightsFiltered) AddFlightMarkers()
  {
    var filteredFlights = GetFilteredFlights();

    // remember the currently selected marker
    MarkerFlight? currentlySelected = flightsOverlay.Markers.OfType<MarkerFlight>().Where(m => m.IsSelected).FirstOrDefault();

    flightsOverlay.Markers.Clear();

    foreach (var flight in filteredFlights)
    {
      var marker = new MarkerFlight(flight.Callsign, new PointLatLng(flight.Latitude, flight.Longitude), flight.Heading, flight.AltitudeFeet, false);
      marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;

      (string iataCode, string airlineName, string flightNumber) = AirlineCodes.GetDetailsForCallsign(flight.Callsign);
      if (iataCode == "")
        marker.ToolTipText = $"{flight.Callsign} -  Altitude:{flight.AltitudeFeet:N0} feet   Speed:{flight.VelocityKnots} knots";
      else
        marker.ToolTipText = $"{iataCode}{flightNumber} ({airlineName}) -  Altitude:{flight.AltitudeFeet:N0} feet   Speed:{flight.VelocityKnots} knots";

      // keep the previously selected marker selected
      marker.IsSelected = currentlySelected != null && currentlySelected.CallSign == marker.CallSign;

      flightsOverlay.Markers.Add(marker);
    }

    Invalidate();  // refresh the map with the markers

    return (allFlights.Count, filteredFlights.Count);
  }

  List<FlightData> GetFilteredFlights()
  {
    if (string.IsNullOrEmpty(flightNumberFilter)) return allFlights;

    var flights = allFlights.Where(f => f.IataFlightNumber.StartsWith(flightNumberFilter)).ToList();
    var callsignFlights = allFlights.Where(f => f.Callsign.StartsWith(flightNumberFilter)).ToList();
    flights.AddRange(callsignFlights);
    return flights;
  }

  protected override void OnPaint(PaintEventArgs e)
  {
    if (DesignMode)
      OnPaintBackground(e);  // to avoid painting blank tiles at designtime
    else
      base.OnPaint(e);
  }

  protected override void OnMouseDown(MouseEventArgs e)
  {
    base.OnMouseDown(e);
    if (e.Button != MouseButtons.Left) return;

    foreach (GMapOverlay overlay in Overlays)
    {
      if (!overlay.IsVisibile) continue;  // skip hidden overlays

      foreach (GMapMarker marker in overlay.Markers)
      {
        if (marker is MarkerFlight mf)
        {
          mf.IsSelected = marker.IsMouseOver;
          if (mf.IsSelected)
            Debug.WriteLine($"Selecting flight with CallSign = {mf.CallSign}");
        }
      }
    }
  }

  void GMapControl_MouseWheel(object? sender, MouseEventArgs e)
  {
    if (e.Delta > 0)
    {
      base.Zoom = Math.Min(base.MaxZoom, base.Zoom + 1);
    }
    else if (e.Delta < 0)
    {
      base.Zoom = Math.Max(base.MinZoom, base.Zoom - 1);
    }

    // Suppress further handling so default map processing doesn't double-zoom
    ((HandledMouseEventArgs)e).Handled = true;
  }
}

