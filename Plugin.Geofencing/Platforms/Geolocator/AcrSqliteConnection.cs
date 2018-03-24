using System;
using System.IO;
using SQLite;


namespace Plugin.Geofencing
{
    public class AcrSqliteConnection : SQLiteConnectionWithLock
    {
#if __ANDROID__
        public AcrSqliteConnection() :
            base(new SQLiteConnectionString(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "geofences.db"),
                true
            ), SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.ReadWrite)
        {
            this.CreateTable<DbGeofenceRegion>();
        }
#else
        public AcrSqliteConnection() :
            base(new SQLiteConnectionString(
                "geofences.db",
                true
            ), SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.ReadWrite)
        {
            this.CreateTable<DbGeofenceRegion>();
        }
#endif
        public TableQuery<DbGeofenceRegion> GeofenceRegions => this.Table<DbGeofenceRegion>();
    }
}