using System.Drawing.Drawing2D;

using GMap.NET;
using GMap.NET.WindowsForms;

namespace FlightViewer;

/// <summary>
/// Draws airplanes as "markers" on maps
/// </summary>
internal class MarkerFlight : GMapMarker
{
  public string CallSign { get; set; }  // e.g. DAL120
  public float Heading { get; set; }  // angle in degrees(0 = North, 90 = East, 180 = South, 270 = West).
  public float AltitudeFeet { get; set; }
  public bool IsSelected { get; set; }

  public MarkerFlight(string callsign, PointLatLng position, double heading, double altitudeFeet, bool isSelected)
      : base(position)
  {
    CallSign = callsign;
    Heading = (float) heading;
    AltitudeFeet = (float) altitudeFeet;
    IsSelected = isSelected;

    // Define bounding dimensions
    Size = new(20, 20);

    // Offset to keep the center of the icon pinned to the geographic location
    Offset = new Point(-Size.Width / 2, -Size.Height / 2);
  }

  public override void OnRender(Graphics g)
  {
    //if (_icon == null) return;

    // Save original graphics state matrix
    Matrix originalState = g.Transform;

    // Enable smooth rotation rendering
    g.SmoothingMode = SmoothingMode.AntiAlias;
    g.InterpolationMode = InterpolationMode.HighQualityBicubic;

    try
    {
      DrawPlane(g);
    }
    finally
    {
      // Restore original transform matrix so subsequent map layers render correctly
      g.Transform = originalState;
    }
  }

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
}