using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using SuchByte.MacroDeck.Plugins;

namespace airxiti.Euroscope
{
    public class ConfigData
    {
        public List<string> Airports { get; set; } = new() { "EDDB" };
        public string VatsimId { get; set; } = "";
    }

    public class Configurator
    {
        private static readonly string ConfigPath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
    "Macro Deck", "configs", "EuroscopeConfig.json");

        private static Configurator _instance;
        public static Configurator Instance => _instance ??= new Configurator();

        private ConfigData _data = new();

        public List<string> Airports
        {
            get => _data.Airports;
            set { _data.Airports = value; Save(); }
        }

        public string VatsimId
        {
            get => _data.VatsimId;
            set { _data.VatsimId = value; Save(); }
        }

        private Configurator()
        {
            Load();
        }

        public void Load()
        {
            if (File.Exists(ConfigPath))
            {
                var json = File.ReadAllText(ConfigPath);
                _data = JsonSerializer.Deserialize<ConfigData>(json) ?? new ConfigData();
            }
        }

        public void Save()
        {
            var json = JsonSerializer.Serialize(_data, new JsonSerializerOptions { WriteIndented = true });
            Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath)!);
            File.WriteAllText(ConfigPath, json);
        }
    }

    public class EuroscopeConfiguratorForm : Form
    {
        private TextBox txtAirports;
        private Button btnSave;
        private Label lblInfo;
        private ListBox lstAirports;
        private TextBox txtVatsimId;
        private Label lblVatsimId;
        private Button btnDeleteAirport;
        public EuroscopeConfiguratorForm()
        {
            InitializeComponent();
        }
        private void InitializeComponent()
        {
            var darkBack = System.Drawing.ColorTranslator.FromHtml("#2d2d2d");
            var whiteFore = System.Drawing.Color.White;

            this.BackColor = darkBack;
            this.ForeColor = whiteFore;

            this.Text = "Euroscope Configurator";
            this.Size = new System.Drawing.Size(400, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            lblInfo = new Label()
            {
                Text = "Flughäfen (ICAO, Komma-getrennt):",
                Location = new System.Drawing.Point(10, 20),
                AutoSize = true
            };
            this.Controls.Add(lblInfo);

            txtAirports = new TextBox()
            {
                Location = new System.Drawing.Point(10, 45),
                Width = 360,
                Text = string.Join(", ", Configurator.Instance)
            };
            this.Controls.Add(txtAirports);

            lstAirports = new ListBox()
            {
                Location = new System.Drawing.Point(10, 80),
                Width = 360,
                Height = 100
            };
            lstAirports.Items.AddRange(Configurator.Instance.Airports.ToArray());
            this.Controls.Add(lstAirports);

            // Button zum Löschen
            btnDeleteAirport = new Button()
            {
                Text = "Ausgewählten Flughafen löschen",
                Location = new System.Drawing.Point(120, 190),
                Width = 200
            };
            btnDeleteAirport.Click += BtnDeleteAirport_Click;
            this.Controls.Add(btnDeleteAirport);

            // VATSIM-ID Label und TextBox
            lblVatsimId = new Label()
            {
                Text = $"VATSIM-ID: {Configurator.Instance.VatsimId}",
                Location = new System.Drawing.Point(10, 215),
                AutoSize = true
            };
            this.Controls.Add(lblVatsimId);


            txtVatsimId = new TextBox()
            {
                Location = new System.Drawing.Point(10, 250),
                Width = 360,
                Text = Configurator.Instance.VatsimId
            };
            this.Controls.Add(txtVatsimId);


            btnSave = new Button()
            {
                Text = "Speichern",
                Location = new System.Drawing.Point(10, 275),
                Width = 100
            };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var airports = txtAirports.Text
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(a => a.Trim().ToUpper())
                .Where(a => a.Length == 4)
                .ToList();
            lblVatsimId.Text = $"VATSIM-ID: {txtVatsimId.Text}";

            Configurator.Instance.VatsimId = txtVatsimId.Text;
            Configurator.Instance.Airports = airports;
            MessageBox.Show("Konfiguration gespeichert.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        
        private void BtnDeleteAirport_Click(object sender, EventArgs e)
        {
            if (lstAirports.SelectedItem is string selectedAirport)
            {
                // Airport aus Config entfernen
                var airports = Configurator.Instance.Airports.ToList();
                airports.Remove(selectedAirport);
                Configurator.Instance.Airports = airports;

                // Variable entfernen (wenn MacroDeck läuft)
                try
                {
                    SuchByte.MacroDeck.Variables.VariableManager.DeleteVariable($"Euroscope_{selectedAirport.ToLower()}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Löschen der Variable: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                };

                // ListBox aktualisieren
                lstAirports.Items.Clear();
                lstAirports.Items.AddRange(airports.ToArray());
                // TextBox aktualisieren
                txtAirports.Text = string.Join(", ", airports);
                Main.Instance?.Enable();
                
            }
            else
            {
                MessageBox.Show("Bitte einen Flughafen auswählen.", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
