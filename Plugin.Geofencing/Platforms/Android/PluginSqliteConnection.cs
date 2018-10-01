using System;
using System.IO;
using SQLite;

namespace Plugin.Geofencing.Platforms.Android
{
    public class PluginSqliteConnection : SQLiteConnectionWithLock
    {
        public PluginSqliteConnection() : base(
            new SQLiteConnectionString(
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                    "acrgeofences.db"
                ),
                true,
                null
            ),
            SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.ReadWrite)
        {
            this.CreateTable<DbGeofenceRegion>();
            this.GeofenceRegions = this.Table<DbGeofenceRegion>();
        }


        public TableQuery<DbGeofenceRegion> GeofenceRegions { get; }
    }
}
