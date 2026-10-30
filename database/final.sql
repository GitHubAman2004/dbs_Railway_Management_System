-- Create and use the database
CREATE DATABASE IF NOT EXISTS railway_fin;
USE railway_fin;

-- Table creation
CREATE TABLE admin (
    admin_id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(50) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    name VARCHAR(100) NOT NULL,
    contact VARCHAR(15) UNIQUE NOT NULL
);

CREATE TABLE train (
    train_id INT PRIMARY KEY,
    train_name VARCHAR(100) NOT NULL,
    train_type VARCHAR(50) NOT NULL,
    capacity INT NOT NULL
);

CREATE TABLE station (
    station_id INT PRIMARY KEY,
    station_name VARCHAR(100) UNIQUE NOT NULL,
    location VARCHAR(100) NOT NULL
);

CREATE TABLE staff (
    staff_id INT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    role VARCHAR(50) NOT NULL,
    contact VARCHAR(15) UNIQUE NOT NULL
);

CREATE TABLE passenger (
    passenger_id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    contact VARCHAR(15) UNIQUE NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL
);

CREATE TABLE seat (
    seat_id INT PRIMARY KEY,
    train_id INT NOT NULL,
    seat_number INT NOT NULL,
    class VARCHAR(50) NOT NULL,
    FOREIGN KEY (train_id) REFERENCES train(train_id) ON DELETE CASCADE,
    UNIQUE (train_id, seat_number, class)
);

CREATE TABLE ticket (
    ticket_id INT AUTO_INCREMENT PRIMARY KEY,
    passenger_id INT NOT NULL,
    journey_date DATE NOT NULL,
    seat_id INT NOT NULL,
    FOREIGN KEY (passenger_id) REFERENCES passenger(passenger_id) ON DELETE CASCADE,
    FOREIGN KEY (seat_id) REFERENCES seat(seat_id) ON DELETE CASCADE,
    UNIQUE (seat_id, journey_date)
);

CREATE TABLE feedback (
    ticket_id INT PRIMARY KEY,
    rating INT CHECK (rating BETWEEN 1 AND 5),
    comments TEXT,
    FOREIGN KEY (ticket_id) REFERENCES ticket(ticket_id) ON DELETE CASCADE
);

CREATE TABLE staff_train (
    staff_id INT,
    train_id INT,
    PRIMARY KEY (staff_id, train_id),
    FOREIGN KEY (staff_id) REFERENCES staff(staff_id) ON DELETE CASCADE,
    FOREIGN KEY (train_id) REFERENCES train(train_id) ON DELETE CASCADE
);

CREATE TABLE schedule (
    schedule_id INT PRIMARY KEY,
    train_id INT NOT NULL,
    src_station_id INT NOT NULL,
    dest_station_id INT NOT NULL,
    arrival_time TIME NOT NULL,
    departure_time TIME NOT NULL,
    day_of_week ENUM('Mon','Tue','Wed','Thu','Fri','Sat','Sun') NOT NULL,
    FOREIGN KEY (train_id) REFERENCES train(train_id) ON DELETE CASCADE,
    FOREIGN KEY (src_station_id) REFERENCES station(station_id),
    FOREIGN KEY (dest_station_id) REFERENCES station(station_id)
);

CREATE TABLE platform (
    platform_number INT,
    station_id INT,
    PRIMARY KEY (station_id, platform_number),
    FOREIGN KEY (station_id) REFERENCES station(station_id) ON DELETE CASCADE
);

CREATE TABLE staff_station (
    staff_id INT,
    station_id INT,
    PRIMARY KEY (staff_id, station_id),
    FOREIGN KEY (staff_id) REFERENCES staff(staff_id) ON DELETE CASCADE,
    FOREIGN KEY (station_id) REFERENCES station(station_id) ON DELETE CASCADE,
    UNIQUE (staff_id)
);

-- Admin insert
INSERT INTO admin (admin_id, username, password, name, contact) VALUES
(1, 'admin_raj', '5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8', 'Raj Kumar', '9876543210'),
(2, 'admin_priya', '5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8', 'Priya Sharma', '9876543211'),
(3, 'admin_amit', '5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8', 'Amit Singh', '9876543212'),
(4, 'admin_neha', '5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8', 'Neha Patel', '9876543213'),
(5, 'admin_vikram', '5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8', 'Vikram Mehta', '9876543214');

-- Train
INSERT INTO train (train_id, train_name, train_type, capacity) VALUES
(1, 'Rajdhani Express', 'Superfast', 6),
(2, 'Shatabdi Express', 'Superfast', 6),
(3, 'Duronto Express', 'Premium Superfast', 3),
(4, 'Tejas Express', 'High-Speed', 3),
(5, 'MEMU Daily', 'Daily Passenger', 3);
-- Station
INSERT INTO station (station_id, station_name, location) VALUES
(1, 'New Delhi Railway Station', 'Delhi'),
(2, 'Mumbai Central', 'Mumbai'),
(3, 'Chennai Central', 'Chennai'),
(4, 'Howrah Junction', 'Kolkata'),
(5, 'Bengaluru City Junction', 'Bengaluru'),
(6, 'Agra Cantt', 'Agra');

-- Staff
INSERT INTO staff (staff_id, name, role, contact) VALUES
(1, 'Suresh Kumar', 'Station Master', '8765432101'),
(2, 'Ramesh Patel', 'Train Driver', '8765432102'),
(3, 'Anjali Gupta', 'Ticket Collector', '8765432103'),
(4, 'Deepak Sharma', 'Guard', '8765432104'),
(5, 'Sunita Verma', 'Cleaner', '8765432105');

-- Stored Procedures
DELIMITER $$

CREATE PROCEDURE CheckLogin(
    IN p_username VARCHAR(50),
    IN p_hashed_password CHAR(64)
)
BEGIN
    SELECT
        CASE
            WHEN EXISTS (
                SELECT 1 FROM admin
                WHERE username = p_username
                AND password = p_hashed_password
            ) THEN 2
            WHEN EXISTS (
                SELECT 1 FROM passenger
                WHERE name = p_username
                AND password = p_hashed_password
            ) THEN 1
            ELSE 0
        END AS login_status;
END$$

CREATE PROCEDURE SignupUser(
    IN p_username VARCHAR(50),
    IN p_contact VARCHAR(15),
    IN p_email VARCHAR(255),
    IN p_password VARCHAR(255)
)
BEGIN
    INSERT INTO passenger (name, contact, email, password)
    VALUES (p_username, p_contact, p_email, SHA2(p_password, 256));
END$$

DELIMITER ;

-- Passenger (via stored procedure)
CALL SignupUser('Rahul Sharma', '7654321098', 'rahul.sharma@example.com', 'password');
CALL SignupUser('Anita Patel', '7654321099', 'anita.patel@example.com', 'password');
CALL SignupUser('Vijay Malhotra', '7654321090', 'vijay.malhotra@example.com', 'password');
CALL SignupUser('Sneha Reddy', '7654321091', 'sneha.reddy@example.com', 'password');
CALL SignupUser('Ravi Kumar', '7654321092', 'ravi.kumar@example.com', 'pass');
CALL SignupUser('Alok Mehta', '7654321093', 'alok.mehta@example.com', '12345');
CALL SignupUser('Meena Iyer', '7654321094', 'meena.iyer@example.com', 'password');
CALL SignupUser('Karan Joshi', '7654321095', 'karan.joshi@example.com', 'password456');
CALL SignupUser('Tanya Sen', '7654321096', 'tanya.sen@example.com', 'password123');

-- Seat
INSERT INTO seat (seat_id, train_id, seat_number, class) VALUES
-- Train 1: 6 seats (3 classes, 2 seats each)
(1, 1, 1, 'AC Third Class'),
(2, 1, 2, 'AC Third Class'),
(3, 1, 1, 'AC Second Class'),
(4, 1, 2, 'AC Second Class'),
(5, 1, 1, 'AC First Class'),
(6, 1, 2, 'AC First Class'),

-- Train 2: 6 seats (3 classes, 2 seats each)
(7, 2, 1, 'AC Second Class'),
(8, 2, 2, 'AC Second Class'),
(9, 2, 1, 'AC First Class'),
(10, 2, 2, 'AC First Class'),
(11, 2, 1, 'AC Third Class'),
(12, 2, 2, 'AC Third Class'),

-- Train 3: 3 seats (1 class)
(13, 3, 1, 'AC First Class'),
(14, 3, 2, 'AC First Class'),
(15, 3, 3, 'AC First Class'),

-- Train 4: 3 seats (1 class)
(16, 4, 1, 'Sleeper'),
(17, 4, 2, 'Sleeper'),
(18, 4, 3, 'Sleeper'),

-- Train 5: 3 seats (1 class)
(19, 5, 1, 'Second Seater'),
(20, 5, 2, 'Second Seater'),
(21, 5, 3, 'Second Seater');


-- Ticket
INSERT INTO ticket (ticket_id, passenger_id, journey_date, seat_id) VALUES
(1, 1, '2025-04-10', 1),
(2, 2, '2025-04-10', 2),
(3, 3, '2025-04-15', 3),
(4, 4, '2025-04-17', 4),
(5, 5, '2025-04-20', 5),
(6, 6, '2025-04-22', 6),
(7, 7, '2025-04-23', 7),
(8, 8, '2025-04-24', 8),
(9, 9, '2025-04-25', 9);

-- Feedback
INSERT INTO feedback (ticket_id, rating, comments) VALUES
(1, 5, 'Excellent service and comfortable journey'),
(2, 4, 'Very good experience, food could be better'),
(3, 3, 'Average experience, train was delayed'),
(4, 5, 'Perfect journey, very punctual'),
(5, 4, 'Great amenities, but AC was too cold');

-- Staff-train
INSERT INTO staff_train (staff_id, train_id) VALUES
(2, 1),
(3, 2),
(4, 3);

-- Schedule
INSERT INTO schedule (schedule_id, train_id, src_station_id, dest_station_id, arrival_time, departure_time, day_of_week) VALUES
(1, 1, 1, 2, '06:00:00', '06:10:00', 'Mon'),
(2, 1, 1, 2, '06:00:00', '06:10:00', 'Wed'),
(3, 2, 1, 5, '08:00:00', '08:15:00', 'Tue'),
(4, 3, 2, 3, '10:00:00', '10:10:00', 'Mon'),
(8, 3, 2, 3, '09:00:00', '09:15:00', 'Tue'),
(5, 4, 5, 4, '11:00:00', '11:15:00', 'Wed'),
(9, 4, 3, 5, '13:00:00', '13:20:00', 'Fri'),
(10, 5, 1, 6, '07:00:00', '07:10:00', 'Sat');

-- Platform
INSERT INTO platform (platform_number, station_id) VALUES
(1, 1),
(2, 1),
(1, 2),
(1, 3),
(1, 4);

-- Staff-station
INSERT INTO staff_station (staff_id, station_id) VALUES
(1, 1),
(5, 2);



-- Create a logging table if not already created
CREATE TABLE IF NOT EXISTS operation_log (
    log_id INT AUTO_INCREMENT PRIMARY KEY,
    operation_type ENUM('INSERT', 'UPDATE', 'DELETE') NOT NULL,
    table_name VARCHAR(50) NOT NULL,
    record_id INT NOT NULL,
    operation_timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    additional_info VARCHAR(255) NULL
);

DELIMITER $$

-- Create stored procedure for logging
CREATE PROCEDURE LogOperation(
    IN p_operation_type VARCHAR(10),
    IN p_table_name VARCHAR(50),
    IN p_record_id INT,
    IN p_additional_info VARCHAR(255)
)
BEGIN
    INSERT INTO operation_log (operation_type, table_name, record_id, additional_info)
    VALUES (p_operation_type, p_table_name, p_record_id, p_additional_info);
END$$

-- PASSENGER TABLE TRIGGERS
CREATE TRIGGER log_passenger_crud
AFTER INSERT ON passenger
FOR EACH ROW
BEGIN
    CALL LogOperation('INSERT', 'passenger', NEW.passenger_id, 
                     CONCAT('New passenger: ', NEW.name, ' (', NEW.contact, ')'));
END$$

CREATE TRIGGER log_passenger_update
AFTER UPDATE ON passenger
FOR EACH ROW
BEGIN
    CALL LogOperation('UPDATE', 'passenger', NEW.passenger_id,
                     CONCAT('Updated passenger: ', NEW.name, ' (', NEW.contact, ')'));
END$$

CREATE TRIGGER log_passenger_delete
BEFORE DELETE ON passenger
FOR EACH ROW
BEGIN
    CALL LogOperation('DELETE', 'passenger', OLD.passenger_id,
                     CONCAT('Deleted passenger: ', OLD.name, ' (', OLD.contact, ')'));
END$$

-- TICKET TABLE TRIGGERS
CREATE TRIGGER log_ticket_insert
AFTER INSERT ON ticket
FOR EACH ROW
BEGIN
    DECLARE passenger_name VARCHAR(100);
    
    SELECT name INTO passenger_name
    FROM passenger
    WHERE passenger_id = NEW.passenger_id;
    
    CALL LogOperation('INSERT', 'ticket', NEW.ticket_id,
                     CONCAT('Ticket booked by ', passenger_name, ' for ', NEW.journey_date));
END$$

CREATE TRIGGER log_ticket_update
AFTER UPDATE ON ticket
FOR EACH ROW
BEGIN
    CALL LogOperation('UPDATE', 'ticket', NEW.ticket_id,
                     CONCAT('Updated ticket ID ', NEW.ticket_id));
END$$

CREATE TRIGGER log_ticket_delete
BEFORE DELETE ON ticket
FOR EACH ROW
BEGIN
    CALL LogOperation('DELETE', 'ticket', OLD.ticket_id,
                     CONCAT('Deleted ticket ID ', OLD.ticket_id));
END$$

DELIMITER ;

