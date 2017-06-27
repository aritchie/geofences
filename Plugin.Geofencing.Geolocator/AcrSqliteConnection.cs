using System;
using System.IO;
using SQLite;


namespace Plugin.Geofencing
{
    public class AcrSqliteConnection : SQLiteConnectionWithLock
    {
        public AcrSqliteConnection() :
            base(new SQLiteConnectionString("geofences.db", true),
            SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.ReadWrite)
        {
            this.CreateTable<DbGeofenceRegion>();
        }


        public TableQuery<DbGeofenceRegion> GeofenceRegions => this.Table<DbGeofenceRegion>();
    }
}