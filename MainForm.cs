using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NRUAGuestManager
{
    public partial class MainForm : Form
    {
        private DataTable guestData;
        private string currentFilePath = "";

        public MainForm()
        {
            InitializeComponent();
            InitializeDataTable();
            SetupDataGridView();
        }

        private void InitializeComponent()
        {
            this.Text = "NRUA Guest Manager - Gestor de Huéspedes";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Menu Strip
            var menuStrip = new MenuStrip();
            var fileMenu = new ToolStripMenuItem("Archivo");
            var newFile = new ToolStripMenuItem("Nuevo", null, (s, e) => NewFile());
            var openFile = new ToolStripMenuItem("Abrir CSV...", null, (s, e) => OpenCSV());
            var saveFile = new ToolStripMenuItem("Guardar", null, (s, e) => SaveFile());
            var saveAs = new ToolStripMenuItem("Guardar Como...", null, (s, e) => SaveFileAs());
            var exportN2 = new ToolStripMenuItem("Exportar para N2...", null, (s, e) => ExportForN2());
            var exit = new ToolStripMenuItem("Salir", null, (s, e) => Application.Exit());

            fileMenu.DropDownItems.AddRange(new ToolStripItem[] { newFile, openFile, saveFile, saveAs,
                new ToolStripSeparator(), exportN2, new ToolStripSeparator(), exit });
            menuStrip.Items.Add(fileMenu);
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            // Toolbar
            var toolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(10)
            };

            var btnAdd = new Button
            {
                Text = "➕ Añadir Registro",
                Left = 10,
                Top = 10,
                Width = 130,
                Height = 30
            };
            btnAdd.Click += (s, e) => AddNewRecord();

            var btnDelete = new Button
            {
                Text = "❌ Eliminar",
                Left = 150,
                Top = 10,
                Width = 100,
                Height = 30
            };
            btnDelete.Click += (s, e) => DeleteSelected();

            var btnValidate = new Button
            {
                Text = "✓ Validar Datos",
                Left = 260,
                Top = 10,
                Width = 120,
                Height = 30
            };
            btnValidate.Click += (s, e) => ValidateData();

            var btnExport = new Button
            {
                Text = "📤 Exportar N2",
                Left = 390,
                Top = 10,
                Width = 120,
                Height = 30,
                BackColor = Color.LightGreen
            };
            btnExport.Click += (s, e) => ExportForN2();

            var lblRecords = new Label
            {
                Text = "Registros: 0",
                Left = 550,
                Top = 15,
                Width = 150,
                Name = "lblRecordCount"
            };

            toolbar.Controls.AddRange(new Control[] { btnAdd, btnDelete, btnValidate, btnExport, lblRecords });
            this.Controls.Add(toolbar);

            // DataGridView
            dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = true,
                AllowUserToDeleteRows = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = true
            };
            this.Controls.Add(dataGridView);

            // Status Bar
            var statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel("Listo");
            statusStrip.Items.Add(statusLabel);
            this.Controls.Add(statusStrip);
        }

        private DataGridView dataGridView;
        private ToolStripStatusLabel statusLabel;

        private void InitializeDataTable()
        {
            guestData = new DataTable();
            guestData.Columns.Add("NRUA", typeof(string));
            guestData.Columns.Add("Fecha_Entrada", typeof(DateTime));
            guestData.Columns.Add("Fecha_Salida", typeof(DateTime));
            guestData.Columns.Add("Huespedes", typeof(int));
            guestData.Columns.Add("Finalidad", typeof(string));
            guestData.Columns.Add("Codigo_Finalidad", typeof(int));
        }

        private void SetupDataGridView()
        {
            dataGridView.DataSource = guestData;

            // Configure columns
            dataGridView.Columns["NRUA"].HeaderText = "NRUA";
            dataGridView.Columns["NRUA"].Width = 300;
            dataGridView.Columns["NRUA"].ToolTipText = "Ejemplo: ESHFTU00004501300027277450000000000000000000000000008";

            dataGridView.Columns["Fecha_Entrada"].HeaderText = "Check-in";
            dataGridView.Columns["Fecha_Entrada"].DefaultCellStyle.Format = "dd/MM/yyyy";

            dataGridView.Columns["Fecha_Salida"].HeaderText = "Check-out";
            dataGridView.Columns["Fecha_Salida"].DefaultCellStyle.Format = "dd/MM/yyyy";

            dataGridView.Columns["Huespedes"].HeaderText = "Nº Huéspedes";
            dataGridView.Columns["Huespedes"].Width = 100;

            // Finalidad dropdown
            var finalidadCol = new DataGridViewComboBoxColumn
            {
                Name = "Finalidad",
                HeaderText = "Finalidad",
                DataSource = new[]
                {
                    "Vacacional/Turístico",
                    "Laboral",
                    "Estudios",
                    "Causas médicas",
                    "Otros"
                },
                DataPropertyName = "Finalidad"
            };
            dataGridView.Columns.Remove("Finalidad");
            dataGridView.Columns.Insert(4, finalidadCol);

            dataGridView.Columns["Codigo_Finalidad"].Visible = false;

            // Event handlers
            dataGridView.CellValueChanged += DataGridView_CellValueChanged;
            dataGridView.RowsAdded += (s, e) => UpdateRecordCount();
            dataGridView.RowsRemoved += (s, e) => UpdateRecordCount();
        }

        private void DataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Update Codigo_Finalidad based on Finalidad
            if (e.ColumnIndex == dataGridView.Columns["Finalidad"].Index)
            {
                var row = dataGridView.Rows[e.RowIndex];
                var finalidad = row.Cells["Finalidad"].Value?.ToString();
                
                int codigo = finalidad switch
                {
                    "Vacacional/Turístico" => 1,
                    "Laboral" => 2,
                    "Estudios" => 3,
                    "Causas médicas" => 4,
                    "Otros" => 5,
                    _ => 1
                };

                row.Cells["Codigo_Finalidad"].Value = codigo;
            }
        }

        private void UpdateRecordCount()
        {
            var count = guestData.Rows.Count;
            var lblCount = this.Controls.Find("lblRecordCount", true).FirstOrDefault() as Label;
            if (lblCount != null)
            {
                lblCount.Text = $"Registros: {count}";
            }
        }

        private void NewFile()
        {
            if (MessageBox.Show("¿Crear nuevo archivo? Los datos no guardados se perderán.", 
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                guestData.Clear();
                currentFilePath = "";
                statusLabel.Text = "Nuevo archivo creado";
            }
        }

        private void OpenCSV()
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                dialog.Title = "Abrir archivo CSV";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        ImportCSV(dialog.FileName);
                        currentFilePath = dialog.FileName;
                        statusLabel.Text = $"Archivo abierto: {Path.GetFileName(dialog.FileName)}";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al abrir archivo: {ex.Message}", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ImportCSV(string filePath)
        {
            guestData.Clear();
            int imported = 0;
            int errors = 0;
            var errorLines = new List<string>();

            var lines = File.ReadAllLines(filePath, Encoding.UTF8);
            bool hasHeader = lines.Length > 0 && lines[0].ToUpper().Contains("NRUA");
            int startLine = hasHeader ? 1 : 0;

            for (int i = startLine; i < lines.Length; i++)
            {
                var parts = lines[i].Split(';');
                if (parts.Length < 4) continue;

                try
                {
                    var row = guestData.NewRow();
                    row["NRUA"] = parts[0].Trim();
                    
                    row["Fecha_Entrada"] = ParseDate(parts[1].Trim());
                    
                    if (!string.IsNullOrWhiteSpace(parts[2]))
                        row["Fecha_Salida"] = ParseDate(parts[2].Trim());
                    
                    row["Huespedes"] = int.Parse(parts[3].Trim());
                    
                    int codigoFinalidad = 1;
                    if (parts.Length > 4 && int.TryParse(parts[4].Trim(), out int codigo))
                    {
                        codigoFinalidad = codigo;
                    }
                    
                    row["Codigo_Finalidad"] = codigoFinalidad;
                    row["Finalidad"] = GetFinalidadText(codigoFinalidad);

                    guestData.Rows.Add(row);
                    imported++;
                }
                catch (Exception ex)
                {
                    errors++;
                    errorLines.Add($"Línea {i + 1}: {ex.Message}");
                }
            }

            MessageBox.Show($"Importación completada:\n\n" +
                          $"✓ Registros importados: {imported}\n" +
                          $"✗ Errores: {errors}" +
                          (errors > 0 ? "\n\n" + string.Join("\n", errorLines.Take(5)) : ""),
                          "Resultado de Importación", 
                          MessageBoxButtons.OK, 
                          errors > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
        }

        private DateTime ParseDate(string dateStr)
        {
            var formats = new[] { "dd/MM/yyyy", "dd-MM-yyyy", "dd.MM.yyyy", "yyyy-MM-dd" };
            return DateTime.ParseExact(dateStr, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
        }

        private string GetFinalidadText(int codigo)
        {
            return codigo switch
            {
                1 => "Vacacional/Turístico",
                2 => "Laboral",
                3 => "Estudios",
                4 => "Causas médicas",
                5 => "Otros",
                _ => "Vacacional/Turístico"
            };
        }

        private void SaveFile()
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                SaveFileAs();
            }
            else
            {
                SaveToCSV(currentFilePath);
            }
        }

        private void SaveFileAs()
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "CSV files (*.csv)|*.csv";
                dialog.Title = "Guardar archivo";
                dialog.FileName = $"guests_{DateTime.Now:yyyyMMdd}.csv";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    SaveToCSV(dialog.FileName);
                    currentFilePath = dialog.FileName;
                }
            }
        }

        private void SaveToCSV(string filePath)
        {
            try
            {
                using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    writer.WriteLine("NRUA;checkin;checkout;huespedes;codigo_finalidad");
                    
                    foreach (DataRow row in guestData.Rows)
                    {
                        var nrua = row["NRUA"];
                        var checkin = row["Fecha_Entrada"] is DateTime dt1 ? dt1.ToString("dd/MM/yyyy") : "";
                        var checkout = row["Fecha_Salida"] is DateTime dt2 ? dt2.ToString("dd/MM/yyyy") : "";
                        var guests = row["Huespedes"];
                        var finalidad = row["Codigo_Finalidad"];

                        writer.WriteLine($"{nrua};{checkin};{checkout};{guests};{finalidad}");
                    }
                }

                statusLabel.Text = "Archivo guardado correctamente";
                MessageBox.Show("Archivo guardado correctamente", "Éxito", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportForN2()
        {
            if (guestData.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar", "Aviso", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var errors = ValidateData();
            if (errors.Count > 0)
            {
                var result = MessageBox.Show(
                    $"Se encontraron {errors.Count} errores de validación.\n\n" +
                    "¿Desea exportar de todos modos?\n\n" +
                    string.Join("\n", errors.Take(3)),
                    "Errores de Validación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.No)
                    return;
            }

            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "CSV files (*.csv)|*.csv";
                dialog.Title = "Exportar para N2";
                dialog.FileName = $"N2_export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    SaveToCSV(dialog.FileName);
                    
                    MessageBox.Show(
                        "Archivo exportado correctamente.\n\n" +
                        "Ahora puede:\n" +
                        "1. Abrir la aplicación N2\n" +
                        "2. Ir a Formulario → Importar datos...\n" +
                        "3. Seleccionar este archivo CSV\n" +
                        "4. Revisar y confirmar la importación",
                        "Exportación Exitosa",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        }

        private List<string> ValidateData()
        {
            var errors = new List<string>();
            int rowNum = 1;

            foreach (DataRow row in guestData.Rows)
            {
                // NRUA validation
                if (row["NRUA"] == DBNull.Value || string.IsNullOrWhiteSpace(row["NRUA"].ToString()))
                {
                    errors.Add($"Fila {rowNum}: NRUA vacío");
                }
                else
                {
                    var nrua = row["NRUA"].ToString();
                    if (nrua.Length != 56)
                    {
                        errors.Add($"Fila {rowNum}: NRUA debe tener 56 caracteres");
                    }
                }

                // Check-in validation
                if (row["Fecha_Entrada"] == DBNull.Value)
                {
                    errors.Add($"Fila {rowNum}: Fecha de entrada requerida");
                }
                else
                {
                    var checkin = (DateTime)row["Fecha_Entrada"];
                    if (checkin.Year < 2020 || checkin.Year > 2030)
                    {
                        errors.Add($"Fila {rowNum}: Fecha de entrada sospechosa ({checkin:dd/MM/yyyy})");
                    }
                }

                // Check-out validation
                if (row["Fecha_Salida"] != DBNull.Value)
                {
                    var checkout = (DateTime)row["Fecha_Salida"];
                    var checkin = row["Fecha_Entrada"] is DateTime dt ? dt : DateTime.MinValue;
                    
                    if (checkout < checkin)
                    {
                        errors.Add($"Fila {rowNum}: Fecha de salida anterior a fecha de entrada");
                    }
                }

                // Guests validation
                if (row["Huespedes"] == DBNull.Value)
                {
                    errors.Add($"Fila {rowNum}: Número de huéspedes requerido");
                }
                else
                {
                    var guests = Convert.ToInt32(row["Huespedes"]);
                    if (guests < 1 || guests > 50)
                    {
                        errors.Add($"Fila {rowNum}: Número de huéspedes sospechoso ({guests})");
                    }
                }

                rowNum++;
            }

            if (errors.Count > 0)
            {
                MessageBox.Show(
                    $"Encontrados {errors.Count} errores:\n\n" +
                    string.Join("\n", errors.Take(10)) +
                    (errors.Count > 10 ? $"\n\n... y {errors.Count - 10} más" : ""),
                    "Errores de Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show("✓ Todos los datos son válidos", "Validación Exitosa",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            return errors;
        }

        private void AddNewRecord()
        {
            var row = guestData.NewRow();
            row["NRUA"] = "";
            row["Fecha_Entrada"] = DateTime.Today;
            row["Fecha_Salida"] = DateTime.Today.AddDays(7);
            row["Huespedes"] = 2;
            row["Finalidad"] = "Vacacional/Turístico";
            row["Codigo_Finalidad"] = 1;
            guestData.Rows.Add(row);
        }

        private void DeleteSelected()
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                var result = MessageBox.Show(
                    $"¿Eliminar {dataGridView.SelectedRows.Count} registro(s)?",
                    "Confirmar",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    foreach (DataGridViewRow row in dataGridView.SelectedRows)
                    {
                        if (!row.IsNewRow)
                        {
                            dataGridView.Rows.Remove(row);
                        }
                    }
                }
            }
        }

        private bool CheckLicense()
        {
            // Try to read stored license key
            string storedKey = LicenseValidator.ReadStoredKey();
            if (storedKey != null && LicenseValidator.IsValidKey(storedKey))
            {
                return true;
            }

            // Prompt user for license key
            return ShowLicenseDialog();
        }

        private bool ShowLicenseDialog()
        {
            using var dialog = new Form
            {
                Text = "NRUA Guest Manager - Activación de Licencia",
                Size = new Size(500, 250),
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var lblPrompt = new Label
            {
                Text = "Introduzca su clave de licencia:",
                Location = new Point(20, 20),
                Size = new Size(440, 20)
            };

            var lblFormat = new Label
            {
                Text = "Formato: NRUA-XXXX-XXXX-XXXX-XXXX",
                Location = new Point(20, 45),
                Size = new Size(440, 20),
                ForeColor = Color.Gray
            };

            var txtKey = new TextBox
            {
                Location = new Point(20, 75),
                Size = new Size(340, 25),
                Font = new Font("Consolas", 12),
                CharacterCasing = CharacterCasing.Upper
            };

            var lblStatus = new Label
            {
                Location = new Point(20, 110),
                Size = new Size(440, 20),
                ForeColor = Color.Red
            };

            var btnActivate = new Button
            {
                Text = "Activar",
                Location = new Point(370, 73),
                Size = new Size(90, 30)
            };

            var btnCancel = new Button
            {
                Text = "Cancelar",
                Location = new Point(370, 110),
                Size = new Size(90, 30),
                DialogResult = DialogResult.Cancel
            };

            bool activated = false;

            btnActivate.Click += (s, e) =>
            {
                string key = txtKey.Text.Trim();
                if (LicenseValidator.IsValidKey(key))
                {
                    LicenseValidator.SaveKey(key);
                    activated = true;
                    dialog.DialogResult = DialogResult.OK;
                    dialog.Close();
                }
                else
                {
                    lblStatus.Text = "Clave de licencia no válida. Verifique e intente de nuevo.";
                    lblStatus.ForeColor = Color.Red;
                }
            };

            dialog.Controls.AddRange(new Control[] { lblPrompt, lblFormat, txtKey, lblStatus, btnActivate, btnCancel });
            dialog.AcceptButton = btnActivate;
            dialog.CancelButton = btnCancel;
            dialog.ShowDialog();

            return activated;
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var form = new MainForm();
            if (!form.CheckLicense())
            {
                MessageBox.Show(
                    "Se requiere una licencia válida para usar NRUA Guest Manager.",
                    "Licencia requerida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            form.Text = "NRUA Guest Manager - Gestor de Huéspedes [Licenciado]";
            Application.Run(form);
        }
    }
}
