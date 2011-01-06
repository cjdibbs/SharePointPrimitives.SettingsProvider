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
using System.Configuration;
using System.Reflection;

namespace SharePointPrimitives.SettingsProvider.Reflection {
    public static class SettingsTools {
        static private readonly Type ApplicationSettingsBaseT = typeof(ApplicationSettingsBase);

        /// <summary>
        /// Not if this does not match the name of the provider class, and number of checks will break.
        /// </summary>
        private const string ProviderClassName = "SharePointPrimitives.SettingsProvider.Provider";

        /// <summary>
        /// Checks to see if the assembly uses ApplcationSettings at all
        /// </summary>
        /// <param name="assembly">Assembly to check</param>
        /// <returns>if there are any instances of ApplcationSettingsBase</returns>
        public static bool HasSettings(this Assembly assembly) {
            return assembly.GetTypes().Any(t => t.BaseType == ApplicationSettingsBaseT);
        }

        public static bool UsesSettingsProvider(this Assembly assembly) {
            return assembly.GetSettingsType() != null;
        }

        /// <summary>
        /// Get the ApplcationSettings type from the assembly <br/>
        /// if the assembly does not use settings or the settings are not wired to the SharePoint 
        /// Primitives provder class it will return null other wise it will return the Type of
        /// the settings class
        /// </summary>
        /// <param name="assembly">Assembly to search in</param>
        /// <returns>Type of the settings class</returns>
        public static Type GetSettingsType(this Assembly assembly) {
            Type settingsT = assembly.GetTypes().FirstOrDefault(t => t.BaseType == ApplicationSettingsBaseT);

            var attr = settingsT.GetCustomAttribute<SettingsProviderAttribute>(true);
            if (attr == null || !attr.ProviderTypeName.StartsWith(ProviderClassName)) {
                return null;
            }
            return settingsT;
        }

        /// <summary>
        /// Get the default setting of a setting based on the DefaultSettingValueAttribute
        /// </summary>
        /// <param name="info">property in the settings object to check</param>
        /// <returns>the assembly defined default value</returns>
        public static string DefaultValue(this PropertyInfo info) {
            string value = null;

            DefaultSettingValueAttribute defaultValue = info.GetCustomAttribute<DefaultSettingValueAttribute>(true);
            if (defaultValue != null)
                value = defaultValue.Value;
            return value;
        }

        /// <summary>
        /// Checks to see if the setting is a connection string, test for a
        /// SpecialSettingAttribute with a value of SpecialSetting.ConnectionString
        /// </summary>
        /// <param name="setting">property in the settings object to check</param>
        /// <returns>true if it is a connection string</returns>
        public static bool IsConnectionString(this PropertyInfo setting) {
            SpecialSettingAttribute special = setting.GetCustomAttribute<SpecialSettingAttribute>(true);
            return special != null && special.SpecialSetting == SpecialSetting.ConnectionString;
        }
    }
}
