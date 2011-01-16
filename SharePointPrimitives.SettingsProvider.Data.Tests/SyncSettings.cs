using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;

namespace SharePointPrimitives.SettingsProvider.Data.Tests {
    [TestClass]
    public class TestPatch {
        [TestMethod]
        public void TestAddViaXml() {
            string section = Guid.NewGuid().ToString();
            RunAddViaXml(section);

            var state = SnapShot.GetFor(section);
            Assert.AreEqual(2, state.Settings.Count);
            Assert.AreEqual(1, state.ConnectionStrings.Count);
            Assert.AreEqual("10", state.Settings["ExampleInt"]);
            Assert.AreEqual("Testing update", state.Settings["ExampleString"]);
            Assert.AreEqual("Data Source=NOWBI-ADMIN;Initial Catalog=NowBI.VI.ReportingDB;Integrated Security=True", state.ConnectionStrings["ExampleConnection"]);
        }

        [TestMethod]
        public void TestUpdateViaXml() {
            string section = Guid.NewGuid().ToString();
            RunAddViaXml(section);
            RunUpdateViaXml(section);

            var state = SnapShot.GetFor(section);
            Assert.AreEqual(2, state.Settings.Count);
            Assert.AreEqual(1, state.ConnectionStrings.Count);
            Assert.AreEqual("20", state.Settings["ExampleInt"]);
            Assert.AreEqual("Testing update", state.Settings["ExampleString"]);
            Assert.AreEqual("Data Source=NOWBI-ADMIN;Initial Catalog=NowBI.VI.ReportingDB;Integrated Security=True", state.ConnectionStrings["ExampleConnection"]);

        }

        [TestMethod]
        public void TestDeleteViaXml() {
            string section = Guid.NewGuid().ToString();
            RunAddViaXml(section);
            RunDeleteViaXml(section);

            var state = SnapShot.GetFor(section);
            Assert.AreEqual(1, state.Settings.Count);
            Assert.AreEqual(1, state.ConnectionStrings.Count);
            Assert.IsFalse(state.Settings.ContainsKey("ExampleInt"));
            Assert.AreEqual("Testing update", state.Settings["ExampleString"]);
            Assert.AreEqual("Data Source=NOWBI-ADMIN;Initial Catalog=NowBI.VI.ReportingDB;Integrated Security=True", state.ConnectionStrings["ExampleConnection"]);

        }

        private static void RunAddViaXml(string section) {
            string patch =
@"<patch assembly='SharePointPrimitives.SettingsProvider.Data.Tests' section='" + section + @"' >
    <action type='Insert' name='ExampleInt' value='10' is-connection-string='false' />
    <action type='Insert' name='ExampleString' value='Testing update' is-connection-string='false' />
    <action type='Insert' name='ExampleConnection' value='Data Source=NOWBI-ADMIN;Initial Catalog=NowBI.VI.ReportingDB;Integrated Security=True' is-connection-string='true' />
</patch>";

            Patch.FromXml(XElement.Parse(patch)).Apply(new Patch.ApplyOptions());
        }

        private static void RunUpdateViaXml(string section) {
            string patch =
@"<patch assembly='SharePointPrimitives.SettingsProvider.Data.Tests' section='" + section + @"' >
    <action type='Update' name='ExampleInt' value='20' is-connection-string='false' />
</patch>";

            Patch.FromXml(XElement.Parse(patch)).Apply(new Patch.ApplyOptions());
        }

        private static void RunDeleteViaXml(string section) {
            string patch =
@"<patch assembly='SharePointPrimitives.SettingsProvider.Data.Tests' section='" + section + @"' >
    <action type='Delete' name='ExampleInt' value='20' is-connection-string='false' />
</patch>";

            var p = Patch.FromXml(XElement.Parse(patch));
            p.Apply(new Patch.ApplyOptions());
        }
    }
}
