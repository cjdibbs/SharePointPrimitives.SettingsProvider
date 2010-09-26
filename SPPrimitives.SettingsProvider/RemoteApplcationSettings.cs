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
// THIS SOFTWARE IS PROVIDED BY <COPYRIGHT HOLDER> ``AS IS'' AND ANY EXPRESS OR IMPLIED
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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml;
using System.Globalization;
using System.Data.SqlClient;
using System.Collections.Specialized;

namespace SPPrimitives.SettingsProvider {
    public class RemoteApplcationSettings : System.Configuration.SettingsProvider, IApplicationSettingsProvider {
        public RemoteApplcationSettings() {
        }
        SQLConnectionStringStore sqlConnectionStrings = null;
        SettingsConfigurationStore applicationSettingsStore = null;

        public override string ApplicationName { get; set; }

        public override void Initialize(string name, NameValueCollection values) {
            if (string.IsNullOrEmpty(name)) {
                name = "SPPrimitives.SettingsProvider.RemoteApplcationSettings";
            }
            base.Initialize(name, values);
        }



        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection properties) {
            using (sqlConnectionStrings = new SQLConnectionStringStore()) {
                using (applicationSettingsStore = new SettingsConfigurationStore()) {
                    SettingsPropertyValueCollection values = new SettingsPropertyValueCollection();
                    string section = GetSectionName(context);
                    var storeValues = applicationSettingsStore.GetSectionValues(section);

                    foreach (SettingsProperty property in properties) {

                        SpecialSettingAttribute attribute = property.Attributes[typeof(SpecialSettingAttribute)] as SpecialSettingAttribute;
                        if (attribute != null && (attribute.SpecialSetting == SpecialSetting.ConnectionString)) {
                            values.Add(GetConnectionStringValue(section, property));
                        } else if (IsApplicationSetting(property)) {
                            values.Add(GetApplcationSetting(storeValues, property));
                        } else {
                            throw new ConfigurationErrorsException("User settings are not supported in this provider");
                        }
                    }

                    return values;
                }
            }
        }

        private static SettingsPropertyValue GetApplcationSetting(Dictionary<string, SettingDatum> storeValues, SettingsProperty property) {
            SettingsPropertyValue value = new SettingsPropertyValue(property);

            if (storeValues.ContainsKey(property.Name)) {
                var val = storeValues[property.Name];
                if (val.SerializeAs == SettingsSerializeAs.String)
                    value.SerializedValue = val.Value; //todo unescape the string
                else
                    value.SerializedValue = val.Value;
            } else if (property.DefaultValue != null)
                value.SerializedValue = property.DefaultValue;
            else
                value.PropertyValue = null;
            value.IsDirty = false;
            return value;
        }

        SettingsPropertyValue GetConnectionStringValue(string section, SettingsProperty property) {
            SettingsPropertyValue value = new SettingsPropertyValue(property);
            string settingName = section + "." + property.Name;
            string catalog = null;
            if (property.DefaultValue != null) {
                SqlConnectionStringBuilder connection = new SqlConnectionStringBuilder(property.DefaultValue as string);
                catalog = connection.InitialCatalog;
                value.PropertyValue = sqlConnectionStrings.Search(settingName, catalog) ?? property.DefaultValue;
            } else {
                value.PropertyValue = sqlConnectionStrings.GetByName(settingName) ?? "";
            }
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
            if (user && application) {
                throw new ConfigurationErrorsException("Both Scope Attributes");
            }
            if (!user && !application) {
                throw new ConfigurationErrorsException("No Scope Attributes");
            }
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
