using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml.Linq;

namespace SharePointPrimitives.SettingsProvider {
    public class Patch {

        public struct ApplyOptions {
            public bool UseExistingConnectionStrings { get; set; }
            public bool MustKnowCurrentValueToDelete { get; set; }
        }

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
        }

        public string Assembly { get; set; }
        public string Section { get; set; }
        public List<Action> Actions { get; private set; }
        

        public Patch() { Actions = new List<Action>(); }

        public Patch(SnapShot inital) : this(){
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


        public Patch(SnapShot from, SnapShot to) : this(){
            this.Section = to.Section;
            this.Assembly = to.Assembly;

            DiffDictionary(from.Settings, to.Settings, false);
            DiffDictionary(from.ConnectionStrings, to.ConnectionStrings, true);
        }

        private void DiffDictionary(Dictionary<string, string> from, Dictionary<string,string> to, bool areConnectionStrings) {
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
                new XAttribute("section",Section)
            );
            foreach(Action action in Actions)
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

        //public static Patch FromXml(XElement element) {
        //}

        //public void Apply(ApplyOptions options) {
        //}

        //public XElement Test() {
        //}
    }
}
