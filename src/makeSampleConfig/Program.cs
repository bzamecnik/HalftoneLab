using System;
using Halftone;

namespace HalftoneLab.SampleConfig
{
    class Program
    {
        static void Main(string[] args) {
            SampleConfig.run();
        }

        class SampleConfig
        {
            public static void run() {
                ConfigManager config = new ConfigManager()
                {
                    ConfigFileName = "halftonelab.cfg"
                };
                //config.load();
                //printConfig(config);

                

                printConfig(config);

                //config.deleteModule(typeof(HilbertScanningOrder), "Hilbert");

            }

            public static void printConfig(ConfigManager config) {
                foreach (Module module in config.listAllModules()) {
                    Console.Out.WriteLine("type: {0}, name: {1}, desc: {2}",
                        module.GetType(), module.Name, module.Description);
                }
            }
        }
    }
}
