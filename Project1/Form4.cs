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
    public partial class Form4 : Form
    {
        string username, password, from, to, date, train, type;
        string name, age, gender, contact;
        // Connection string should match your database - update to railway_fin per your schema
        private readonly string connectionString = "Server=res-crypt.southindia.cloudapp.azure.com;Port=3306;Database=railway;Uid=dbs;Pwd=salted_password_#;";

        public Form4(string a, string b, string c, string d, string e, string f, string g)
        {
            username = a;
            password = b;
            from = c;
            to = d;
            date = e;
            train = f;
            type = g;

            InitializeComponent();
            this.DoubleBuffered = true;
            this.Paint += new PaintEventHandler(Form4_Paint);
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;

            // Style controls
            StyleTextBox(textBox1);
            StyleTextBox(textBox2);
            StyleTextBox(textBox3);
            StyleTextBox(textBox4);

            button1.BackColor = Color.FromArgb(40, 80, 120);
            button1.ForeColor = Color.White;
            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;
            button1.Font = new Font("Segoe UI", 12F, FontStyle.Bold);

            // Add a bold title label
            Label labelTitle = new Label();
            labelTitle.Text = "Passenger Details";
            labelTitle.Font = new Font("Segoe UI", 26F, FontStyle.Bold);
            labelTitle.ForeColor = Color.White;
            labelTitle.BackColor = Color.Transparent;
            labelTitle.AutoSize = true;
            labelTitle.Location = new Point(50, 30);
            this.Controls.Add(labelTitle);
        }

        private void Form4_Paint(object sender, PaintEventArgs e)
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle,
                Color.FromArgb(52, 73, 94),    // Dark blue-gray railway tone
                Color.FromArgb(149, 165, 166), // Light gray steel tone
                90F))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }

        private void StyleTextBox(TextBox tb)
        {
            tb.BackColor = Color.White;
            tb.ForeColor = Color.Black;
            tb.Font = new Font("Segoe UI", 10F);
            tb.BorderStyle = BorderStyle.FixedSingle;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            name = textBox1.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            age = textBox2.Text;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            gender = textBox3.Text;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            contact = textBox4.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(age) ||
                string.IsNullOrWhiteSpace(gender) ||
                string.IsNullOrWhiteSpace(contact))
            {
                MessageBox.Show("Please fill in all passenger details before proceeding.",
                                "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // First, get the passenger ID
                    int passengerId = GetPassengerId(conn);
                    if (passengerId == -1) return;

                    // Get train ID from train name
                    int trainId = GetTrainId(conn);
                    if (trainId == -1) return;

                    // Get source and destination station IDs
                    int srcStationId = GetStationId(conn, from);
                    int destStationId = GetStationId(conn, to);
                    if (srcStationId == -1 || destStationId == -1) return;

                    // Check if the train runs on this route on the selected date
                    if (!VerifySchedule(conn, trainId, srcStationId, destStationId, date))
                    {
                        MessageBox.Show("No schedule found for the selected train on this route and date.",
                                        "Schedule Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Find an available seat based on the train and class type
                    int seatId = FindAvailableSeat(conn, trainId, type, date);
                    if (seatId == -1)
                    {
                        MessageBox.Show("No seats available for the selected train and class on this date.",
                                        "Booking Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Create the ticket
                    int ticketId = CreateTicket(conn, passengerId, seatId, date);
                    if (ticketId == -1) return;

                    // Get platform number
                    int platformNumber = GetPlatformNumber(conn, srcStationId);

                    // Show success message with ticket details
                    MessageBox.Show("Ticket successfully booked!\n\n" +
                                    "Ticket ID: " + ticketId + "\n" +
                                    "Name: " + name + "\n" +
                                    "Age: " + age + "\n" +
                                    "Gender: " + gender + "\n" +
                                    "Contact: " + contact + "\n\n" +
                                    "From: " + from + "\n" +
                                    "To: " + to + "\n" +
                                    "Date: " + date + "\n" +
                                    "Train: " + train + "\n" +
                                    "Coach Type: " + type + "\n" +
                                    "Seat ID: " + seatId + "\n" +
                                    "Platform No.: " + platformNumber,
                                    "Booking Confirmed", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating ticket: " + ex.Message,
                                "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int GetPassengerId(MySqlConnection conn)
        {
            try
            {
                string query = "SELECT passenger_id FROM passenger WHERE name = @username";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        return Convert.ToInt32(result);
                    }
                    else
                    {
                        MessageBox.Show("Passenger not found with the provided username.",
                                        "User Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving passenger ID: " + ex.Message,
                                "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        private int GetTrainId(MySqlConnection conn)
        {
            try
            {
                string query = "SELECT train_id FROM train WHERE train_name = @trainName";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@trainName", train);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        return Convert.ToInt32(result);
                    }
                    else
                    {
                        MessageBox.Show("Train not found with the provided name.",
                                        "Train Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving train ID: " + ex.Message,
                                "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        private int GetStationId(MySqlConnection conn, string stationName)
        {
            try
            {
                string query = "SELECT station_id FROM station WHERE station_name = @stationName";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@stationName", stationName);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        return Convert.ToInt32(result);
                    }
                    else
                    {
                        MessageBox.Show("Station not found: " + stationName,
                                        "Station Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving station ID: " + ex.Message,
                                "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        private bool VerifySchedule(MySqlConnection conn, int trainId, int srcStationId, int destStationId, string journeyDate)
        {
            try
            {
                // Convert journey date to day of week
                DateTime journeyDateTime = DateTime.Parse(journeyDate);
                string dayOfWeek = journeyDateTime.ToString("ddd").Substring(0, 3);

                string query = "SELECT COUNT(*) FROM schedule WHERE train_id = @trainId " +
                               "AND src_station_id = @srcStationId AND dest_station_id = @destStationId " +
                               "AND day_of_week = @dayOfWeek";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@trainId", trainId);
                    cmd.Parameters.AddWithValue("@srcStationId", srcStationId);
                    cmd.Parameters.AddWithValue("@destStationId", destStationId);
                    cmd.Parameters.AddWithValue("@dayOfWeek", dayOfWeek);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error verifying schedule: " + ex.Message,
                                "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private int FindAvailableSeat(MySqlConnection conn, int trainId, string seatClass, string journeyDate)
        {
            try
            {
                // Query to find a seat that is not already booked for this date
                string query = @"
                    SELECT s.seat_id 
                    FROM seat s
                    WHERE s.train_id = @trainId 
                    AND s.class = @seatClass
                    AND s.seat_id NOT IN (
                        SELECT t.seat_id 
                        FROM ticket t 
                        WHERE t.journey_date = @journeyDate
                    )
                    ORDER BY s.seat_id
                    LIMIT 1";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@trainId", trainId);
                    cmd.Parameters.AddWithValue("@seatClass", seatClass);
                    cmd.Parameters.AddWithValue("@journeyDate", journeyDate);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        return Convert.ToInt32(result);
                    }

                    // No seat available
                    return -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error finding available seat: " + ex.Message,
                                "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        private int CreateTicket(MySqlConnection conn, int passengerId, int seatId, string journeyDate)
        {
            try
            {
                string query = @"
                    INSERT INTO ticket (passenger_id, journey_date, seat_id)
                    VALUES (@passengerId, @journeyDate, @seatId);
                    SELECT LAST_INSERT_ID();";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@passengerId", passengerId);
                    cmd.Parameters.AddWithValue("@journeyDate", journeyDate);
                    cmd.Parameters.AddWithValue("@seatId", seatId);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        return Convert.ToInt32(result);
                    }

                    MessageBox.Show("Failed to create ticket entry.",
                                    "Booking Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating ticket: " + ex.Message,
                                "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        private int GetPlatformNumber(MySqlConnection conn, int stationId)
        {
            try
            {
                string query = "SELECT platform_number FROM platform WHERE station_id = @stationId LIMIT 1";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@stationId", stationId);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        return Convert.ToInt32(result);
                    }

                    // Default platform if not found
                    return 1;
                }
            }
            catch
            {
                // Default platform on error
                return 1;
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {
            // Placeholder - optional
        }
    }
}