using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Halftone
{
    // a base for serializable modules (configuration presets)
    [Serializable]
    public abstract class Module
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Module deepCopy() {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, this);
            stream.Position = 0;
            return (Module)formatter.Deserialize(stream);
        }
    }
}
