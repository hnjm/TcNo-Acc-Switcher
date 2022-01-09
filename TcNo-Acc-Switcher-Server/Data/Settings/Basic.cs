﻿// TcNo Account Switcher - A Super fast account switcher
// Copyright (C) 2019-2022 TechNobo (Wesley Pyburn)
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.IO;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TcNo_Acc_Switcher_Globals;
using TcNo_Acc_Switcher_Server.Pages.General;
using TcNo_Acc_Switcher_Server.Pages.General.Classes;

namespace TcNo_Acc_Switcher_Server.Data.Settings
{
    public class Basic
    {
        private static readonly Lang Lang = Lang.Instance;
        private static Basic _instance = new();

        private static readonly object LockObj = new();
        public static Basic Instance
        {
            get
            {
                lock (LockObj)
                {
                    return _instance ??= new Basic();
                }
            }
            set => _instance = value;
        }

        // Variables
        private string _folderPath = "";

        [JsonProperty("FolderPath", Order = 1)]
        public string FolderPath
        {
            get
            {
                if (_instance._folderPath != "") return _instance._folderPath;

                try
                {
                    _instance._folderPath = ((string)AppData.Instance.BasicCurrentPlatformJson["ExeLocationDefault"])?.Replace(
                        AppData.Instance.BasicCurrentPlatformExe, "");
                }
                catch (Exception)
                {
                    // ignored
                }

                return _instance._folderPath;
            }
            set => _instance._folderPath = value;
        }
        private bool _admin;
        [JsonProperty("Basic_Admin", Order = 2)] public bool Admin { get => _instance._admin; set => _instance._admin = value; }
        private int _trayAccNumber = 3;
        [JsonProperty("Basic_TrayAccNumber", Order = 3)] public int TrayAccNumber { get => _instance._trayAccNumber; set => _instance._trayAccNumber = value; }
        private bool _forgetAccountEnabled;
        [JsonProperty("ForgetAccountEnabled", Order = 4)] public bool ForgetAccountEnabled { get => _instance._forgetAccountEnabled; set => _instance._forgetAccountEnabled = value; }
        private bool _altClose;
        [JsonProperty("AltClose", Order = 5)] public bool AltClose { get => _instance._altClose; set => _instance._altClose = value; }

        private bool _desktopShortcut;
        [JsonIgnore] public bool DesktopShortcut { get => _instance._desktopShortcut; set => _instance._desktopShortcut = value; }

        // Constants
        [JsonIgnore] public static readonly string SettingsFile = "BasicSettings.json";
        /*
            [JsonIgnore] public string BasicImagePath = "wwwroot/img/profiles/basic/";
            [JsonIgnore] public string BasicImagePathHtml = "img/profiles/basic/";
        */
        [JsonIgnore] public static readonly string Processes = "BasicGamesLauncher";
        [JsonIgnore] public readonly string ContextMenuJson = $@"[
				{{""{Lang["Context_SwapTo"]}"": ""swapTo(-1, event)""}},
				{{""{Lang["Context_ChangeName"]}"": ""showModal('changeUsername')""}},
				{{""{Lang["Context_CreateShortcut"]}"": ""createShortcut()""}},
				{{""{Lang["Context_ChangeImage"]}"": ""changeImage(event)""}},
				{{""{Lang["Forget"]}"": ""forget(event)""}}
            ]";


        /// <summary>
        /// Updates the ForgetAccountEnabled bool in settings file
        /// </summary>
        /// <param name="enabled">Whether will NOT prompt user if they're sure or not</param>
        public void SetForgetAcc(bool enabled)
        {
            Globals.DebugWriteLine(@"[Func:Data\Settings\Basic.SetForgetAcc]");
            if (ForgetAccountEnabled == enabled) return; // Ignore if already set
            ForgetAccountEnabled = enabled;
            SaveSettings();
        }

        /// <summary>
        /// Get Basic.exe path from BasicSettings.json
        /// </summary>
        /// <returns>Basic.exe's path string</returns>
        public string Exe() => Path.Join(FolderPath, AppData.Instance.BasicCurrentPlatformExe);


        #region SETTINGS
        /// <summary>
        /// Default settings for BasicSettings.json
        /// </summary>
        public void ResetSettings()
        {
            Globals.DebugWriteLine(@"[Func:Data\Settings\Basic.ResetSettings]");
            _instance.FolderPath = "C:\\Program Files (x86)\\Basic Games\\Launcher\\Portal\\Binaries\\Win32\\";
            _instance.Admin = false;
            _instance.TrayAccNumber = 3;
            _instance._desktopShortcut = Shortcut.CheckShortcuts("Basic");

            SaveSettings();
        }
        public void SetFromJObject(JObject j)
        {
            Globals.DebugWriteLine(@"[Func:Data\Settings\Basic.SetFromJObject]");
            var curSettings = j.ToObject<Basic>();
            if (curSettings == null) return;
            _instance.FolderPath = curSettings.FolderPath;
            _instance.Admin = curSettings.Admin;
            _instance.TrayAccNumber = curSettings.TrayAccNumber;
            _instance._desktopShortcut = Shortcut.CheckShortcuts("Basic");
        }
        public void LoadFromFile() => SetFromJObject(GeneralFuncs.LoadSettings(AppData.Instance.BasicCurrentPlatformSettingsFile, GetJObject()));
        public JObject GetJObject() => JObject.FromObject(this);
        [JSInvokable]
        public void SaveSettings(bool mergeNewIntoOld = false) => GeneralFuncs.SaveSettings(AppData.Instance.BasicCurrentPlatformSettingsFile, GetJObject(), mergeNewIntoOld);
        #endregion
    }
}