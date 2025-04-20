using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        private Color activeColor = Color.Yellow;
        private Color inactiveColor = Color.White;

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

        private void ConfigureDataGridView()
        {
            // Configure DataGridView for editing
            dataGridView1.EditMode = DataGridViewEditMode.EditOnEnter;
            dataGridView1.AllowUserToAddRows = false; // Disable adding new rows
            dataGridView1.AllowUserToDeleteRows = true;
            dataGridView1.MultiSelect = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // We'll add the delete button column in LoadTableData instead
            // to avoid duplicate columns

            // Handle cell click event for delete functionality
            dataGridView1.CellClick += DataGridView1_CellClick;

            // Handle cell value changed event to track modifications
            dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;
        }

        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // This event fires when a cell's value has been changed
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                try
                {
                    // Visual indication that the row has pending changes
                    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightYellow;
                }
                catch (Exception ex)
                {
                    // Just log the error and continue - don't crash the app
                    Console.WriteLine($"Error in CellValueChanged: {ex.Message}");
                }
            }
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the clicked cell is in the Delete button column
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 &&
                dataGridView1.Columns[e.ColumnIndex].Name == "DeleteButton")
            {
                if (MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        // Get the primary key value for this row before deleting
                        string primaryKeyColumn = GetPrimaryKeyColumn(currentTable);

                        if (string.IsNullOrEmpty(primaryKeyColumn))
                        {
                            MessageBox.Show("Could not determine primary key for deletion.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Make sure we can access this as a DataRowView
                        if (dataGridView1.Rows[e.RowIndex].DataBoundItem is DataRowView rowView)
                        {
                            // Get primary key value
                            object keyValue = rowView[primaryKeyColumn];

                            // Delete from database directly
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
                                        // Successfully deleted from database
                                        // Now remove from DataTable and update the view
                                        DataRow row = rowView.Row;
                                        row.Delete();
                                        currentDataTable.AcceptChanges();

                                        // Refresh display and counts
                                        LoadDashboardCounts();

                                        MessageBox.Show("Record deleted successfully.",
                                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                    else
                                    {
                                        MessageBox.Show("Failed to delete the record.",
                                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting record: {ex.Message}",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // Helper method to get the primary key column name for a table
        private string GetPrimaryKeyColumn(string tableName)
        {
            switch (tableName.ToLower())
            {
                case "feedback":
                    return "feedback_id";
                case "staff":
                    return "staff_id";
                case "train":
                    return "train_id";
                case "passenger":
                    return "passenger_id";
                case "ticket":
                    return "ticket_id";
                case "admin":
                    return "admin_id";
                default:
                    return "";
            }
        }

        private void Form6_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;

            LoadDashboardCounts();

            Button dashboardButton = GetDashboardButton();
            if (dashboardButton != null)
            {
                SetActiveButton(dashboardButton);
            }
            else
            {
                MessageBox.Show("Dashboard button not found!");
            }

            dataGridView1.Visible = false;
        }

        private void LoadDashboardCounts()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Load staff count
                    string query = "SELECT COUNT(staff_id) AS staff_count FROM staff";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        c = Convert.ToInt32(result);
                        textBox3.Text = c.ToString();
                    }

                    // Load train count
                    query = "SELECT COUNT(train_id) AS train_count FROM train";
                    cmd = new MySqlCommand(query, conn);
                    result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        b = Convert.ToInt32(result);
                        textBox2.Text = b.ToString();
                    }

                    // Load passenger count
                    query = "SELECT COUNT(passenger_id) AS passenger_count FROM passenger";
                    cmd = new MySqlCommand(query, conn);
                    result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        a = Convert.ToInt32(result);
                        textBox1.Text = a.ToString();
                    }

                    // Load admin count
                    query = "SELECT COUNT(admin_id) AS admin_count FROM admin";
                    cmd = new MySqlCommand(query, conn);
                    result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        d = Convert.ToInt32(result);
                        textBox4.Text = d.ToString();
                    }
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
                MessageBox.Show("No data loaded to update.", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            MySqlConnection conn = null;

            try
            {
                conn = new MySqlConnection(connectionString);
                conn.Open();

                // Ensure primary key exists
                if (currentDataTable.PrimaryKey.Length == 0)
                {
                    // Try to set primary key automatically (assuming first column is ID)
                    DataColumn[] keyColumns = new DataColumn[1];
                    keyColumns[0] = currentDataTable.Columns[0]; // adjust if needed
                    currentDataTable.PrimaryKey = keyColumns;
                }

                // Get the changes
                DataTable changes = currentDataTable.GetChanges();

                if (changes == null || changes.Rows.Count == 0)
                {
                    MessageBox.Show("No changes detected to save.", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Recreate the adapter with select command
                string selectQuery = $"SELECT * FROM `{currentTable}`"; // safe for simple queries
                currentAdapter = new MySqlDataAdapter(selectQuery, conn);

                // Use command builder to auto-generate update/insert/delete commands
                MySqlCommandBuilder builder = new MySqlCommandBuilder(currentAdapter);
                builder.ConflictOption = ConflictOption.OverwriteChanges;

                currentAdapter.UpdateCommand = builder.GetUpdateCommand();
                currentAdapter.InsertCommand = builder.GetInsertCommand();
                currentAdapter.DeleteCommand = builder.GetDeleteCommand();

                // Apply changes
                int rowsAffected = currentAdapter.Update(changes);
                currentDataTable.AcceptChanges();

                LoadDashboardCounts(); // optional refresh

                MessageBox.Show($"Successfully updated {rowsAffected} record(s).", "Update Complete",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Reset row colors
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving changes: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn != null && conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }
        }

        //private void button6_Click(object sender, EventArgs e)
        //{
        //    if (!dataLoaded || currentDataTable == null || string.IsNullOrEmpty(currentTable))
        //    {
        //        MessageBox.Show("No data loaded to update.", "Information",
        //            MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }

        //    MySqlConnection conn = null;
        //    try
        //    {
        //        // Create connection but don't use 'using' statement
        //        conn = new MySqlConnection(connectionString);
        //        conn.Open();

        //        // Get rows with changes
        //        DataTable changes = currentDataTable.GetChanges();

        //        if (changes == null || changes.Rows.Count == 0)
        //        {
        //            MessageBox.Show("No changes detected to save.", "Information",
        //                MessageBoxButtons.OK, MessageBoxIcon.Information);
        //            return;
        //        }

        //        // Create command builder for automatic update query generation
        //        MySqlCommandBuilder builder = new MySqlCommandBuilder(currentAdapter);

        //        // Update the database with changes
        //        int rowsAffected = currentAdapter.Update(changes);

        //        // Accept the changes in the original DataTable
        //        currentDataTable.AcceptChanges();

        //        // Refresh counts after update
        //        LoadDashboardCounts();

        //        MessageBox.Show($"Successfully updated {rowsAffected} record(s).", "Update Complete",
        //            MessageBoxButtons.OK, MessageBoxIcon.Information);

        //        // Reset background colors
        //        foreach (DataGridViewRow row in dataGridView1.Rows)
        //        {
        //            row.DefaultCellStyle.BackColor = Color.White;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Error saving changes: {ex.Message}", "Error",
        //            MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //    finally
        //    {
        //        // Make sure to close the connection in a finally block
        //        if (conn != null && conn.State != ConnectionState.Closed)
        //        {
        //            conn.Close();
        //        }
        //    }
        //}

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
                currentButton.ForeColor = Color.Black;
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

                    // Clear any existing columns and data
                    dataGridView1.DataSource = null;
                    dataGridView1.Columns.Clear();

                    string query = $"SELECT * FROM {tableName}";

                    // Create a new adapter for the table
                    currentAdapter = new MySqlDataAdapter(query, conn);
                    currentDataTable = new DataTable();
                    currentAdapter.Fill(currentDataTable);

                    // Set primary key for the data table to enable proper updates
                    string pkColumn = GetPrimaryKeyColumn(tableName);
                    if (!string.IsNullOrEmpty(pkColumn) && currentDataTable.Columns.Contains(pkColumn))
                    {
                        currentDataTable.PrimaryKey = new DataColumn[] { currentDataTable.Columns[pkColumn] };
                    }

                    // Bind data to grid
                    dataGridView1.DataSource = currentDataTable;

                    // Add delete button column after binding data
                    DataGridViewButtonColumn deleteButtonColumn = new DataGridViewButtonColumn();
                    deleteButtonColumn.HeaderText = "Delete";
                    deleteButtonColumn.Text = "Delete";
                    deleteButtonColumn.UseColumnTextForButtonValue = true;
                    deleteButtonColumn.Name = "DeleteButton";
                    dataGridView1.Columns.Add(deleteButtonColumn);

                    dataGridView1.Visible = true;

                    // Store current table name
                    currentTable = tableName;
                    dataLoaded = true;

                    // Adjust column widths for better viewing
                    dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetActiveButton((Button)sender);
            LoadTableData("ticket");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SetActiveButton((Button)sender);
            LoadTableData("staff");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SetActiveButton((Button)sender);
            LoadTableData("train");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SetActiveButton((Button)sender);
            LoadTableData("passenger");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            SetActiveButton((Button)sender);
            LoadTableData("feedback");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            SetActiveButton((Button)sender);
            dataGridView1.Visible = false;
            dataLoaded = false;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // This is now handled by the DataGridView1_CellClick method
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Left empty intentionally
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            // Left empty intentionally
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // Left empty intentionally
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            // Left empty intentionally
        }
    }
}