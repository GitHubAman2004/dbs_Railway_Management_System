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
| `database/demo.sql` | **Live demo script.** Runs every app feature as SQL in MySQL Workbench (see [Presenting the project](#presenting-the-project-with-mysql-workbench)) |
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

Or use **MySQL Workbench**:
1. Add a connection with the server address, port `3306` and username. Click **Test Connection**. If Workbench warns about an "incompatible/nonstandard server version" (it is tested up to MySQL 8.0), click **Continue Anyway**.
2. Open the connection, then **File → Open SQL Script…** and pick `final.sql`.
3. Make sure nothing is highlighted, then click the first **⚡** button **once** to run the whole file.
4. Every row in *Action Output* should be green. One yellow warning, *"Can't create database 'railway'; database exists"*, is fine if the database was already created (for example as the RDS "Initial database name").

Quick test that it worked. This should return `2`, meaning "valid admin":

```sql
CALL railway.CheckLogin('admin_raj', SHA2('password', 256));
```

> **On AWS RDS (or any server with binary logging turned on),** MySQL blocks creating triggers unless the setting `log_bin_trust_function_creators` is `1`. On RDS, set it in a custom **DB parameter group** attached to the database. Otherwise the script stops at the triggers with *"You do not have the SUPER privilege and binary logging is enabled"*.

### 4. Point the app at the database
The **connection string** is the line that tells the app the server address, database name, username and password. All forms read it from one place: `Project1/Db.cs`, which reads it from a file called **`Project1/ConnectionStrings.config`**.

That file holds the real password, so it is **not in this repository** (it's listed in `.gitignore`). On every computer that runs the app:

1. In the `Project1` folder, copy `ConnectionStrings.example.config` and name the copy `ConnectionStrings.config`.
2. Open it in any text editor and replace `YOUR_PASSWORD_HERE` with the real database password. Change `Server=` too if the database moved.
3. Build and run the app in Visual Studio. The build copies the file next to the `.exe` automatically.

If the file is missing, the app shows a *"Database not configured"* message when it starts, instead of crashing.

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
- [x] Firewall rule allows the developer's current IP address (MySQL/Aurora, TCP 3306)
- [x] Public access confirmed as **Publicly accessible**
- [x] `log_bin_trust_function_creators` set to `1` in `railway-params` (it was blank, meaning off, before)
- [x] Connected with MySQL Workbench from the developer's Mac
- [x] Loaded `database/final.sql` with the database name changed to `railway`: 13 tables, sample data, 3 stored procedures and 6 triggers, all without errors
- [x] Tested `CheckLogin('admin_raj', …)`, which returns `2` (valid admin)
- [x] App code switched to one shared connection string (`Db.cs` + `ConnectionStrings.config`) pointing at AWS
- [ ] Create `ConnectionStrings.config` with the real password on the computer that runs the app
- [ ] Test login, booking and admin screens in the app against the new database

### Allowing a new computer or network ⚠️

The firewall only lets in specific internet (IP) addresses. Your address changes when you use a different Wi-Fi (for example college), and sometimes when the router restarts. If the app or Workbench suddenly says *"Unable to connect to any of the specified MySQL hosts"* (a wrong password says *"Access denied"* instead):

1. Find the computer's real internet address. In a terminal, run `curl https://checkip.amazonaws.com`. Don't rely on a browser or the console's **My IP** option, because iCloud Private Relay or a VPN can show a different address than the one the app really uses.
2. AWS Console → **Aurora and RDS → Databases → railway-db → Connectivity & security**, scroll to **Security group rules**, then click **railway-db-sg**.
3. **Inbound rules → Edit inbound rules**.
4. Set **Source** to **Custom** and enter the address, then **Save rules**. Use **Add rule** (Type **MySQL/Aurora**) to keep the old address too.

**Why the rule uses a `/24` range:** the home internet provider hands out addresses from one group and changed the last number overnight (for example `x.y.z.82`, then `x.y.z.44`). So the rule allows `x.y.z.0/24`, meaning all 256 addresses where the first three numbers match. That's still limited to one provider's block, much safer than `0.0.0.0/0` (the whole internet), and the database password is still required.

On campus, the hostel and classroom Wi-Fi are run by the same provider, so both come from this one range and the single rule covers both. Other students on the same campus network share these addresses too, so the firewall doesn't separate you from them. **The database password is the real protection, so keep it strong and private.**

> Some networks block port 3306, which is common on college or office Wi-Fi. If the address is allowed but the connection still times out, try a phone hotspot.

### Stopping charges when the project is over

AWS → **Databases** → select `railway-db` → **Actions → Delete**. Take a final snapshot if you want to keep the data. Running costs are covered by the AWS Free plan credits until they run out. The public IP address is a small extra charge.

---

## Presenting the project with MySQL Workbench

The C# app is only a front end. Every button sends a SQL query or calls a stored procedure. So the whole project can be shown from **MySQL Workbench** on any laptop (Mac included), without running the Windows app. `database/demo.sql` contains those same queries, in the order of the app's screens.

### Before the demo
1. Check the laptop's internet address is allowed in the firewall (see [Allowing a new computer or network](#allowing-a-new-computer-or-network-️)), then open the **AWS railway** connection in Workbench.
2. **File → Open SQL Script…** → `database/demo.sql`.

### Suggested order (about 10–15 minutes)
1. **The cloud database.** Show `railway-db` as **Available** in the AWS console (Aurora and RDS → Databases). This proves the database runs on Amazon RDS in Mumbai.
2. **ER diagram.** Open `database/railway_schema.mwb` (File → Open Model). Or draw it live from AWS: **Database → Reverse Engineer…** → choose the AWS connection → tick `railway` → keep clicking **Continue/Execute**. Workbench draws the tables and their links.
3. **`demo.sql`, one part at a time.** Highlight a part and click the first **⚡**:

| Part | Shows | App screen |
|---|---|---|
| 0 | Connected to MySQL on AWS | — |
| 1 | 13 tables, row counts, all foreign keys | Data design (report ch. 4) |
| 2 | `CheckLogin`: 2 = admin, 1 = passenger, 0 = wrong | Login (Form1) |
| 3 | `SignupUser` stores a SHA-256 hash, and a trigger logs it | Sign up (Form5) |
| 4 | Search trains by route and weekday, then seat classes | Travel / Train details (Form2, Form3) |
| 5 | Book a ticket: check schedule, find a free seat, create the ticket, show the platform | Passenger details (Form4) |
| 6 | Add feedback (rating 1–5) | Feedback (Form7) |
| 7 | Dashboard counts, timetable, bookings and ratings per train, staff assignments | Admin portal (Form6) |
| 8 | Triggers and the `operation_log` audit history | — |
| 9 | Stored procedures in the database | — |
| 10 | **Reset:** deletes the demo passenger, so the demo can be run again | — |

### From a Mac terminal instead of Workbench

```bash
brew install mysql-client                      # once (needs Homebrew)
echo 'export PATH="$(brew --prefix mysql-client)/bin:$PATH"' >> ~/.zshrc && source ~/.zshrc
mysql -h railway-db.c5gwki0cwy64.ap-south-1.rds.amazonaws.com -P 3306 -u dbs -p railway
```

Type the password when asked. Nothing appears while you type, which is normal. Run the whole demo at once with `mysql -h <endpoint> -u dbs -p -t railway < database/demo.sql`.

**Rules that block bad data:** each part has an `EXPECTED ERROR` line, commented out. Run it on its own (click inside it, then **⌘ + Return**) to show the database refusing a duplicate sign-up, a double-booked seat, or a rating of 6.

The script was tested end to end (twice in a row, with reset) on a local MySQL-compatible server before use.

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
- **Saving feedback (`Form7`) may fail.** The app inserts only `comments`, but `feedback.ticket_id` is a required column with no default value. Also, no other screen opens `Form7` at the moment.
- **Old password in git history:** earlier commits contain the old Azure server's password. That server no longer exists, and the AWS password is kept out of the repository (see step 4).
- *Fixed:* `Form8` used to read a separate `railway_fill` database. It now uses the same `railway` database as every other form.
