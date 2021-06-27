# HHRROrganizer // OrganiTeam

Welcome to the demo of OrganiTeam (previously called HHRROrganizer), an application developed by Asier García Pérez, with the help of his teachers Peio Murguia Lasa and Erlantz Rojo Raño at BBKBootcamp in Bilbao (Vizcaya, Spain). 

Installation (very easy, just create database virtualy in Visual Studio with the next steps):

You must download the project as a zip file, unzip the file in "C:\Users\user\source\repos" (in the analogous path on your computer) to avoid problems. 
Double click on the solution "HHRROrganizer.sln" to open it with Visual Studio. 

Once everything is set up in Visual Studio, as soon as you open it from the solution, you have to create the database on which it works, in SQL Server ALREADY INTEGRATED in Visual Studio. 
To do this, go to Tools - NuGet Package Manager - Package Manager Console, and in the console command box that opens, type the following statements:
    add-migration first (In this way, with " add-migration first" we generate the data "migration" file of our entire application).
    update-database (To create the database in a "virtual" way in Visual Studio)
    
If everything is correct, in View - SQL Server Object Explorer, you should be able to visualize the database. 
Clicking on the top toolbar in IIS Express, or otherwise pressing F5, should load the program.


Application information:

From a very basic point of view, it presents several functionalities of the Model-View-Controller (MVC) offered by ASP.Net, through an employee management application. 
It employs the Identity API giving the application a user interface (UI) login functionality, and also manages users, passwords, profile data, roles, notifications, tokens, e-mail confirmation, etc. 
This application also allows you to assign admin, manager and guest roles, which have the following permissions:

Admin Role: By default, if you log in to the application you will be granted this role. It has permissions to create employees, delete them, increase salary and decrease it. In addition, it has a section available to manage roles of registered users, as well as delete them. You can visualize each of the sections of the application.
Rol Manager: You have permissions to view the different departments, but you cannot create new ones. You can edit employees, and view their details, but you cannot delete them. You cannot increase or decrease their salary. You also do not have the option to modify user permissions.
Role Guest: You have permissions to view the list of employees, as well as departments. It does not have access to the rest of the functionalities.
