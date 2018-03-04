using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace CoAPExplorer.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayValue(this Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            var descriptionAttributes = fieldInfo.GetCustomAttributes(
                typeof(DisplayAttribute), false) as DisplayAttribute[];

            if (descriptionAttributes[0].ResourceType != null)
            {
                foreach (PropertyInfo staticProperty in descriptionAttributes[0].ResourceType.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
                {
                    if (staticProperty.PropertyType == typeof(System.Resources.ResourceManager))
                    {
                        System.Resources.ResourceManager resourceManager = (System.Resources.ResourceManager)staticProperty.GetValue(null, null);
                        return resourceManager.GetString(descriptionAttributes[0].Name);
                    }
                }

                return descriptionAttributes[0].Name; // Fallback with the key name
            }

            if (descriptionAttributes == null) return string.Empty;
            return (descriptionAttributes.Length > 0) ? descriptionAttributes[0].Name : value.ToString();
        }
    }
}
