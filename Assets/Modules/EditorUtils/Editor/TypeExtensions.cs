using System;
using System.Runtime.Serialization;

namespace Modules.EditorUtils.Editor
{
    public static class TypeExtensions
    {
        public static object CreateInstance(this Type self)
        {
            try
            {
                return Activator.CreateInstance(self, true);
            }
            catch
            {
                return FormatterServices.GetUninitializedObject(self);
            }
        }

        public static bool Is(this Type self, Type familyType)
        {
            return self.IsSubclassOf(familyType) || self == familyType;
        }
    }
}