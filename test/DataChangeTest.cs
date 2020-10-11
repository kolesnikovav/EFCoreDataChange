using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using EfCoreDataChange;
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
    }
}
