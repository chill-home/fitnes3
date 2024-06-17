using System;
using System.Collections.Generic;
using System.Text;

namespace fitnes3.Services
{
    public class WaterIntakeService   
    {
        private readonly Database _database;
        public WaterIntakeService(Database database)
        {
            _database = database;
        }

    }
}
