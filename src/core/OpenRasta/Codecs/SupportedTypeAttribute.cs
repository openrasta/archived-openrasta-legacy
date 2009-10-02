using System;

namespace OpenRasta.Codecs
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SupportedTypeAttribute : Attribute
    {
        /// <summary>
        /// Defines a type a codec is supporting by default.
        /// </summary>
        /// <param name="supportedType">The type a codec can convert from / to.</param>
        public SupportedTypeAttribute(Type supportedType)
        {
            Type = supportedType;
        }

        public Type Type { get; set; }
    }
}