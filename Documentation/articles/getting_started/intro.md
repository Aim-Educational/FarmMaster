# Getting started

This section is mostly about getting FarmMaster cloned and setup on your local computer, so you can start
work on developing the program.

## Requisites

* [dotnet 2.2+ SDK and runtime (3.x also works)](https://dotnet.microsoft.com/download)
* [(Optional) Visual Studio 2017+](https://visualstudio.microsoft.com/)
* Access to a [PostgreSQL](https://www.postgresql.org/) server, either remotely or locally (Andy, contact me if you need access to the one I develop on)

## Cloning and building the project

1. Clone the project from https://github.com/Aim-Educational/FarmMaster, e.g. `git clone https://github.com/Aim-Educational/FarmMaster`

2. Use the command `dotnet restore` in the root folder of the project, where the `.sln` file is.

3. Open up the solution. For Visual Studio users this simply involves opening the `.sln` file.

4. Build the `FarmMaster` project. For Visual Studio users, press CTRL+B, F5, or right click the project on the right-hand side and press "build".

5. If the build succeeds, then everything is setup correctly. If something goes wrong, please contact me with the issue and a fix (if you could find one) so I can
   update this document with a list of issues and solutions.

## Configuring the Postgres connection string

At this point, you can *build* the project, but can't really *run* it as it requires access to a postgres server.

There are [many](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1#default-configuration) ways to do this,
however I strongly recommend going the route of modifying the configuration file at `./FarmMaster/appsettings.Development.json` (in Visual Studio,
there should be an arrow next to the `appsettings.json` file that'll show the `.Development` variant).

The biggest reason for this, is that when it comes to creating an EF migration, FarmMaster's custom context factory will be able to automatically
read the connection string from that file, because otherwise you'd have to manually input the connection string via the command line every time you
wanted to create a migration.

When you open the json file, it should look a little like this:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  }
}
```

There are two connection strings that need to exist in this file, one for migrations (which includes applying migrations), and one for general use by
FarmMaster. During development, these may be the same connection string if wanted. During release, these should technically be seperate.

So, we'll create a new "ConnectionStrings" section (just like the "Logging" section), and give it the sub-keys "Migrate" and "General" with our
connection strings. I've provided the general format of the connection string, but of course change the values where needed:

```json
{
  "Logging": { ... },

  "ConnectionStrings": {
    "General": "Host=11.111.11.111;Port=5432;Database=FarmMaster;User Id=farmmaster;Password=AndySmells", // pls no kick me out.
    "Migrate": "Host=11.111.11.111;Port=5432;Database=FarmMaster;User Id=migrator;Password=DebbieAlsoSmells"
  },
}
```

> [!IMPORTANT]
> Please. **PLEASE** do not commit any changes to `appsettings.Development.json` into Git.
>
> A very easy way to avoid this is to execute the following command inside of the `./FarmMaster/` folder:
> `git update-index --assume-unchanged appsettings.Development.json`
>
> The above command makes it so that any changes to the file are ignored by git.

After performing this step, you should now be able to run the program without it instantly throwing an exception.

Again, if you encounter any issues, please contact me so I can update this document.

## Configuring the SMTP settings

> [!NOTE]
> This step is optional for development, and is only really needed if you're using email in some way (e.g. to send a registration email).

Just as we did for the connection strings, all we have to do here is use one of the various methods to provide the SMTP settings into our
program.

Again, I recommend using `appsettings.Development.json`, and I will provide a template of the data you need to pass - simply change the values
as needed:

```json
"Smtp": {
  "Host": "smtp.gmail.com",
  "Port": 587,
  "Username": "debbie@andycorp.com",
  "Password": "Dreaming About That New Caravan123"
}
```

Andy should have the SMTP details for the Office 365 bot account, if you don't want to set something up yourself.

## Conclusion

Your local instance of FarmMaster should now be setup. From here, please work through the rest of the `Getting Started` section (look at the left-hand side).

As a side note, the documentation for the codebase is also available here by clicking `Api Reference` at the top of this page.
