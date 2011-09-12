using System;
using OpenRasta.Web.Markup.Attributes.Annotations;

namespace OpenRasta.Web.Markup.Attributes
{
    public class DatetimeAttribute : PrimaryTypeAttributeCore
    {
        public DatetimeAttribute() :base(Factory<DateTime?>){ }
        public DatetimeAttribute(string attribName) : base(attribName, Factory<DateTime?>) { }
    }
}