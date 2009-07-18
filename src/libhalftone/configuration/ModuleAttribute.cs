using System;

namespace Halftone
{
    public class ModuleAttribute : Attribute
    {
        public string TypeName { get; set; }
        public string TypeDescription { get; set; }
    }
}
