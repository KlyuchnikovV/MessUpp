using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messenger.Model;
using System.Data.SqlClient;

namespace Messenger.DataLayer.SQL
{
    public class ProfilesRepository : IProfilesRepository
    {
        private readonly string connectionString;

        public ProfilesRepository(string _connectionString)
        {
            connectionString = _connectionString;
        }

        public Profile CreateProfile(Profile newProfile)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO Profiles (Id, Login, Password, Name, Surname, Avatar) values (@Id, @Login, @Password, @Name, @Surname, @Avatar)";
                    command.Parameters.AddWithValue("@Id", newProfile.Id);
                    command.Parameters.AddWithValue("@Login", newProfile.Login);
                    command.Parameters.AddWithValue("@Password", newProfile.Password);
                    command.Parameters.AddWithValue("@Name", newProfile.Name);
                    command.Parameters.AddWithValue("@Surname", newProfile.Surname);
                    command.Parameters.AddWithValue("@Avatar", newProfile.Avatar);

                    command.ExecuteNonQuery();
                    return newProfile;
                }
            }
        }

        public Profile ChangeProfileData(Profile newData)
        {
            DeleteProfile(newData.Id);
            return CreateProfile(newData); 
        }

        public Profile GetProfile(Guid id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT TOP(1) Id, Login, Password, Name, Surname, Avatar FROM Profiles WHERE Id = @Id";
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new ArgumentException($"Пользователь с id {id} не найден");

                        return new Profile
                        {
                            Id = reader.GetGuid(reader.GetOrdinal("Id")),
                            Login = reader.GetString(reader.GetOrdinal("Login")),
                            Password = reader.GetString(reader.GetOrdinal("Password")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Surname = reader.GetString(reader.GetOrdinal("Surname")),
                            Avatar = reader.GetSqlBinary(reader.GetOrdinal("Avatar")).Value,
                        };
                    }
                }
            }
        }

        // Find by name etc

        public void DeleteProfile(Guid id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM ChatMembers WHERE ProfileId = @Id";
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM Profiles WHERE Id = @Id";
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
