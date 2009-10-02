using OpenRasta.TypeSystem.ReflectionBased;

namespace OpenRasta.Diagnostics
{
    public static class LogSource<T> where T : ILogSource
    {
        static string _category;

        public static string Category
        {
            get
            {
                if (_category == null)
                {
                    var attr = typeof(T).FindAttribute<LogCategoryAttribute>();
                    _category = attr != null ? attr.CategoryName : typeof(T).Name;
                }
                return _category;
            }
        }
    }
}