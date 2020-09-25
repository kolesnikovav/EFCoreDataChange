using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EfCoreDataChange
{
    /// <summary>
    /// Represents a pair of shadow entity (left) end context entity (right).
    /// </summary>
    internal class PropertyForTransfer
    {
        internal PropertyInfo Left {get;set;}
        internal PropertyInfo Right {get;set;}
    }
    /// <summary>
    /// Represents a pair of context entity and shadow entity.
    /// </summary>
    internal class EntityPropsForTransfer
    {
        internal Type EntityType {get;set;}
        internal Type TrackType {get;set;}
        internal Dictionary<string,PropertyForTransfer>  Props {get;set;} = new Dictionary<string, PropertyForTransfer>();
    }
    /// <summary>
    /// Represents a plugin for Microsoft.EntityFrameworkCore to support track and save enity change or delete.
    /// </summary>
    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// Create extention for track entity change.
        /// </summary>
        /// <param name="modelBuilder">The <see cref="ModelBuilder"/> to enable track and save entity change.</param>
        /// <param name="contextType">The type of DbContext.</param>
        /// <returns>The <see cref="ModelBuilder"/> had enabled track and save entity change feature.</returns>
        public static ModelBuilder CreateDataChangeTracking(this ModelBuilder modelBuilder, Type contextType)
        {
            FieldInfo trackField = contextType.GetField("_isRuntimeConstructedForTrack", BindingFlags.Static | BindingFlags.NonPublic);
            if (trackField != null)
            {
                var p = typeof(RuntimeDBContextExtention<>).MakeGenericType(new Type[] {contextType}).GetProperty("TrackableEntities", BindingFlags.Static | BindingFlags.NonPublic);
                var trackableEntities = p.GetValue(null);
                if (trackableEntities is Dictionary<Type, EntityPropsForTransfer>)
                {
                    foreach (var entityType in (Dictionary<Type, EntityPropsForTransfer>)trackableEntities)
                    {
                        modelBuilder.Entity(entityType.Value.TrackType).HasNoDiscriminator();
                        modelBuilder.Entity(entityType.Value.TrackType).HasKey(entityType.Value.Props.Keys.ToArray());
                        modelBuilder.Entity(entityType.Value.TrackType).Property("Date").HasValueGenerator<ValueGeneratorDataNow>();
                        modelBuilder.Entity(entityType.Value.TrackType).HasIndex("Date").IsUnique(false);
                        modelBuilder.Entity(entityType.Value.TrackType).Property("State").HasValueGenerator<ValueGeneratorEntityState>();
                        modelBuilder.Entity(entityType.Value.TrackType).HasIndex("State").IsUnique(false);
                    }
                }
            }
            return modelBuilder;
        }
    }
}