using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Halftone
{
    /// <summary>
    /// Configuration manager holds module configurations and takes care
    /// of its persistence.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Module configuration consist of stucture of
    /// how module objects are interconnected and of their parameter 
    /// settings. Each configuration is accessible via its type and textual
    /// name.
    /// </para>
    /// <para>
    /// Module configuration is persisted to a file. Binary serialization
    /// method from .NET is used for persistence. File name must be given
    /// using ConfigFileName property before .
    /// </para>
    /// </remarks>
    public class ConfigManager
    {
        private List<Module> _savedModules;

        /// <summary>
        /// Configuration file name.
        /// </summary>
        public string ConfigFileName { get; set; }

        /// <summary>
        /// Create a configuration manager instance.
        /// </summary>
        public ConfigManager() {
            _savedModules = new List<Module>();
        }

        /// <summary>
        /// Load configuration from file to memory.
        /// </summary>
        /// <remarks>
        /// Configuration file name is taken from ConfigFileName property.
        /// If there is any module in the file this function overwrites any
        /// modules held by the configuration manager in memory.
        /// </remarks>
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

        /// <summary>
        /// Save configuration from memory to file (possibly creating a new
        /// file or overwriting an existing file).
        /// </summary>
        /// <remarks>
        /// Configuration file name is taken from ConfigFileName property.
        /// </remarks>
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

        /// <summary>
        /// Find all modules of given type matching a predicate.
        /// </summary>
        /// <param name="predicate">Predicate taking a module a returning true
        /// if modules is accepted.</param>
        /// <returns>List of modules matching a predicate</returns>
        public List<ModuleType> findAllModules<ModuleType>(
            Predicate<ModuleType> predicate) where ModuleType : class
        {
            return _savedModules.FindAll(
                module => (module is ModuleType)
                ).ConvertAll<ModuleType>(module => module as ModuleType)
                .FindAll(predicate);
        }

        /// <summary>
        /// Find all modules matching a predicate.
        /// </summary>
        /// <param name="predicate">Predicate taking a module a returning true
        /// if modules is accepted.</param>
        /// <returns>List of modules matching a predicate</returns>
        public List<Module> findAllModules(Predicate<Module> predicate) {
            return _savedModules.FindAll(predicate);
        }

        public ModuleType getModule<ModuleType>(string moduleName)
            where ModuleType : Module
        {
            if (moduleName == null) {
                return null;
            }
            List<ModuleType> modules = findAllModules<ModuleType>(
                module => module.Name == moduleName);
            return (modules.Count > 0) ?
                (ModuleType)(modules.First().deepCopy()) : null;
        }

        /// <summary>
        /// Get a list of all module configurations held by the manager.
        /// </summary>
        /// <returns>List of all module configurations.</returns>
        public List<Module> listAllModules() {
            return _savedModules;
        }

        /// <summary>
        /// Persistently remove all the module configurations.
        /// </summary>
        public void clear() {
            _savedModules.Clear();
            save();
        }

        /// <summary>
        /// Save a module configuration to the manager.
        /// Changes are persisted automatically.
        /// </summary>
        /// <param name="module">Module to be saved.</param>
        /// <see cref="saveModule(Module module, bool saveImmediately)"/>
        public void saveModule(Module module) {
            saveModule(module, true);
        }

        /// <summary>
        /// Save a module configuration to the manager. Choose whether to
        /// persist the change automatically or manually using save()
        /// function. Manual saving can be useful when adding modules to the
        /// manager in a batch. Then persistence is done at the end once for
        /// all modules.
        /// </summary>
        /// <param name="module">Module to be saved.</param>
        /// <param name="saveImmediately">Save automatically now or manually then?</param>
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

        /// <summary>
        /// Delete a module matching a type and name.
        /// Changes are persisted automatically.
        /// </summary>
        /// <param name="type">Module type</param>
        /// <param name="name">Module configuration name</param>
        public void deleteModule<ModuleType>(string name) {
            _savedModules.RemoveAll(
                (module) => (module is ModuleType) && (name == module.Name)
                );
            save();
        }
    }
}
