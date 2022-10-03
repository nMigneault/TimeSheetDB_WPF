TimeSheetDB
===========

Une application de bureau avec interface qui permet aux employés de remplir leur feuille de temps; 
permet à un manager d'effectuer la validation du temps travaillé par des employés dans le respect 
des règles de l’entreprise en un seul clic. 


Spécifications
--------------

Le projet a été compilé avec le SDK .NET6, minimum requis pour exécuter le programme. Il utilise une base de données MySQL pour 
ses données et pour l'authentification des employés.

La base de données s'appelle TimeSheetDB, et l'application utilise le compte root pour s'y connecter. Le script SQL de création 
de la BD est fourni dans le répertoire Projet_[Visual_studio]+BD_MySQL/scripts_SQL (fichiers create.sql et insert.sql).
 
#### Configuration du serveur MySQL 
Il réside sur la même machine que l'application (localhost) et utilise une configuration basique WAMP : 
le compte **root est sans mot de passe et le port d'écoute est le 3306** (port TCP par défaut).


Contenu des répertoires
-----------------------

### Documentation

Divers documents liés au projet (modélisation UML)

### Executable

Contient l'exécutable de l'application.  Il s'agit d'une application à interface graphique qui démarre
sur une fenêtre de login (connexion). 

Selon le rôle de l'employé connecté (employé ou manager), celui-ci est dirigé vers l'interface appropriée :
- Remplissage de la carte de temps pour un employé. 
- Gestion des employés, gestion des projets et validation de carte de temps déjà sauvegardées dans le base pour le manager.

On assume qu'il y a un seul manager, et plusieurs employés 

Pour simplifier la mémorisation des informations de connexion, nous avons opté pour un login de la forme emp-xxx et 
un mot de passe de la forme pwd-xxx, avec xxx représentant le numéro de l'employé. Pour le manager, nous avons choisi emp-99 pour le login 
et pwd-999 pour le mot de passe. 

Le fichier insert.sql présente la liste des employés qui seront initialement insérés dans la BD lors de la création de cette dernière.
Il est recommandé de le regarder car les mots de passe seront stockés dans la BD avec un hashage de type SHA1.

L'exécutable contient deux sous répertoires que nous jugeons importants (**à ne pas supprimer même s'ils sont vides**)
- *pdf* : répertoire où seront exportées (**en PDF**) les cartes de temps des employés.
- *validations* : répertoire où sera exporté (**enPDF**) le résultat d'une validation dune carte de temps (manager)

**Les opérations d'exportation ne sont pas automatique**


Notes
-----

Projet en cours de développement, il contient encore de anomalies mineures.