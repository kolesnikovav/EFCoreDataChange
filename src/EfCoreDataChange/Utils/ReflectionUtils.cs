using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq.Expressions;

namespace EfCoreDataChange
{
    internal static class ReflectionUtils
    {
        internal static void AddGetSetMethodsForProperty(PropertyBuilder pb, PropertyInfo cProp, TypeBuilder createdType, FieldBuilder fieldBuilder)
        {
            MethodBuilder methodBuilderGet = createdType.DefineMethod("get_" + cProp.Name, MethodAttributes.Public);
            methodBuilderGet.SetReturnType(cProp.PropertyType);
            //create IL code for get
            ILGenerator genusGetIL = methodBuilderGet.GetILGenerator();
            genusGetIL.Emit(OpCodes.Ldarg_0);
            genusGetIL.Emit(OpCodes.Ldfld, fieldBuilder);
            genusGetIL.Emit(OpCodes.Ret);
            pb.SetGetMethod(methodBuilderGet);

            MethodBuilder methodBuilderSet = createdType.DefineMethod("set_" + cProp.Name, MethodAttributes.Public);
            methodBuilderSet.SetParameters(new Type[] { cProp.PropertyType });
            //create IL code for set
            ILGenerator genusSetIL = methodBuilderSet.GetILGenerator();
            genusSetIL.Emit(OpCodes.Ldarg_0);
            genusSetIL.Emit(OpCodes.Ldarg_1);
            genusSetIL.Emit(OpCodes.Stfld, fieldBuilder);
            genusSetIL.Emit(OpCodes.Ret);
            pb.SetSetMethod(methodBuilderSet);
        }

        internal static void AddGetSetMethodsForProperty(PropertyBuilder pb, string propName, Type propType, TypeBuilder createdType, FieldBuilder fieldBuilder)
        {
            MethodBuilder methodBuilderGet = createdType.DefineMethod(CommonUtils.GetMethodName(propName), MethodAttributes.Public);
            methodBuilderGet.SetReturnType(propType);
            //create IL code for get
            ILGenerator genusGetIL = methodBuilderGet.GetILGenerator();
            genusGetIL.Emit(OpCodes.Ldarg_0);
            genusGetIL.Emit(OpCodes.Ldfld, fieldBuilder);
            genusGetIL.Emit(OpCodes.Ret);
            pb.SetGetMethod(methodBuilderGet);

            MethodBuilder methodBuilderSet = createdType.DefineMethod(CommonUtils.SetMethodName(propName), MethodAttributes.Public);
            methodBuilderSet.SetParameters(new Type[] { propType });
            //create IL code for set
            ILGenerator genusSetIL = methodBuilderSet.GetILGenerator();
            genusSetIL.Emit(OpCodes.Ldarg_0);
            genusSetIL.Emit(OpCodes.Ldarg_1);
            genusSetIL.Emit(OpCodes.Stfld, fieldBuilder);
            genusSetIL.Emit(OpCodes.Ret);
            pb.SetSetMethod(methodBuilderSet);
        }
        internal static MethodInfo MethodSet(Type contextType, Type entityType)
        => contextType.GetMethods(BindingFlags.Public).Where(v => v.Name == "Set" && v.IsGenericMethod).First()
          .MakeGenericMethod(new Type[] {entityType});
    }
}