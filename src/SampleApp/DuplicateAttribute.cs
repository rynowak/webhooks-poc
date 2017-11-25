using System;

namespace SampleApp
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class DuplicateAttribute : Attribute
    {
    }
}
