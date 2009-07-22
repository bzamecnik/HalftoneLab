using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Halftone
{
    /// <summary>
    /// A base class for configurable modules which connected together
    /// can form various halftoning algorithms.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Modules act as plugable components. Parameters and sumbodules
    /// can be set using properties.
    /// </para>
    /// <para>
    /// TODO: - serializable, init()
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

        protected Module() {
            Name = "";
            Description = "";
        }

        /// <summary>
        /// Initialize this module and its submodules.
        /// It is intended to initialize all temporary things lost during
        /// serialization/deserialization and give run-time information
        /// about the image on which the whole algorithm is ran.
        /// </summary>
        /// <param name="imageRunInfo">Runtime information about the image
        /// on the algorithm runs.</param>
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
