using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace fitnes3.Services
{
    public class TimerData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string ExerciseType { get; set; }
        public DateTime StartTime { get; set; }
        public bool TimerRunning { get; set; }
        public TimeSpan RemainingTime { get; set; }
    }
}
