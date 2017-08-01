![Build](https://rigantitfs.visualstudio.com/_apis/public/build/definitions/c822ec28-1813-4f5a-beb4-1017e12c262c/73/badge)
# Riganti.Utils.Infrastructure
Riganti.Utils.Infrastructure is an implementation of common enterprise architecture design patterns. This library can be used as a foundation of a business layer in your application.

## How To Install
There are two NuGet packages - *Riganti.Utils.Infrastructure.Core* contains the interfaces for Unit of Work, Query Object, Repository and other patterns, and *Riganti.Utils.Infrastructure.EntityFramework* implements these interfaces to be used with the Entity Framework 6.1. 

    nuget install Riganti.Utils.Infrastructure.Core
    nuget install Riganti.Utils.Infrastructure.EntityFramework
    
## Layers of the Application
In typical business application, the first layer at the bottom is the DAL - Data Access Layer. This layer will most probably contain the Entity Framework entities and your DbContext class which describes the database. Also, EF Migrations can be present in this layer. The DAL is responsible for loading and storing data.

On top of the DAL, there is a business layer. This layer uses the DAL to communicate with the database, and implements the processes which are required by the presentation layer. There are plenty of things in the BL:
- *Domain Model* is a set of classes that represents data or processes in the application (e.g. the Order class, the Customer class). These classes should not be directly Entity Framework entities, however most of them can be very similar to the corresponding entities. In desktop applications, those object can live for quite a long time (e.g. while the window or a dialog is open), in web applications we typically object only for a duration of one HTTP request. The domain model objects are mapped to the entities. You can write those mappings yourself, or you can leverage AutoMapper or a similar library for this purpose.
- *Query Objects* represent each complex database query which is used in the application. Since we use Entity Framework, the Query Object will work with the IQueryable interface. There is no other way to query the database for more than 1 object but using the Query Object. It is much better to represent each query as an object placed in one or more folders, than having hundreds of queries on hundreds of different places in the application. In case you need to resolve some performance issue, it is quite easy to take a Query Object, rewrite it directly in SQL and wrap the SQL call inside the Query Object. The rest of the application will stay untouched.
- *Repositories* can insert, update or delete an object from the database (or other kind of storage) and get an object by its primary key. You can work with small batches of objects however the repository doesn't allow complex database queries.
- *Unit of Work* wraps the DbContext or another kind of business transaction scope and allows you to declare your own scopes and logical contexts when you work with repositories and queries. The Entity Framework UOW implementation supports nesting (if you open one UOW and call a function which opens its own UOW, you can reuse the parent context and commit all changes when you leave the first UOW). You can also register actions which will be performed after and only after all changes made are committed successfully.

