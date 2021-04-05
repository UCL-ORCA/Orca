
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using Orca.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orca.Entities;
using Microsoft.Extensions.Hosting;
using System.IO;
using Npgsql;

namespace Orca.Database
{
    public class DatabaseConnect : IDisposable
    {
        private string _servername;
        private string _username;
        private string _password;
        private string _database;
        private bool _hasDatabase = false;
        public bool _disposedValue;
        public NpgsqlConnection Connection { get; set; }

        public DatabaseConnect(IOptions<DatabaseFields> settings)
        {
            var fields = settings.Value;
            _servername = fields.Servername;
            _username = fields.Username;
            _password = fields.Password;
            _database = fields.Database;
            _hasDatabase = HasDatabase(fields);

            if (_hasDatabase)
            {
                string connString = $"Host={_servername}; Database={_database}; User Id={_username}; Password= {_password}; SSL Mode=Prefer;";
                Connection = new NpgsqlConnection(connString);
                Connection.Open();
            }
        }

        public static bool HasDatabase(DatabaseFields fields)
        {
            return fields.Servername != null && fields.Username != null && fields.Password != null && fields.Database != null;
        }

        public static void CreateDatabase(DatabaseFields settings, IHostEnvironment hostingEnvironment)
        {
            var servername = settings.Servername;
            var database = settings.Database;
            var username = settings.Username;
            var password = settings.Password;
            var pathToTableCreationScript = Path.Combine(hostingEnvironment.ContentRootPath, "Database", "OrcaDatabase.sql");
            var tableCreationScript = File.ReadAllText(pathToTableCreationScript);
            // set Database to postgres in case it hasn't been created yet
            using (var conn = new NpgsqlConnection($"Server = {servername}; Database = postgres; User Id= {username}; Password= {password}; Ssl Mode=Prefer;"))
            {

                conn.Open();
                using (var checkDbExistsCommand = new NpgsqlCommand($"SELECT COUNT(datname) FROM pg_catalog.pg_database WHERE datname = '{database}'", conn))
                {
                    bool dbExists = ((long)checkDbExistsCommand.ExecuteScalar()) > 0;
                    if (!dbExists)
                    {
                        using (var dbCreationCommand = new NpgsqlCommand($"CREATE DATABASE {database} ENCODING = 'UTF8'", conn))
                        {
                            dbCreationCommand.ExecuteNonQuery();
                        }
                    }
                }

            }
            // create DB tables
            using (var conn = new NpgsqlConnection($"Server = {servername}; Database = {database}; User Id= {username}; Password= {password}; Ssl Mode=Prefer;"))
            {
                conn.Open();
                using (var sqlScript = new NpgsqlCommand(tableCreationScript, conn))
                {
                    sqlScript.ExecuteNonQuery();
                }

            }
        }


        public async Task StoreEventToDatabase(StudentEvent studentEvent)
        {
            string sql = "INSERT INTO event(student_ID, course_ID, Timestamp, event_type, activity_name, activity_details) VALUES(@student_ID,@course_ID, @Timestamp, @event_type, @activity_name, @activity_details)";
            NpgsqlParameter[] parameters = {
                                            new NpgsqlParameter("@student_ID", studentEvent.Student.ID),
                                            new NpgsqlParameter("@course_ID", studentEvent.CourseID),
                                            new NpgsqlParameter("@Timestamp", studentEvent.Timestamp),
                                            new NpgsqlParameter("@event_type", studentEvent.EventType.ToString()),
                                            new NpgsqlParameter("@activity_name", studentEvent.ActivityName),
                                            new NpgsqlParameter("@activity_details", studentEvent.ActivityType)
                                        };

            await AddInfoToDatabase(sql, parameters);


        }

        public async Task StoreStudentToDatabase(StudentEvent studentEvent)
        {

            string sql = "INSERT INTO student(id, first_name, last_name,email) VALUES(@ID, @first_name, @last_name,@studentEmail) ON CONFLICT DO NOTHING";
            NpgsqlParameter[] parameters = {
                                                        new NpgsqlParameter("@ID", studentEvent.Student.ID),
                                                        new NpgsqlParameter("@first_name", studentEvent.Student.FirstName),
                                                        new NpgsqlParameter("@last_name", studentEvent.Student.LastName),
                                                        new NpgsqlParameter("@studentEmail", studentEvent.Student.Email)
                                                    };

            await AddInfoToDatabase(sql, parameters);

        }

        public async Task AddInfoToDatabase(string sql, NpgsqlParameter[] parameters)
        {
            if (_hasDatabase)
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, Connection))
                {
                    cmd.Parameters.AddRange(parameters);
                    await cmd.ExecuteScalarAsync();
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {

            if (!_disposedValue)
            {
                if (disposing)
                {
                    Connection?.Dispose();
                }
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
        }
    }
}
