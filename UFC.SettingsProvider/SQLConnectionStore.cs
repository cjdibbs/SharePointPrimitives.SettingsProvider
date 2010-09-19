using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UFC.SettingsProviders {
    interface ISQLConnectionStringStore {
        string GetByName(string name);
        string GetByCatalog(string catalog);
        string Search(string name, string catalog);
    }
}
