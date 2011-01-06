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

        public Patch(SnapShot inital)
            : this() {
            this.Assembly = inital.Assembly;
            this.Section = inital.Section;
            foreach (var setting in inital.Settings)
                Actions.Add(new Action() {
                    Name = setting.Key,
                    Value = setting.Value,
                    IsConnectionString = false,
                    Type = Action.ActionType.Insert
                });
            foreach (var connection in inital.ConnectionStrings) {
                Actions.Add(new Action() {
                    Name = connection.Key,
                    Value = connection.Value,
                    IsConnectionString = true,
                    Type = Action.ActionType.Insert
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
                        Type = Action.ActionType.Update
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

        public override string ToString() {
            return String.Join("\n",
                Actions.Select(a => String.Format("{0} {1}='{2}'", a.Type, a.Name, a.Value))
                .ToArray()
            );
        }

        //public static Patch FromXml(XElement element) {
        //}

        public struct ApplyOptions {
            public bool UseExistingConnectionStrings { get; set; }
            public bool MustKnowCurrentValueToDelete { get; set; }
            public bool CleanUpZombiedConnectionStrings { get; set; }
        }

        public void Apply(ApplyOptions options) {
            using (var database = new SettingsProviderDatabase()) {
                var section = database.Sections.FirstOrDefault(s => s.Name == Section);
                if (section == null) {
                    section = new Section() { Name = Section, AssemblyName = Assembly };
                    database.AddToSections(section);
                }

                foreach (Action action in Actions) {
                    switch (action.Type) {
                        case Action.ActionType.Insert:
                            if (action.IsConnectionString) {
                                var name = new SqlConnectionName() {
                                    Name = action.Name
                                };
                                var connection = new SqlConnectionString() {
                                    ConnectionString = action.Value
                                };

                                database.AddToSqlConnectionNames(name);
                                database.AddToSqlConnectionStrings(connection);

                                name.Section = section;
                                name.SqlConnectionString = connection;
                            } else {
                                var setting = new ApplicationSetting() {
                                    Name = action.Name,
                                    Value = action.Value
                                };
                                setting.Section = section;
                            }
                            break;
                    }
                }
                database.SaveChanges();
            }
        }
    }
}
