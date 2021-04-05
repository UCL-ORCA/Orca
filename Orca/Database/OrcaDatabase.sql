-- -----------------------------------------------------
-- Schema for ORCA
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Table student
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS student (
  id VARCHAR(45) NOT NULL,
  first_name VARCHAR(45) NULL,
  last_name VARCHAR(45) NULL,
  email VARCHAR(90) NOT NULL,
  PRIMARY KEY (id));

-- -----------------------------------------------------
-- Table event
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS event (
  id SERIAL,
  student_id VARCHAR(45) NOT NULL,
  course_id VARCHAR(60)  NULL,
  timestamp TIMESTAMP NOT NULL,
  event_type VARCHAR(45) NOT NULL,
  activity_name VARCHAR(45) NULL,
  activity_details VARCHAR(45) NULL,
  PRIMARY KEY (id),
  CONSTRAINT student_ID
    FOREIGN KEY (student_ID)
    REFERENCES student (id)
    ON DELETE CASCADE
    ON UPDATE CASCADE);