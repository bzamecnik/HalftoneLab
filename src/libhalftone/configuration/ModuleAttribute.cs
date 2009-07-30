using System;

namespace HalftoneLab
{
    /// <summary>
    /// A .NET attribute for describing modules.
    /// </summary>
    /// <remarks>
    /// Name and description can be statically attached to each module class.
    /// Then it can be fetched via reflection.
    /// </remarks>
    /// <see cref="Module"/>
    public class ModuleAttribute : Attribute
    {
        /// <summary>
        /// Module name.
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Module description.
        /// </summary>
        public string TypeDescription { get; set; }
    }
}
