using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Halftone
{
    public class ConfigManager
    {
        private List<Module> _savedModules;

        public string ConfigFileName { get; set; }

        public ConfigManager() {
            _savedModules = new List<Module>();
        }

        // load config from file to memory
        public void load() {
            Stream streamRead = null;
            try {
                streamRead = File.OpenRead(ConfigFileName);
                BinaryFormatter binaryRead = new BinaryFormatter();
                List<Module> loadedModules = binaryRead.Deserialize(streamRead) as List<Module>;
                if (loadedModules != null) {
                    _savedModules = loadedModules;
                }
                // else: report an error
            } catch (FileNotFoundException ex) {
                Console.Out.WriteLine(ex.Message);
            } finally {
                if (null != streamRead) {
                    streamRead.Close();
                }
            }
        }

        // save config from memory to file (possibly creating a new file
        // or overwriting an existing file)
        public void save() {
            Stream streamWrite = null;
            try {
                streamWrite = File.Create(ConfigFileName);
                BinaryFormatter binaryWrite = new BinaryFormatter();
                binaryWrite.Serialize(streamWrite, _savedModules);
            } finally {
                if (null != streamWrite) {
                    streamWrite.Close();
                }
            }
        }

        // find all items matching a predicate
        public List<Module> findAllModules(Predicate<Module> predicate) {
            return _savedModules.FindAll(predicate);
        }

        public List<Module> listAllModules() {
            return _savedModules;
        }

        public void clear() {
            _savedModules.Clear();
            save();
        }

        // save an item
        public void saveModule(Module module) {
            saveModule(module, true);
        }

        public void saveModule(Module module, bool saveImmediately) {
            Module moduleCopy = module.deepCopy();
            int index = _savedModules.FindIndex(
                (mod) => (mod.GetType() == moduleCopy.GetType())
                    && (mod.Name == moduleCopy.Name)
                );
            if (index < 0) {
                // not found, add a new item
                _savedModules.Add(moduleCopy);
            } else {
                // found, overwrite
                _savedModules[index] = moduleCopy;
            }
            if (saveImmediately) {
                save(); // immediately save to a file
            }
        }

        // delete an item (items) matching a predicate
        public void deleteModule(Type type, string name) {
            _savedModules.RemoveAll(
                (module) => (type == module.GetType()) && (name == module.Name)
                );
            save();
        }
    }
}
