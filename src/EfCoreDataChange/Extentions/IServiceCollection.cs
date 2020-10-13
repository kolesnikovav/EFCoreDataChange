using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace EfCoreDataChange
{
    /// <summary>
    /// Extentions of IServiceCollection
    /// </summary>
    public static class ServiceExtentions
    {
        /// <summary>
        /// Adds DbContext to IServiceCollection instance
        /// <param name="services">The <see cref="IServiceCollection"/> Instance of you IServiceCollection to add DbContext.</param>
        /// <param name="dbContextType">Type of DbContext</param>
        /// <param name="options">DbContext options</param>
        /// <param name="serviceLifetime">Lifetime of DbContext Default = Scoped</param>
        /// <param name="optionsLifeTime">Lifetime of DbContext options Default = Scoped</param>
        /// </summary>
        public static void AddDbContext(this IServiceCollection services,
         Type dbContextType, Action<DbContextOptionsBuilder> options,
         ServiceLifetime serviceLifetime = ServiceLifetime.Scoped,
          ServiceLifetime optionsLifeTime = ServiceLifetime.Scoped)
        {
            var actType = typeof(Action<DbContextOptionsBuilder>);
            typeof(EntityFrameworkServiceCollectionExtensions).GetMethods(BindingFlags.Static| BindingFlags.Public)
            .Where(v => v.Name == "AddDbContext" && v.IsGenericMethod && v.GetGenericArguments().Count() == 1)
            .Where(v => v.GetParameters().Count() == 4)
            .Where(v => v.GetParameters().Where(p => p.ParameterType == actType).Count() == 1)
            .First()
            .MakeGenericMethod(new Type[] {dbContextType})
            .Invoke(null, new object[] {
                services, options, serviceLifetime, optionsLifeTime
            });
        }
        /// <summary>
        /// Adds DbContext to IServiceCollection instance
        /// <param name="services">The <see cref="IServiceCollection"/> Instance of you IServiceCollection to add DbContext.</param>
        /// <param name="dbContextType">Type of DbContext</param>
        /// <param name="serviceLifetime">Lifetime of DbContext Default = Scoped</param>
        /// <param name="optionsLifeTime">Lifetime of DbContext options Default = Scoped</param>
        /// </summary>
        public static void AddDbContext(this IServiceCollection services,
         Type dbContextType,
         ServiceLifetime serviceLifetime = ServiceLifetime.Scoped,
          ServiceLifetime optionsLifeTime = ServiceLifetime.Scoped)
        {
            typeof(EntityFrameworkServiceCollectionExtensions).GetMethods(BindingFlags.Static| BindingFlags.Public)
            .Where(v => v.Name == "AddDbContext" && v.IsGenericMethod && v.GetGenericArguments().Count() == 1)
            .Where(v => v.GetParameters().Count() == 3)
            .First()
            .MakeGenericMethod(new Type[] {dbContextType})
            .Invoke(null, new object[] {
                services, serviceLifetime, optionsLifeTime
            });
        }
    }
}