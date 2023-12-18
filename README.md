# CPSC-4910 - Good (Truck) Driver Incentive Program

This project was a semester-long, group based project to gain experience on how a real-world software solution is designed and developed. We focused on the software development life cycle (SDLC) with weekly sprints and scrum meetings to mark our progress.

My group and I chose to develop a C# .NET MVC Web Application. The main reason for choosing .NET over other popular frameworks (NodeJS, Django, etc.) was .NET's Entity Framwork Core. This O/RM allowed us to create the entire MySql database without writing any SQL.


## Running the Project
The first step to running the application is to clone this github repository to your local machine and open a terminal. Navigate into the Team22.Web directory with the src code in it. Once you're in the correct directory run the following command:

```
dotnet user-secrets set "Team22ConnectionString" "Server={host IP address};Port=3306;Database={database name};User Id={user name};Password={password}"
```

where:
 * Host IP address is the IP address where your MySQL server is loaded
 * Port is the port that MySQL is running on (default is 3306)
 * Database name is the name of the database to use
 * Password is your MySQL password

The reason for this is that we are using .NET’s Secret Manager (as opposed to using a json file) to store our database settings. You can verify that the settings were updated by typing:

```
dotnet user-secrets list
```


You can then run the project from cmd line by typing:
```
dotnet run
```

## Using the Application
Once you have everything setup and running, you can begin actually using the app. First, you need to create an account by clicking the green `Create Account` button on the home page. Then you’ll be prompted to login using the credentials you created the account with. I recommend creating an account for all 3 user types (driver, sponsor, and admin) to use the different features.


## Technologies
  * ASP.NET MVC 6
  * EntityFramework Core
  * .NET Identity
  * MySql
  * AWS ElasticBeanstalk
  * C#

## Teammates
 * Spencer Shaw
 * Blue Hartsell
 * Justin Kristensen
 * Jeremiah Fenner





