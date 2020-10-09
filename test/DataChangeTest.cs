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
                var qq = db.Cats.Deleted<TestDBContext,Cat>(db,DateTime.MinValue).ToList();
                // db.SaveChangesWithTrackInfo();
                // Assert.Equal(1,1);
            }

        }
    }
}
