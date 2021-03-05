using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace JsonMerger
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ValidateArgs(args); // If some args are incorrect, then it will exit.

            string operation = args[Array.IndexOf(args, "-op") + 1];
            string pathToConfig = string.Empty;
            if (operation == "merge")
            {
                pathToConfig = args[Array.IndexOf(args, "-c") + 1];
            }

            string pathToExe = args[Array.IndexOf(args, "-e") + 1];
            string pathToExeOutput = args[Array.IndexOf(args, "-out") + 1];

            // Our configuration identifier in exe.
            const string configStartSignature = "RATR8GDKddFkFMEidhhxHzHRAT";

            // This MUST be appended to our config in the very end to determine max config length during removal/replace.
            string configEndSignature = "58cERyKedv";

            if (operation == "merge")
            {
                byte[] configurationInBytes = GetFileBytes(pathToConfig, extension: ".json");
                string jsonConfiguration = Encoding.ASCII.GetString(configurationInBytes);
                ValidateJson(jsonConfiguration); // If is bad json, this will not go further

                const byte firstKey = 60; // Out first key for xor encryption
                const byte secondKey = 70; // Out second key for xor encryption
                byte[] encryptedConfiguration = Xor(Xor(configurationInBytes, firstKey), secondKey); // Double xor
                string configurationEncrypted = Encoding.ASCII.GetString(encryptedConfiguration);

                string payloadWithConfig = configStartSignature + configurationEncrypted + configEndSignature;
                byte[] payloadWithConfigInBytes = Encoding.ASCII.GetBytes(payloadWithConfig);

                byte[] exeInBytes = GetFileBytes(pathToExe, extension: ".exe");
                exeInBytes = DeletePreviousConfigurationFromExe(exeInBytes, configStartSignature, configEndSignature); // If there is already some configuration, then delete it.
                byte[] exeWithAppendedConfigurationInBytes = exeInBytes.Concat(payloadWithConfigInBytes).ToArray();

                // Merge configuration to exe.
                WriteBytesToOutput(pathToExeOutput, exeWithAppendedConfigurationInBytes);

                Console.WriteLine($"Successfully merged .exe with configuration.\nResult available in {pathToExeOutput}");
                Console.ReadKey();
                Environment.Exit(0);
            }
            // Remove configuration from exe.
            else
            {
                byte[] exeInBytes = GetFileBytes(pathToExe, extension: ".exe");
                exeInBytes = DeletePreviousConfigurationFromExe(exeInBytes, configStartSignature, configEndSignature);
                WriteBytesToOutput(pathToExeOutput, exeInBytes);

                Console.WriteLine($"Successfully removed configuration from .exe.\nResult available in {pathToExeOutput}");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Writes sequence of bytes to output.
        /// </summary>
        /// <param name="path">Path where to write bytes.</param>
        /// <param name="bytes">Bytes that should be written to output.</param>
        private static void WriteBytesToOutput(string path, byte[] bytes)
        {
            try
            {
                File.WriteAllBytes(path, bytes);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error during write to output: {exception.Message}");
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Validates JSON configuration.
        /// </summary>
        /// <param name="jsonConfig">JSON configuration in string format.</param>
        private static void ValidateJson(string jsonConfig)
        {
            try
            {
                object tempJson = null;
                JsonConvert.DeserializeAnonymousType(jsonConfig, tempJson);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Bad json file: {exception.Message}");
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Gets file bytes by provided path, but before that checks if file extension by provided path is same as pased in parameter.
        /// </summary>
        /// <param name="path">Path to file.</param>
        /// <param name="extension">Extension of file.</param>
        /// <returns>Sequence of byte for file contents.</returns>
        private static byte[] GetFileBytes(string path, string extension)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine($"File on path {path} not found");
                Environment.Exit(0);
            }

            if (Path.GetExtension(path) != extension)
            {
                Console.WriteLine($"File extension should be {extension}");
                Environment.Exit(0);
            }

            byte[] file = null;
            try
            {
                file = File.ReadAllBytes(path);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error during file read: {exception.Message}");
                Environment.Exit(0);
            }

            return file;
        }

        /// <summary>
        /// Xoring(encryption) function.
        /// </summary>
        /// <param name="bytes">Bytes to encrypt.</param>
        /// <param name="key">Key to encrypt all bytes with.</param>
        /// <returns>Encrypted byte sequence.</returns>
        private static byte[] Xor(byte[] bytes, byte key)
        {
            byte[] encryptedBytes = new byte[bytes.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                encryptedBytes[i] = bytes[i];
                encryptedBytes[i] ^= key;
            }

            return encryptedBytes;
        }

        /// <summary>
        /// Removes previous configuration from exe (if it exists there).
        /// NOTE: Despite of configuration being appended to the very end of exe (that's the plan)
        /// configuration removal is done by "cutting" out configuration with signatures based on it length,
        /// this is done for cases when something else is appended to exe after our configuration or if our configuration is not in the very end of exe (unlikely to happen, but just in case.)
        /// </summary>
        /// <param name="fileBytes">Exe bytes.</param>
        /// <param name="configStartSignature">Signature of our configuration.</param>
        /// <param name="configEndSignature">Config ending signature for our configuration.</param>
        private static byte[] DeletePreviousConfigurationFromExe(byte[] fileBytes, string configStartSignature, string configEndSignature)
        {
            // Get indexes (positions) of out signatures in .exe
            byte[] signatureStartInByteFormat = Encoding.ASCII.GetBytes(configStartSignature);
            byte[] configEndSignatureInByteFormat = Encoding.ASCII.GetBytes(configEndSignature);
            long indexOfStartSignatureInExe = GetIndexOfSignatureInExe(fileBytes, signatureStartInByteFormat);
            long indexOfEndSignatureInExe = GetIndexOfSignatureInExe(fileBytes, configEndSignatureInByteFormat);
            if (indexOfStartSignatureInExe == -1 || indexOfEndSignatureInExe == -1)
            {
                Console.WriteLine("Configuration not found");
                return fileBytes;
            }

            long configurationEndPosition = indexOfEndSignatureInExe + configEndSignatureInByteFormat.Length;
            long configurationLength = (configurationEndPosition - indexOfStartSignatureInExe);
            long bytesCountAfterConfigurationEnd = fileBytes.Length - configurationEndPosition;
            byte[] exeWithoutConfiguration = new byte[fileBytes.Length - configurationLength];
            Array.Copy(fileBytes, 0, exeWithoutConfiguration, 0, indexOfStartSignatureInExe); // Copy everything before start of configuration.
            Array.Copy(fileBytes, configurationEndPosition, exeWithoutConfiguration, indexOfStartSignatureInExe, bytesCountAfterConfigurationEnd); // Copy everything after end of configuration.
            return exeWithoutConfiguration;
        }

        /// <summary>
        /// Validates arguments that came with call of program.
        /// </summary>
        /// <param name="args">Program arguments.</param>
        private static void ValidateArgs(string[] args)
        {
            // Path to EXE file
            if (!args.Contains("-e") || args[Array.IndexOf(args, "-e") + 1] == null)
            {
                Console.WriteLine("Error: path to exe not provided");
                ShowHelp();
                Environment.Exit(0);
            }

            // Path to file output
            if (!args.Contains("-out") || args[Array.IndexOf(args, "-o") + 1] == null)
            {
                Console.WriteLine("Error: output path for exe not provided");
                ShowHelp();
                Environment.Exit(0);
            }

            // Operation - merge configuration, remove configuration...
            if (!args.Contains("-op") || args[Array.IndexOf(args, "-op") + 1] == null)
            {
                Console.WriteLine("Error: operation not provided");
                ShowHelp();
                Environment.Exit(0);
            }

            string operation = args[Array.IndexOf(args, "-op") + 1];
            if (operation != "merge" && operation != "remove")
            {
                Console.WriteLine("Error: unknown operation\nPossible operation: merge|remove");
                ShowHelp();
                Environment.Exit(0);
            }

            if (operation == "merge" && !args.Contains("-c") || args[Array.IndexOf(args, "-c") + 1] == null)
            {
                Console.WriteLine("Error: path to config for merge not provided");
                ShowHelp();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Simply print-out help (how-to-use) message.
        /// </summary>
        private static void ShowHelp()
        {
            Console.WriteLine("Remove or merge JSON configuration to .exe");
            Console.WriteLine("Arguments: merge.exe -c <configuration> -e <exe that should be processed> -out <output> -op <operation(merge|remove)>");
            Console.WriteLine("Usage example (merge): merge.exe -c \"C:\\path\\to\\config.json\" -e \"path\\to\\file.exe\" -out \"path\\to\\output.exe\" -op merge");
            Console.WriteLine("Usage example (remove): merge.exe -e \"path\\to\\file.exe\" -out \"path\\to\\output.exe\" -op remove");
        }

        // Simple (naive) string search algorithm to find sequence of bytes (signature) in other sequence of bytes (exe) as there is no built-in method for bytes in C# libs.
        private static int GetIndexOfSignatureInExe(byte[] exeInByteFormat, byte[] signatureInByteFormat)
        {
            for (int i = 0; i < exeInByteFormat.Length; i++)
            {
                if (IsSequenceOfBytesSameAsInSignature(i, exeInByteFormat, signatureInByteFormat))
                {
                    return i;
                }
            }

            return -1;
        }

        private static bool IsSequenceOfBytesSameAsInSignature(int position, byte[] exeInByteFormat, byte[] signatureInByteFormat)
        {
            int validCount = 0;
            int signatureMaxValidCount = signatureInByteFormat.Length;
            for (int i = position; i < exeInByteFormat.Length; i++)
            {
                if (exeInByteFormat[i] == signatureInByteFormat[validCount])
                {
                    validCount++;
                    if (validCount == signatureMaxValidCount)
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }

            return false;
        }
    }
}