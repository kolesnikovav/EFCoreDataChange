using System;

namespace EfCoreDataChange
{
    internal static class CommonUtils
    {
        internal static string TrackerTypeName(string entityTypeName)
        => entityTypeName + "_Track";

        internal static string FieldName(string propertyName)
        => "_" + propertyName.ToLowerInvariant();

        internal static string GetMethodName(string propertyName)
        => "get__" + propertyName;

        internal static string SetMethodName(string propertyName)
        => "set__" + propertyName;
    }
}