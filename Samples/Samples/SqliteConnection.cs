using System;
using SQLite;


namespace Samples
{
    public class SqliteConnection : SQLiteConnection
    {
        public SqliteConnection(string databasePath, bool storeDateTimeAsTicks = true) : base(databasePath, storeDateTimeAsTicks)
        {
            this.CreateTable<GeofenceEvent>();
        }


        public SqliteConnection(string databasePath, SQLiteOpenFlags openFlags, bool storeDateTimeAsTicks = true) : base(databasePath, openFlags, storeDateTimeAsTicks)
        {
        }


        public TableQuery<GeofenceEvent> GeofenceEvents => this.Table<GeofenceEvent>();
    }
}
