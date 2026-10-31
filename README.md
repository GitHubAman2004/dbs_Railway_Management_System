# Railway Management System

A desktop app for booking train tickets and managing a railway, built as the **Database Systems (DBS)** course project for B.Tech Information Technology at **Manipal Institute of Technology**.

- **Front end:** Windows Forms app written in C# (.NET Framework 4.7.2)
- **Back end:** MySQL 8 database, with stored procedures and triggers
- **Team:** Aman Garg, Rishabh Bachhawat, Anikait Chitlangia
- **Guides:** Dr. Sumith N, Mrs. Swathi B P (Department of I&CT)

Passengers can sign up, log in, search trains between two stations, and book a ticket. The app picks a free seat and a platform automatically. Admins get a dashboard where they can view, edit, and delete records in every table.

---

## What is in this repository

| Path | What it is |
|---|---|
| `Project1/` | The C# Windows Forms app (open `Project1.sln` in Visual Studio) |
| `database/final.sql` | **The full database script.** It creates every table, the stored procedures and the triggers, and fills in sample data |
| `database/railway_schema.mwb` | MySQL Workbench model of the final database design |
| `database/er_dig.mwb` | MySQL Workbench model of the ER diagram |
| `docs/Final_report-RailwayMgmtSystem.pdf` | Project report, including the SRS (Software Requirements Specification), ER diagram, normalization and screenshots |

> **How to open `.mwb` files:** install [MySQL Workbench](https://dev.mysql.com/downloads/workbench/), then use **File → Open Model**. An `.mwb` file is a saved diagram of the database (tables, columns and the lines connecting them).

---

## Screens in the app

| Form | Title | What it does |
|---|---|---|
| `Form1` | Login | Passenger login, admin login, and buttons to sign up or exit |
| `Form5` | New User Login | Sign up as a new passenger (calls the `SignupUser` procedure) |
| `Form2` | Travel Details | Choose the from station, to station and date |
| `Form3` | Train Details | Pick a train and a seat class from the trains running that day |
| `Form4` | Passenger Details | Enter passenger details and book. The app finds a free seat, creates the ticket and shows the platform |
| `Form6` | Admin Portal | Counts of passengers, trains, staff and admins, plus an editable grid for each table |
| `Form7` | Feedback | Sends passenger feedback to the `feedback` table |
| `Form8` | — | Lists the admins (opened from the **Exit** button on the login screen) |

---

## Database design

### Tables

| Table | Holds |
|---|---|
| `admin` | Admin accounts (username, hashed password, name, contact) |
| `passenger` | Passenger accounts. Passengers log in with their **name** as the username |
| `train` | Trains (name, type, capacity) |
| `station` | Stations (name, city) |
| `platform` | Platform numbers at each station |
| `schedule` | Which train runs from which station to which station, on which weekday and at what time |
| `seat` | Seats on each train, with class (AC First, AC Second, Sleeper, …) |
| `ticket` | Bookings. Each one links a passenger, a seat and a journey date. One seat can only be booked once per date |
| `feedback` | Rating (1–5) and comments for a ticket |
| `staff` | Railway staff (name, role, contact) |
| `staff_train`, `staff_station` | Which staff member works on which train or at which station |
| `operation_log` | An automatic history of changes, filled by the triggers below |

The design is normalized up to **3NF (Third Normal Form)**, meaning no repeated or duplicated data. Chapter 4 of the report shows each step.

### Stored procedures
A stored procedure is a small program saved inside the database that the app can call by name.

- **`CheckLogin(username, hashed_password)`** returns `2` for an admin, `1` for a passenger and `0` if the login is wrong.
- **`SignupUser(username, contact, email, password)`** adds a new passenger and stores the password as a **SHA-256 hash**. A hash is a scrambled one-way version of the password, so the real password is never saved.
- **`LogOperation(...)`** writes one row into `operation_log`. The triggers use it.

### Triggers
A trigger runs automatically when data changes. Six triggers record every insert, update and delete on the `passenger` and `ticket` tables into `operation_log`.

---

## Setting up the database

### 1. Get a MySQL server
Any MySQL 8.x server works: one on your own PC, a cloud virtual machine, or a managed cloud database. The project currently uses **AWS RDS**. See [Cloud hosting: AWS RDS](#cloud-hosting-aws-rds) below.

### 2. Fix the database name before importing ⚠️
`final.sql` creates a database called **`railway_fin`**, but the app connects to a database called **`railway`**. Change the first lines of the script from:

```sql
CREATE DATABASE IF NOT EXISTS railway_fin;
USE railway_fin;
```

to:

```sql
CREATE DATABASE IF NOT EXISTS railway;
USE railway;
```

### 3. Run the script
From a terminal, using the MySQL command-line client:

```bash
mysql -h <server-address> -P 3306 -u <username> -p < database/final.sql
```

You can also open `final.sql` in MySQL Workbench and run it there.

> **On AWS RDS (or any server with binary logging turned on),** MySQL blocks creating triggers unless the setting `log_bin_trust_function_creators` is `1`. On RDS, set it in a custom **DB parameter group** attached to the database. Otherwise the script stops at the triggers with *"You do not have the SUPER privilege and binary logging is enabled"*.

### 4. Point the app at the database
The connection string (the line that tells the app the server address, database name, username and password) is currently written separately inside each form (`Form1`–`Form8`). Change the `Server=`, `Uid=` and `Pwd=` values in all of them to match your server.

---

## Cloud hosting: AWS RDS

The database first ran on an **Azure** virtual machine. When the Azure free credits ran out (October 2026), it was moved to **Amazon RDS for MySQL**. RDS is AWS's managed database service, where AWS runs and backs up the database for you.

### Current setup

| Setting | Value | Why |
|---|---|---|
| Region | Asia Pacific (Mumbai), `ap-south-1` | Closest AWS region to India, so it's faster |
| Endpoint (server address) | `railway-db.c5gwki0cwy64.ap-south-1.rds.amazonaws.com` | Goes in `Server=` in the app's connection string |
| Port | `3306` | Standard MySQL port, already used by the app |
| Engine | MySQL Community **8.4.11** | Matches the `railway-params` parameter group |
| Instance class | `db.t4g.micro` (2 vCPU, 1 GiB RAM) | Free-tier size |
| Storage | 20 GiB General Purpose SSD (gp2), autoscaling **off** | Free-tier size. Autoscaling off so the disk can't grow and add charges |
| Deployment | Single-AZ (one data centre) | Enough for a project. Multi-AZ costs double |
| Master username | `dbs` | Same user the app already logs in with |
| Initial database | `railway` | The database name the app connects to |
| Parameter group | `railway-params` | Custom settings. `log_bin_trust_function_creators` must be `1` so the triggers can be created |
| Public access | Yes | The app runs on a PC outside AWS, so the database must be reachable over the internet |
| Security group (firewall) | `railway-db-sg` | Only allows chosen IP addresses on port 3306 |
| Automated backups | On, kept for 1 day | Free daily safety copy |
| Encryption | On (default `aws/rds` key) | Free, protects data stored on disk |
| Deletion protection | Off | So the database can be deleted easily when the project is over |

The database password is **not** stored in this repository.

### Migration progress

- [x] Database created on AWS RDS and showing **Available**
- [x] Firewall rule allows the developer's current IP address
- [ ] Confirm `log_bin_trust_function_creators = 1` in `railway-params`
- [ ] Load `database/final.sql` (with the database name changed to `railway`)
- [ ] Update the connection strings in the app to the new endpoint
- [ ] Test login, booking and admin screens against the new database

### Allowing a new computer or network ⚠️

The firewall only lets in specific internet (IP) addresses. Your address changes when you use a different Wi-Fi (for example college), and sometimes when the router restarts. If the app suddenly says *"Unable to connect to any of the specified MySQL hosts"*:

1. AWS Console → **Aurora and RDS → Databases → railway-db → Connectivity & security**.
2. Click the security group **railway-db-sg**.
3. **Inbound rules → Edit inbound rules**.
4. Change **Source** to **My IP** (or **Add rule** → Type **MySQL/Aurora** → Source **My IP** to keep the old one too) → **Save rules**.

### Stopping charges when the project is over

AWS → **Databases** → select `railway-db` → **Actions → Delete**. Take a final snapshot if you want to keep the data. Running costs are covered by the AWS Free plan credits until they run out. The public IP address is a small extra charge.

---

## Sample logins (from `final.sql`)

| Role | Username | Password |
|---|---|---|
| Admin | `admin_raj` (also `admin_priya`, `admin_amit`, `admin_neha`, `admin_vikram`) | `password` |
| Passenger | `Rahul Sharma` | `password` |
| Passenger | `Alok Mehta` | `12345` |
| Passenger | `Tanya Sen` | `password123` |

Use **Login Admin** for admin accounts and **Login** for passengers.

---

## Known issues

- **Database name mismatch:** see step 2 above.
- **`Form8` reads a different database, `railway_fill`.** `final.sql` doesn't create it, so that screen shows an error unless the database exists.
- **Saving feedback (`Form7`) may fail.** The app inserts only `comments`, but `feedback.ticket_id` is a required column with no default value. Also, no other screen opens `Form7` at the moment.
- **The database password is written in the source code.** Because this repository is public, change the password on the server and move the connection string into `Project1/App.config` instead.
