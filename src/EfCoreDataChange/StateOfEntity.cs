using System;
using Microsoft.EntityFrameworkCore;

namespace EfCoreDataChange
{
    /// <summary>
    /// This is the analog of EF Core EntityState enum
    /// </summary>
    internal enum StateOfEntity
    {
        /// <summary>
        /// Added entity
        /// </summary>
        Added,
        /// <summary>
        /// Modified entity
        /// </summary>
        Modified,
        /// <summary>
        /// Deleted entity
        /// </summary>
        Deleted,
        /// <summary>
        /// Undefined. All anover Entity state
        /// </summary>
        Undefined
    }
    internal static class StateConversion
    {
        internal static Func<EntityState, StateOfEntity> GetState = (a) =>
        {
            if (a == EntityState.Added) return StateOfEntity.Added;
            if (a == EntityState.Modified) return StateOfEntity.Modified;
            if (a == EntityState.Deleted) return StateOfEntity.Deleted;
            return StateOfEntity.Undefined;
        };
    }
}