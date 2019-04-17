# Object/Relational Mapping and Entity Framework

An Object/Relational Mapper (ORM) is a software tool for automatically connecting C# objects to relational database tables. You've likely noticed that when working with ADO<span>.NET</span> you have to write a lot of SQL and C# code that is largely repetitive. An ORM is a tool that does the repetitive SQL and boilerplate C# for you. An ORM greatly reduces - often eliminates - the need to write SQL in your application.

## Entity Framework

Entity Framework (EF) is a popular ORM created by Microsoft. It allows you to use LINQ methods in conjunction with your data models to interact with your database (e.g. SELECT. INSERT, UPDATE, DELETE data). EF then determines the SQL syntax needed to perform the appropriate action(s).

## Tutorial

Please do the [ASP.NET Core MVC with Entity Framework Core - Tutorial](https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/intro?view=aspnetcore-2.2) in which you will build a small web application using EF. It shows you how to configure your application for using it, how to set up a database context, and how to use that database context with LINQ statements to interact with a database.

## References

### Startup.cs

To enable EF model-first migrations, this is what your `ConfigureServices` method should be in the `Startup.cs` file.

```cs
public void ConfigureServices (IServiceCollection services) {
    services.Configure<CookiePolicyOptions> (options => {
        // This lambda determines whether user consent for non-essential cookies is needed for a given request.
        options.CheckConsentNeeded = context => true;
        options.MinimumSameSitePolicy = SameSiteMode.None;
    });

    services.AddDbContext<ApplicationDbContext> (options =>
        options.UseSqlServer (
            Configuration.GetConnectionString ("DefaultConnection")));

    services.AddMvc ().SetCompatibilityVersion (CompatibilityVersion.Version_2_2);
}
```