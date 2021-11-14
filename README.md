# ConsoleTimeLogger
My first C# application, and first time using Visual Studio. 

Console based CRUD application to track time spent coding.
Developed using C# and SQLite.


# Given Requirements:
- [x] When the application starts, it should create a sqlite database, if one isn’t present.
- [x] It should also create a table in the database, where the hours will be logged.
- [x] You need to be able to insert, delete, update and view your logged hours. 
- [x] You should handle all possible errors so that the application never crashes 
- [x] The application should only be terminated when the user inserts 0. 
- [x] You can only interact with the database using raw SQL. You can’t use mappers such as Entity Framework
- [x] Reporting Capabilities

# Features

* SQLite database connection

	- The program uses a SQLite db connection to store and read information. 
	- If no database exists, or the correct table does not exist they will be created on program start.

* A console based UI where users can navigate by key presses
 
 	![image](https://user-images.githubusercontent.com/15159720/141688100-ec6130da-33d6-4a30-ad3c-1d7f546da58a.png)

* CRUD DB functions

	- From the main menu users can Create, Read, Update or Delete entries for whichever date they want, entered in mm-DD-yyyy format. Duplicate days will not be inputted. 
	- Time and Dates inputted are checked to make sure they are in the correct and realistic format. 

* Basic Reports of Cumulative hours

	- ![image](https://user-images.githubusercontent.com/15159720/141688399-9a4697d3-a143-4ed6-bad0-038268ddacaf.png)

* Reporting and other data output uses ConsoleTableExt library to output in a more pleasant way

	- ![image](https://user-images.githubusercontent.com/15159720/141688462-e5dc465c-f188-4ac9-a166-397653c53c41.png)
	- [GitHub for ConsoleTableExt Library](https://github.com/minhhungit/ConsoleTableExt)
