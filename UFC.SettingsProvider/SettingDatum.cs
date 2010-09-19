using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace UFC.SettingsProviders {
    [Serializable]
    public sealed class SettingDatum {
        public SettingsSerializeAs SerializeAs { get; private set; }
        public string InnerValue{get; private set;}
    }
}
