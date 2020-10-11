using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Reflection;

namespace EfCoreDataChange
{
    internal class KeyComparer : IEqualityComparer<Dictionary<string, object>>
    {
        public bool Equals(Dictionary<string, object> x, Dictionary<string, object> y)
        {
            bool res = true;
            foreach (var v in x)
            {
                res = res && y.ContainsKey(v.Key) && v.Value.Equals(y[v.Key]);
                if (!res) return false;
            }
            return res;
        }
        public int GetHashCode(Dictionary<string, object> obj)
        {
            int res = 0;
            foreach(var v in obj)
            {
                res += v.Key.GetHashCode() + v.Value.GetHashCode();
            }
            return res;
        }
    }
    /// <summary>
    /// Extentions of IQueryable
    /// </summary>
    public static class QueryableExtentions
    {
        private static IQueryable<TDest> FromTrack<TSource, TDest>(this IQueryable<TSource> source) where TSource : class
                                                                                                     where TDest : class
        {
            Type sourceType = (source.Count() == 0) ? typeof(TSource) : source.First<TSource>().GetType();
            var destType = typeof(TDest);
            List<TDest> destinationList = new List<TDest>();
            Dictionary<PropertyInfo,PropertyInfo> propDict = new Dictionary<PropertyInfo,PropertyInfo>();
            // Only keys is needed hire
            foreach(var p in sourceType.GetProperties().Where(v => v.Name != "StateOwner" && v.Name != "Date"))
            {
                propDict.Add(p, destType.GetProperty(p.Name));
            }
            foreach (var el in source)
            {
                var n = Activator.CreateInstance<TDest>();
                foreach(var prop in propDict)
                {
                    prop.Value.SetValue(n, prop.Key.GetValue(el));
                }
                destinationList.Add(n);
            }
            return destinationList.AsQueryable<TDest>();
        }
        /// <summary>
        /// Retrieves keys of entities deleted since moment.
        /// </summary>
        public static IQueryable<TSource> Deleted<TSource>(this IQueryable<TSource> source,DbContext context, DateTime sinceMoment) where TSource: class
        {
            var trackableEntities = DbContextExtention.GetTrackInfoRuntime(context.GetType());
            if (trackableEntities != null && trackableEntities.ContainsKey(typeof(TSource)))
            {
                var trackData = trackableEntities[typeof(TSource)];
                var propTrackDbSet = context.GetType().GetProperties().Where(v => v.Name ==trackData.NameOfTrackDbSet ).First();
                var trackSet = propTrackDbSet.GetValue(context);
                Type trackType = trackData.TrackType;
                Func<object,bool> findPredicate = (a) => {
                    var res = StateOfEntity.Deleted.Equals(a.GetType().GetProperty("StateOwner").GetValue(a));
                    return res && (DateTime)a.GetType().GetProperty("Date").GetValue(a) > sinceMoment;
                };
                return (trackSet as IQueryable<object>).Where<object>(findPredicate).AsQueryable<object>().FromTrack<object, TSource>();
            }
            return source;
        }
        /// <summary>
        /// Retrieves keys of changed entities (added or modified) since moment.
        /// </summary>
        public static IQueryable<TSource> AddedOrChanged<TSource>(this IQueryable<TSource> source,DbContext context, DateTime sinceMoment) where TSource: class
        {
            var trackableEntities = DbContextExtention.GetTrackInfoRuntime(context.GetType());
            if (trackableEntities != null && trackableEntities.ContainsKey(typeof(TSource)))
            {
                var trackData = trackableEntities[typeof(TSource)];
                var propTrackDbSet = context.GetType().GetProperties().Where(v => v.Name ==trackData.NameOfTrackDbSet ).First();
                var trackSet = propTrackDbSet.GetValue(context);
                var entityKeys = context.Model.GetEntityTypes(typeof(TSource)).First().GetKeys();
                Type trackType = trackData.TrackType;

                var trackKeys = context.Model.GetEntityTypes(trackType).First().GetKeys();

                Func<object,bool> findPredicate = (a) => {
                    var res = StateOfEntity.Modified.Equals(a.GetType().GetProperty("StateOwner").GetValue(a))
                         || StateOfEntity.Added.Equals(a.GetType().GetProperty("StateOwner").GetValue(a));
                    return res && (DateTime)a.GetType().GetProperty("Date").GetValue(a) > sinceMoment;
                };
                Func<TSource,Dictionary<string,object>> innerKeySelector = (a) => {
                    Dictionary<string,object> keyVals = new Dictionary<string,object>();
                    foreach(var k in entityKeys)
                    {
                        foreach(var p in k.Properties)
                        {
                            keyVals.Add(p.Name,p.PropertyInfo.GetValue(a));
                        }
                    }
                    return keyVals;
                };
                Func<object,Dictionary<string,object>> outerKeySelector = (a) => {
                    Dictionary<string,object> keyVals = new Dictionary<string,object>();
                    foreach(var k in trackKeys)
                    {
                        foreach(var p in k.Properties)
                        {
                            keyVals.Add(p.Name,p.PropertyInfo.GetValue(a));
                        }
                    }
                    return keyVals;
                };
                Func<object,TSource,TSource> resultSelector = (a,b) => {
                    return b;
                };
                var foundedData = (trackSet as IQueryable<object>).Where<object>(findPredicate);

                return foundedData
                         .Join<object, TSource,Dictionary<string,object>, TSource>(source,outerKeySelector, innerKeySelector, resultSelector, new KeyComparer()).AsQueryable<TSource>();
            }
            return source;
        }
    }
}