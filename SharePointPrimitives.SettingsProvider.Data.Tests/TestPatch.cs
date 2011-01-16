using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;

namespace SharePointPrimitives.SettingsProvider.Data.Tests {
    [TestFixture]
    public class TestPatch {
        [Test]
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

        [Test]
        public void TestUpdateViaXml() {
            string section = Guid.NewGuid().ToString();
            RunAddViaXml(section);
            RunUpdateViaXml(section);

            var state = SnapShot.GetFor(section);
            Assert.AreEqual(2, state.Settings.Count);
            Assert.AreEqual(1, state.ConnectionStrings.Count);
            Assert.AreEqual("20", state.Settings["ExampleInt"]);
            Assert.AreEqual("Testing update", state.Settings["ExampleString"]);
            Assert.AreEqual("Data Source=NOWBI-ADMIN;Initial Catalog=NowBI.VI.ReportingDB;Integrated Security=False", state.ConnectionStrings["ExampleConnection"]);

        }

        [Test]
        public void TestDeleteViaXml() {
            string section = Guid.NewGuid().ToString();
            RunAddViaXml(section);
            RunDeleteViaXml(section);

            var state = SnapShot.GetFor(section);
            Assert.AreEqual(1, state.Settings.Count);
            Assert.AreEqual(0, state.ConnectionStrings.Count);
            Assert.IsFalse(state.Settings.ContainsKey("ExampleInt"));
            Assert.AreEqual("Testing update", state.Settings["ExampleString"]);

        }

        [Test]
        public void TestToXml() {
            string section = Guid.NewGuid().ToString();
            string patchTxt =
@"<patch assembly='SharePointPrimitives.SettingsProvider.Data.Tests' section='" + section + @"' >
    <action type='Insert' name='ExampleInt' value='10' is-connection-string='false' />
    <action type='Insert' name='ExampleString' value='Testing update' is-connection-string='false' />
    <action type='Insert' name='ExampleConnection' value='Data Source=NOWBI-ADMIN;Initial Catalog=NowBI.VI.ReportingDB;Integrated Security=True' is-connection-string='true' />
</patch>";
            var elmIn = XElement.Parse(patchTxt);
            Patch patch = Patch.FromXml(elmIn);
            var elmOut = patch.ToXml();
            Assert.AreEqual(elmIn.ToString(), elmOut.ToString());
        }

        [Test]
        public void TestEmptyPatch() {
            var empty = new Patch();
            empty.Apply(new Patch.ApplyOptions());
        }

        [Test]
        public void TestToString() {
            string section = Guid.NewGuid().ToString();
            string patchTxt =
@"<patch assembly='SharePointPrimitives.SettingsProvider.Data.Tests' section='" + section + @"' >
    <action type='Insert' name='ExampleInt' value='10' is-connection-string='false' />
    <action type='Insert' name='ExampleString' value='Testing update' is-connection-string='false' />
    <action type='Insert' name='ExampleConnection' value='Data Source=NOWBI-ADMIN;Initial Catalog=NowBI.VI.ReportingDB;Integrated Security=True' is-connection-string='true' />
</patch>";
            var elmIn = XElement.Parse(patchTxt);
            Patch patch = Patch.FromXml(elmIn);
            var text = patch.ToString();
            var lines = text.Split('\n');
            Assert.AreEqual(3, lines.Length);
            StringAssert.StartsWith("Insert", lines[0]);
            StringAssert.StartsWith("Insert", lines[1]);
            StringAssert.StartsWith("Insert", lines[2]);
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
    <action type='Update' name='ExampleConnection' value='Data Source=NOWBI-ADMIN;Initial Catalog=NowBI.VI.ReportingDB;Integrated Security=False' is-connection-string='true' />
</patch>";

            Patch.FromXml(XElement.Parse(patch)).Apply(new Patch.ApplyOptions());
        }

        private static void RunDeleteViaXml(string section) {
            string patch =
@"<patch assembly='SharePointPrimitives.SettingsProvider.Data.Tests' section='" + section + @"' >
    <action type='Delete' name='ExampleInt' value='20' is-connection-string='false' />
    <action type='Delete' name='ExampleConnection' value='Data Source=NOWBI-ADMIN;Initial Catalog=NowBI.VI.ReportingDB;Integrated Security=True' is-connection-string='true' />
</patch>";

            var p = Patch.FromXml(XElement.Parse(patch));
            p.Apply(new Patch.ApplyOptions());
        }
    }
}
