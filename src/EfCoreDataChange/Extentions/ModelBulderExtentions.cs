using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Reflection.Emit;

namespace EfCoreDataChange
{
    /// <summary>
    /// Represents a plugin for Microsoft.EntityFrameworkCore to support track and save enity change or delete.
    /// </summary>
    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// Create extention for track entity change.
        /// </summary>
        /// <param name="modelBuilder">The <see cref="ModelBuilder"/> to enable track and save entity change.</param>
        /// <param name="context">The <see cref="DbContext"/> Instance of you DBContext to be configured.</param>
        /// <returns>The <see cref="ModelBuilder"/> had enabled track and save entity change feature.</returns>
        public static void CreateDataChangeTracking(this ModelBuilder modelBuilder, DbContext context)
        {
            AssemblyName myAsmName = new AssemblyName();
            AssemblyBuilder myAsmBuilder = AssemblyBuilder.DefineDynamicAssembly(myAsmName,AssemblyBuilderAccess.Run);
            //myAsmName.Name = Path.GetFileNameWithoutExtension(path)+".proxy.dll";
            foreach( var entityType in modelBuilder.Model.GetEntityTypes().Where(v => !v.IsKeyless))
            {
                var w = entityType.GetKeys().ToArray();

            }
        }
    }
}