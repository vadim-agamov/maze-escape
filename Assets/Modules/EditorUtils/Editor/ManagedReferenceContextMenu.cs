using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Modules.EditorUtils.Editor
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class HideInGenericMenuAttribute : Attribute
    {
    }

    static class ManagedReferenceContextMenu
    {
        private static void SetInstance(this SerializedProperty property, object instance)
        {
            var serializedObject = property.serializedObject;
            property.managedReferenceValue = instance;
            serializedObject.ApplyModifiedProperties();
        }
        
        private static readonly GetFieldInfoAndStaticTypeFromProperty _getFieldInfoAndStaticTypeFromProperty;

        private static readonly GetFieldInfoFromProperty _getFieldInfoFromProperty;

        static ManagedReferenceContextMenu()
        {
            _getFieldInfoAndStaticTypeFromProperty = CreateDelegate<GetFieldInfoAndStaticTypeFromProperty>("ScriptAttributeUtility", nameof(GetFieldInfoAndStaticTypeFromProperty));
            _getFieldInfoFromProperty = CreateDelegate<GetFieldInfoFromProperty>("ScriptAttributeUtility", nameof(GetFieldInfoFromProperty));
        }

        private static T CreateDelegate<T>(string className, string methodName) where T : Delegate
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.DefinedTypes)
                .Where(typeInfo => typeInfo.Name == className)
                .SelectMany(typeInfo => typeInfo.DeclaredMethods)
                .Where(methodInfo => methodInfo.Name == methodName)
                .Select(methodInfo => methodInfo.CreateDelegate(typeof(T)) as T)
                .FirstOrDefault();
        }

        private static void OnContextualPropertyMenu(GenericMenu menu, SerializedProperty property)
        {
            if (property.propertyType != SerializedPropertyType.ManagedReference)
            {
                return;
            }

            var menuFunctions = GetMenuFunctions(property)
                .OrderBy(menuFunction => menuFunction.Label);

            foreach (var (label, menuFunction) in menuFunctions)
            {
                menu.AddItem(new GUIContent(label), false, menuFunction);
            }
        }

        private static IEnumerable<(string Label, GenericMenu.MenuFunction MenuFunction)> GetMenuFunctions(SerializedProperty property)
        {
            _getFieldInfoAndStaticTypeFromProperty(property, out var fieldType);
            _getFieldInfoFromProperty(property, out var valueType);

            var serializedProperty = property.Copy();
            var derivedTypes = TypeCache.GetTypesDerivedFrom(fieldType);

            foreach (var derivedType in derivedTypes)
            {
                if (derivedType.IsAbstract || derivedType.IsInterface || derivedType == valueType || derivedType.IsDefined(typeof(HideInGenericMenuAttribute)))
                {
                    continue;
                }

                yield return ($"Convert/{derivedType.Name}", () => serializedProperty.SetInstance(derivedType.CreateInstance()));
            }

            if (valueType.IsAbstract || valueType.IsInterface)
            {
                yield break;
            }

            yield return ("Clear", () => serializedProperty.SetInstance(null));
            yield return ("Reset", () => serializedProperty.SetInstance(valueType.CreateInstance()));
        }

        [InitializeOnLoadMethod]
        private static void Setup()
        {
            try
            {
                if (_getFieldInfoAndStaticTypeFromProperty == null)
                {
                    throw new NullReferenceException($"{nameof(_getFieldInfoAndStaticTypeFromProperty)}");
                }

                if (_getFieldInfoFromProperty == null)
                {
                    throw new NullReferenceException($"{nameof(_getFieldInfoFromProperty)}");
                }

                EditorApplication.contextualPropertyMenu += OnContextualPropertyMenu;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        private delegate FieldInfo GetFieldInfoAndStaticTypeFromProperty(SerializedProperty property, out Type type);

        private delegate FieldInfo GetFieldInfoFromProperty(SerializedProperty property, out Type type);
    }
}
