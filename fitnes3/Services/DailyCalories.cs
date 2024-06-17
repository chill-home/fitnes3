using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace fitnes3.Services
{
    public class DailyCalories
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; } // Добавление первичного ключа для базы данных
        public string DayOfWeek { get; set; }
        public int CaloriesBurned { get; set; }
    }
}
