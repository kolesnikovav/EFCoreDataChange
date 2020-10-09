using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EfCoreDataChange
{
    /// <summary>
    /// Extention of DbContext
    /// </summary>
    public static class DbContextExtention
    {
        internal static Dictionary<Type, EntityPropsForTransfer> GetTrackInfo(Type contextType)
        {
            var p = typeof(RuntimeDBContextExtention<>).MakeGenericType(new Type[] { contextType.BaseType }).GetProperty("TrackableEntities", BindingFlags.Static | BindingFlags.NonPublic);
            var trackableEntities = p.GetValue(null);
            if (trackableEntities is Dictionary<Type, EntityPropsForTransfer>) return (Dictionary<Type, EntityPropsForTransfer>)trackableEntities;
            return null;
        }
        internal static Dictionary<Type, EntityPropsForTransfer> GetTrackInfoRuntime(Type contextType)
        {
            var p = typeof(RuntimeDBContextExtention<>).MakeGenericType(new Type[] { contextType }).GetProperty("TrackableEntities", BindingFlags.Static | BindingFlags.NonPublic);
            var trackableEntities = p.GetValue(null);
            if (trackableEntities is Dictionary<Type, EntityPropsForTransfer>) return (Dictionary<Type, EntityPropsForTransfer>)trackableEntities;
            return null;
        }
        /// <summary>
        /// Create extention for track entity change.
        /// </summary>
        /// <param name="context">DbContext</param>
        public static void PrepareTrackInfo(this DbContext context)
        {
            if (context.ChangeTracker.HasChanges())
            {
                var trackableEntities = GetTrackInfo(context.GetType());
                if (trackableEntities != null)
                {
                    var changed = context.ChangeTracker.Entries().ToArray();
                    foreach (var entry in changed.Where(v => v.State == EntityState.Added || v.State == EntityState.Deleted || v.State ==EntityState.Modified))
                    {
                        if (trackableEntities.ContainsKey(entry.Metadata.ClrType))
                        {
                            EntityPropsForTransfer data = trackableEntities[entry.Metadata.ClrType];
                            var trackInstance = Activator.CreateInstance(data.TrackType);
                            foreach(var p in data.Props)
                            {
                                p.Value.Left.SetValue(trackInstance, p.Value.Right.GetValue(entry.Entity));
                            }
                            data.StatePropertyInfo.SetValue(trackInstance, entry.State);
                            try
                            {
                                context.Add(trackInstance);
                            }
                            catch
                            {
                                context.Update(trackInstance);
                            }
                        }
                    }
                }
            }
        }

    }
}