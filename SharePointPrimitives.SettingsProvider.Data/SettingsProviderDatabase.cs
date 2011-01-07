using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;

namespace SharePointPrimitives.SettingsProvider {
    public partial class SettingsProviderDatabase {
        public void ReloadFromAuditLog(DateTime time) {
            using (DbConnection conn = this.Connection) {
                DbCommand cmd = conn.CreateCommand();
                //command text is of the format of 'ContainerName.Stored Proc Name' 
                cmd.CommandText = String.Format("{0}.ReloadFromAuditLogs", this.DefaultContainerName);
                
                var param = cmd.CreateParameter();
                param.ParameterName = "pointInTime";
                param.Value = time;
                param.Direction = ParameterDirection.Input;

                cmd.Parameters.Add(param);

                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                if (this.Connection.State != System.Data.ConnectionState.Open)
                    this.Connection.Open();


                var reader = cmd.ExecuteNonQuery();
            }
        }
    }
}
