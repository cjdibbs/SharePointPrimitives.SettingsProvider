#region BSD license
// Copyright 2010 Chris Dibbs. All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
//
//   1. Redistributions of source code must retain the above copyright notice, this list of
//      conditions and the following disclaimer.
//
//   2. Redistributions in binary form must reproduce the above copyright notice, this list
//      of conditions and the following disclaimer in the documentation and/or other materials
//      provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY Chris Dibbs ``AS IS'' AND ANY EXPRESS OR IMPLIED
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL Chris Dibbs OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
// ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
// ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// The views and conclusions contained in the software and documentation are those of the
// authors and should not be interpreted as representing official policies, either expressed
// or implied, of Chris Dibbs.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using SharePointPrimitives.SettingsProvider.Reflection;
using System.Configuration;

namespace SharePointPrimitives.SettingsProvider {
    public class SnapShot {
        public string Assembly { get; set; }
        public string Section { get; set; }
        public Dictionary<string, string> Settings { get; private set; }
        public Dictionary<string, string> ConnectionStrings { get; private set; }

        public bool IsEmpty() { return Settings.Count == 0 && ConnectionStrings.Count == 0; }

        public SnapShot() {
            Settings = new Dictionary<string, string>();
            ConnectionStrings = new Dictionary<string, string>();
        }

        /// <summary>
        /// builds a patch from the defaults as defined in the assembly<br/>
        /// Returns an empty patch for assemblies that don't use settings or
        /// are not using the settings provider
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static SnapShot BuildFrom(Assembly assembly) {
            SnapShot ret = new SnapShot();
            ret.Assembly = assembly.FullName;

            Type settingsT = assembly.GetSettingsType();
            if (settingsT == null)
                return null;
            ret.Section = settingsT.FullName;

            var settings = settingsT.GetProperties().Where(p => p.HasCustomAttribute<ApplicationScopedSettingAttribute>(true));
            
            foreach (var setting in settings) {
                if (setting.IsConnectionString())
                    ret.ConnectionStrings.Add(settingsT.FullName + "." + setting.Name, setting.DefaultValue());
                else
                    ret.Settings.Add(setting.Name, setting.DefaultValue());
            }
            return ret;
        }

        public static SnapShot GetFor(Assembly assembly) {
            Type settingsT = assembly.GetSettingsType();
            if (settingsT == null)
                return new SnapShot();
            else
                return GetFor(settingsT.FullName);
            
        }

        public static SnapShot GetFor(string sectionName) {
            SnapShot ret = new SnapShot();
            ret.Section = sectionName;
            
            using (var database = new SettingsProviderDatabase()) {
                var section = database.Sections.FirstOrDefault(s => s.Name == sectionName);
                if (section != null)
                    ret.Assembly = section.AssemblyName;
                ret.Settings = database.GetApplcationSettingsFor(sectionName);
                ret.ConnectionStrings = database.GetNamedConnectionsFor(sectionName);
            }
            return ret;
        }
    }
}
