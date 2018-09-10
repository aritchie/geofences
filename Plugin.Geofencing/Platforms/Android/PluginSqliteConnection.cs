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
#if __ANDROID__
                    Environment.GetFolderPath(Environment.SpecialFolder.Personal),
#elif WINDOWS_UWP
                    Windows.Storage.ApplicationData.Current.LocalFolder.Path,
#endif
                    "acrgeofences.db"
                ),
                true
            ),
            SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.ReadWrite)
        {
            this.CreateTable<DbGeofenceRegion>();
            this.GeofenceRegions = this.Table<DbGeofenceRegion>();
        }


        public TableQuery<DbGeofenceRegion> GeofenceRegions { get; }
    }
}
