using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Autodesk.Connectivity.WebServices;
using Autodesk.Connectivity.WebServicesTools;
using VDF = Autodesk.DataManagement.Client.Framework;

namespace ZSharpVault15lib
{
    public class GenHelper
    {
        private static VDF.Vault.Currency.Connections.Connection connection;
        public static VDF.Vault.Currency.Connections.Connection getVaultConnection(string server, string vault, string uName, string pass)
        {
            VDF.Vault.Results.LogInResult results = VDF.Vault.Library.ConnectionManager.LogIn(server, vault, uName, pass, VDF.Vault.Currency.Connections.AuthenticationFlags.Standard, null);
            connection = results.Connection;
            return connection;

        }

        public VDF.Vault.Currency.Connections.Connection getpsVaultConn(string server, string vault, string uName, string pass)
        {
            VDF.Vault.Results.LogInResult results = VDF.Vault.Library.ConnectionManager.LogIn(server, vault, uName, pass, VDF.Vault.Currency.Connections.AuthenticationFlags.Standard, null);
            connection = results.Connection;
            return connection;
        }
    }
}
