﻿using System;
using SQLite;


namespace Samples
{
    public class GeofenceEvent
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public bool Entered { get; set; }
        public string Identifier { get; set; }
        public DateTime Date { get; set; }


        public string Text => this.Identifier;
        public string Detail => this.Entered
            ? $"Entered on {this.Date:MMM d a\t h:mm tt}"
            : $"Exited on {this.Date:MMM d a\t h:mm tt}";
    }
}
