namespace FlightViewer;

public partial class FormMain : Form
{
  const int RefreshIntervalSeconds = 30;
  int refreshCountdown = RefreshIntervalSeconds;

  public FormMain()
  {
    InitializeComponent();

    flightViewer.CreateTileCache(); // copies our map tiles to a local cache for GMap to access
  }

  void MainForm_Shown(object sender, EventArgs e)
  {
    ShowFlightsAsync();
  }

  void timerRefreshCountdown_Tick(object sender, EventArgs e)
  {
    refreshCountdown--;
    labelStatusBarRefreshCountdown.Text = $"Refresh in {refreshCountdown} secs";
    if (refreshCountdown > 0) return;

    refreshCountdown = RefreshIntervalSeconds;

    ShowFlightsAsync();
  }

  void textBoxFilterByFlightNumber_KeyDown(object sender, KeyEventArgs e)
  {
    if (e.KeyCode != Keys.Enter) return;

    e.SuppressKeyPress = true; // to suppress the ding sound
    ApplyFilter();
  }

  void buttonApplyFilter_Click(object sender, EventArgs e)
  {
    ApplyFilter();
  }

  void ApplyFilter()
  {
    (int flightsFound, int flightsFiltered) = flightViewer.FilterByFlightNumber(textBoxFilterByFlightNumber.Text);
    labelStatusBarFlights.Text = $"Flights: {flightsFiltered} shown of total {flightsFound}";
  }

  async void ShowFlightsAsync()
  {
    try
    {
      labelStatusBarMessage.Text = "Fetching flight data...";

      (int flightsFound, int flightsFiltered) = await flightViewer.ShowFlightsAsync();

      labelStatusBarMessage.Text = $"Data updated at {DateTime.Now:T}";
      labelStatusBarFlights.Text = $"Flights: {flightsFiltered} shown of total {flightsFound}";
    }
    catch (Exception ex)
    {
      labelStatusBarMessage.Text = $"Exception: {ex.Message}";
    }
  }
}
