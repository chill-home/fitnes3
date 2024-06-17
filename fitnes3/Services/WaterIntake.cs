using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace fitnes3.Services
{
    public class WaterIntake
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Amount { get; set; }
        public DateTime TimeAdded { get; set; }
        public int UserId { get; set; }
    }
}
