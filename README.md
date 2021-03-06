<a href="https://www.nuget.org/packages/EFCoreDataChange">
    <img alt="Nuget (with prereleases)" src="https://img.shields.io/nuget/vpre/EFCoreDataChange">
</a>
<a href="https://www.nuget.org/packages/EFCoreDataChange">
    <img alt="Nuget" src="https://img.shields.io/nuget/dt/EFCoreDataChange">
</a>

# EFCoreDataChange
This is yet anover audit plugin for Entity Framework Core. It provide store about entity changes, and retrives deleted or modified data after specified time point with special requests.

# How to use

`EfCoreDataChange` will automaticly save the moment, when entity has been changed and last state of entity.
The entity type must have simple or composite key

1. Install EfCoreDataChange Package

Run the following command in the `Package Manager Console` to install EfCoreDataChange

`PM> Install-Package EfCoreDataChange`

2. Include this code in "OnModelCreating" method like this

```csharp
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Your code ----------
            modelBuilder.CreateDataChangeTracking(this.GetType());
        }
```

3. Use special DbContext instead of Your DbContext.
Call PrepareTrackInfo() method before SaveChanges()

```csharp
            using (var db = RuntimeDBContextExtention<YourDBContext>.RuntimeContext)
            {
                // Your code
                db.PrepareTrackInfo();
                db.SaveChanges();
            }
```
4. To obtain added/changed/deleted entities, use special metods.

```csharp
            using (var db = RuntimeDBContextExtention<YourDBContext>.RuntimeContext)
            {
                // Your code
                // get deleted cats
                var deletedCats = db.Cats.Deleted<Cat>(db,DateTime.Now);

                // get changed cats
                var modifiedCats = db.Cats.AddedOrChanged<Cat>(db,DateTime.Now);

            }
```

5. Register in Dependency Injection.
Below is sample code how You can use DI.

```csharp
using EfCoreDataChange;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext(
                RuntimeDBContextExtention<MyDBContext>.RuntimeContextType,
                options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
        }
```
