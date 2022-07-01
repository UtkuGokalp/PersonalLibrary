#nullable  enable

using System;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using Unity.VisualScripting;
using Object = UnityEngine.Object;

namespace Utility.Development
{
    [AttributeUsage(AttributeTargets.Field)]
    public class FetchAttribute : Attribute
    {
        #region SearchType
        public enum SearchType
        {
            GameObject,
            Children,
            World
        }
        #endregion
        
        #region Variables
        private SearchType searchType;
        #endregion

        #region Constructor
        public FetchAttribute(SearchType searchType = SearchType.GameObject)
        {
            this.searchType = searchType;
        }
        #endregion

        #region Fetch
        private void Fetch<T>(T instance, FieldInfo field) where T : Component
        {
            Component component = searchType switch
            {
                SearchType.GameObject => instance.gameObject.GetComponent(field.FieldType),
                SearchType.Children => instance.gameObject.GetComponentInChildren(field.FieldType),
                SearchType.World => Object.FindObjectOfType(field.FieldType).GetComponent(field.FieldType),
                _ => throw new ArgumentOutOfRangeException()
            };
            field.SetValue(instance, component);
        }
        #endregion

        #region RunFetch
        /// <summary>
        /// Runs all the fetch attributes declared for the fields.
        /// The instance variable needs to be the type of the class, pass "this" instead of "gameObject".
        /// </summary>
        public static void RunFetch<T>(T instance) where T : Component
        {
            Type type = instance.GetType();
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                IEnumerable<FetchAttribute> fetchAttributes = fieldInfo.GetCustomAttributes<FetchAttribute>();
                foreach (FetchAttribute fetchAttribute in fetchAttributes)
                {
                    fetchAttribute.Fetch(instance, fieldInfo);
                }
            }
        }
        #endregion
    }
}
