using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UFC.SettingsProviders {
    interface SettingsConfigurationStore {
        Dictionary<string, SettingDatum> GetSectionValues(string section);
    }
}
