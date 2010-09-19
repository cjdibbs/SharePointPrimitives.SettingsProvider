using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml;
using System.Globalization;
using System.Data.SqlClient;

namespace UFC.SettingsProviders {
    public class RemoteApplcationSettings : SettingsProvider, IApplicationSettingsProvider {

        ISQLConnectionStringStore sqlConnectionStrings = null;
        SettingsConfigurationStore applicationSettingsStore = null;

        public override string ApplicationName { get; set; }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection properties) {
            SettingsPropertyValueCollection values = new SettingsPropertyValueCollection();
            string section = GetSectionName(context);
            var storeValues = applicationSettingsStore.GetSectionValues(section);

            foreach (SettingsProperty property in properties) {
                
                SpecialSettingAttribute attribute = property.Attributes[typeof(SpecialSettingAttribute)] as SpecialSettingAttribute;
                if (attribute != null && (attribute.SpecialSetting == SpecialSetting.ConnectionString)) {
                    values.Add(GetConnectionStringValue(section,property));
                } else if (IsApplicationSetting(property)) {
                    values.Add(GetApplcationSetting(storeValues, property));
                } else {
                    throw new ConfigurationErrorsException("User settings are not supported in this provider");
                }
            }

            return values;
        }

        private static SettingsPropertyValue GetApplcationSetting(Dictionary<string, SettingDatum> storeValues, SettingsProperty property) {
            SettingsPropertyValue value = new SettingsPropertyValue(property);

            if (storeValues.ContainsKey(property.Name)) {
                var val = storeValues[property.Name];
                if (val.SerializeAs == SettingsSerializeAs.String)
                    value.SerializedValue = val.InnerValue; //todo unescape the string
                else
                    value.SerializedValue = val.InnerValue;
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
