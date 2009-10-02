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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Rasta.Testing
{
    public static class AssertExtensions
    {
        public static T ShouldThrow<T>(this Action codeToExecute) where T:Exception
        {
            try
            {
                codeToExecute();
            }
            catch (Exception e)
            {
                if (!typeof(T).IsInstanceOfType(e))
                    Assert.Fail("Expected exception of type \"{0}\" but got \"{1}\" instead.", typeof(T).Name, e.GetType().Name);
                else
                    return (T)e;
            }

            Assert.Fail("Expected an exception of type \"{0}\" but none were thrown.", typeof(T).Name);
            return null; // this never happens as Fail will throw...
        }
        public static void AreEqual<R>(IEnumerable<R> r1, IEnumerable<R> r2)
        {
            r1.ShouldBe(r2);

        }
        public static void AreEqual(NameValueCollection expectedResult, NameValueCollection actualResult)
        {
            AreEqual(expectedResult, actualResult, null);
        }
        public static void AreEqual(NameValueCollection expectedResult, NameValueCollection actualResult, string message)
        {
            Assert.AreEqual(expectedResult.Count, actualResult.Count);
            foreach (string key in expectedResult.Keys)
            {
                Assert.AreEqual(expectedResult[key], actualResult[key], message);
            }
        }
        public static bool ShouldBe(this NameValueCollection expectedResult, NameValueCollection actualResult)
        {
            Assert.AreEqual(expectedResult.Count, actualResult.Count);
            foreach (string key in expectedResult.Keys)
            {
                Assert.AreEqual(expectedResult[key], actualResult[key]);
                if (expectedResult[key] != actualResult[key])
                    return false;
            }
            return true;
        }
        public static bool ShouldBe<T>(this IEnumerable<T> r1, IEnumerable<T> r2)
        {
            IEnumerator<T> enumerator1 = r1.GetEnumerator();
            IEnumerator<T> enumerator2 = r2.GetEnumerator();
            while (enumerator1.MoveNext() && enumerator2.MoveNext())
                Assert.AreEqual(enumerator1.Current, enumerator2.Current);
            return true;

        }
        public static void ShouldBeLessThan<T>(this T actual, T expected) where T:IComparable
        {
            Assert.That(actual, Is.LessThan(expected));
        }
        public static void ShouldBeGreaterThan<T>(this T actual, T expected) where T : IComparable
        {
            Assert.That(actual, Is.GreaterThan(expected));
        }
        public static bool ShouldBe<T, V>(this IEnumerable<T> r1, IEnumerable<V> r2, Func<T, V, bool> comparer)
        {
            
            using (IEnumerator<T> enumerator1 = r1.GetEnumerator())
            using (IEnumerator<V> enumerator2 = r2.GetEnumerator())
            {
                while (true)
                {
                    bool enum1HasMoved = enumerator1.MoveNext();
                    bool enum2HasMoved = enumerator2.MoveNext();
                    if (!enum1HasMoved && !enum2HasMoved)
                        return true;
                    if (enum1HasMoved != enum2HasMoved)
                        return false;
                    Assert.IsTrue(comparer(enumerator1.Current, enumerator2.Current), "Two elements didnt match:\na:\n{0}\nb:\n{1}", enumerator1.Current.ToString(), enumerator2.Current.ToString());
                }
                return true;
            }
        }
        public static bool IsEqualTo(this Collection<string> actualValue, Collection<string> expectedValue)
        
        {
            if ((actualValue == null) != (expectedValue == null))
                return false;
            if (actualValue == null && expectedValue == null)
                return true;
            if (actualValue.Count != expectedValue.Count)
                return false;
            for (int i = 0; i < actualValue.Count; i++)
                if (actualValue[i] != expectedValue[i])
                    return false;
            return true;
        }
        public static bool ShouldBe<T>(this Type type)
        {
            return type == typeof(T);
        }
        public static bool ShouldBe<T>(this T valueToAnalyse, T expectedValue)
        {
            Assert.AreEqual(expectedValue, valueToAnalyse);
            return true;
        }
        public static bool ShouldBeTrue(this bool value)
        {
            Assert.IsTrue(value);
            return value == true;
        }
        public static void ShouldBeOfType<TExpected>(this object obj)
        {
            Assert.That(obj, Is.InstanceOfType(typeof(TExpected)));
        }
        public static bool ShouldBeFalse(this bool value)
        {
            Assert.IsFalse(value);
            return value == false;
        }
        public static bool ShouldBeNull<T>(this T obj)
        {
            Assert.IsNull(obj);
            return obj == null;
        }
        public static bool ShouldNotBeNull<T>(this T obj) where T : class
        {
            Assert.IsNotNull(obj);
            return obj != null;
        }
        public static void ShoudContainExactly(this string baseString, string textToFind)
        {
            if (baseString.IndexOf(textToFind) == -1)
                Assert.Fail("text '{0}' not found in '{1}'", textToFind, baseString);
        }
        public static void ShouldNotContain(this string baseString, string textToFind)
        {

            if (baseString.IndexOf(textToFind) != -1)
                Assert.Fail("text '{0}' found in '{1}'", textToFind, baseString);
        }
        public static Exception ShouldBeThrownBy(this Type exceptionType, Action method)
        {
            Exception exception = null;

            try
            {
                method();
            }
            catch (Exception e)
            {
                Assert.AreEqual(exceptionType, e.GetType());
                exception = e;
            }

            if (exception == null)
            {
                Assert.Fail(String.Format("Expected {0} to be thrown.", exceptionType.FullName));
            }

            return exception;
        }
    }
}

#region Full license
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion
