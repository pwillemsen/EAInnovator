using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Aras.IOM;

namespace EAInnovator.Services
{
    /// <summary>
    /// Innovator services for connection, data mapping and item handling
    /// </summary>
    public sealed class InnovatorService
    {

        private string serverURL = "";
        private string database = "";
        private string username = "";
        private string password = "";

        private static volatile InnovatorService instance;
        private static object syncRoot = new Object();

        private HttpServerConnection connection = null;
        private Innovator innovator = null;

        private string lastError = "";

        public static InnovatorService _
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new InnovatorService();
                    }
                }

                return instance;
            }
        }

        public string ServerURL { get { return serverURL; } }
        public string Database { get { return database; } }
        public string Username { get { return username; } }

        public string LastError { get { return lastError; } }
        public bool IsLoggedIn { get { return innovator != null; } }

        public static string[] GetDatabases(string serverURL)
        {
            HttpServerConnection conn = IomFactory.CreateHttpServerConnection(serverURL);
            return conn.GetDatabases();
        }

        private InnovatorService()
        {
        }

        public bool Login(string serverURL, string database, string username, string password)
        {
            this.serverURL = serverURL;
            this.database = database;
            this.username = username;
            this.password = password;

            connection = IomFactory.CreateHttpServerConnection(serverURL, database, username, password);
            lastError = "";

            Item rv = connection.Login();

            if (SetError(rv))
            {
                connection = null;
                return false;
            }

            innovator = new Innovator(connection);
            return true;
        }

        public void Logout()
        {
            this.password = "";
            connection.Logout();
            innovator = null;
            connection = null;
        }

        private bool SetError(Item error)
        {
            if (error.isError())
            {
                lastError = string.Format(@"{0}", error.getErrorString());
            }
            else
                lastError = "";

            return error.isError();
        }

        public Item GetItemByGUID(string itemType, string guid)
        {
            Item req = innovator.newItem(itemType, "get");
            req.setProperty("external_id", guid);
            return req.apply();
        }

        public Item GetItemByID(string itemType, string id)
        {
            Item req = innovator.newItem(itemType, "get");
            req.setID(id);
            return req.apply();
        }

        public Item GetItemTypes()
        {
            // Get all ItemTypes in Aras Innovator
            Item plmItemTypes = innovator.newItem("ItemType", "get");
            plmItemTypes.setProperty("is_relationship", "0");
            plmItemTypes = plmItemTypes.apply();
            return plmItemTypes;
        }

        public Item GetRelationshipTypes()
        {
            // Get all ItemTypes in Aras Innovator
            Item plmRelationshipTypes = innovator.newItem("RelationshipType", "get");
            plmRelationshipTypes = plmRelationshipTypes.apply();
            return plmRelationshipTypes;
        }

    }
}