using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;

namespace SharePointPrimitives.SettingsProvider {
    public static class DatabaseExtensions {

        private static Func<SettingsProviderDatabase, string, IQueryable<ApplicationSetting>> SectionData =
            CompiledQuery.Compile((SettingsProviderDatabase context, string sectionName) =>
                context.ApplicationSettings.Include("Section")
                       .Where(setting => setting.Section.Name == sectionName)
            );

        public static Dictionary<string, string> GetApplcationSettingsFor(this SettingsProviderDatabase database, string sectionName) {
            return SectionData(database, sectionName).ToDictionary(
                k => k.Name,
                v => v.Value
            );
        }

        public static Dictionary<string, string> GetNamedConnectionsFor(this SettingsProviderDatabase database, string sectionName) {
            return database.SqlConnectionNames
                .Include("Section")
                .Include("SqlConnectionString")
                .Where(name => name.Section.Name == sectionName)
                .ToDictionary(k => k.Name, v => v.SqlConnectionString.ConnectionString);
        }
    }
}
