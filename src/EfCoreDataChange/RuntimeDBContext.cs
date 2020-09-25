using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Reflection;
using System.Reflection.Emit;

namespace EfCoreDataChange
{
    /// <summary>
    /// Runtime DbContext type generator
    /// </summary>
    public static class RuntimeDBContextExtention<T> where T : DbContext, IDisposable
    {
        private static readonly IModel _originalModel;
        private static readonly Dictionary<Type, EntityPropsForTransfer> _dTrackKeys = new Dictionary<Type, EntityPropsForTransfer>();

        private static Type _dbContextType;

        /// <summary>
        /// Runtime DbContext Type instance with track entities
        /// </summary>
        public static T RuntimeContext
        {
            get
            {
                var instance = (T)Activator.CreateInstance(_dbContextType);
                return instance;
            }
        }
        /// <summary>
        /// Runtime DbContext Type instance with track entities
        /// </summary>
        internal static Dictionary<Type, EntityPropsForTransfer> TrackableEntities
        {
            get
            {
                return _dTrackKeys;
            }
        }
        /// <summary>
        /// Runtime DbContext Type instance with track entities
        /// </summary>
        internal static Dictionary<Type, EntityPropsForTransfer> GetTrackableEntities(Type contextType)
        {
                return _dTrackKeys;
        }
        private static Type CreateContextType()
        {
            AssemblyName myAsmName = new AssemblyName("___runtime_dbcontext_assembly___");
            AssemblyBuilder myAsmBuilder = AssemblyBuilder.DefineDynamicAssembly(myAsmName, AssemblyBuilderAccess.Run);
            ModuleBuilder myModBuilder =
                  myAsmBuilder.DefineDynamicModule("___runtime_track_module___");
            var createdType = myModBuilder.DefineType(typeof(T).FullName + "_Runtime", TypeAttributes.Class | TypeAttributes.Public, typeof(T));
            List<IMutableEntityType> entityTypes = _originalModel.GetEntityTypes().Cast<IMutableEntityType>().Where(v => !v.IsKeyless).ToList();
            foreach (var entityType in entityTypes)
            {
                Dictionary<string, Type> entityKeys = new Dictionary<string, Type>();
                if (entityType != null)
                {
                    foreach (var key in entityType.GetKeys())
                    {
                        foreach (var keyProp in key.Properties)
                        {
                            entityKeys.Add(keyProp.Name, keyProp.ClrType);
                        }
                    }
                }
                if (entityKeys.Count() > 0)
                {
                    string nameTrack = CommonUtils.TrackerTypeName(entityType.Name);
                    TypeBuilder createdTypeProp = myModBuilder.DefineType(nameTrack, TypeAttributes.Class | TypeAttributes.Public);
                    EntityPropsForTransfer entityKeyProps = new EntityPropsForTransfer();
                    entityKeyProps.EntityType = entityType.ClrType;
                    foreach (var k in entityKeys)
                    {
                        FieldBuilder fieldBuilder = createdTypeProp.DefineField(CommonUtils.FieldName(k.Key), k.Value, FieldAttributes.Private);
                        var pb = createdTypeProp.DefineProperty(k.Key, PropertyAttributes.None, CallingConventions.Standard, k.Value, null);
                        ReflectionUtils.AddGetSetMethodsForProperty(pb, k.Key, k.Value, createdTypeProp, fieldBuilder);
                    }
                    FieldBuilder fieldBuilderState = createdTypeProp.DefineField(CommonUtils.FieldName("_state"), typeof(EntityState), FieldAttributes.Private);
                    var pbState = createdTypeProp.DefineProperty("State", PropertyAttributes.None, CallingConventions.Standard, typeof(EntityState), null);
                    ReflectionUtils.AddGetSetMethodsForProperty(pbState, "State", typeof(EntityState), createdTypeProp, fieldBuilderState);

                    FieldBuilder fieldBuilderDate = createdTypeProp.DefineField(CommonUtils.FieldName("_date"), typeof(DateTime), FieldAttributes.Private);
                    var pbData = createdTypeProp.DefineProperty("Date", PropertyAttributes.None, CallingConventions.Standard, typeof(EntityState), null);
                    ReflectionUtils.AddGetSetMethodsForProperty(pbData, "Date", typeof(DateTime), createdTypeProp, fieldBuilderDate);
                    createdTypeProp.CreateTypeInfo();

                    Type tP = typeof(DbSet<>).MakeGenericType(new Type[] { createdTypeProp });
                    FieldBuilder fieldBuilderDbSet = createdType.DefineField(CommonUtils.FieldName(entityType.ClrType + "_Track"), tP, FieldAttributes.Private);
                    var pbTrack = createdType.DefineProperty(nameTrack, PropertyAttributes.None, CallingConventions.Standard, tP, null);
                    ReflectionUtils.AddGetSetMethodsForProperty(pbTrack, pbTrack, createdType, fieldBuilderDbSet);
                    entityKeyProps.TrackType = createdTypeProp;
                    foreach (var k in entityKeys)
                    {
                        entityKeyProps.Props.Add(k.Key, new PropertyForTransfer()
                        {
                            Left = createdTypeProp.GetRuntimeProperties().Where(v => v.Name == k.Key).First(),
                            Right = entityType.ClrType.GetRuntimeProperties().Where(v => v.Name == k.Key).First()
                        });
                    }
                    _dTrackKeys.Add(entityKeyProps.EntityType, entityKeyProps);
                }
            }
            var fldTrackInfo = createdType.DefineField("_isRuntimeConstructedForTrack", typeof(byte), FieldAttributes.Static | FieldAttributes.Private);
            createdType.CreateTypeInfo();
            //fldTrackInfo.SetValue(null, _dTrackKeys);
            return createdType;
        }

        static RuntimeDBContextExtention()
        {
            using (T instance = Activator.CreateInstance<T>())
            {
                _originalModel = instance.Model;
                _dbContextType = CreateContextType();
            }
        }
    }


}