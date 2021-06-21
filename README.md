# 👨🏻‍🌾 That's My Farmer! - ASP .NET Core Final Project 👨🏻‍🌾
# Description

This is the final source code for the final ASP .NET Core project that was created for the purpose of the Developing ASP .NET Applications programming course, with professor [Gregory Prokopski](https://www.linkedin.com/in/prokopski/) 🔗.

Using the MS Visual Studio Code IDE, we created an ASP .NET Core/Razor Pages Web application that delivers dynamic content.  

In this course, we learned about the fundamentals of Web application site implementation by using
Microsoft ASP.NET Core, Razor Pages and the C# programming language. We were introduced to the concept of Views and Models with Razor Pages and to the ASP .NET Model-View-Controller (MVC) web development framework. Also, we covered many web development related concepts such as databases, servers, configuration files, LINQ, SQLITE, Entity Framework and page validation. 

**Please note** that, due to the really short length of the Developing ASP .NET Applications class (2 weeks and a half, approximately), not all features are fully implemented, the use of CSS doesn't fully respect the industry standards, and testing wasn't implemented.

## Usage

Download the project folder and run it in MS Visual Studio Code. 

**Make sure** you have .NET installed on your machine. If you don't, head over to [.NET](https://dotnet.microsoft.com/download) 🔗.

Open a terminal window and run these commands for DB migration :

* dotnet add package Microsoft.EntityFrameworkCore.Design
* dotnet ef migrations add CreateDatabase --output-dir Data/Migrations
* dotnet ef database update

You can open the DB in [DB Browser for SQLite](https://sqlitebrowser.org/) 🔗 and put in some seed data, but **I don't recommend you do it this way**. Instead, follow the next few steps.

* Login as admin
  * Username: admin@admin.com
  * Password: Admin123!
* Add some data in services and food.
* Logout and Register as a farmer.
* Head over to Add Farm, in the dropdown menu of the navbar, and add one or two farms.
* Logout and Register as a regular user.
* Login and explore the farms with the search bar and add write your first review!

To experiment the use of authentication and authorization, that are implemented to make sure that a regular user can't access admin functionalities for example, try accessing /Admin as a regular user and /Account/EditUser without being signed in.

## Technologies Used

* Razor Pages
* .NET Core
* Identity in ASP .NET Core
* Authentication/Authorization
* SQLite
* Entity Framework
* Bootstrap


## Contributors
[Anastassia Titova](https://www.linkedin.com/in/anastassia-titova-204380202/) 🔗   
[Andrey Lyamkin](https://www.linkedin.com/in/andreylyamkin/) 🔗   
[myself, Emma De Barros](https://www.linkedin.com/in/emma-de-barros/) 🔗

## Resources

Here is the link to our [Trello Board](https://trello.com/b/4wEqAm3C/thats-my-farmer) 🔗.

You'll also find in this repository our DailyScrum folder, that demonstrates our understanding and use of delivery methodologies used in the industry to date such as SCRUM, LEAN, AGILE and Version Control. 

If you're interested in seeing all of the team's commits and final presentation of the project, you can also take a look at our [Bitbucket repository](https://bitbucket.org/emma96/thats-my-farmer/src/master/) 🔗 and [Final Presentation PowerPoint](https://johnabbott-my.sharepoint.com/:p:/r/personal/6169535_johnabbottcollege_net/_layouts/15/Doc.aspx?sourcedoc=%7BFCA8168A-8008-41EF-8695-B6D676448DAE%7D&file=MyFarmerPresentation.pptx&wdOrigin=OFFICECOM-WEB.START.REC&ct=1624239955110&action=edit&mobileredirect=true) 🔗.

You can find the ppt pdf [here](https://github.com/emmadebarros/That-s-My-Farmer/tree/main/Presentation) 🔗.
