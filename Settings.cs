namespace Little_Registry_Cleaner.Properties {
    
    
    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
    internal sealed partial class Settings {
        
        public Settings() {
            // // To add event handlers for saving and changing settings, uncomment the lines below:
            //
            // this.SettingChanging += this.SettingChangingEventHandler;
            //
            // this.SettingsSaving += this.SettingsSavingEventHandler;
            //
        }
        
        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e) {
            // Add code to handle the SettingChangingEvent event here.
        }
        
        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e) {
            // Add code to handle the SettingsSaving event here.
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SettingsSerializeAs(System.Configuration.SettingsSerializeAs.Binary)]
        public global::Little_Registry_Cleaner.ExcludeList.ExcludeArray arrayExcludeList
        {
            get
            {
                if (((global::Little_Registry_Cleaner.ExcludeList.ExcludeArray)(this["arrayExcludeList"])) == null)
                    ((this["arrayExcludeList"])) = new Little_Registry_Cleaner.ExcludeList.ExcludeArray();

                return ((global::Little_Registry_Cleaner.ExcludeList.ExcludeArray)(this["arrayExcludeList"]));
            }
            set
            {
                this["arrayExcludeList"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public string strProgramSettingsDir
        {
            get
            {
                if (string.IsNullOrEmpty(this["strProgramSettingsDir"] as string))
                    this["strProgramSettingsDir"] = string.Format("{0}\\Little Registry Cleaner", global::System.Environment.GetFolderPath(global::System.Environment.SpecialFolder.CommonProgramFiles));

                if (!global::System.IO.Directory.Exists(this["strProgramSettingsDir"] as string))
                    global::System.IO.Directory.CreateDirectory(this["strProgramSettingsDir"] as string);

                return this["strProgramSettingsDir"] as string;
            }
            set { this["strProgramSettingsDir"] = value; }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public string strOptionsBackupDir
        {
            get
            {
                if (string.IsNullOrEmpty(this["strOptionsBackupDir"] as string))
                    this["strOptionsBackupDir"] = string.Format("{0}\\Backups", strProgramSettingsDir);

                if (!global::System.IO.Directory.Exists(this["strOptionsBackupDir"] as string))
                    global::System.IO.Directory.CreateDirectory(this["strOptionsBackupDir"] as string);

                return this["strOptionsBackupDir"] as string;;
            }
            set { this["strOptionsBackupDir"] = value; }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public string strOptionsLogDir
        {
            get
            {
                if (string.IsNullOrEmpty(this["strOptionsLogDir"] as string))
                    this["strOptionsLogDir"] = string.Format("{0}\\Logs", strProgramSettingsDir);

                if (!global::System.IO.Directory.Exists(this["strOptionsLogDir"] as string))
                    global::System.IO.Directory.CreateDirectory(this["strOptionsLogDir"] as string);

                return this["strOptionsLogDir"] as string;
            }
            set { this["strOptionsLogDir"] = value; }
        }
    }
}
