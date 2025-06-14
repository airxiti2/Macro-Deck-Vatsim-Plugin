using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.Text.Json;


namespace airxiti.Vatsim;

public class Configurator
{
    private static readonly string ConfigPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Macro Deck", "configs", "airxiti_vatsim plugin.json");

    private static Configurator _instance;
    public static Configurator Instance => _instance ??= new Configurator();

    private VatsimConfiguratorForm.ConfigData data = new();

    public List<string> MetarAirports
    {
        get => data.MetarAirports;
        set { data.MetarAirports = value; Save(); }
    }
    
    public List<string> AtisAirports
    {
        get => data.AtisAirports;
        set { data.AtisAirports = value; Save(); }
    }

    public string VatsimId
    {
        get => data.VatsimId;
        set { data.VatsimId = value; Save(); }
    }

    private Configurator()
    {
        Load();
    }

    private void Load()
    {
        if (File.Exists(ConfigPath))
        {
            var json = File.ReadAllText(ConfigPath);
            data = JsonSerializer.Deserialize<VatsimConfiguratorForm.ConfigData>(json) ?? new VatsimConfiguratorForm.ConfigData();
        }
    }

    private void Save()
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath)!);
        File.WriteAllText(ConfigPath, json);
    }
}
public class VatsimConfiguratorForm : Form
{
    public class ConfigData
    {
        public List<string> MetarAirports { get; set; } = ["EDDB"];
        public List<string> AtisAirports { get; set; } = ["EDDB"];
        public string VatsimId { get; set; } = "Empty";
    }
    public VatsimConfiguratorForm()
    {
        InitializeComponent();
        LoadConfigurationToUi();
    }

    private void LoadConfigurationToUi()
    {
        lstMetarAirports.Items.Clear();
        lstMetarAirports.Items.AddRange(Configurator.Instance.MetarAirports.ToArray());
        
        lstAtisAirports.Items.Clear();
        lstAtisAirports.Items.AddRange(Configurator.Instance.AtisAirports.ToArray());
        
        lblVatsimId.Text = $"Vatsim ID: {Configurator.Instance.VatsimId}";
        txtVatsimId.Text = Configurator.Instance.VatsimId;
    }


    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        lblMetarAirports = new System.Windows.Forms.Label();
        atisLabel1 = new System.Windows.Forms.Label();
        lstMetarAirports = new System.Windows.Forms.ListBox();
        txtMetarAirports = new System.Windows.Forms.TextBox();
        lstAtisAirports = new System.Windows.Forms.ListBox();
        txtAtisAirports = new System.Windows.Forms.TextBox();
        btnDeleteMetar = new System.Windows.Forms.Button();
        btnSaveMetar = new System.Windows.Forms.Button();
        btnSaveAtis = new System.Windows.Forms.Button();
        btnDeleteAtis = new System.Windows.Forms.Button();
        lblVatsimId = new System.Windows.Forms.Label();
        txtVatsimId = new System.Windows.Forms.TextBox();
        btnSaveVatsimId = new System.Windows.Forms.Button();
        SuspendLayout();
        // 
        // LblMetarAirports
        // 
        lblMetarAirports.Location = new System.Drawing.Point(12, 9);
        lblMetarAirports.Name = "lblMetarAirports";
        lblMetarAirports.Size = new System.Drawing.Size(225, 15);
        lblMetarAirports.TabIndex = 0;
        lblMetarAirports.Text = "METAR Airports";
        // 
        // AtisLabel1
        // 
        atisLabel1.Location = new System.Drawing.Point(257, 9);
        atisLabel1.Name = "atisLabel1";
        atisLabel1.Size = new System.Drawing.Size(225, 15);
        atisLabel1.TabIndex = 1;
        atisLabel1.Text = "ATIS Airports (Have to be in vATIS!)";
        // 
        // LstMetarAirports
        // 
        lstMetarAirports.BackColor = System.Drawing.SystemColors.WindowFrame;
        lstMetarAirports.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        lstMetarAirports.ForeColor = System.Drawing.SystemColors.Window;
        lstMetarAirports.FormattingEnabled = true;
        lstMetarAirports.ItemHeight = 15;
        lstMetarAirports.Items.AddRange(new object[] { "EDDH" });
        lstMetarAirports.Location = new System.Drawing.Point(11, 63);
        lstMetarAirports.Name = "lstMetarAirports";
        lstMetarAirports.Size = new System.Drawing.Size(225, 107);
        lstMetarAirports.TabIndex = 2;
        // 
        // TxtMetarAirports
        // 
        txtMetarAirports.BackColor = System.Drawing.SystemColors.WindowFrame;
        txtMetarAirports.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        txtMetarAirports.ForeColor = System.Drawing.SystemColors.Window;
        txtMetarAirports.Location = new System.Drawing.Point(11, 34);
        txtMetarAirports.Name = "txtMetarAirports";
        txtMetarAirports.Size = new System.Drawing.Size(225, 23);
        txtMetarAirports.TabIndex = 4;
        // 
        // LstAtisAirports
        // 
        lstAtisAirports.BackColor = System.Drawing.SystemColors.WindowFrame;
        lstAtisAirports.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        lstAtisAirports.ForeColor = System.Drawing.SystemColors.Window;
        lstAtisAirports.FormattingEnabled = true;
        lstAtisAirports.ItemHeight = 15;
        lstAtisAirports.Items.AddRange(new object[] { "EDDB" });
        lstAtisAirports.Location = new System.Drawing.Point(257, 63);
        lstAtisAirports.Name = "lstAtisAirports";
        lstAtisAirports.Size = new System.Drawing.Size(225, 107);
        lstAtisAirports.TabIndex = 5;
        // 
        // TxtAtisAirports
        // 
        txtAtisAirports.BackColor = System.Drawing.SystemColors.WindowFrame;
        txtAtisAirports.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        txtAtisAirports.ForeColor = System.Drawing.SystemColors.Window;
        txtAtisAirports.Location = new System.Drawing.Point(257, 34);
        txtAtisAirports.Name = "txtAtisAirports";
        txtAtisAirports.Size = new System.Drawing.Size(225, 23);
        txtAtisAirports.TabIndex = 6;
        // 
        // BtnDeleteMetar
        // 
        btnDeleteMetar.BackColor = System.Drawing.Color.DarkOrange;
        btnDeleteMetar.FlatAppearance.BorderSize = 0;
        btnDeleteMetar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        btnDeleteMetar.Location = new System.Drawing.Point(11, 176);
        btnDeleteMetar.Name = "btnDeleteMetar";
        btnDeleteMetar.Size = new System.Drawing.Size(95, 28);
        btnDeleteMetar.TabIndex = 7;
        btnDeleteMetar.Text = "Delete";
        btnDeleteMetar.UseVisualStyleBackColor = false;
        btnDeleteMetar.Click += BtnDeleteMetar_Click;
        // 
        // BtnSaveMetar
        // 
        btnSaveMetar.BackColor = System.Drawing.Color.DodgerBlue;
        btnSaveMetar.FlatAppearance.BorderSize = 0;
        btnSaveMetar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        btnSaveMetar.Location = new System.Drawing.Point(116, 176);
        btnSaveMetar.Name = "btnSaveMetar";
        btnSaveMetar.Size = new System.Drawing.Size(120, 28);
        btnSaveMetar.TabIndex = 8;
        btnSaveMetar.Text = "Add";
        btnSaveMetar.UseVisualStyleBackColor = false;
        btnSaveMetar.Click += BtnSaveMetar_Click;
        // 
        // BtnSaveAtis
        // 
        btnSaveAtis.BackColor = System.Drawing.Color.DodgerBlue;
        btnSaveAtis.FlatAppearance.BorderSize = 0;
        btnSaveAtis.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        btnSaveAtis.Location = new System.Drawing.Point(362, 176);
        btnSaveAtis.Name = "btnSaveAtis";
        btnSaveAtis.Size = new System.Drawing.Size(120, 28);
        btnSaveAtis.TabIndex = 9;
        btnSaveAtis.Text = "Add";
        btnSaveAtis.UseVisualStyleBackColor = false;
        btnSaveAtis.Click += BtnSaveAtis_Click;
        // 
        // BtnDeleteAtis
        // 
        btnDeleteAtis.BackColor = System.Drawing.Color.DarkOrange;
        btnDeleteAtis.FlatAppearance.BorderSize = 0;
        btnDeleteAtis.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        btnDeleteAtis.Location = new System.Drawing.Point(257, 176);
        btnDeleteAtis.Name = "btnDeleteAtis";
        btnDeleteAtis.Size = new System.Drawing.Size(95, 28);
        btnDeleteAtis.TabIndex = 10;
        btnDeleteAtis.Text = "Delete";
        btnDeleteAtis.UseVisualStyleBackColor = false;
        btnDeleteAtis.Click += BtnDeleteAtis_Click;
        // 
        // LblVatsimId
        // 
        lblVatsimId.Location = new System.Drawing.Point(11, 229);
        lblVatsimId.Name = "lblVatsimId";
        lblVatsimId.Size = new System.Drawing.Size(225, 15);
        lblVatsimId.TabIndex = 11;
        lblVatsimId.Text = $"Vatsim ID: {Configurator.Instance.VatsimId}";
        // 
        // TxtVatsimId
        // 
        txtVatsimId.BackColor = System.Drawing.SystemColors.WindowFrame;
        txtVatsimId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        txtVatsimId.ForeColor = System.Drawing.SystemColors.Window;
        txtVatsimId.Location = new System.Drawing.Point(12, 247);
        txtVatsimId.Name = "txtVatsimId";
        txtVatsimId.Size = new System.Drawing.Size(225, 23);
        txtVatsimId.TabIndex = 12;
        // 
        // btnSaveVatsimId
        // 
        btnSaveVatsimId.BackColor = System.Drawing.Color.DodgerBlue;
        btnSaveVatsimId.FlatAppearance.BorderSize = 0;
        btnSaveVatsimId.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        btnSaveVatsimId.Location = new System.Drawing.Point(12, 276);
        btnSaveVatsimId.Name = "btnSaveVatsimId";
        btnSaveVatsimId.Size = new System.Drawing.Size(120, 28);
        btnSaveVatsimId.TabIndex = 13;
        btnSaveVatsimId.Text = "Save";
        btnSaveVatsimId.UseVisualStyleBackColor = false;
        btnSaveVatsimId.Click += btnSaveVatsimId_Click;
        // 
        // testConfig
        // 
        BackColor = System.Drawing.Color.FromArgb(((int)((byte)64)), ((int)((byte)64)), ((int)((byte)64)));
        ClientSize = new System.Drawing.Size(494, 311);
        Controls.Add(btnSaveVatsimId);
        Controls.Add(txtVatsimId);
        Controls.Add(lblVatsimId);
        Controls.Add(btnDeleteAtis);
        Controls.Add(btnSaveAtis);
        Controls.Add(btnSaveMetar);
        Controls.Add(btnDeleteMetar);
        Controls.Add(txtAtisAirports);
        Controls.Add(lstAtisAirports);
        Controls.Add(txtMetarAirports);
        Controls.Add(lstMetarAirports);
        Controls.Add(atisLabel1);
        Controls.Add(lblMetarAirports);
        ForeColor = System.Drawing.Color.White;
        Text = "Vatsim Configurator";
        ResumeLayout(false);
        PerformLayout();
    }

    private System.Windows.Forms.Button btnSaveVatsimId;
    private System.Windows.Forms.Button btnSaveAtis;
    private System.Windows.Forms.Button btnDeleteAtis;
    private System.Windows.Forms.Label lblVatsimId;
    private System.Windows.Forms.TextBox txtVatsimId;
    private System.Windows.Forms.Button btnDeleteMetar;
    private System.Windows.Forms.Button btnSaveMetar;
    private System.Windows.Forms.ListBox lstAtisAirports;
    private System.Windows.Forms.TextBox txtAtisAirports;
    private System.Windows.Forms.TextBox txtMetarAirports;
    private System.Windows.Forms.ListBox lstMetarAirports;
    private System.Windows.Forms.Label lblMetarAirports;
    private System.Windows.Forms.Label atisLabel1;

    private bool IsValidIcaoCode(string code)
    {
        return code.Length == 4 && code.All(c => char.IsLetterOrDigit(c));
    }
    
    private void BtnSaveMetar_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(txtMetarAirports.Text))
            {
                MessageBox.Show("Please enter a valid ICAO", "Input Error");
                return;
            }

            var newMetarAirports = txtMetarAirports.Text
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(a => a.Trim().ToUpper())
                .Where(a => IsValidIcaoCode(a))
                .ToList();

            if (!newMetarAirports.Any())
            {
                MessageBox.Show("Couldn't find valid ICAO", "Validation Error");
                return;
            }
            var existingAirports = Configurator.Instance.MetarAirports.ToList();
            foreach (var airport in newMetarAirports)
            {
                if (!existingAirports.Contains(airport))
                {
                    existingAirports.Add(airport);
                    lstMetarAirports.Items.Add(airport);
                }
            }
            txtMetarAirports.Clear();
            Configurator.Instance.MetarAirports = existingAirports;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving: {ex.Message}", "Error");
        }
    }
    
    private void BtnSaveAtis_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(txtAtisAirports.Text))
            {
                MessageBox.Show("Please enter a valid ICAO", "Input Error");
                return;
            }

            var newAtisAirports = txtAtisAirports.Text
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(a => a.Trim().ToUpper())
                .Where(a => IsValidIcaoCode(a))
                .ToList();

            if (!newAtisAirports.Any())
            {
                MessageBox.Show("Couldn't find valid ICAO", "Validation Error");
                return;
            }
            var existingAirports = Configurator.Instance.AtisAirports.ToList();
            foreach (var airport in newAtisAirports)
            {
                if (!existingAirports.Contains(airport))
                {
                    existingAirports.Add(airport);
                    lstAtisAirports.Items.Add(airport);
                }
            }
            txtAtisAirports.Clear();
            Configurator.Instance.AtisAirports = existingAirports;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving: {ex.Message}", "Error");
        }
    }

    private void BtnDeleteMetar_Click(object sender, EventArgs e)
    {
        if (lstMetarAirports.SelectedItem is string selectedAirport)
        {
            var airports = Configurator.Instance.MetarAirports.ToList();
            airports.Remove(selectedAirport);
            Configurator.Instance.MetarAirports = airports;

            try
            {
                SuchByte.MacroDeck.Variables.VariableManager.DeleteVariable($"vatsim_metar_{selectedAirport.ToLower()}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while deleting METAR: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } ;

            lstMetarAirports.Items.Clear();
            lstMetarAirports.Items.AddRange(airports.ToArray());
            txtMetarAirports.Text = string.Join(", ", airports);
            Main.Instance?.Enable();
        }
        else
        {
            MessageBox.Show("Please select a METAR", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void BtnDeleteAtis_Click(object sender, EventArgs e)
    {
        if (lstAtisAirports.SelectedItem is string selectedAirport)
        {
            var airports = Configurator.Instance.AtisAirports.ToList();
            airports.Remove(selectedAirport);
            Configurator.Instance.AtisAirports = airports;
            
            try
            {
                SuchByte.MacroDeck.Variables.VariableManager.DeleteVariable($"vatsim_atis_{selectedAirport.ToLower()}_letter");
                SuchByte.MacroDeck.Variables.VariableManager.DeleteVariable($"vatsim_atis_{selectedAirport.ToLower()}_text");
                SuchByte.MacroDeck.Variables.VariableManager.DeleteVariable($"vatsim_atis_{selectedAirport.ToLower()}_runways");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while deleting ATIS: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            ;

            lstAtisAirports.Items.Clear();
            lstAtisAirports.Items.AddRange(airports.ToArray());
            txtAtisAirports.Text = string.Join(", ", airports);
            Main.Instance?.Enable();
        }
        else
        {
            MessageBox.Show("Please select an ATIS", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void btnSaveVatsimId_Click(object sender, EventArgs e)
    {
        lblVatsimId.Text = $"VATSIM-ID: {txtVatsimId.Text}";
        Configurator.Instance.VatsimId = txtVatsimId.Text;
    }
}