using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Reflection;

namespace EfCoreDataChange
{
    /// <summary>
    /// Extentions of IQueryable
    /// </summary>
    public static class QueryableExtentions
    {
        /// <summary>
        /// Retrieves keys of entities deleted since moment.
        /// </summary>
        public static IQueryable<TSource> Deleted<TContext, TSource>(this IQueryable<TSource> source,TContext context, DateTime sinceMoment) where TContext: DbContext
        {
            var trackableEntities = DbContextExtention.GetTrackInfoRuntime(typeof(TContext));
            if (trackableEntities != null && trackableEntities.ContainsKey(typeof(TSource)))
            {
                var trackData = trackableEntities[typeof(TSource)];
                var trackSet = trackData.DbSetPropertyInfo.GetValue(context);
                Type trackType = trackData.TrackType;
                Func<TSource, object, bool> firstPredicate = (a, b) =>
                {
                    var res = true;
                    foreach (var p in trackData.Props)
                    {
                        res = res && p.Value.Left.GetValue(b) == p.Value.Right.GetValue(a);
                        if (!res) return false;
                    }
                    res = res && EntityState.Deleted.Equals(trackData.DbSetPropertyInfo.GetValue(b));
                    DateTime d = (DateTime)trackData.DatePropertyInfo.GetValue(b);
                    res = res && d > sinceMoment;
                    return res;
                };
                Func<TSource, bool> fnc = (a) =>
                {
                    return (trackSet as DbSet<object>).Single(v =>  firstPredicate(a,v)) != null;
                };
                return source.Where<TSource>(fnc).AsQueryable<TSource>();
            }
            return source;
        }
        /// <summary>
        /// Retrieves keys of changed entities (added or modified) since moment.
        /// </summary>
        public static IQueryable<TSource> Changed<TSource>(this IQueryable<TSource> source, DateTime sinceMoment)
        {
            return null;
            // return source.Where<TSource>(p).AsQueryable();
        }
    }
}