using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace HalftoneLab
{
    /// <summary>
    /// A base class for configurable modules which can form various
    /// halftoning algorithms when connected together.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Modules act as plugable components. Parameters and sumbodules
    /// can be set using properties.
    /// </para>
    /// <para>
    /// Run-time information about the image on which the algorithm runs
    /// can be propagated and temporary module parts can be initialized
    /// with init() function. Modules can be deep copied with deepCopy()
    /// function. Module instances can bear a name and descriptions, as well
    /// as the module classes itself.
    /// </para>
    /// </remarks>
    [Serializable]
    public abstract class Module
    {
        /// <summary>
        /// Name of the current module configuration, eg. module state with
        /// parameters set and submodules plugged in.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the current module configuration.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Create a new module.
        /// </summary>
        protected Module() {
            Name = "";
            Description = "";
        }

        /// <summary>
        /// Initialize this module and its submodules.
        /// </summary>
        /// <remarks>
        /// This function is intended to initialize all temporary things
        /// (especially those lost during serialization/deserialization) and
        /// propagate run-time information about the image on which the whole
        /// algorithm is ran.
        /// </remarks>
        /// <param name="imageRunInfo">Runtime information about the image
        /// on which the algorithm runs.</param>
        public virtual void init(Image.ImageRunInfo imageRunInfo) {}

        /// <summary>
        /// Make a deep copy of a module, ie. copy this module and
        /// all its submodules.
        /// </summary>
        /// <returns>Module deep copy</returns>
        public Module deepCopy() {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, this);
            stream.Position = 0;
            return (Module)formatter.Deserialize(stream);
        }
    }
}
