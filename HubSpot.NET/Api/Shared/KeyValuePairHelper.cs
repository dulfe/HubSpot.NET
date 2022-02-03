namespace HubSpot.NET.Api.Shared
{
    using HubSpot.NET.Core;
    using HubSpot.NET.Core.Attributes;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    
    public class KeyValuePairHelper
	{
        public void ToPropertyTransportModel<TModel, TKeyValue>(TModel model, ICollection<TKeyValue> properties)
            where TKeyValue : IKeyValuePair, new()
        {
            PropertyInfo[] modelProperties = model.GetType().GetProperties();

            foreach (PropertyInfo prop in modelProperties)
            {
                var memberAttrib = prop.GetCustomAttribute(typeof(DataMemberAttribute)) as DataMemberAttribute;
                if (memberAttrib == null)
                    continue;
                var ignoreAttrib = prop.GetCustomAttribute(typeof(IgnoreDataMemberAttribute)) as IgnoreDataMemberAttribute;
                if (ignoreAttrib != null)
                    continue;

                object propValue = prop.GetValue(model);
                if (propValue == null)
                    continue;

                // IF we have an complex type on the entity that we are trying to convert, let's NOT get the 
                // string value of it, but simply pass the object along - it will be serialized later as JSON...
                bool isLongDateTime = Attribute.GetCustomAttributes(prop).Any(a => a.GetType() == typeof(LongDateTimeAttribute));
                bool isLongDate = Attribute.GetCustomAttributes(prop).Any(a => a.GetType() == typeof(LongDateAttribute));
                object value = propValue.IsComplexType()
                    ? propValue
                    :
                        prop.PropertyType == typeof(DateTime?) || prop.PropertyType == typeof(DateTime)
                            ?
                                isLongDateTime
                                    ? propValue == null ? null : (object)Convert.ToInt64(Math.Floor(((DateTime)propValue).Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds))
                                    :
                                        isLongDate
                                            ? propValue == null ? null : (object)Convert.ToInt64(Math.Floor(((DateTime)propValue).Date.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds))
                                            : propValue == null ? null : (object)((DateTime)propValue).ToString("yyyy-MM-dd")
                            :
                                prop.PropertyType == typeof(bool)
                                    ? propValue?.ToString().ToLowerInvariant()
                                    :
                                        prop.PropertyType == typeof(bool?)
                                            ? propValue?.ToString().ToLowerInvariant()
                                            : propValue?.ToString();

                if (prop.PropertyType.IsArray && typeof(TKeyValue).IsAssignableFrom(prop.PropertyType.GetElementType()))
                {
                    TKeyValue[] pairs = value as TKeyValue[];
                    foreach (TKeyValue item in pairs)
                        properties.Add(item);
                    continue;
                }
                else if (typeof(IEnumerable<>).IsAssignableFrom(prop.PropertyType) && typeof(TKeyValue).IsAssignableFrom(prop.PropertyType.GetElementType()))
                {
                    IEnumerable<TKeyValue> pairs = value as IEnumerable<TKeyValue>;
                    foreach (var item in pairs)
                        properties.Add(item);
                    continue;
                }

                properties.Add(new TKeyValue { Key = memberAttrib.Name, Value = value.ToString() });
            }
        }

        public TModel FromPropertyTransportModel<TModel, TKeyValue>(IEnumerable<TKeyValue> properties)
             where TKeyValue : IKeyValuePair
        {
            var model = (TModel)Assembly.GetAssembly(typeof(TModel)).CreateInstance(typeof(TModel).FullName);

            FromPropertyTransportModel(model, properties);

            return model;
        }

        public void FromPropertyTransportModel<TModel, TKeyValue>(TModel model, IEnumerable<TKeyValue> properties)
             where TKeyValue : IKeyValuePair
        {
            PropertyInfo[] modelProperties = model.GetType().GetProperties();

            foreach(TKeyValue keyValuePair in properties)
			{
                foreach (PropertyInfo prop in modelProperties)
                {
                    var memberAttrib = prop.GetCustomAttribute(typeof(DataMemberAttribute)) as DataMemberAttribute;

                    if (memberAttrib == null || memberAttrib.Name != keyValuePair.Key)
                        continue;

                    TKeyValue pair = properties.FirstOrDefault(x => x.Key == memberAttrib.Name);
                    if (pair != null)
					{
                        prop.SetValue(model, pair.Value);
                        break;
                    }
                }
            }
        }
    }
}
