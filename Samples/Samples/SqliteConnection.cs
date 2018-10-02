using System;
using System.IO;
using Acr.IO;
using SQLite;


namespace Samples
{
    public class SqliteConnection : SQLiteConnection
    {
        public SqliteConnection() : base(
            Path.Combine(FileSystem.Current.AppData.FullName, "geofences.db"),
            SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite,
            true
        )
        {
            this.CreateTable<GeofenceEvent>();
        }


        public TableQuery<GeofenceEvent> GeofenceEvents => this.Table<GeofenceEvent>();
    }
}
