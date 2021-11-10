# ConsoleTimeLogger
My first C# application.

A C# console based time logger app that can be used to track time spent coding!


# Requirements:
- [x] When the application starts, it should create a sqlite database, if one isn’t present.
- [x] It should also create a table in the database, where the hours will be logged.
- [x] You need to be able to insert, delete, update and view your logged hours. 
- [ ] You should handle all possible errors so that the application never crashes 
- [x] The application should only be terminated when the user inserts 0. 
- [x] You can only interact with the database using raw SQL. You can’t use mappers such as Entity Framework