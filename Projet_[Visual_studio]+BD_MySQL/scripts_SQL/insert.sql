USE TimeSheetDB;

/*

Attention 
TRUNCATE TABLE ProjectTimeEntry;
TRUNCATE TABLE Employee;
TRUNCATE TABLE Project;
*/

INSERT INTO Employee(employeeId, firstName, lastName, address, gender, login, password, role)
VALUES
	(99, 'Sauron', 'Le Maya', '123, rue du Mordor', 'M', 'emp-99', SHA1('pwd-999'), 'Manager'),
	(100, 'Harry', 'Potter', '123, rue de Poudlard', 'M', 'emp-100', SHA1('pwd-100'), 'Employee'), 
	(101, 'Sherlock', 'Holmes', '221B, Baker street', 'M', 'emp-101', SHA1('pwd-101'), 'Employee'),
	(102, 'John', 'Watson', '221B, Baker street', 'M', 'emp-102', SHA1('pwd-102'), 'Employee'),
	(103, 'James', 'Moriarty', '212 rue X', 'M', 'emp-103', SHA1('pwd-103'), 'Employee'), 
	(1000, 'Gandalf', 'Le girs', 'Valinor', 'M', 'emp-1000', SHA1('pwd-1000'), 'Employee'), 
	(1001, 'Saroumane', 'Le blanc', 'Valinor', 'M' ,'emp-1001', SHA1('pwd-1001'), 'Employee'),
	(1002, 'Radagast', 'Le brun', 'Valinor','M','emp-1002', SHA1('pwd-1002'), 'Employee');


INSERT INTO Project(projectId, projectName, projectDescription)
VALUES	(600, 'Big Husky', 'Lorem ipsum dolor sit amet, consectetur adipiscing elit.'), 
	(700, 'Apollo', 'Proin pellentesque ultricies suscipit. Integer id porta metus.'), 
	(800, 'Astro', 'Integer pulvinar felis ac viverra malesuada.'),
	(801, 'Barcelona', 'Ut libero metus, lacinia quis turpis sit amet, bibendum ullamcorper velit.'),
	(900, 'Phoenix', 'Suspendisse euismod hendrerit pretium.'), 
	(995, 'Congé parental', 'L’employée qui vient d’accouché a droit à une semaine de congé parental, excluant la fin de semaine'),
	(996, 'Transport en voiture', 'La moitié de ce temps est comptée comme du temps au bureau, pour un maximum de 1/2 heure par jour.'), 
	(997, 'Transport en commun', 'Ce temps est compté comme du temps au bureau, pour un maximum de 1 heure par jour.'),
	(998, 'Congé férié', 'Les 420 minutes sont considérées comme du temps de présence au bureau. Il est permis de faire du télétravail en plus durant la journée.'),
	(999, 'Congé de maladie', 'Les 420 minutes sont considérées comme du temps de présence au bureau. Il n’est pas permis d’utiliser les congés de maladie la fin de semaine'),
	(1000, 'FreshMove', 'Sed sagittis purus at magna consectetur efficitur.'), 
	(1001, 'Steamy Ray', 'Phasellus sit amet ipsum blandit, malesuada ex aliquet, blandit nulla.'), 
	(1002, 'Aurora', 'Etiam aliquet ipsum vitae sollicitudin elementum.'), 
	(1003, 'Firecracker', 'Praesent sit amet enim turpis.');


INSERT INTO ProjectTimeEntry(employeeId, projectId, startDate, endDate, projectEntryDate, duration)
VALUES 
	(100, 600, STR_TO_DATE('12-09-2022', '%d-%m-%Y'), STR_TO_DATE('18-09-2022', '%d-%m-%Y'), STR_TO_DATE('12-09-2022', '%d-%m-%Y'), 420),
	(100, 700, STR_TO_DATE('12-09-2022', '%d-%m-%Y'), STR_TO_DATE('18-09-2022', '%d-%m-%Y'), STR_TO_DATE('12-09-2022', '%d-%m-%Y'), 120),
	(1000, 600, STR_TO_DATE('12-09-2022', '%d-%m-%Y'), STR_TO_DATE('18-09-2022', '%d-%m-%Y'), STR_TO_DATE('12-09-2022', '%d-%m-%Y'), 420),
	(1000, 700, STR_TO_DATE('12-09-2022', '%d-%m-%Y'), STR_TO_DATE('18-09-2022', '%d-%m-%Y'), STR_TO_DATE('12-09-2022', '%d-%m-%Y'), 120),
	(1000, 700, STR_TO_DATE('12-09-2022', '%d-%m-%Y'), STR_TO_DATE('18-09-2022', '%d-%m-%Y'), STR_TO_DATE('13-09-2022', '%d-%m-%Y'), 420),
	(100, 600, STR_TO_DATE('12-09-2022', '%d-%m-%Y'), STR_TO_DATE('18-09-2022', '%d-%m-%Y'), STR_TO_DATE('13-09-2022', '%d-%m-%Y'), 420),
	
	(1000, 700, STR_TO_DATE('12-09-2022', '%d-%m-%Y'), STR_TO_DATE('18-09-2022', '%d-%m-%Y'), STR_TO_DATE('14-09-2022', '%d-%m-%Y'), 420),
	(100, 700, STR_TO_DATE('12-09-2022', '%d-%m-%Y'), STR_TO_DATE('18-09-2022', '%d-%m-%Y'), STR_TO_DATE('14-09-2022', '%d-%m-%Y'), 420),
	(100, 900, STR_TO_DATE('12-09-2022', '%d-%m-%Y'), STR_TO_DATE('18-09-2022', '%d-%m-%Y'), STR_TO_DATE('15-09-2022', '%d-%m-%Y'), 420),
	(1000, 900, STR_TO_DATE('12-09-2022', '%d-%m-%Y'), STR_TO_DATE('18-09-2022', '%d-%m-%Y'), STR_TO_DATE('15-09-2022', '%d-%m-%Y'), 420),

	(1000, 600, STR_TO_DATE('12-09-2022', '%d-%m-%Y'), STR_TO_DATE('18-09-2022', '%d-%m-%Y'), STR_TO_DATE('16-09-2022', '%d-%m-%Y'), 420),
	(100, 600, STR_TO_DATE('12-09-2022', '%d-%m-%Y'), STR_TO_DATE('18-09-2022', '%d-%m-%Y'), STR_TO_DATE('16-09-2022', '%d-%m-%Y'), 420),
	
	(100, 600, STR_TO_DATE('12-09-2022', '%d-%m-%Y'), STR_TO_DATE('18-09-2022', '%d-%m-%Y'), STR_TO_DATE('17-09-2022', '%d-%m-%Y'), 420),
	(1000, 600, STR_TO_DATE('12-09-2022', '%d-%m-%Y'), STR_TO_DATE('18-09-2022', '%d-%m-%Y'), STR_TO_DATE('17-09-2022', '%d-%m-%Y'), 420);