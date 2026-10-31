-- =====================================================================
--  Railway Management System: live demo script for MySQL Workbench
-- =====================================================================
--  Runs every feature of the app as plain SQL against the AWS database.
--  The queries are the same ones the C# app sends (Form1 to Form8).
--
--  HOW TO USE
--  * Open this file in Workbench while connected to the AWS database.
--  * Run ONE PART at a time: highlight the part's lines, then click the
--    first lightning bolt. Only the highlighted lines run.
--  * To run a single statement: click inside it and press Cmd + Return.
--  * Each SELECT opens a "Result" tab above the Action Output panel.
--  * Lines marked "EXPECTED ERROR" fail on purpose to show a rule
--    working. Run them on their own (Cmd + Return).
--  * PART 10 removes the demo data, so the demo can be repeated.
-- =====================================================================

USE railway;


-- =====================================================================
-- PART 0: Where are we?
-- Say: "The database runs on Amazon RDS in Mumbai. I'm connected from
--       MySQL Workbench on my laptop."
-- =====================================================================
SELECT VERSION()      AS mysql_version,
       DATABASE()     AS current_database,
       CURRENT_USER() AS logged_in_as,
       NOW()          AS server_time;


-- =====================================================================
-- PART 1: Database design (13 tables, normalized to 3NF)
-- Say: "Each table stores one kind of thing. Foreign keys connect them,
--       so a ticket can't point to a passenger or seat that doesn't exist."
-- =====================================================================
SHOW TABLES;

-- How many rows each table has
SELECT 'admin'         AS table_name, COUNT(*) AS total_rows FROM admin
UNION ALL SELECT 'passenger',     COUNT(*) FROM passenger
UNION ALL SELECT 'train',         COUNT(*) FROM train
UNION ALL SELECT 'station',       COUNT(*) FROM station
UNION ALL SELECT 'platform',      COUNT(*) FROM platform
UNION ALL SELECT 'schedule',      COUNT(*) FROM schedule
UNION ALL SELECT 'seat',          COUNT(*) FROM seat
UNION ALL SELECT 'ticket',        COUNT(*) FROM ticket
UNION ALL SELECT 'feedback',      COUNT(*) FROM feedback
UNION ALL SELECT 'staff',         COUNT(*) FROM staff
UNION ALL SELECT 'staff_train',   COUNT(*) FROM staff_train
UNION ALL SELECT 'staff_station', COUNT(*) FROM staff_station
UNION ALL SELECT 'operation_log', COUNT(*) FROM operation_log;

-- Every relationship (foreign key) between tables
SELECT table_name             AS child_table,
       column_name            AS child_column,
       referenced_table_name  AS parent_table,
       referenced_column_name AS parent_column
FROM information_schema.key_column_usage
WHERE table_schema = 'railway'
  AND referenced_table_name IS NOT NULL
ORDER BY child_table, child_column;

-- Structure of the most connected table
DESCRIBE ticket;


-- =====================================================================
-- PART 2: Login (stored procedure CheckLogin, used by Form1)
-- Say: "Passwords are never stored as plain text. The app turns the password
--       into a SHA-256 hash and the procedure compares hashes.
--       2 = admin, 1 = passenger, 0 = wrong login."
-- =====================================================================
CALL CheckLogin('admin_raj',    SHA2('password', 256));   -- 2 = admin
CALL CheckLogin('Rahul Sharma', SHA2('password', 256));   -- 1 = passenger
CALL CheckLogin('Rahul Sharma', SHA2('wrongpass', 256));  -- 0 = invalid

-- What a stored password actually looks like
SELECT name, email, password AS stored_password_hash
FROM passenger
LIMIT 3;


-- =====================================================================
-- PART 3: Sign up a new passenger (stored procedure SignupUser, Form5)
-- Say: "Signing up calls a procedure that hashes the password before saving.
--       A trigger also writes an entry into operation_log automatically."
-- =====================================================================
CALL SignupUser('Demo Student', '9000000001', 'demo.student@example.com', 'demo123');

SELECT passenger_id, name, contact, email, password AS stored_password_hash
FROM passenger
WHERE email = 'demo.student@example.com';

-- The new passenger can log in right away (returns 1)
CALL CheckLogin('Demo Student', SHA2('demo123', 256));

-- Written by the trigger log_passenger_crud, not by us
SELECT * FROM operation_log ORDER BY log_id DESC LIMIT 3;

-- EXPECTED ERROR (run on its own): the same contact/email can't sign up twice
-- CALL SignupUser('Copy Cat', '9000000001', 'demo.student@example.com', 'x');


-- =====================================================================
-- PART 4: Search trains (Form2 and Form3)
-- Say: "The passenger picks From, To and a date. The app finds trains that
--       run on that weekday, then shows the seat classes of the chosen train."
-- Change these values to search something else.
-- 2 November 2026 is a Monday, and Rajdhani Express runs Delhi to Mumbai on Mondays.
-- =====================================================================
SET @from_station = 'New Delhi Railway Station';
SET @to_station   = 'Mumbai Central';
SET @journey_date = '2026-11-02';

SELECT @journey_date AS journey_date, DATE_FORMAT(@journey_date, '%a') AS day_of_week;

-- Trains on this route on that weekday (same query as Form3)
SELECT DISTINCT t.train_name
FROM train t
JOIN schedule sc ON t.train_id = sc.train_id
JOIN station s1  ON sc.src_station_id  = s1.station_id
JOIN station s2  ON sc.dest_station_id = s2.station_id
WHERE s1.station_name = @from_station
  AND s2.station_name = @to_station
  AND sc.day_of_week  = DATE_FORMAT(@journey_date, '%a');

-- Seat classes on the chosen train (same query as Form3)
SET @train_name = 'Rajdhani Express';

SELECT DISTINCT s.class
FROM seat s
JOIN train t ON s.train_id = t.train_id
WHERE t.train_name = @train_name;


-- =====================================================================
-- PART 5: Book a ticket (Form4), step by step like the app
-- Say: "The app looks up the IDs, checks the train really runs that day,
--       picks the first free seat in that class for that date, then creates
--       the ticket and shows the platform."
-- Run PART 4 first (it sets @from_station, @to_station, @journey_date, @train_name).
-- =====================================================================
SET @passenger_name = 'Demo Student';
SET @seat_class     = 'AC Third Class';

-- Clear old values (SELECT ... INTO keeps the old value if it finds no row)
SET @pid = NULL, @tid = NULL, @src = NULL, @dst = NULL, @seat = NULL, @ticket = NULL;

-- 1. Look up the IDs
SELECT passenger_id INTO @pid FROM passenger WHERE name = @passenger_name LIMIT 1;
SELECT train_id     INTO @tid FROM train     WHERE train_name = @train_name;
SELECT station_id   INTO @src FROM station   WHERE station_name = @from_station;
SELECT station_id   INTO @dst FROM station   WHERE station_name = @to_station;

-- 2. Does the train run on this route that weekday? (1 or more = yes)
SELECT COUNT(*) AS schedule_matches
FROM schedule
WHERE train_id = @tid AND src_station_id = @src AND dest_station_id = @dst
  AND day_of_week = DATE_FORMAT(@journey_date, '%a');

-- 3. First seat in this class that nobody has booked for this date
SELECT s.seat_id INTO @seat
FROM seat s
WHERE s.train_id = @tid
  AND s.class = @seat_class
  AND s.seat_id NOT IN (SELECT t.seat_id FROM ticket t WHERE t.journey_date = @journey_date)
ORDER BY s.seat_id
LIMIT 1;

SELECT @pid AS passenger_id, @tid AS train_id, @src AS from_station_id,
       @dst AS to_station_id, @seat AS free_seat_id;
-- If free_seat_id is NULL, this class is full on this date, and the INSERT below is refused.

-- 4. Create the ticket
INSERT INTO ticket (passenger_id, journey_date, seat_id) VALUES (@pid, @journey_date, @seat);
SET @ticket = LAST_INSERT_ID();

-- 5. Booking confirmation (the same details the app's "Booking Confirmed" box shows)
SELECT tk.ticket_id,
       p.name           AS passenger,
       @from_station    AS from_station,
       @to_station      AS to_station,
       tk.journey_date,
       tr.train_name,
       s.class          AS coach_type,
       s.seat_number,
       (SELECT platform_number FROM platform WHERE station_id = @src LIMIT 1) AS platform_no
FROM ticket tk
JOIN passenger p ON p.passenger_id = tk.passenger_id
JOIN seat s      ON s.seat_id = tk.seat_id
JOIN train tr    ON tr.train_id = s.train_id
WHERE tk.ticket_id = @ticket;

-- The booking trigger logged it too
SELECT * FROM operation_log ORDER BY log_id DESC LIMIT 3;

-- EXPECTED ERROR (run on its own): the same seat can't be sold twice for one date
-- INSERT INTO ticket (passenger_id, journey_date, seat_id) VALUES (@pid, @journey_date, @seat);


-- =====================================================================
-- PART 6: Passenger feedback (Form7)
-- Say: "Each ticket can get one rating from 1 to 5. A CHECK rule rejects anything else."
-- =====================================================================
INSERT INTO feedback (ticket_id, rating, comments)
VALUES (@ticket, 5, 'Smooth journey, booked during the live demo!');

SELECT f.ticket_id, p.name AS passenger, f.rating, f.comments
FROM feedback f
JOIN ticket tk   ON tk.ticket_id = f.ticket_id
JOIN passenger p ON p.passenger_id = tk.passenger_id
ORDER BY f.ticket_id DESC;

-- EXPECTED ERROR (run on its own): rating 6 breaks the CHECK rule (1 to 5 only)
-- INSERT INTO feedback (ticket_id, rating, comments) VALUES (1, 6, 'Too good');


-- =====================================================================
-- PART 7: Admin dashboard (Form6)
-- Say: "The admin portal shows these counts and lets the admin view and edit every table."
-- =====================================================================
SELECT (SELECT COUNT(passenger_id) FROM passenger) AS registered_passengers,
       (SELECT COUNT(train_id)     FROM train)     AS no_of_trains,
       (SELECT COUNT(staff_id)     FROM staff)     AS no_of_staff,
       (SELECT COUNT(admin_id)     FROM admin)     AS no_of_admins;

-- Full timetable with station names
SELECT tr.train_name, tr.train_type,
       s1.station_name AS from_station,
       s2.station_name AS to_station,
       sc.day_of_week, sc.arrival_time, sc.departure_time
FROM schedule sc
JOIN train tr   ON tr.train_id = sc.train_id
JOIN station s1 ON s1.station_id = sc.src_station_id
JOIN station s2 ON s2.station_id = sc.dest_station_id
ORDER BY tr.train_id, FIELD(sc.day_of_week, 'Mon','Tue','Wed','Thu','Fri','Sat','Sun');

-- Bookings (across all dates) and average rating per train
SELECT tr.train_name,
       COUNT(tk.ticket_id)     AS tickets_all_dates,
       tr.capacity             AS seats_per_journey,
       ROUND(AVG(f.rating), 1) AS avg_rating
FROM train tr
LEFT JOIN seat s      ON s.train_id = tr.train_id
LEFT JOIN ticket tk   ON tk.seat_id = s.seat_id
LEFT JOIN feedback f  ON f.ticket_id = tk.ticket_id
GROUP BY tr.train_id, tr.train_name, tr.capacity
ORDER BY tickets_all_dates DESC;

-- Where each staff member works (train or station)
SELECT st.name, st.role, 'Train' AS assigned_to, tr.train_name AS place
FROM staff st JOIN staff_train sx ON sx.staff_id = st.staff_id JOIN train tr ON tr.train_id = sx.train_id
UNION ALL
SELECT st.name, st.role, 'Station', sn.station_name
FROM staff st JOIN staff_station sy ON sy.staff_id = st.staff_id JOIN station sn ON sn.station_id = sy.station_id
ORDER BY name;


-- =====================================================================
-- PART 8: Triggers and the audit log
-- Say: "Six triggers record every insert, update and delete on passengers and tickets,
--       so there is a history of who changed what and when."
-- =====================================================================
SHOW TRIGGERS;

-- An update is logged automatically too
UPDATE passenger SET contact = '9000000002' WHERE email = 'demo.student@example.com';

SELECT * FROM operation_log ORDER BY log_id DESC LIMIT 5;


-- =====================================================================
-- PART 9: Stored procedures in the database
-- =====================================================================
SHOW PROCEDURE STATUS WHERE Db = 'railway';
SHOW CREATE PROCEDURE CheckLogin;


-- =====================================================================
-- PART 10: Reset the demo (run when finished)
-- Deleting the demo passenger also removes their ticket and feedback,
-- because of ON DELETE CASCADE. The trigger logs the delete.
-- =====================================================================
DELETE FROM passenger WHERE email = 'demo.student@example.com';

SELECT * FROM operation_log ORDER BY log_id DESC LIMIT 3;
