using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Project1
{
    public partial class Form6 : Form
    {
        private Button currentButton;
        private Color activeColor = Color.Gold;
        private Color inactiveColor = Color.FromArgb(30, 60, 170); // Railway blue

        int a, b, c, d;
        String uname;
        private string currentTable = "";
        private DataTable currentDataTable;
        private bool dataLoaded = false;
        private MySqlDataAdapter currentAdapter;
        private string connectionString = "Server=res-crypt.southindia.cloudapp.azure.com;Port=3306;Database=railway;Uid=dbs;Pwd=salted_password_#;";

        public Form6()
        {
            InitializeComponent();
            ConfigureDataGridView();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle,
                Color.FromArgb(20, 51, 2),    // Dark railway blue
                Color.FromArgb(102, 178, 255), // Light sky blue
                90F)) // Vertical gradient
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }

        private void ConfigureDataGridView()
        {
            dataGridView1.EditMode = DataGridViewEditMode.EditOnEnter;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = true;
            dataGridView1.MultiSelect = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.CellClick += DataGridView1_CellClick;
            dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;
        }

        private void Form6_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;

            StyleButtons();
            StyleTextBoxes();

            LoadDashboardCounts();

            Button dashboardButton = GetDashboardButton();
            if (dashboardButton != null)
                SetActiveButton(dashboardButton);
            else
                MessageBox.Show("Dashboard button not found!");

            dataGridView1.Visible = false;
        }

        private void StyleButtons()
        {
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Button btn)
                {
                    btn.BackColor = inactiveColor;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.ForeColor = Color.White;
                    btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                    btn.FlatAppearance.BorderSize = 0;
                    btn.Cursor = Cursors.Hand;
                }
            }
        }

        private void StyleTextBoxes()
        {
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is TextBox tb)
                {
                    tb.BorderStyle = BorderStyle.FixedSingle;
                    tb.BackColor = Color.WhiteSmoke;
                    tb.Font = new Font("Segoe UI", 10, FontStyle.Regular);
                }
            }
        }

        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                try
                {
                    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightYellow;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in CellValueChanged: {ex.Message}");
                }
            }
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 &&
                dataGridView1.Columns[e.ColumnIndex].Name == "DeleteButton")
            {
                if (MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        string primaryKeyColumn = GetPrimaryKeyColumn(currentTable);
                        if (string.IsNullOrEmpty(primaryKeyColumn)) return;

                        if (dataGridView1.Rows[e.RowIndex].DataBoundItem is DataRowView rowView)
                        {
                            object keyValue = rowView[primaryKeyColumn];

                            using (MySqlConnection conn = new MySqlConnection(connectionString))
                            {
                                conn.Open();
                                string deleteQuery = $"DELETE FROM {currentTable} WHERE {primaryKeyColumn} = @id";
                                using (MySqlCommand cmd = new MySqlCommand(deleteQuery, conn))
                                {
                                    cmd.Parameters.AddWithValue("@id", keyValue);
                                    int rowsAffected = cmd.ExecuteNonQuery();

                                    if (rowsAffected > 0)
                                    {
                                        DataRow row = rowView.Row;
                                        row.Delete();
                                        currentDataTable.AcceptChanges();
                                        LoadDashboardCounts();
                                        MessageBox.Show("Record deleted successfully.");
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting record: {ex.Message}");
                    }
                }
            }
        }

        private string GetPrimaryKeyColumn(string tableName)
        {
            switch (tableName.ToLower())
            {
                case "feedback": return "feedback_id";
                case "staff": return "staff_id";
                case "train": return "train_id";
                case "passenger": return "passenger_id";
                case "ticket": return "ticket_id";
                case "admin": return "admin_id";
                default: return "";
            }
        }

        private void LoadDashboardCounts()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query;
                    MySqlCommand cmd;
                    object result;

                    query = "SELECT COUNT(staff_id) FROM staff";
                    cmd = new MySqlCommand(query, conn);
                    result = cmd.ExecuteScalar();
                    c = Convert.ToInt32(result);
                    textBox3.Text = c.ToString();

                    query = "SELECT COUNT(train_id) FROM train";
                    cmd = new MySqlCommand(query, conn);
                    result = cmd.ExecuteScalar();
                    b = Convert.ToInt32(result);
                    textBox2.Text = b.ToString();

                    query = "SELECT COUNT(passenger_id) FROM passenger";
                    cmd = new MySqlCommand(query, conn);
                    result = cmd.ExecuteScalar();
                    a = Convert.ToInt32(result);
                    textBox1.Text = a.ToString();

                    query = "SELECT COUNT(admin_id) FROM admin";
                    cmd = new MySqlCommand(query, conn);
                    result = cmd.ExecuteScalar();
                    d = Convert.ToInt32(result);
                    textBox4.Text = d.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading dashboard counts: " + ex.Message);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (!dataLoaded || currentDataTable == null || string.IsNullOrEmpty(currentTable))
            {
                MessageBox.Show("No data loaded to update.");
                return;
            }

            MySqlConnection conn = null;

            try
            {
                conn = new MySqlConnection(connectionString);
                conn.Open();

                if (currentDataTable.PrimaryKey.Length == 0)
                    currentDataTable.PrimaryKey = new DataColumn[] { currentDataTable.Columns[0] };

                DataTable changes = currentDataTable.GetChanges();

                if (changes == null || changes.Rows.Count == 0)
                {
                    MessageBox.Show("No changes detected.");
                    return;
                }

                string selectQuery = $"SELECT * FROM `{currentTable}`";
                currentAdapter = new MySqlDataAdapter(selectQuery, conn);
                MySqlCommandBuilder builder = new MySqlCommandBuilder(currentAdapter);

                currentAdapter.Update(changes);
                currentDataTable.AcceptChanges();

                LoadDashboardCounts();
                MessageBox.Show("Changes saved successfully.");

                foreach (DataGridViewRow row in dataGridView1.Rows)
                    row.DefaultCellStyle.BackColor = Color.White;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
            finally
            {
                if (conn != null && conn.State != ConnectionState.Closed)
                    conn.Close();
            }
        }

        private Button GetDashboardButton()
        {
            return this.Controls.Find("button8", true).FirstOrDefault() as Button;
        }

        private void SetActiveButton(Button button)
        {
            if (button == null) return;

            if (currentButton != null)
            {
                currentButton.BackColor = inactiveColor;
                currentButton.ForeColor = Color.White;
            }

            button.BackColor = activeColor;
            button.ForeColor = Color.Black;
            currentButton = button;
        }

        private void LoadTableData(string tableName)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    dataGridView1.DataSource = null;
                    dataGridView1.Columns.Clear();

                    string query = $"SELECT * FROM {tableName}";
                    currentAdapter = new MySqlDataAdapter(query, conn);
                    currentDataTable = new DataTable();
                    currentAdapter.Fill(currentDataTable);

                    string pkColumn = GetPrimaryKeyColumn(tableName);
                    if (!string.IsNullOrEmpty(pkColumn) && currentDataTable.Columns.Contains(pkColumn))
                        currentDataTable.PrimaryKey = new DataColumn[] { currentDataTable.Columns[pkColumn] };

                    dataGridView1.DataSource = currentDataTable;

                    DataGridViewButtonColumn deleteButtonColumn = new DataGridViewButtonColumn
                    {
                        HeaderText = "Delete",
                        Text = "Delete",
                        UseColumnTextForButtonValue = true,
                        Name = "DeleteButton"
                    };
                    dataGridView1.Columns.Add(deleteButtonColumn);

                    dataGridView1.Visible = true;
                    currentTable = tableName;
                    dataLoaded = true;
                    dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e) => LoadAndSetButton((Button)sender, "ticket");
        private void button2_Click(object sender, EventArgs e) => LoadAndSetButton((Button)sender, "staff");
        private void button3_Click(object sender, EventArgs e) => LoadAndSetButton((Button)sender, "train");
        private void button4_Click(object sender, EventArgs e) => LoadAndSetButton((Button)sender, "passenger");
        private void button7_Click(object sender, EventArgs e) => LoadAndSetButton((Button)sender, "feedback");
        private void button8_Click(object sender, EventArgs e)
        {
            SetActiveButton((Button)sender);
            dataGridView1.Visible = false;
            dataLoaded = false;
        }
        private void button5_Click(object sender, EventArgs e) => this.Close();

        private void LoadAndSetButton(Button sender, string tableName)
        {
            SetActiveButton(sender);
            LoadTableData(tableName);
        }

        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void textBox3_TextChanged(object sender, EventArgs e) { }
        private void textBox4_TextChanged(object sender, EventArgs e) { }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
    }
}
