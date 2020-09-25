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
        public static IQueryable<TSource> Deleted<TContext, TSource>(this IQueryable<TSource> source, TContext context, DateTime sinceMoment) where TContext: DbContext
        {
            return null;
            // var trackableEntities = DbContextExtention.GetTrackInfo(typeof(TContext));
            // if (trackableEntities != null && trackableEntities.ContainsKey(typeof(TSource)))
            // {
            //     var mI = ReflectionUtils.MethodSet(typeof(TContext), typeof(TSource));
            //     var res = mI.Invoke(context, null).Where 

            // }
            // // var p = typeof(RuntimeDBContextExtention<>).MakeGenericType(new Type[] { typeof(TContext).BaseType }).GetProperty("TrackableEntities", BindingFlags.Static | BindingFlags.NonPublic);
            // // var trackableEntities = p.GetValue(null);
            // // if (trackableEntities is Dictionary<Type, EntityPropsForTransfer>)
            // // {
            // //     if ((trackableEntities as Dictionary<Type, EntityPropsForTransfer>).ContainsKey(typeof(TSource)))
            // //     {
            // //         var deletedKeys = (trackableEntities as Dictionary<Type, EntityPropsForTransfer>)[typeof(TSource)].TrackType;
            // //     }

            // // }
            // return source.Where<TSource>().AsQueryable();
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