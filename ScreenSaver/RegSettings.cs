﻿using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Aerial
{
    public class RegSettings
    {
        readonly string keyAddress = @"SOFTWARE\Ratajik Software\AerialPlus";
        [Obsolete("Replaced with MultiMonitorMode")]
        private bool DifferentMoviesOnDual = false;
        [Obsolete("Replaced with MultiMonitorMode")]
        private bool MultiscreenDisabled = true;
        public MultiMonitorModeEnum MultiMonitorMode = RegSettings.MultiMonitorModeEnum.MainOnly;
        public bool UseTimeOfDay = true;
        public bool CacheVideos = true;
        public string CacheLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Aerial");
        public string ChosenMovies = "";
        public bool CheckForNewVersion = true;
        public int LastVersionChecked = AerialGlobalVars.version;        
        private string jsonURL = AerialGlobalVars.ratajikVideosURI;

        public string JsonURL { 
            get
            {
                if (Debugger.IsAttached)
                {
                    var jsonLocal = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\" + "AerialPlus.json";
                    return jsonLocal;                  
                }
                else
                {
                    return jsonURL;
                }
                
            } 
            set => jsonURL = value; 
        }

        private string versionURL = AerialGlobalVars.versionURI;

        public string VersionURL
        {
            get
            {
                if (Debugger.IsAttached)
                {
                    var jsonLocal = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\" + "AerialPlus_Version.json";
                    return jsonLocal;
                }
                else
                {
                    return versionURL;
                }

            }
            set => jsonURL = value;
        }

#pragma warning disable CS0618 // Type or member is obsolete
        public RegSettings()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(keyAddress);
            if (key != null)
            {
                DifferentMoviesOnDual = bool.Parse(key.GetValue(nameof(DifferentMoviesOnDual)) as string ?? "True");
                MultiscreenDisabled = bool.Parse(key.GetValue(nameof(MultiscreenDisabled)) as string ?? "True");

                if (!Enum.TryParse(key.GetValue(nameof(MultiMonitorMode)) as string, out MultiMonitorMode))
                {
                    // load value from legacy settings
                    MultiMonitorMode =
                        MultiscreenDisabled ? MultiMonitorModeEnum.MainOnly
                        : DifferentMoviesOnDual ? MultiMonitorModeEnum.DifferentVideos : MultiMonitorModeEnum.SameOnEach;
                }
                                
                LastVersionChecked = int.Parse(key.GetValue(nameof(LastVersionChecked)) as string ?? AerialGlobalVars.version.ToString());
                CheckForNewVersion = bool.Parse(key.GetValue(nameof(CheckForNewVersion)) as string ?? "True");
                ChosenMovies = (key.GetValue(nameof(ChosenMovies)) as string ?? "");
                UseTimeOfDay = bool.Parse(key.GetValue(nameof(UseTimeOfDay)) as string ?? "True");
                CacheVideos = bool.Parse(key.GetValue(nameof(CacheVideos)) as string ?? "True");
                CacheLocation = key.GetValue(nameof(CacheLocation)) as string;
                ChosenMovies = (key.GetValue(nameof(ChosenMovies)) as string ?? "");
                JsonURL = key.GetValue(nameof(JsonURL)) as string;
            }
        }

        /// <summary>
        /// Save text into the Registry.
        /// </summary>
        public void SaveSettings()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(keyAddress);

            key.SetValue(nameof(LastVersionChecked), LastVersionChecked.ToString());
            key.SetValue(nameof(CheckForNewVersion), CheckForNewVersion);

            key.SetValue(nameof(MultiMonitorMode), MultiMonitorMode);
            key.SetValue(nameof(UseTimeOfDay), UseTimeOfDay);
            key.SetValue(nameof(CacheVideos), CacheVideos);
            key.SetValue(nameof(CacheLocation), CacheLocation);
            key.SetValue(nameof(ChosenMovies), ChosenMovies);
            key.SetValue(nameof(JsonURL), JsonURL);

            // delete old keys
            key.DeleteValue(nameof(DifferentMoviesOnDual), throwOnMissingValue: false);
            key.DeleteValue(nameof(MultiscreenDisabled), throwOnMissingValue: false);
        }
#pragma warning restore CS0618 // Type or member is obsolete

        public enum MultiMonitorModeEnum
        {
            [Description("Show on Main Screen only")]
            MainOnly = 0,
            [Description("Show same video on each screen")]
            SameOnEach = 1,
            [Description("Show different video on each screen")]
            DifferentVideos = 5,
            [Description("Span single video across all screens")]
            SpanAll = 10,
        }
    }
}
