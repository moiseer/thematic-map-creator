using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ThematicMapCreator.Api.Core
{
    public class KeyJsonConverter<T, TKey> : JsonConverter
    {
        private readonly string keyPropertyName;
        private readonly Dictionary<TKey, Type> registeredTypes = new Dictionary<TKey, Type>();

        public KeyJsonConverter(Expression<Func<T, TKey>>  keySelector)
        {
            keyPropertyName = GetPropertyInfo(keySelector).Name;
        }

        public override bool CanWrite { get; } = false;

        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            if (!jObject.TryGetValue(keyPropertyName, StringComparison.InvariantCultureIgnoreCase, out var jKey))
            {
                throw new Exception("Can`t find key.");
            }

            var key = jKey.ToObject<TKey>();
            if (!registeredTypes.TryGetValue(key, out Type targetType))
            {
                throw new Exception("Not registered type.");
            }

            return jObject.ToObject(targetType);
        }

        public KeyJsonConverter<T, TKey> RegisterType<TTarget>(TKey key)
            where TTarget : T, new()
        {
            registeredTypes[key] = typeof(TTarget);
            return this;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        private static PropertyInfo GetPropertyInfo(Expression<Func<T, TKey>> propertyLambda)
        {
            MemberExpression member = propertyLambda.Body as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a method, not a property.");
            }

            PropertyInfo propertyInfo = member.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a field, not a property.");
            }

            Type type = typeof(T);
            if (type != propertyInfo.ReflectedType && !type.IsSubclassOf(propertyInfo.ReflectedType))
            {
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a property that is not from type {type}.");
            }

            return propertyInfo;
        }
    }
}
