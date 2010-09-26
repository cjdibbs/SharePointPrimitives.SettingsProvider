// Copyright 2010 Chris Dibbs. All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
//
//   1. Redistributions of source code must retain the above copyright notice, this list of
//      conditions and the following disclaimer.
//
//   2. Redistributions in binary form must reproduce the above copyright notice, this list
//      of conditions and the following disclaimer in the documentation and/or other materials
//      provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY <COPYRIGHT HOLDER> ``AS IS'' AND ANY EXPRESS OR IMPLIED
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL Chris Dibbs OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
// ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
// ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// The views and conclusions contained in the software and documentation are those of the
// authors and should not be interpreted as representing official policies, either expressed
// or implied, of Chris Dibbs.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace SPPrimitives.SettingsProvider {
    internal class SettingsConfigurationStore : IDisposable {
        Database database;

        public SettingsConfigurationStore() {
            database = new Database();
        }

        ~SettingsConfigurationStore() { Dispose(false); }

        public void Dispose() { Dispose(true);  }

        public virtual void Dispose(bool disposing) {
            if (disposing && database != null)
                database.Dispose();
        }

        public Dictionary<string, SettingDatum> GetSectionValues(string sectionName) {
            var settingsData = from section in database.Sections
                               join setting in database.ApplcationSettings on section.Id equals setting.SectionId
                               where section.Name == sectionName
                               select new {
                                   setting.Name,
                                   setting.SerializeAs,
                                   setting.Value
                               };

            return settingsData.ToDictionary(
                datum => datum.Name, 
                datum => new SettingDatum((SettingsSerializeAs)datum.SerializeAs, datum.Value)
            );
        }
    }
}
