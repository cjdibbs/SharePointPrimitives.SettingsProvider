using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace UFC.SettingsProvider.Example {
    [SettingsProvider(typeof(UFC.SettingsProviders.RemoteApplcationSettings))]
    internal sealed partial class Settings { }
}
