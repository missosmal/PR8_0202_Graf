using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace PR8.Classes
{
    public class WeatherCache
    {
        private const string DbPath = "weatherCache.db";

        public static void InitializeDatabase()
        {
            if (!System.IO.File.Exists(DbPath))
            {
                SQLiteConnection.CreateFile(DbPath);
            }

            using (var connection = new SQLiteConnection($"Data Source={DbPath};Version=3;"))
            {
                connection.Open();
                string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS WeatherData (
                    City TEXT NOT NULL,
                    DateTime TEXT NOT NULL,
                    Temperature TEXT,
                    Pressure TEXT,
                    Humidity TEXT,
                    WindSpeed TEXT,
                    FeelsLike TEXT,
                    WeatherDescription TEXT,
                    RequestDate TEXT NOT NULL
                )";

                var command = new SQLiteCommand(createTableQuery, connection);
                command.ExecuteNonQuery();
            }
        }

        public static void SaveWeatherData(string city, string dateTime, string temperature, string pressure, string humidity, string windSpeed, string feelsLike, string weatherDescription)
        {
            using (var connection = new SQLiteConnection($"Data Source={DbPath};Version=3;"))
            {
                connection.Open();
                string insertQuery = @"
                INSERT INTO WeatherData (City, DateTime, Temperature, Pressure, Humidity, WindSpeed, FeelsLike, WeatherDescription, RequestDate)
                VALUES (@City, @DateTime, @Temperature, @Pressure, @Humidity, @WindSpeed, @FeelsLike, @WeatherDescription, @RequestDate)";

                var command = new SQLiteCommand(insertQuery, connection);
                command.Parameters.AddWithValue("@City", city);
                command.Parameters.AddWithValue("@DateTime", dateTime);
                command.Parameters.AddWithValue("@Temperature", temperature);
                command.Parameters.AddWithValue("@Pressure", pressure);
                command.Parameters.AddWithValue("@Humidity", humidity);
                command.Parameters.AddWithValue("@WindSpeed", windSpeed);
                command.Parameters.AddWithValue("@FeelsLike", feelsLike);
                command.Parameters.AddWithValue("@WeatherDescription", weatherDescription);
                command.Parameters.AddWithValue("@RequestDate", DateTime.Now.ToString("yyyy-MM-dd"));

                command.ExecuteNonQuery();
            }
        }

        public static List<WeatherData> GetWeatherData(string city)
        {
            List<WeatherData> weatherDataList = new List<WeatherData>();

            using (var connection = new SQLiteConnection($"Data Source={DbPath};Version=3;"))
            {
                connection.Open();
                string selectQuery = @"
                SELECT * FROM WeatherData
                WHERE City = @City AND RequestDate = @RequestDate";

                var command = new SQLiteCommand(selectQuery, connection);
                command.Parameters.AddWithValue("@City", city);
                command.Parameters.AddWithValue("@RequestDate", DateTime.Now.ToString("yyyy-MM-dd"));

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        weatherDataList.Add(new WeatherData
                        {
                            DateTime = reader["DateTime"].ToString(),
                            Temperature = reader["Temperature"].ToString(),
                            Pressure = reader["Pressure"].ToString(),
                            Humidity = reader["Humidity"].ToString(),
                            WindSpeed = reader["WindSpeed"].ToString(),
                            FeelsLike = reader["FeelsLike"].ToString(),
                            WeatherDescription = reader["WeatherDescription"].ToString()
                        });
                    }
                }
            }
            return weatherDataList;
        }

        public static int GetRequestCountForToday()
        {
            int requestCount = 0;

            using (var connection = new SQLiteConnection($"Data Source={DbPath};Version=3;"))
            {
                connection.Open();
                string selectQuery = "SELECT COUNT(*) FROM WeatherData WHERE RequestDate = @RequestDate";
                var command = new SQLiteCommand(selectQuery, connection);
                command.Parameters.AddWithValue("@RequestDate", DateTime.Now.ToString("yyyy-MM-dd"));

                requestCount = Convert.ToInt32(command.ExecuteScalar());
            }

            return requestCount;
        }
    }
}