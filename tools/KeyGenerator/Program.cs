using System;
using System.IO;
using NRUAGuestManager;

namespace NRUAKeyGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            int count = 1;
            string outputFile = null;

            for (int i = 0; i < args.Length; i++)
            {
                if ((args[i] == "--count" || args[i] == "-n") && i + 1 < args.Length)
                {
                    count = int.Parse(args[++i]);
                }
                else if ((args[i] == "--output" || args[i] == "-o") && i + 1 < args.Length)
                {
                    outputFile = args[++i];
                }
                else if (args[i] == "--help" || args[i] == "-h")
                {
                    Console.WriteLine("NRUA Guest Manager - Key Generator");
                    Console.WriteLine();
                    Console.WriteLine("Usage: NRUAKeyGenerator [options]");
                    Console.WriteLine();
                    Console.WriteLine("Options:");
                    Console.WriteLine("  -n, --count <N>      Number of keys to generate (default: 1)");
                    Console.WriteLine("  -o, --output <file>  Write keys to file");
                    Console.WriteLine("  -h, --help           Show this help");
                    return;
                }
            }

            Console.WriteLine($"Generating {count} license key(s)...");
            Console.WriteLine();

            StreamWriter writer = null;
            if (outputFile != null)
            {
                writer = new StreamWriter(outputFile, false);
            }

            for (int i = 0; i < count; i++)
            {
                string key = LicenseValidator.GenerateKey();
                Console.WriteLine(key);
                writer?.WriteLine(key);

                // Verify each generated key
                if (!LicenseValidator.IsValidKey(key))
                {
                    Console.Error.WriteLine($"ERROR: Generated key failed validation: {key}");
                    Environment.Exit(1);
                }
            }

            if (writer != null)
            {
                writer.Close();
                Console.WriteLine();
                Console.WriteLine($"Keys written to: {outputFile}");
            }
        }
    }
}
