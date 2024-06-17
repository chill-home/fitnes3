using CloudKit;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace fitnes3.Services
{
    public class Database
    {
        private readonly SQLiteAsyncConnection _database;

        public Database(string dbPath)
        {
            if (string.IsNullOrEmpty(dbPath))
                throw new ArgumentNullException(nameof(dbPath), "Path cannot be null");

            _database = new SQLiteAsyncConnection(dbPath);
            

            // Создаем все таблицы на раз
            Task.WhenAll(
                _database.CreateTableAsync<TimerData>(),
                _database.CreateTableAsync<DataPoint>(),
                _database.CreateTableAsync<User>(),
                _database.CreateTableAsync<WaterIntake>(),
                _database.CreateTableAsync<DailyCalories>(),
                _database.CreateTableAsync<PreviousWaterIntake>()
            ).Wait();
        }

        public Task<int> SaveUserAsync(User user)
        {
            return _database.InsertAsync(user);
        }

        public Task<User> GetUserAsync(string username, string password)
        {
            return _database.Table<User>().FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
        }

        public Task<List<DataPoint>> GetDataPointsAsync()
        {
            return _database.Table<DataPoint>().ToListAsync();
        }

        public Task<int> SaveDataPointAsync(DataPoint dataPoint)
        {
            if (dataPoint.Id != 0)
            {
                return _database.UpdateAsync(dataPoint);
            }
            else
            {
                return _database.InsertAsync(dataPoint);
            }
        }

        public Task<int> DeleteDataPointAsync(DataPoint dataPoint)
        {
            return _database.DeleteAsync(dataPoint);
        }

        public Task<List<TimerData>> GetTimerDataAsync()
        {
            return _database.Table<TimerData>().ToListAsync();
        }

        public Task<int> SaveTimerDataAsync(TimerData timerData)
        {
            if (timerData.Id != 0)
            {
                return _database.UpdateAsync(timerData);
            }
            else
            {
                return _database.InsertAsync(timerData);
            }
        }

        public Task<int> DeleteTimerDataAsync(TimerData timerData)
        {
            return _database.DeleteAsync(timerData);
        }
        public Task<List<DailyCalories>> GetDailyCaloriesAsync()
        {
            return _database.Table<DailyCalories>().ToListAsync();
        }

        public Task<int> SaveDailyCaloriesAsync(DailyCalories dailyCalories)
        {
            return _database.InsertAsync(dailyCalories);
        }

        public Task<int> UpdateDailyCaloriesAsync(DailyCalories dailyCalories)
        {
            return _database.UpdateAsync(dailyCalories);
        }

        public Task<int> DeleteDailyCaloriesAsync(DailyCalories dailyCalories)
        {
            return _database.DeleteAsync(dailyCalories);
        }
        public Task<List<WaterIntake>> GetWaterIntakesAsync()
        {
            return _database.Table<WaterIntake>().ToListAsync();
        }

        public Task<int> SaveWaterIntakeAsync(WaterIntake intake)
        {
            return _database.InsertAsync(intake);
        }
        public Task<int> DeleteAllWaterIntakesAsync()
        {
            return _database.DeleteAllAsync<WaterIntake>();
        }
        public Task<int> SavePreviousWaterIntakeAsync(PreviousWaterIntake previousIntake)
        {
            return _database.InsertAsync(previousIntake);
        }
    }
}