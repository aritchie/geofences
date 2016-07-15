using System;
using System.IO;
using Samples.Models;
using SQLite.Net;
using SQLite.Net.Interop;


namespace Samples
{
    public class SampleDbConnection : SQLiteConnectionWithLock
    {
        public SampleDbConnection(ISQLitePlatform sqlitePlatform, string path)
            : base(sqlitePlatform, new SQLiteConnectionString(Path.Combine(path, "Samples.db"), true))
        {
            this.CreateTable<GeofenceEvent>();
        }


        public TableQuery<GeofenceEvent> GeofenceEvents => this.Table<GeofenceEvent>();
    }
}
