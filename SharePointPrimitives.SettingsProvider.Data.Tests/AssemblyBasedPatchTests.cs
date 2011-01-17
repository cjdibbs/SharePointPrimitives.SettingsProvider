using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Reflection;
using System.IO;

namespace SharePointPrimitives.SettingsProvider.Data.Tests {
    [TestFixture]
    public class AssemblyBasedPatchTests {
        [Test]
        public void TestEmptyAssembly(){
            var folder = @"Resources\Assembly-Empty\ProviderTestLib.dll";

            var empty = LoadAssembly(folder);
            try {
                var snapshop = SnapShot.BuildFrom(empty);
            } catch (ArgumentException) {
                return;    
            }
            Assert.Fail();
        }

        [Test]
        public void TestGetForEmptyAssembly() {
            var folder = @"Resources\Assembly-Empty\ProviderTestLib.dll";

            var empty = LoadAssembly(folder);
            var snapshop = SnapShot.GetFor(empty);
            Assert.IsTrue(snapshop.IsEmpty);
        }

        private static Assembly LoadAssembly(string folder) {
            var dir = @"C:\Users\Chris\Desktop\SharePointPrimitives.SettingsProvider\SharePointPrimitives.SettingsProvider.Data.Tests\";
            return Assembly.LoadFile(Path.Combine(dir, folder));
        }

        [Test]
        public void TestUnwiredAssembly() {
            var folder = @"Resources\Assembly-No Provider\ProviderTestLib.dll";
            var unwired = LoadAssembly(folder);
            try {
                var snapshop = SnapShot.BuildFrom(unwired);
            } catch (ArgumentException) {
                return;
            }
            Assert.Fail();
        }

        [Test]
        public void FullTest() {
            var sectionName = "ProviderTestLib.Properties.Settings";
            ClearSection(sectionName);

            var snapshot = SnapShot.GetFor(sectionName);

            Assert.IsTrue(snapshot.IsEmpty);
            Assert.IsTrue(snapshot.Settings.Count == 0);
            Assert.IsTrue(snapshot.ConnectionStrings.Count == 0);

            Testv1( );
            Testv2( );
            Testv3( );
            Testv4( );
        }

        private static void Testv1( ) {
            var folder = @"Resources\Assembly-1.0\ProviderTestLib.dll";
            var assembly = LoadAssembly(folder);

            var snapshot = SnapShot.BuildFrom(assembly);
            Assert.IsFalse(snapshot.IsEmpty);
            Assert.IsTrue(snapshot.Settings.Count == 1);
            Assert.IsTrue(snapshot.ConnectionStrings.Count == 0);

            var patch1 = new Patch(snapshot, Patch.Action.ActionType.Insert);
            patch1.Apply(new Patch.ApplyOptions());

            snapshot = SnapShot.GetFor(snapshot.Section);

            Assert.IsFalse(snapshot.IsEmpty);
            Assert.IsTrue(snapshot.Settings.Count == 1);
            Assert.IsTrue(snapshot.ConnectionStrings.Count == 0);
        }

        private static void Testv2( ) {
            var folder = @"Resources\Assembly-2.0\ProviderTestLib.dll";
            var assembly = LoadAssembly(folder);

            var snapshot = SnapShot.BuildFrom(assembly);
            var current = SnapShot.GetFor(assembly);
            Assert.IsFalse(snapshot.IsEmpty);
            Assert.IsTrue(snapshot.Settings.Count == 1);
            Assert.IsTrue(snapshot.ConnectionStrings.Count == 1);

            var patch1 = new Patch(current, snapshot);
            patch1.Apply(new Patch.ApplyOptions());

            snapshot = SnapShot.GetFor(snapshot.Section);

            Assert.IsFalse(snapshot.IsEmpty);
            Assert.IsTrue(snapshot.Settings.Count == 1);
            Assert.IsTrue(snapshot.ConnectionStrings.Count == 1);
        }

        private static void Testv3() {
            var folder = @"Resources\Assembly-3.0\ProviderTestLib.dll";
            var assembly = LoadAssembly(folder);

            var snapshot = SnapShot.BuildFrom(assembly);
            var current = SnapShot.GetFor(snapshot.Section);
            Assert.IsFalse(snapshot.IsEmpty);
            Assert.IsTrue(snapshot.Settings.Count == 1);
            Assert.IsTrue(snapshot.ConnectionStrings.Count == 1);

            var patch1 = new Patch(current, snapshot);
            patch1.Apply(new Patch.ApplyOptions());

            snapshot = SnapShot.GetFor(snapshot.Section);

            Assert.IsFalse(snapshot.IsEmpty);
            Assert.IsTrue(snapshot.Settings.Count == 1);
            Assert.IsTrue(snapshot.ConnectionStrings.Count == 1);
        }

        private static void Testv4() {
            var folder = @"Resources\Assembly-4.0\ProviderTestLib.dll";
            var assembly = LoadAssembly(folder);

            var snapshot = SnapShot.BuildFrom(assembly);
            var current = SnapShot.GetFor(snapshot.Section);
            Assert.IsFalse(snapshot.IsEmpty);
            Assert.IsTrue(snapshot.Settings.Count == 1);
            Assert.IsTrue(snapshot.ConnectionStrings.Count == 1);

            var patch1 = new Patch(current, snapshot);
            patch1.Apply(new Patch.ApplyOptions());

            snapshot = SnapShot.GetFor(snapshot.Section);

            Assert.IsFalse(snapshot.IsEmpty);
            Assert.IsTrue(snapshot.Settings.Count == 1);
            Assert.IsTrue(snapshot.ConnectionStrings.Count == 1);
        }

        private static void ClearSection(string sectionName) {
            var section = SnapShot.GetFor(sectionName);
            var patch = new Patch(section, Patch.Action.ActionType.Delete);
            patch.Apply(new Patch.ApplyOptions());
        }
    }
}
