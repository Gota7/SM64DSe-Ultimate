using System;

namespace SM64DSe.Properties
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyCodeSource : Attribute
    {
        public string Value { get; private set; }

        public AssemblyCodeSource() : this("") { }
        public AssemblyCodeSource(string value) { Value = value; }
    }
}