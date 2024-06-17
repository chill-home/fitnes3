using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace fitnes3.Services
{
    public class DataPoint
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int Month { get; set; }
        public double Weight { get; set; }
    }
}
