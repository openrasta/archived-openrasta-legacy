#region License
/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using OpenRasta.Binding;

namespace OpenRasta.TypeSystem.ReflectionBased
{
    public static class ReflectionExtensions
    {
        public static string ConvertToString(this object target)
        {
            if (target == null) return null;
            var targetType = target.GetType();
            if (targetType.IsPrimitive)
                return Convert.ToString(target);
#if !SILVERLIGHT
            var converter = TypeDescriptor.GetConverter(target);

            if (converter != null && converter.CanConvertTo(typeof(string)))
                return converter.ConvertToString(target);
#endif
            return target.ToString();
        }

        public static object CreateInstance(this Type type, params object[] constructorArguments)
        {
            try
            {
                return Activator.CreateInstance(type, constructorArguments);
            }
            catch (Exception)
            {
                return CreateInstance(type);
            }
        }

        public static object CreateInstance(this Type type)
        {
            if (type.IsInterface)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                    return Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(type.GetGenericArguments()));

                if (type.FindInterface(typeof(IEnumerable<>)) != null)
                    return Activator.CreateInstance(typeof(List<>).MakeGenericType(type.GetGenericArguments()));
            }

            return Activator.CreateInstance(type);
        }

        /// <summary>
        /// Creates a type using the provided string to initialize its values.
        /// </summary>
        /// <param name="type">The type of object to create.</param>
        /// <param name="propertyValue">The value to assign to the created object.</param>
        /// <returns>The created object.</returns>
        public static object CreateInstanceFrom(this Type type, string propertyValue)
        {
            return CreateInstanceFromString(type, propertyValue, null);
        }

        /// <exception cref="NotSupportedException">You can only pass an array of strings or a single string</exception>
        public static object CreateInstanceFrom(this Type type, object content)
        {
            string parameterValueAsString;
            if ((parameterValueAsString = content as string) != null)
            {
                return type.CreateInstanceFrom(parameterValueAsString);
            }

            string[] parameterValueAsStringArray;
            if ((parameterValueAsStringArray = content as string[]) != null)
            {
                return type.CreateInstanceFrom(parameterValueAsStringArray);
            }

            if (type.IsAssignableFrom(content.GetType()))
                return content;
            throw new NotSupportedException("You can only pass an array of strings or a single string");
        }

        /// <exception cref="NotSupportedException"><c>NotSupportedException</c>.</exception>
        public static object CreateInstanceFrom<T>(this Type type,
                                                   IEnumerable<T> propertyValues,
                                                   ValueConverter<T> converter)
        {
            // identity conversion
            if (type == propertyValues.GetType())
                return propertyValues;

            // arrays
            var propertyValuesAsArray = propertyValues.ToArray();
            if (type.IsArray)
            {
                var elementType = type.GetElementType();

                var values = Array.CreateInstance(elementType, propertyValuesAsArray.Length);
                int i = 0;
                if (TryConvert(propertyValuesAsArray,
                           elementType,
                           converter,
                           converted => values.SetValue(converted, i++)))
                    return values;
                throw new NotSupportedException("Could not convert he values to an array");
            }
            // IEnumerable<>
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                type = typeof(IList<>).MakeGenericType(type.GetGenericArguments());
            }

            // ICollection<T>
            if (type.Implements(typeof(ICollection<>)))
            {
                var collection = type.CreateInstance();
                var elementType = type.GetGenericArguments()[0];
                var interfaceMethod = typeof(ICollection<>).MakeGenericType(elementType).GetMethod("Add",
                                                                                                   new[] { elementType });

                if (TryConvert(propertyValuesAsArray,
                           elementType,
                           converter,
                           converted => interfaceMethod.Invoke(collection, new[] { converted })))
                    return collection;
            }

            if (propertyValuesAsArray.Length == 1)
            {
                var bindingResult = converter(propertyValuesAsArray[0], type);
                if (bindingResult.Successful)
                    return bindingResult.Instance;
                throw new NotSupportedException("The single value could not be converted.");
            }

            // IList
            if (type.Implements(typeof(IList)) && type.HasDefaultConstructor())
            {
                var list = (IList)Activator.CreateInstance(type);

                foreach (var item in propertyValues)
                {
                    list.Add(item);
                }
                return list;
            }

            throw new NotSupportedException(
                string.Format("TargetType {0} couldn't be instantiated from the provided values", type.Name));
        }

        public static object CreateInstanceFrom(this Type type, string[] propertyValues)
        {
            return CreateInstanceFrom(type,
                                      propertyValues,
                                      (str, destinationType) =>
                                      {
                                          var destination = CreateInstanceFrom(destinationType, str);
                                          return BindingResult.Success(destination);
                                      });
        }

        public static T FindAttribute<T>(this ICustomAttributeProvider mi) where T : Attribute
        {
            return mi.GetCustomAttributes(true).OfType<T>().FirstOrDefault();
        }

        /// <exception cref="InvalidOperationException"><c>InvalidOperationException</c>.</exception>
        public static KeyValuePair<PropertyInfo, object[]>? FindIndexer(this IEnumerable<KeyValuePair<PropertyInfo, ParameterInfo[]>> indexers, params string[] values)
        {
            if (values == null || indexers == null)
                throw new InvalidOperationException();
            foreach (var indexer in indexers)
            {
                var results = new object[indexer.Value.Length];
                try
                {
                    for (int i = 0; i < indexer.Value.Length; i++)
                        results[i] = indexer.Value[i].ParameterType.CreateInstanceFrom(values[i]);
                }
                catch
                {
                    continue;
                }

                return new KeyValuePair<PropertyInfo, object[]>(indexer.Key, results);
            }

            return null;
        }

        /// <summary>
        /// Gets a list of default properties (indexers) with the provided number of parameters.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="parameterCount"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><c>target</c> is null.</exception>
        public static IEnumerable<KeyValuePair<PropertyInfo, ParameterInfo[]>> FindIndexers(this Type target, int parameterCount)
        {
            if (target == null) throw new ArgumentNullException("target");
            return from property in target.GetDefaultMembers().OfType<PropertyInfo>()
                   let parameters = property.GetIndexParameters()
                   where parameters.Length == parameterCount
                   select new KeyValuePair<PropertyInfo, ParameterInfo[]>(property, parameters);
        }

        /// <summary>
        /// Returns the interface implemented by a type, or null if no matching interface was found.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="interfaceType">The interface to look for, including open generic types, for exemple <c>typeof(IList&lt;&gt;)</c></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><c>type</c> is null.</exception>
        /// <exception cref="ArgumentException">The type is not an interface</exception>
        public static Type FindInterface(this Type type, Type interfaceType)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (interfaceType == null) throw new ArgumentNullException("interfaceType");
            if (!interfaceType.IsInterface) throw new ArgumentException("The type is not an interface", "interfaceType");
            if (type == interfaceType)
                return type;

            if (!interfaceType.IsGenericTypeDefinition)
                return type.GetInterface(interfaceType.FullName, false);
            IEnumerable<Type> interfacesToSearchFor;
            if (type.IsInterface)
                interfacesToSearchFor = new[] { type };
            else
                interfacesToSearchFor = new Type[0];
            return interfacesToSearchFor.Concat(type.GetInterfaces())
                .FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == interfaceType);
        }

        public static PropertyInfo FindPropertyCaseInvariant(this Type type, string propertyName)
        {
            try
            {
                return type.GetProperty(propertyName,
                                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase,
                                        null,
                                        null,
                                        Type.EmptyTypes,
                                        null);
            }
            catch
            {
                return null;
            }
        }

        public static object GetDefaultValue(this Type type)
        {
            if (type.IsValueType)
                return Activator.CreateInstance(type);
            return null;
        }

        public static IEnumerable<Type> GetInheritanceChain(this Type type)
        {
            if (type.IsInterface)
                yield break;
            do
            {
                yield return type;
                type = type.BaseType;
            }
            while (type != null);
        }

        /// <summary>
        /// Returns the distance in the inheritance chain between this type and the parent type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parentType"></param>
        /// <returns></returns>
        public static int GetInheritanceDistance(this Type type, Type parentType)
        {
            int distance = 0;
            if (parentType.IsInterface)
            {
                if (parentType.IsAssignableFrom(type))
                    return 0;
            }

            // For general purpose registrations on object, we want to pretend interfaces inherit from objects
            if (type.IsInterface && parentType == typeof(object))
                return 1;
            foreach (var typeInChain in type.GetInheritanceChain())
                if (typeInChain == parentType)
                    return distance;
                else
                    distance++;

            return -1;
        }

        public static string GetTypeString(this Type type)
        {
            return GetTypeString(type, false);
        }

        public static string GetTypeString(this Type type, bool flattenGenericTypeDefinitions)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            var builder = new StringBuilder();
            if (type.IsNested)
            {
                builder.Append(GetTypeString(type.DeclaringType, true));
                builder.Append('.');
            }

            if (type.IsGenericType)
            {
                if (type.IsGenericTypeDefinition && !flattenGenericTypeDefinitions)
                    throw new InvalidOperationException(
                        string.Format(
                            "{0} is a generic type definition and does not have a type string.\r\nTry providing a constructed generic type instead.",
                            type.Name));
                var genericType = type.GetGenericTypeDefinition();
                var genericTypeArguments = type.GetGenericArguments();
                builder.Append(genericType.Name.Substring(0, genericType.Name.IndexOf('`')));
                if (genericTypeArguments.Length > 0)
                {
                    builder.Append('(');
                    for (int i = 0; i < genericTypeArguments.Length; i++)
                    {
                        builder.Append(genericTypeArguments[i].Name);
                        if (i > 0) builder.Append(',');
                    }

                    builder.Append(')');
                }
            }
            else
            {
                builder.Append(type.Name);
            }

            return builder.ToString();
        }

        public static string GetTypeString(this object target)
        {
            return GetTypeString(target.GetType());
        }

        public static bool HasDefaultConstructor(this Type type)
        {
            return type.GetConstructor(Type.EmptyTypes) != null;
        }

        public static bool Implements<T>(this Type type)
        {
            return Implements(type, typeof(T));
        }

        /// <summary>
        /// Returns <c>true</c> if the type implements the interface.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"><paramref name="interfaceType"/> is not an interface. Use the <see cref="InheritsFrom"/> method instead.</exception>
        public static bool Implements(this Type type, Type interfaceType)
        {
            if (!interfaceType.IsInterface)
                throw new ArgumentException("interfaceType is not an interface. Use InheritsFrom instead.", "interfaceType");
            if (type == null)
                return false;
            return type.FindInterface(interfaceType) != null;
        }

        public static bool InheritsFrom(this Type type, Type superType)
        {
            if (superType.IsGenericTypeDefinition && type.IsGenericType)
                return type.GetGenericTypeDefinition() == superType;
            return superType.IsAssignableFrom(type);
        }
        public static bool IsAssignableTo(this Type type, Type destinationType)
        {
            return destinationType.IsAssignableFrom(type);
        }
        public static bool IsAssignableTo<T>(this Type type)
        {
            return type.IsAssignableTo(typeof(T));
        }

        static object CreateInstanceFromString(this Type type, string propertyValue, Stack<Type> recursionDefender)
        {
            if (type == null || propertyValue == null) return null;
            if (type == typeof(string))
                return propertyValue;

            if (type == typeof(bool))
            {
                switch (propertyValue)
                {
                    case "0":
                    case "-0":
                    case "false":
                    case "off":
                    case "null":
                    case "NaN":
                    case "undefined":
                    case "":
                        return false;
                    default:
                        return true;
                }
            }

            if (type.IsPrimitive)
            {
                try
                {
                    return Convert.ChangeType(propertyValue, type);
                }
                catch
                {
                }
            }

            recursionDefender = recursionDefender ?? new Stack<Type>();
            foreach (var constructor in type.GetConstructors())
            {
                var parameters = constructor.GetParameters();
                if (parameters.Length != 1) continue;
                object value;
                if (recursionDefender.Contains(parameters[0].ParameterType))
                    continue;
                recursionDefender.Push(type);
                try
                {
                    value = CreateInstanceFromString(parameters[0].ParameterType, propertyValue, recursionDefender);
                }
                catch
                {
                    continue;
                }
                finally
                {
                    recursionDefender.Pop();
                }

                if (value != null)
                    return constructor.Invoke(new[] { value });
            }

#if !SILVERLIGHT
            var converter = TypeDescriptor.GetConverter(type);
            if (converter == null || !converter.CanConvertFrom(typeof(string)))
                throw new InvalidCastException("Cannot convert the string \"" + propertyValue + "\" to type "
                                               + type.Name);
            return TypeDescriptor.GetConverter(type).ConvertFromInvariantString(propertyValue);
#else
                return null;
#endif
        }

        /// <exception cref="NotSupportedException">Could not convert element {0} to {1}.</exception>
        static bool TryConvert<T>(T[] source,
                                  Type destinationType,
                                  ValueConverter<T> converter,
                                  Action<object> onConvertSuccessful)
        {
            for (int i = 0; i < source.Length; i++)
            {
                try
                {
                    var bindingResult = converter(source[i], destinationType);
                    if (!bindingResult.Successful)
                        return false;
                    onConvertSuccessful(bindingResult.Instance);
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }
    }
}

#region Full license
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion