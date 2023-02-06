# SynelCsvImport
###
This is a test task for Synel.<br>
This application has been developed to cover the logic described below. In short - this is a single-page MVC website connected to the MSSQL database. <br>
To build this application locally You can use docker functionality (if the docker is installed on your machine). Copy the file docker-compose-uploaded to your local and in the command prompt (while in the directory containing the file) write the following-command: "docker-compose-uploaded up -d". Then open the browser and type the following address: http://localhost:8001. The application will be run in the container. <br>
Also it is possible to run this application in the container as well but from the local repo if you clone the repository and start debugging the app with docker-compose. Although it might bring some problems if you run in on devices with MacOS. 
The task was to build a single-page MVC website which can acquire .csv files and parse them into data which next will be saved into the database and showed on the main page in corresponding table or grid, reporting about how many rows have been successfully processed. <br>
The table must have sorting, searching and edit functionality. <br>
All of the above-listed requirements were implemented in this web-application using Entity-Framework and CsvHelper services. Also auto-mapper is used to simplify the transition between WiewModels and models for picturing and also for testing purposes to avoid assignments inside methods. This app uses repository pattern to move all the database logic to a separate abstraction. 
