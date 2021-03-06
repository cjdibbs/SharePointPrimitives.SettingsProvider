﻿#region BSD license 
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
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Xml;

namespace SharePointPrimitives.SettingsProvider {
    public class Provider : System.Configuration.SettingsProvider, IApplicationSettingsProvider {

        public override string ApplicationName { get; set; }


        /// <summary>
        /// Based on the a configuration provider in the System.dll
        /// </summary>
        /// <param name="name"></param>
        /// <param name="values">Passed through to base unchanged</param>
        public override void Initialize(string name, NameValueCollection values) {
            if (string.IsNullOrEmpty(name)) {
                name = "SPPrimitives.SettingsProvider";
            }
            base.Initialize(name, values);
        }

        private SnapShot settings;

        /// <summary>
        /// Loads the settings from the database. This is only called when a propery in the Settings object
        /// is used the first time, each time after that the assembly uses it's local cache
        /// </summary>
        /// <param name="context"></param>
        /// <param name="properties"></param>
        /// <returns>The requested settings</returns>
        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection properties) {
            SettingsPropertyValueCollection values = new SettingsPropertyValueCollection();
            string section = GetSectionName(context);
            settings = SnapShot.GetFor(section);
            foreach (SettingsProperty property in properties) {

                SpecialSettingAttribute attribute = property.Attributes[typeof(SpecialSettingAttribute)] as SpecialSettingAttribute;
                if (attribute != null && (attribute.SpecialSetting == SpecialSetting.ConnectionString))
                    values.Add(GetConnectionStringValue(section, property));
                else if (IsApplicationSetting(property))
                    values.Add(GetApplcationSetting(property));
                else
                    throw new ConfigurationErrorsException("User settings are not supported in this provider");
            }
            
            return values;

        }

        /// <summary>
        /// builds a setting object as requested by property
        /// 
        /// This assumes that the ApplcationCache has been loaded before this is called
        /// </summary>
        /// <param name="property">infomation about the requested setting</param>
        /// <returns>the value of the setting</returns>
        private SettingsPropertyValue GetApplcationSetting(SettingsProperty property) {
            SettingsPropertyValue value = new SettingsPropertyValue(property);

            if (settings.Settings.ContainsKey(property.Name))
                value.SerializedValue = settings.Settings[property.Name];
            else if (property.DefaultValue != null)
                value.SerializedValue = property.DefaultValue;
            else
                value.PropertyValue = null;
            value.IsDirty = false;
            return value;
        }

        /// <summary>
        /// Gets the connection string as requested by property
        /// 
        /// This assumes that the ConnectionCache has been loaded before this is caleed
        /// </summary>
        /// <param name="section">name of the section</param>
        /// <param name="property">infomation about the requested connection string</param>
        /// <returns>the value of the connection</returns>
        SettingsPropertyValue GetConnectionStringValue(string section, SettingsProperty property) {
            SettingsPropertyValue value = new SettingsPropertyValue(property);
            string settingName = section + "." + property.Name;

            if (settings.ConnectionStrings.ContainsKey(settingName))
                value.PropertyValue = settings.ConnectionStrings[settingName];
            else if (property.DefaultValue != null)
                value.PropertyValue = property.DefaultValue;

            value.IsDirty = false;
            return value;
        }

        /// <summary>
        /// based on code decompied from the System.dll and the LocalFileSettingsProvider 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string GetSectionName(SettingsContext context) {
            string name = (string)context["GroupName"];
            string key = (string)context["SettingsKey"];
            if (!string.IsNullOrEmpty(key)) {
                name = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", new object[] { name, key });
            }
            return XmlConvert.EncodeLocalName(name);
        }

        /// <summary>
        /// based on code decompied from the System.dll and the LocalFileSettingsProvider 
        /// </summary>
        private bool IsApplicationSetting(SettingsProperty setting) {
            bool user = setting.Attributes[typeof(UserScopedSettingAttribute)] is UserScopedSettingAttribute;
            bool application = setting.Attributes[typeof(ApplicationScopedSettingAttribute)] is ApplicationScopedSettingAttribute;

            if (user && application)
                throw new ConfigurationErrorsException("Both Scope Attributes");
            
            if (!user && !application) 
                throw new ConfigurationErrorsException("No Scope Attributes");
            
            return application;
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection) {
            throw new NotImplementedException();
        }

        public SettingsPropertyValue GetPreviousVersion(SettingsContext context, SettingsProperty property) {
            throw new NotImplementedException();
        }

        public void Reset(SettingsContext context) {
            throw new NotImplementedException();
        }

        public void Upgrade(SettingsContext context, SettingsPropertyCollection properties) {
            throw new NotImplementedException();
        }
    }
}
