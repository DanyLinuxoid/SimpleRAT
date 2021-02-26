﻿using Newtonsoft.Json;
using System;
using System.IO;

namespace RAT.src.Configuration
{
    /// <summary>
    /// Contains logic to work with configuration.
    /// </summary>
    public static class BackdoorConfigurator
    {
        /// <summary>
        /// Retrieves configuration from JSON file.
        /// </summary>
        /// <returns>Class that represents file configuration.</returns>
        public static BackdoorConfiguration GetBackdoorConfiguration()
        {
            // This will get the current PROJECT directory
            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory.Trim()).Parent.Parent.FullName;
            string filePath = projectDirectory + "\\src" + "\\Configurations\\" + "BackdoorConfig.json";

            using (StreamReader stream = new StreamReader(filePath))
            {
                return JsonConvert.DeserializeObject<BackdoorConfiguration>(stream.ReadToEnd());
            }
        }
    }
}