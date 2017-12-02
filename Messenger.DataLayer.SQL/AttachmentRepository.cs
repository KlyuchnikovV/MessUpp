using System;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using Messenger.Model;
using NLog;

namespace Messenger.DataLayer.SQL
{
    /// <summary>
    ///     Реализация интерфейса для методов работы с таблицей "Вложения".
    /// </summary>
    [SuppressMessage("ReSharper", "InheritdocConsiderUsage")]
    public class AttachmentRepository : IAttachmentRepository
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly string _connectionString;

        /// <summary>
        ///     Инициализация строки подключения для работы с таблицей "Вложения".
        /// </summary>
        /// <param name="connectionString">Строка подключения.</param>
        public AttachmentRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <inheritdoc />
        public Attachment LoadAttachment(Attachment file)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                Logger.Debug("Загрузка файла в базу данных...");
                try
                {
                    connection.Open();
                }
                catch (SqlException exception)
                {
                    Logger.Error($"Не могу подключиться к БД, {exception.Message}");
                    throw;
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT AttachId FROM Attachments WHERE Data = @Data AND Type = @Type";
                    command.Parameters.AddWithValue("@Data", file.Data);
                    command.Parameters.AddWithValue("@Type", file.Type);
                    Logger.Info("Проверка на наличие данного файла в базе...");
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException exception)
                    {
                        Logger.Error(exception.Message);
                        throw;
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            return new Attachment
                            {
                                AttachId = reader.GetGuid(reader.GetOrdinal("AttachId")),
                                Data = file.Data
                            };
                    }
                    Logger.Info("Файла нет в базе, создаем новый...");
                    file.AttachId = Guid.NewGuid();
                    command.CommandText =
                        "INSERT INTO Attachments (AttachId, Data, Type) VALUES (@AttachId, @Data, @Type)";
                    command.Parameters.AddWithValue("@AttachId", file.AttachId);
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException exception)
                    {
                        Logger.Error(exception.Message);
                        throw;
                    }
                    Logger.Info($"Готово ИД файла {file.AttachId}.");
                    return file;
                }
            }
        }

        /// <inheritdoc />
        public Attachment GetAttachment(Guid id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                Logger.Debug("Поиск файла...");
                try
                {
                    connection.Open();
                }
                catch (SqlException exception)
                {
                    Logger.Error($"Не могу подключиться к БД, {exception.Message}");
                    throw;
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT Data FROM Attachments WHERE AttachId = @AttachId";
                    command.Parameters.AddWithValue("@AttachId", id);
                    Logger.Info($"Поиск файла с ИД {id}...");
                    try
                    {
                        var data = command.ExecuteScalar() as string;
                        return new Attachment
                        {
                            AttachId = id,
                            Data = data
                        };
                    }
                    catch (SqlException exception)
                    {
                        Logger.Error(exception.Message);
                        throw;
                    }
                }
            }
        }

        /// <inheritdoc />
        public void DeleteAttachment(Guid id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                Logger.Debug("Поиск файла...");
                try
                {
                    connection.Open();
                }
                catch (SqlException exception)
                {
                    Logger.Error($"Не могу подключиться к БД, {exception.Message}");
                    throw;
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM Attachments WHERE AttachId = @AttachId";
                    command.Parameters.AddWithValue("@AttachId", id);
                    Logger.Info($"Удаление файла с ИД {id}...");
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException exception)
                    {
                        Logger.Error(exception.Message);
                        throw;
                    }
                    Logger.Info($"Файл с ИД {id} удален.");
                }
            }
        }
    }
}