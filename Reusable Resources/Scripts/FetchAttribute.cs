#nullable  enable

using System;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

namespace Utility.Development
{
    [AttributeUsage(AttributeTargets.Field)]
    public class FetchAttribute : Attribute
    {
        #region Variables
        private bool searchInChildren;
        #endregion

        #region Constructor
        public FetchAttribute(bool searchInChildren = false)
        {
            this.searchInChildren = searchInChildren;
        }
        #endregion

        #region Fetch
        private void Fetch<T>(T instance, FieldInfo field) where T : Component
        {
            Component component;
            if (searchInChildren)
            {
                component = instance.gameObject.GetComponentInChildren(field.FieldType);
            }
            else
            {
                component = instance.gameObject.GetComponent(field.FieldType);
            }
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
