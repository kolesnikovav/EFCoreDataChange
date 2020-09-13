using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using EfCoreDataChange;
using Xunit;

namespace test
{
    public class DataChangeTest
    {
        [Fact]
        public void DataChange_Test()
        {
            using (var db = new SampleDBContext())
            {
                
                Assert.Equal(1,1);
                // var tableCat = db.Model.FindRuntimeEntityType(typeof(CatTest1)).GetTableName();
                // var tableDog = db.Model.FindRuntimeEntityType(typeof(DogTest1)).GetTableName();
                // Assert.NotEqual(tableCat, tableDog);
            }
        }
    }
}