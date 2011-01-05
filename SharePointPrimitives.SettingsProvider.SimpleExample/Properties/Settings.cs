using System.Configuration;
using SharePointPrimitives.SettingsProvider;

namespace SimpleExample.Properties {
    [SettingsProvider(typeof(Provider))]
    internal sealed partial class Settings { }
}
