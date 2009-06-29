using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Halftone
{
    /// <summary>
    /// A base class for serializable modules (configuration presets)
    /// </summary>
    [Serializable]
    public abstract class Module
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual void init(Image.ImageRunInfo imageRunInfo) {}

        public Module deepCopy() {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, this);
            stream.Position = 0;
            return (Module)formatter.Deserialize(stream);
        }
    }
}
