using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DoubleA.Models;
using SQLite;

namespace DoubleA.Data
{
    public class SettingsDB
    {
        readonly SQLiteAsyncConnection database;

        public SettingsDB(String dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<UserSettings>().Wait();

            if (database.Table<UserSettings>().CountAsync().Result == 0)
            {
                UserSettings defaultSettings = new UserSettings();
                defaultSettings.Id = 1;
                defaultSettings.DefaultListSource = "MAL";
                database.InsertAsync(defaultSettings).Wait();
            }
        }

        public Task<UserSettings> GetSettingsAsync()
        {
            return database.Table<UserSettings>().Where(settings => settings.Id == 1).FirstOrDefaultAsync();
        }

        public Task<int> UpdateSettingsAsync(UserSettings settings)
        {
            return database.UpdateAsync(settings);
        }

        public Task<int> DeleteSettingsAsync()
        {
            return database.DropTableAsync<UserSettings>();
        }
    }
}
