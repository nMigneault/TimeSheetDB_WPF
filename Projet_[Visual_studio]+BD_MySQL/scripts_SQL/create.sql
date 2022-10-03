DROP DATABASE IF EXISTS TimeSheetDB;
CREATE DATABASE TimeSheetDB CHARACTER SET 'utf8';
USE TimeSheetDB;

DROP TABLE IF EXISTS ProjectTimeEntry;
DROP TABLE IF EXISTS Project;
DROP TABLE IF EXISTS Employee;  

CREATE TABLE Project(
  projectId INTEGER CHECK(projectId > 0),
  projectName VARCHAR(50),
  projectDescription VARCHAR(150),
  CONSTRAINT Pk_Project PRIMARY KEY(projectId)
  ); 

 
CREATE TABLE Employee(
  employeeId INTEGER,
  firstName VARCHAR(50) NOT NULL, 
  lastName VARCHAR(50) NOT NULL,
  address VARCHAR(100),
  gender CHAR(1) NOT NULL CHECK(gender IN('M', 'F')),
  login VARCHAR(25) NOT NULL, 
  password VARCHAR(40),
  role VARCHAR(25) NOT NULL CHECK(role in('Employee', 'Manager')),
  CONSTRAINT Un_login UNIQUE(login),
  CONSTRAINT Pk_Employee PRIMARY KEY(employeeId)
  );


CREATE TABLE ProjectTimeEntry(
  entryId INTEGER NOT NULL AUTO_INCREMENT,
  employeeId INTEGER NOT NULL, 
  projectId INTEGER NOT NULL, 
  startDate DATE NOT NULL,
  endDate DATE NOT NULL,
  projectEntryDate DATE NOT NULL, 
  duration INTEGER NOT NULL CHECK(duration > 0),
  CONSTRAINT Pk_ProjectEntry PRIMARY KEY(entryId),
  CONSTRAINT Fk_ProjectEntry_Employee FOREIGN KEY(employeeId) REFERENCES Employee(employeeId),
  CONSTRAINT Fk_ProjectEntry_Project FOREIGN KEY(projectId) REFERENCES Project(projectId)
  );
 
 
 
/*
*Extraire les feuilles de temps pour une semaine donnée : projets ayant eu lieu à une date 
* comprise entre celle du Lundi et celle du dimanche de la semaine en question.
* Ces deux dates peuvent être calculées par l'application étant donné le jours de la semaine sélectionée
SELECT * FROM `projecttimeentry` WHERE 
projectEntryDate BETWEEN STR_TO_DATE('12-09-2022', '%d-%m-%Y') AND STR_TO_DATE('20-09-2022', '%d-%m-%Y')
*/