using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml.Linq;

namespace SharePointPrimitives.SettingsProvider {
    public class Patch {

        public class Action {
            public enum ActionType {
                Update,
                Insert,
                Delete
            }

            public ActionType Type { get; set; }
            public bool IsConnectionString { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
            //public string ValueType { get; set; }
        }

        public string Assembly { get; set; }
        public string Section { get; set; }
        public List<Action> Actions { get; private set; }

        public Patch() { Actions = new List<Action>(); }

        public Patch(SnapShot inital, Action.ActionType action)
            : this() {
            this.Assembly = inital.Assembly;
            this.Section = inital.Section;
            foreach (var setting in inital.Settings)
                Actions.Add(new Action() {
                    Name = setting.Key,
                    Value = setting.Value,
                    IsConnectionString = false,
                    Type = action
                });
            foreach (var connection in inital.ConnectionStrings) {
                Actions.Add(new Action() {
                    Name = connection.Key,
                    Value = connection.Value,
                    IsConnectionString = true,
                    Type = action
                });
            }
        }


        public Patch(SnapShot from, SnapShot to)
            : this() {
            this.Section = to.Section;
            this.Assembly = to.Assembly;

            DiffDictionary(from.Settings, to.Settings, false);
            DiffDictionary(from.ConnectionStrings, to.ConnectionStrings, true);
        }

        private void DiffDictionary(Dictionary<string, string> from, Dictionary<string, string> to, bool areConnectionStrings) {
            foreach (var setting in to) {
                if (!from.ContainsKey(setting.Key))
                    Actions.Add(new Action() {
                        Name = setting.Key,
                        Value = setting.Value,
                        IsConnectionString = areConnectionStrings,
                        Type = Action.ActionType.Insert
                    });
                else if (from[setting.Key] != setting.Value) {
                    Actions.Add(new Action() {
                        Name = setting.Key,
                        Value = setting.Value,
                        IsConnectionString = areConnectionStrings,
                        Type = Action.ActionType.Update
                    });
                }
            }

            foreach (var setting in from) {
                if (!to.ContainsKey(setting.Key))
                    Actions.Add(new Action() {
                        Name = setting.Key,
                        Value = setting.Value,
                        IsConnectionString = areConnectionStrings,
                        Type = Action.ActionType.Delete
                    });
            }
        }

        public bool IsEmpty { get { return Actions.Count == 0; } }

        public XElement ToXml() {
            XElement ret = new XElement("patch",
                new XAttribute("assembly", Assembly),
                new XAttribute("section", Section)
            );
            foreach (Action action in Actions)
                ret.Add(
                    new XElement("action",
                        new XAttribute("type", action.Type.ToString()),
                        new XAttribute("name", action.Name),
                        new XAttribute("value", action.Value),
                        new XAttribute("is-connection-string", action.IsConnectionString)
                    )
                );
            return ret;
        }

        public static Patch FromXml(XElement element) {
            Patch ret = new Patch();
            if (element.Name == "patch") {
                ret.Assembly = element.Attribute("assembly").Value;
                ret.Section = element.Attribute("section").Value;
                foreach(var action in element.Elements("action")){
                    ret.Actions.Add(
                        new Action() {
                            Type = (Action.ActionType)Enum.Parse(typeof(Action.ActionType), action.Attribute("type").Value),
                            Name = action.Attribute("name").Value,
                            Value = action.Attribute("value").Value,
                            IsConnectionString = Boolean.Parse(action.Attribute("is-connection-string").Value)
                        }
                    );
                }
            }
            return ret;
        }

        public override string ToString() {
            return String.Join("\n",
                Actions.Select(a => String.Format("{0} {1}='{2}'", a.Type, a.Name, a.Value))
                .ToArray()
            );
        }



        public struct ApplyOptions {
            public bool UseExistingConnectionStrings { get; set; }
            public bool MustKnowCurrentValueToDelete { get; set; }
            public bool CleanUpZombiedConnectionStrings { get; set; }
        }

        public void Apply(ApplyOptions options) {
            if (IsEmpty)
                return;

            using (var database = new SettingsProviderDatabase()) {
                var section = database.Sections.FirstOrDefault(s => s.Name == Section);
                if (section == null) {
                    section = new Section() { Name = Section, AssemblyName = Assembly };
                    database.AddToSections(section);
                }

                foreach (Action action in Actions) {
                    switch (action.Type) {
                        case Action.ActionType.Insert:
                            if (action.IsConnectionString) 
                                InsertConnectionString(options, database, section, action);
                            else 
                                InsertApplcationSetting(section, action);
                            break;
                        case Action.ActionType.Delete:
                            if (action.IsConnectionString) {
                                var names = from connectionName in database.SqlConnectionNames.Include("Section")
                                            where connectionName.Section.Id == section.Id && connectionName.Name == action.Name
                                            select connectionName;
                                foreach (var name in names) database.DeleteObject(name);
                            } else {
                                var settings = from setting in database.ApplicationSettings
                                               where setting.SectionId == section.Id && setting.Name == action.Name
                                               select setting;
                                foreach (var name in settings) database.DeleteObject(name);
                            }
                            break;
                        case Action.ActionType.Update:
                            if (action.IsConnectionString) {
                                SqlConnectionString connection = database.SqlConnectionNames
                                    .Include("Section")
                                    .Include("SqlConnectionString")
                                    .Where(sec => sec.Section.Id == section.Id)
                                    .Select(sec => sec.SqlConnectionString)
                                    .FirstOrDefault();
                                if(connection != null)
                                    connection.ConnectionString = action.Value;
                            } else {
                                ApplicationSetting setting = database.ApplicationSettings
                                    .Include("Section")
                                    .Where(s => s.Section.Id == section.Id && s.Name == action.Name)
                                    .FirstOrDefault();
                                if (setting != null)
                                    setting.Value = action.Value;
                            }
                            break;
                    }
                }
                database.SaveChanges();
            }
        }

        private static void InsertApplcationSetting(SettingsProvider.Section section, Action action) {
            var setting = new ApplicationSetting() {
                Name = action.Name,
                Value = action.Value
            };
            setting.Section = section;
        }

        private static void InsertConnectionString(ApplyOptions options, SettingsProviderDatabase database, SettingsProvider.Section section, Action action) {
            var name = new SqlConnectionName() {
                Name = action.Name
            };

            SqlConnectionString connection = null;
            if (!options.UseExistingConnectionStrings) {
                connection = database.SqlConnectionStrings.FirstOrDefault(c => c.ConnectionString == action.Value);
            }
            if (connection == null) {
                connection = new SqlConnectionString() {
                    ConnectionString = action.Value
                };
                database.AddToSqlConnectionStrings(connection);
            }

            database.AddToSqlConnectionNames(name);

            name.Section = section;
            name.SqlConnectionString = connection;
        }
    }
}
