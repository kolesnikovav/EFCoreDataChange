using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using EfCoreDataChange;

namespace test
{

    public class SampleDBContext : DbContext
    {
        public DbSet<Cat> Cats { get; set; }
        public DbSet<Dog> Dogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("test");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dog>().HasKey(v => new {v.Name, v.Age});
            modelBuilder.CreateDataChangeTracking(this);
        }
    }

    public class Cat
    {
        [Key]
        public int Id {get;set;}
        public string Name {get;set;}
        public int Age {get;set;}
    }
    // Suppose this entity has composite key
    public class Dog
    {
        public string Name {get;set;}
        public int Age {get;set;}
    }
}