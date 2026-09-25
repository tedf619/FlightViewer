//using static System.Net.Mime.MediaTypeNames;

namespace FlightViewer
{
  partial class FormMain
  {
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      components = new System.ComponentModel.Container();
      panelControls = new Panel();
      buttonApplyFilter = new Button();
      label11 = new Label();
      textBoxFilterByFlightNumber = new TextBox();
      flightViewer = new FlightViewer();
      panelLegend = new Panel();
      label9 = new Label();
      label7 = new Label();
      label8 = new Label();
      label6 = new Label();
      label4 = new Label();
      label5 = new Label();
      label3 = new Label();
      label2 = new Label();
      label1 = new Label();
      toolTip = new ToolTip(components);
      panelStatusBar = new Panel();
      labelStatusBarMessage = new Label();
      labelStatusBarFlights = new Label();
      labelStatusBarRefreshCountdown = new Label();
      timerRefreshCountdown = new System.Windows.Forms.Timer(components);
      panelControls.SuspendLayout();
      panelLegend.SuspendLayout();
      panelStatusBar.SuspendLayout();
      SuspendLayout();
      // 
      // panelControls
      // 
      panelControls.BackColor = SystemColors.Control;
      panelControls.BorderStyle = BorderStyle.FixedSingle;
      panelControls.Controls.Add(buttonApplyFilter);
      panelControls.Controls.Add(label11);
      panelControls.Controls.Add(textBoxFilterByFlightNumber);
      panelControls.Dock = DockStyle.Top;
      panelControls.Location = new Point(0, 0);
      panelControls.Name = "panelControls";
      panelControls.Padding = new Padding(10);
      panelControls.Size = new Size(882, 40);
      panelControls.TabIndex = 0;
      // 
      // buttonApplyFilter
      // 
      buttonApplyFilter.Location = new Point(287, 8);
      buttonApplyFilter.Name = "buttonApplyFilter";
      buttonApplyFilter.Size = new Size(93, 23);
      buttonApplyFilter.TabIndex = 0;
      buttonApplyFilter.Text = "Apply Filter";
      toolTip.SetToolTip(buttonApplyFilter, "Filter the displayed flights with the data entered");
      buttonApplyFilter.UseVisualStyleBackColor = true;
      buttonApplyFilter.Click += ButtonApplyFilter_Click;
      // 
      // label11
      // 
      label11.AutoSize = true;
      label11.Location = new Point(13, 13);
      label11.Name = "label11";
      label11.Size = new Size(129, 15);
      label11.TabIndex = 2;
      label11.Text = "Filter by Flight Number";
      // 
      // textBoxFilterByFlightNumber
      // 
      textBoxFilterByFlightNumber.Location = new Point(148, 9);
      textBoxFilterByFlightNumber.Name = "textBoxFilterByFlightNumber";
      textBoxFilterByFlightNumber.PlaceholderText = "e.g. UA or UA123";
      textBoxFilterByFlightNumber.Size = new Size(118, 23);
      textBoxFilterByFlightNumber.TabIndex = 3;
      toolTip.SetToolTip(textBoxFilterByFlightNumber, "Enter the airline code without numbers to see all flights for a given airline");
      textBoxFilterByFlightNumber.KeyDown += TextBoxFilterByFlightNumber_KeyDown;
      // 
      // flightViewer
      // 
      flightViewer.Bearing = 0F;
      flightViewer.BorderStyle = BorderStyle.FixedSingle;
      flightViewer.Dock = DockStyle.Fill;
      flightViewer.EmptyTileColor = Color.Black;
      flightViewer.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
      flightViewer.Location = new Point(0, 70);
      flightViewer.MaxZoom = 8;
      flightViewer.MinZoom = 4;
      flightViewer.MouseWheelZoomEnabled = true;
      flightViewer.Name = "flightViewer";
      flightViewer.RetryLoadTile = 0;
      flightViewer.SelectedAreaFillColor = Color.FromArgb(33, 65, 105, 225);
      flightViewer.ShowTileGridLines = false;
      flightViewer.Size = new Size(882, 438);
      flightViewer.TabIndex = 9;
      flightViewer.Zoom = 4D;
      // 
      // panelLegend
      // 
      panelLegend.Controls.Add(label9);
      panelLegend.Controls.Add(label7);
      panelLegend.Controls.Add(label8);
      panelLegend.Controls.Add(label6);
      panelLegend.Controls.Add(label4);
      panelLegend.Controls.Add(label5);
      panelLegend.Controls.Add(label3);
      panelLegend.Controls.Add(label2);
      panelLegend.Controls.Add(label1);
      panelLegend.Dock = DockStyle.Top;
      panelLegend.Location = new Point(0, 40);
      panelLegend.Name = "panelLegend";
      panelLegend.Size = new Size(882, 30);
      panelLegend.TabIndex = 10;
      // 
      // label9
      // 
      label9.BackColor = Color.FromArgb(168, 85, 247);
      label9.BorderStyle = BorderStyle.FixedSingle;
      label9.Location = new Point(770, 3);
      label9.Name = "label9";
      label9.Size = new Size(23, 23);
      label9.TabIndex = 10;
      // 
      // label7
      // 
      label7.BackColor = Color.FromArgb(128, 255, 255);
      label7.BorderStyle = BorderStyle.FixedSingle;
      label7.Location = new Point(597, 3);
      label7.Name = "label7";
      label7.Size = new Size(23, 23);
      label7.TabIndex = 9;
      // 
      // label8
      // 
      label8.AutoSize = true;
      label8.ForeColor = Color.Teal;
      label8.Location = new Point(796, 7);
      label8.Name = "label8";
      label8.Size = new Size(43, 15);
      label8.TabIndex = 8;
      label8.Text = "Higher";
      // 
      // label6
      // 
      label6.AutoSize = true;
      label6.ForeColor = Color.Teal;
      label6.Location = new Point(623, 7);
      label6.Name = "label6";
      label6.Size = new Size(62, 15);
      label6.TabIndex = 6;
      label6.Text = "< 25,000 ft";
      // 
      // label4
      // 
      label4.AutoSize = true;
      label4.ForeColor = Color.Teal;
      label4.Location = new Point(467, 7);
      label4.Name = "label4";
      label4.Size = new Size(62, 15);
      label4.TabIndex = 4;
      label4.Text = "< 10,000 ft";
      // 
      // label5
      // 
      label5.BackColor = Color.FromArgb(0, 180, 0);
      label5.BorderStyle = BorderStyle.FixedSingle;
      label5.Location = new Point(441, 3);
      label5.Name = "label5";
      label5.Size = new Size(23, 23);
      label5.TabIndex = 3;
      // 
      // label3
      // 
      label3.AutoSize = true;
      label3.ForeColor = Color.Teal;
      label3.Location = new Point(314, 7);
      label3.Name = "label3";
      label3.Size = new Size(85, 15);
      label3.TabIndex = 2;
      label3.Text = "On the ground";
      // 
      // label2
      // 
      label2.BackColor = Color.White;
      label2.BorderStyle = BorderStyle.FixedSingle;
      label2.Location = new Point(288, 3);
      label2.Name = "label2";
      label2.Size = new Size(23, 23);
      label2.TabIndex = 1;
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.ForeColor = Color.Teal;
      label1.Location = new Point(12, 6);
      label1.Name = "label1";
      label1.Size = new Size(184, 15);
      label1.TabIndex = 0;
      label1.Text = "Plane colors are based on altitude";
      // 
      // panelStatusBar
      // 
      panelStatusBar.BorderStyle = BorderStyle.Fixed3D;
      panelStatusBar.Controls.Add(labelStatusBarMessage);
      panelStatusBar.Controls.Add(labelStatusBarFlights);
      panelStatusBar.Controls.Add(labelStatusBarRefreshCountdown);
      panelStatusBar.Dock = DockStyle.Bottom;
      panelStatusBar.Location = new Point(0, 482);
      panelStatusBar.Name = "panelStatusBar";
      panelStatusBar.Size = new Size(882, 26);
      panelStatusBar.TabIndex = 11;
      // 
      // labelStatusBarMessage
      // 
      labelStatusBarMessage.Dock = DockStyle.Fill;
      labelStatusBarMessage.Location = new Point(0, 0);
      labelStatusBarMessage.Name = "labelStatusBarMessage";
      labelStatusBarMessage.Size = new Size(578, 22);
      labelStatusBarMessage.TabIndex = 2;
      labelStatusBarMessage.TextAlign = ContentAlignment.MiddleLeft;
      // 
      // labelStatusBarFlights
      // 
      labelStatusBarFlights.Dock = DockStyle.Right;
      labelStatusBarFlights.Location = new Point(578, 0);
      labelStatusBarFlights.Name = "labelStatusBarFlights";
      labelStatusBarFlights.Size = new Size(192, 22);
      labelStatusBarFlights.TabIndex = 1;
      labelStatusBarFlights.Text = "Flights: 100 shown of total 1000";
      labelStatusBarFlights.TextAlign = ContentAlignment.MiddleLeft;
      // 
      // labelStatusBarRefreshCountdown
      // 
      labelStatusBarRefreshCountdown.Dock = DockStyle.Right;
      labelStatusBarRefreshCountdown.Location = new Point(770, 0);
      labelStatusBarRefreshCountdown.Name = "labelStatusBarRefreshCountdown";
      labelStatusBarRefreshCountdown.Size = new Size(108, 22);
      labelStatusBarRefreshCountdown.TabIndex = 0;
      labelStatusBarRefreshCountdown.Text = "Refresh in 30 secs";
      labelStatusBarRefreshCountdown.TextAlign = ContentAlignment.MiddleLeft;
      // 
      // timerRefreshCountdown
      // 
      timerRefreshCountdown.Enabled = true;
      timerRefreshCountdown.Interval = 1000;
      timerRefreshCountdown.Tick += TimerRefreshCountdown_Tick;
      // 
      // FormMain
      // 
      AcceptButton = buttonApplyFilter;
      ClientSize = new Size(882, 508);
      Controls.Add(panelStatusBar);
      Controls.Add(flightViewer);
      Controls.Add(panelLegend);
      Controls.Add(panelControls);
      Name = "FormMain";
      StartPosition = FormStartPosition.CenterScreen;
      Text = "FlightViewer";
      Shown += MainForm_Shown;
      panelControls.ResumeLayout(false);
      panelControls.PerformLayout();
      panelLegend.ResumeLayout(false);
      panelLegend.PerformLayout();
      panelStatusBar.ResumeLayout(false);
      ResumeLayout(false);
    }

    private Panel panelControls;
    private Label label11;
    private TextBox textBoxFilterByFlightNumber;
    private FlightViewer flightViewer;
    private Panel panelLegend;
    private Label label2;
    private Label label1;
    private Label label8;
    private Label label6;
    private Label label4;
    private Label label5;
    private Label label3;
    private Label label9;
    private Label label7;
    private Button buttonApplyFilter;
    private ToolTip toolTip;
    private Panel panelStatusBar;
    private Label labelStatusBarMessage;
    private Label labelStatusBarFlights;
    private Label labelStatusBarRefreshCountdown;
    private System.Windows.Forms.Timer timerRefreshCountdown;
  }
}