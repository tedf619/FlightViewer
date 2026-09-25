# FlightViewer a .NET 10 WinForms Flight Tracker

## Requirements
- Uses the adsb.lol web service for realtime flight information.
- Uses GMap.NET to display flights on an OpenStreetMap of the continental US.

FlightViewer is a desktop app that uses a single Form to host a WinForms UserControl that shows aircraft over a map of the US. The app is largely based on my project MapViewer.
The aircraft information is obtained from adsb.lol. The Top Level Domain ".lol" designates the service as a community-supported site, as opposed to a commercial one.
The following figure shows the app in action.

<img width="884" height="540" alt="image" src="https://github.com/user-attachments/assets/e405d338-6218-4c86-91cc-90f18d78d503" />

*Figure 1* - FlightViewer showing the large number of flights over the US.

Planes are colored according to their altitude, as described in the color panel above the map. The flights refresh every 30 seconds. 
If you try to refresh the flights faster than that, the web service may throttle you by returning HTTP Status 429 *Too Many Requests*. 
The rightmost field in the status bar shows the countdown to the next refresh.
<br>
Given the large number of flights, the map can get super-cluttered so the app lets you apply filtering. The status bar shows how many flights
were filtered out of the full list. The following figure shows the flights filtered by airline.

<img width="884" height="540" alt="image" src="https://github.com/user-attachments/assets/4c483309-6411-4548-938a-dc4de9fa53c3" />

*Figure 2* - Tracking all flights by United Airlines.

By specifying a full flight number you can track a specific plane, as shown in the next figure.

<img width="884" height="540" alt="image" src="https://github.com/user-attachments/assets/02656273-0d22-4929-9e9d-47be254f0122" />

*Figure 3* - Tracking a specific flight.

You can use the mouse to zoom and pan the map. The following figure shows a view of the New York area.

<img width="884" height="540" alt="image" src="https://github.com/user-attachments/assets/e935530e-9f3c-4558-ad32-1fe1353f9d35" />

*Figure 4* - Zooming in on the New York City area.

Mousing over an aircraft will show a tooltip with flight information, as seen in the next figure.

<img width="884" height="540" alt="image" src="https://github.com/user-attachments/assets/ea8dfa88-631c-4781-b50e-1c21c367a649" />

*Figure 5* - Tooltips providing flight identification data.

## UI Layout

FlightViewer uses a few docked controls to achieve the layout, as shown in the next figure.

<img width="779" height="540" alt="image" src="https://github.com/user-attachments/assets/478ec608-21a3-468b-b988-36d978b2440a" />

*Figure 6* - Laying out the UI with docked controls.

To achieve this layout, the controls must be added in the correct back-to-front order:

1. Panel Controls. This becomes the backmost control.
2. Panel Legend.
3. Panel StatusBar.
4. UserControl FlightViewer. This is the frontmost control.

Controls with Dock=Fill must always be added last to their host control, in order for them to fill the available space.

## How it Works

FormMain is very simple, handling mostly the layout of the controls. The heavy lifting is performed by the FlightViewer control, which hosts the map. FlightViewer is derived from GMap.Net.GMapControl,
which in turn is derived from UserControl, as shown in the next figure.

<img width="114" height="327" alt="image" src="https://github.com/user-attachments/assets/1012288c-fffc-4906-acfb-ca52111c87cf" />

*Figure 7* - The class hierarchy of FlightViewer.

GMapControl displays an OpenStreetMap and supports any number of overlays, which are designed to hold *markers*. I use one overlay for the flight markers. A marker is an object that describes a point on the map, and can contain text and graphics objects. FlightViewer uses a marker for each aircraft, with tooltips to show aircraft details. Markers are described in more detail later.

### Setting up the Map
The US map is contained in the project file "US Map Tiles.gmdb", which is configured to be copied to the output directory.

<img width="597" height="488" alt="image" src="https://github.com/user-attachments/assets/beecc091-4854-45bb-acd7-6ebbe85e0ba5" />

*Figure 8* - Copying the map to the output directory.

The file was downloaded using GMap.Net. See my project MapViewer for details. Having the file in the output directory isn't sufficient because GMap.NET looks for the file

```<..\FlightViewer>\bin\Debug\net10.0-windows\TileDBv5\en\Data.gmdb```

When the app starts up, we check to see if this file is already there. If not, we copy "US Map Tiles.gmdb" from the output directory to the TileDBv5\en directory and rename the file. 
Here is the code:

```csharp
public partial class FormMain : Form
{
  //...

  public FormMain()
  {
    InitializeComponent();

    flightViewer.CreateTileCache(); // copies our map tiles to a local cache for GMap to access
  }
}

public partial class FlightViewer : GMap.NET.WindowsForms.GMapControl
{
  //...

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
  //...
}
```
*Listing 1* - Creating the map file at startup time.

With the file in place, we need to setup GMapControl to display it properly. Here is the code:

```csharp
public partial class FlightViewer : GMap.NET.WindowsForms.GMapControl
{
  const double UsMinLat = 24.5;   // Southern border
  const double UsMaxLat = 53.0;   // Northern border
  const double UsMinLng = -125.0; // West Coast
  const double UsMaxLng = -66.5;  // East Coast

  string executablePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath)!;
  RectLatLng continentalUsArea;
 
  public FlightViewer()
  {
    //...

    base.MinZoom = 4;
    base.MaxZoom = 8;  // values appropriate for the continental US map

    continentalUsArea = RectLatLng.FromLTRB(UsMinLng, UsMaxLat, UsMaxLng, UsMinLat);

    InitializeMap();
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
  //...
}
```
*Listing 2* - Initializing GMapControl.

GMapControl can operate in three different modes:

  * *ServerOnly*. Retrieves map file from server.
  * *ServerUpdate*. Retrieves file from server and saves to local cache.
  * *Cache Only*. Retrieves file from local cache. Server not needed.

FlightViewer has the map file locally, so it uses the CacheOnly mode. The area shown by the map is set with the code

```  base.BoundsOfMap = continentalUsArea;```

The Zoom level is initially set to 4, which is appropriate to show the entire continental US. At runtime you can use the mouse wheel to zoom in further.
The max level is set to 8. OpenStreetMap supports much higher levels, but those correspond to exponentially larger tile files. A Zoom level of 8 generates a file over 60 MB.
A Zoom level of 9 would generate a file over 120 MB, which is over the max size supported by ordinary GitHub projects.
If you want to try Zoom levels over 8, use the MapViewer instructions on downloading the tile file.

## Showing Flights

The app uses a 30-second timer in FormMain to retrieve flight information, like this:

```csharp
public partial class FormMain : Form
{
  const int RefreshIntervalSeconds = 30;
  int refreshCountdown = RefreshIntervalSeconds;

  void TimerRefreshCountdown_Tick(object sender, EventArgs e)
  {
    refreshCountdown--;
    if (refreshCountdown > 0) return;

    refreshCountdown = RefreshIntervalSeconds;

    ShowFlightsAsync();
  }

  async void ShowFlightsAsync()
  {
    try
    {
      (int flightsFound, int flightsFiltered) = await flightViewer.ShowFlightsAsync();

      labelStatusBarMessage.Text = $"Data updated at {DateTime.Now:T}";
      labelStatusBarFlights.Text = $"Flights: {flightsFiltered} shown of total {flightsFound}";
    }
    catch (Exception ex)
    {
      labelStatusBarMessage.Text = $"Exception: {ex.Message}";
    }
  }
```

*Listing 3* - Scheduling the flight refresh every 30 seconds.

The retrieval of flight data is handled by the FlightViewer control like this:

```csharp
public partial class FlightViewer : GMap.NET.WindowsForms.GMapControl
{
  RectLatLng continentalUsArea;
  List<FlightData> allFlights = new();

public async Task<(int flightsFound, int flightsFiltered)> ShowFlightsAsync()
{
  double usCenterLat = UsMinLat + (UsMaxLat - UsMinLat) / 2;
  double usCenterLon = UsMinLng + (UsMaxLng - UsMinLng) / 2;

  allFlights = await flightService.FetchFlightsAsync(usCenterLat, usCenterLon, radius: 1600); // covers continental US
  if (allFlights == null) return (0, 0);

  return AddFlightMarkers();
}
```
*Listing 4* - Invoking the retrieval of flight data.

There are several web services that provide realtime flight tracking data but most of them require you to register for an API key.
One service that doesn't require keys is adsb.lol, a community-supported service with data provided by volunteer feeders worldwide.
Data is collected directly from Raspberry Pi receivers and software-defined radios (SDRs) running lightweight feeder containers.
To get the flight data from adsb.lol, I use class AdsbService, implemented like this:

```csharp
public class AdsbService
{
  readonly HttpClient httpClient;

  public AdsbService()
  {
    httpClient = new HttpClient();
    httpClient.BaseAddress = new Uri("https://api.adsb.lol/v2/");
  }

  public async Task<List<FlightData>> FetchFlightsAsync(double latitude, double longitude, int radius)
  {
    var flights = new List<FlightData>();

    httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("FlightViewer/1.0");

    var request = $"lat/{latitude}/lon/{longitude}/dist/{radius}";
    var response = await httpClient.GetAsync(request);

    using var stream = await response.Content.ReadAsStreamAsync();
    var data = await JsonSerializer.DeserializeAsync<AdsbLolResponse>(stream);
    if (data?.Aircraft == null) return flights;

    foreach (var aircraft in data!.Aircraft)
    {
      FlightData? flight = GetFlight(aircraft);
      if (flight == null) continue;
      flights.Add(flight);
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
```
*Listing 5* - Retrieving and parsing flight data from the adsb.lol web service.

## Drawing Aircraft

As stated earlier, flights are displayed as markers on a map overlay. I used class MarkerFlight for this purpose:

```csharp
internal class MarkerFlight : GMapMarker
{
  public string CallSign { get; set; }  // e.g. DAL120
  public float Heading { get; set; }    // angle in degrees(0 = North, 90 = East, 180 = South, 270 = West).
  public float AltitudeFeet { get; set; }
  public bool IsSelected { get; set; }

  public override void OnRender(Graphics g) {...}
  void DrawPlane(Graphics g) {...}
}
```
*Listing 6* - The marker class used to display flights on the map.

Most of the marker code is devoted to rendering. The OnRender method basically calls DrawPlane to handle the details like this:

```csharp
  void DrawPlane(Graphics g)
  {
    Color darkGreen = Color.FromArgb(0, 180, 0);
    Color lightCyan = Color.FromArgb(128, 255, 255);
    Color purple = Color.FromArgb(168, 85, 247);
    Color red = Color.FromArgb(239, 68, 68);

    Color planeColor = AltitudeFeet switch
    {
      < 10 => Color.White,
      < 10000 => darkGreen,
      < 25000 => lightCyan,
      _ => purple
    };

    if (IsSelected) 
      planeColor = red;

    var state = g.Save();  // save matrix transforms before making changes to it

    // move the plane to the center of the screen
    g.TranslateTransform(LocalPosition.X - Offset.X, LocalPosition.Y - Offset.Y);

    // rotate it to match the plane's direction
    g.RotateTransform((float)Heading);

    using var brush = new SolidBrush(planeColor);
    using var pen = new Pen(IsSelected ? Color.White : Color.Black, 1f);

    // draw the plane

    PointF[] shape = new PointF[]
    {
        new PointF(0, -9), new PointF(2, -3), new PointF(8, 2),
        new PointF(8, 4), new PointF(2, 2), new PointF(2, 6),
        new PointF(5, 8), new PointF(5, 9), new PointF(0, 7),
        new PointF(-5, 9), new PointF(-5, 8), new PointF(-2, 6),
        new PointF(-2, 2), new PointF(-8, 4), new PointF(-8, 2),
        new PointF(-2, -3)
    };

    g.FillPolygon(brush, shape);
    g.DrawPolygon(pen, shape);

    g.Restore(state);  // restore the original matrix transforms
  }
```
*Listing 7* - Drawing each aircraft.

The code ```var state = g.Save()``` saves the state of the Graphics space before we manipulate it. Once done drawing the aircraft, we restore the old Graphics state with the code ```g.Restore(state)```.
To draw each aircraft at the proper position and with the proper heading, we need to do a translation and rotation of the Graphics surface.
First we move the surface so the aircraft is shown centered with the code 

``` g.TranslateTransform(LocalPosition.X - Offset, LocalPosition.X - Offset. Y)```

Then we rotate the surface with the code

```g.RotateTransform(Heading);```.

With the graphics surface transformed like this, we draw the outline of the aircraft using a polygon like this:

```csharp
    PointF[] shape = new PointF[]
    {
      new PointF(0, -9), new PointF(2, -3), new PointF(8, 2),
      new PointF(8, 4), new PointF(2, 2), new PointF(2, 6),
      new PointF(5, 8), new PointF(5, 9), new PointF(0, 7),
      new PointF(-5, 9), new PointF(-5, 8), new PointF(-2, 6),
      new PointF(-2, 2), new PointF(-8, 4), new PointF(-8, 2),
      new PointF(-2, -3)
    };

    g.FillPolygon(brush, shape);
    g.DrawPolygon(pen, shape);
```
*Listing 7* - Drawing each aircraft using a polygon.

After drawing the plane, we restore the Graphics surface to its original state.

## Showing Flights on the Map

All the aircraft markers are added to a map overlay managed by GMapControl, which is the base class of FlightViewer.
The following listing shows the basic code:

```csharp
public partial class FlightViewer : GMap.NET.WindowsForms.GMapControl
{
  List<FlightData> allFlights = new();
  GMapOverlay flightsOverlay = new GMapOverlay("flights"); // the map overlay that contains the aircraft markers

  public (int flightsFound, int flightsFiltered) AddFlightMarkers()
  {
    var filteredFlights = GetFilteredFlights();

    flightsOverlay.Markers.Clear();

    foreach (var flight in filteredFlights)
    {
      var marker = new MarkerFlight(flight.Callsign, new PointLatLng(flight.Latitude, flight.Longitude), flight.Heading, flight.AltitudeFeet, false);
      marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
      marker.ToolTipText = $"{iataCode}{flightNumber} ({airlineName}) -  Altitude:{flight.AltitudeFeet:N0} feet   Speed:{flight.VelocityKnots} knots";

      flightsOverlay.Markers.Add(marker);
    }

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
}
```
*Listing 7* - Adding flight markers to the map overlay.

The code supports filtering by airline code or flight number.

## Closing Notes
If you want to display flights for a geographic area of your choosing, you'll need to make some changes to class FlightViewer, as highlighted in the next figure:

<img width="851" height="341" alt="image" src="https://github.com/user-attachments/assets/4179bbfa-cba3-4ee9-af38-6a95aebf043f" />

*Figure 9* - Adjusting FlightViewer to accommodate different geographic areas.

In order the the new area to display correctly, GMapControl will need to have the tiles for that area. These will need to be downloaded from OpenStreetMap. See my app MapViewer for details on how to accomplish this.

