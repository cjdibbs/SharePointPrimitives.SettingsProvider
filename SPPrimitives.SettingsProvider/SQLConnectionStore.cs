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

namespace SPPrimitives.SettingsProvider {
    class SQLConnectionStringStore : IDisposable {
        Database database;

        public SQLConnectionStringStore() {
            database = new Database();
        }

        ~SQLConnectionStringStore() { Dispose(false); }

        public void Dispose() { Dispose(true);  }

        public virtual void Dispose(bool disposing) {
            if (disposing && database != null)
                database.Dispose();
        }

        /// <summary>
        /// Get a database connection string searching by the setting name. Returns null if it is not found
        /// </summary>
        /// <param name="name">Name of the setting to look for</param>
        /// <returns>Conneciton string or null if none is stored</returns>
        public string GetByName(string name) {
            if (name == null)
                return null;

            var sql = from connectionName in database.SqlConnectionNames
                      join connection in database.SqlConnectionStrings on connectionName.SqlConnectionId equals connection.Id
                      where connectionName.Name == name
                      select connection.ConnectionString;

            return sql.FirstOrDefault();
        }

        /// <summary>
        /// Gets a database connection string searching by the catalog of the database, this will grab the first conneciton
        /// string in the database should only be used if you know there is only one
        /// </summary>
        /// <param name="catalog">Catalog of the database</param>
        /// <returns>Conneciton string or null if none is stored</returns>
        public string GetByCatalog(string catalog) {
            if (catalog == null)
                return null;

            return database.SqlConnectionStrings.Where(row => row.Catalog == catalog)
                 .Select(row => row.ConnectionString)
                 .FirstOrDefault();
        }

        /// <summary>
        /// Searchs by name then by catalog reutrning null if both fail.
        /// </summary>
        /// <param name="name">Setting name to look up</param>
        /// <param name="catalog">Catalog of the database to search for</param>
        /// <returns>Conneciton string or null if none is stored</returns>
        public string Search(string name, string catalog) {
            return GetByName(name) ?? GetByCatalog(catalog);
        }
    }
}

