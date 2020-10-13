using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using EfCoreDataChange;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace test
{
    public class UnitTest1
    {
        [Fact]
        public void DataChangeTest1()
        {
            using (var db = RuntimeDBContextExtention<TestDBContext>.RuntimeContext)
            {
                var cat1 = new Cat { Name = "Alice", Age = 2};
                db.Cats.Add(cat1);
                db.PrepareTrackInfo();
                db.SaveChanges();
                db.Cats.Remove(cat1);
                db.PrepareTrackInfo();
                db.SaveChanges();
                var catsDeleted = db.Cats.Deleted<Cat>(db,DateTime.MinValue);
                //Assert.Contains<Cat>(new Cat() {Id = 1}, catsDeleted);

                // db.Cats.Add(cat1);
                // db.PrepareTrackInfo();
                // db.SaveChanges();
                var catsDeleted2 = db.Cats.Deleted<Cat>(db,DateTime.MinValue);
                //Assert.Empty(catsDeleted2);

                var catsChanged = db.Cats.AddedOrChanged<Cat>(db,DateTime.MinValue);

                var aa=catsChanged.ToList();
                // db.SaveChangesWithTrackInfo();
                // Assert.Equal(1,1);
            }
        }

        [Fact]
        public void DITest()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddDbContext(typeof(TestDBContext), options => {

            });
            var servContext = services.First(v => v.ServiceType == typeof(TestDBContext));
            var servContextOption = services.First(v => v.ServiceType == typeof(DbContextOptions<TestDBContext>));
            Assert.NotNull(servContext);
            Assert.NotNull(servContextOption);

            IServiceCollection services2 = new ServiceCollection();
            services2.AddDbContext(typeof(TestDBContext));
            var servContext2 = services2.First(v => v.ServiceType == typeof(TestDBContext));
            Assert.NotNull(servContext2);

            IServiceCollection services3 = new ServiceCollection();
            var t = RuntimeDBContextExtention<TestDBContext>.RuntimeContextType;
            services3.AddDbContext(t);
            var servContext3 = services3.First(v => v.ServiceType == t);
            Assert.NotNull(servContext3);
        }
    }
}
