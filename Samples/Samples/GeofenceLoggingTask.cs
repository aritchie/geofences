using System;
using Autofac;
using Plugin.Geofencing;


namespace Samples
{
    public class GeofenceLoggingTask : IStartable
    {
        readonly SqliteConnection conn;
        readonly IGeofenceManager geofences;


        public GeofenceLoggingTask(SqliteConnection conn, IGeofenceManager geofences)
        {
            this.conn = conn;
            this.geofences = geofences;
        }


        public void Start()
        {
            try
            {
                this.geofences.RegionStatusChanged += (sender, args) => this.conn.Insert(new GeofenceEvent
                {
                    Identifier = args.Region.Identifier,
                    Entered = args.Status == GeofenceStatus.Entered,
                    Date = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
