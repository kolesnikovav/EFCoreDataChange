using System;
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
            // Type T = RuntimeDBContextExtention<TestDBContext>.CreateContextType();
            // var qq = Activator.CreateInstance(T);
            // var qqq=1;
            using (var db = RuntimeDBContextExtention<TestDBContext>.RuntimeContext)
            {
                var cat1 = new Cat { Name = "Alice", Age = 2};
                db.Cats.Add(cat1);
                // db.SaveChangesWithTrackInfo();
                // Assert.Equal(1,1);
            }

        }
    }
}
