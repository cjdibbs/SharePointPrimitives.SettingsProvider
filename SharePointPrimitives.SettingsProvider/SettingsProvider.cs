using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Xml;
using SharePointPrimitives.SettingsProvider;

namespace SharePointPremitives {
    public class SettingsProvider : System.Configuration.SettingsProvider, IApplicationSettingsProvider {

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

        /// <summary>
        /// Cache of the sql connections used by this assembly
        /// </summary>
        private Dictionary<string, string> ConnectionCache;

        /// <summary>
        /// Cache of the application settings used by this assembly
        /// </summary>
        private Dictionary<string, string> ApplicationCache;

        /// <summary>
        /// Loads the settings from the database. This is only called when a propery in the Settings object
        /// is used the first time, each time after that the assembly uses it's local cache
        /// </summary>
        /// <param name="context"></param>
        /// <param name="properties"></param>
        /// <returns>The requested settings</returns>
        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection properties) {
            using (var database = new SettingsProviderDatabase()) {
                SettingsPropertyValueCollection values = new SettingsPropertyValueCollection();
                string section = GetSectionName(context);
                ApplicationCache = database.GetApplcationSettingsFor(section);
                ConnectionCache = database.GetNamedConnectionsFor(section);
                foreach (SettingsProperty property in properties) {

                    SpecialSettingAttribute attribute = property.Attributes[typeof(SpecialSettingAttribute)] as SpecialSettingAttribute;
                    if (attribute != null && (attribute.SpecialSetting == SpecialSetting.ConnectionString))
                        values.Add(GetConnectionStringValue(section, property));
                    else if (IsApplicationSetting(property))
                        values.Add(GetApplcationSetting(property));
                    else
                        throw new ConfigurationErrorsException("User settings are not supported in this provider");

                }
                ApplicationCache = null;
                ConnectionCache = null;
                return values;
            }
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

            if (ApplicationCache.ContainsKey(property.Name))
                value.SerializedValue = ApplicationCache[property.Name];
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

            if (ConnectionCache.ContainsKey(settingName))
                value.PropertyValue = ConnectionCache[settingName];
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
