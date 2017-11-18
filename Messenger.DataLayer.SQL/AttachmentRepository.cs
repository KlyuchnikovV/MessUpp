using System;
using Messenger.Model;
using NLog;
using System.Data.SqlClient;

namespace Messenger.DataLayer.SQL
{
    public class AttachmentRepository : IAttachmentRepository
    {
        private readonly string connectionString;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public AttachmentRepository(string _connectionString)
        {
            connectionString = _connectionString;
        }

        // Загрузка файла в базу данных. //
        public Attachment LoadAttachment(Attachment file)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                logger.Debug($"Загрузка файла в базу данных...");
                try
                {
                    connection.Open();
                }
                catch (SqlException exception)
                {
                    logger.Error($"Не могу подключиться к БД, {exception.Message}");
                    throw exception;
                }
                
                using (var command = connection.CreateCommand())
                {
                    logger.Info($"Проверка на наличие данного файла в базе...");
                    command.CommandText = "SELECT AttachId FROM Attachments WHERE Data = @Data And Type = @Type";
                    command.Parameters.AddWithValue("@Data", file.Data);
                    command.Parameters.AddWithValue("@Type", file.Type);
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException exception)
                    {
                        logger.Error(exception.Message);
                        throw exception;
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        // Если что-то есть то он прочитает и вернет. // 
                        if(reader.Read())
                            return new Attachment
                            {
                                AttachId = reader.GetGuid(reader.GetOrdinal("AttachId")),
                                Data = file.Data
                            };
                    }
                    logger.Info($"Файла нет в базе, создаем новый...");
                    file.AttachId = Guid.NewGuid();
                    command.CommandText = "INSERT INTO Attachments (AttachId, Data, Type) VALUES (@AttachId, @Data, @Type)";
                    command.Parameters.AddWithValue("@AttachId", file.AttachId);
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException exception)
                    {
                        logger.Error(exception.Message);
                        throw exception;
                    }
                    logger.Info($"Готово ИД файла {file.AttachId}.");
                    return file;
                }

            }
        }

        // Получение файла по ИД. // 
        public Attachment GetAttachment(Guid id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                logger.Debug($"Поиск файла...");
                try
                {
                    connection.Open();
                }
                catch (SqlException exception)
                {
                    logger.Error($"Не могу подключиться к БД, {exception.Message}");
                    throw exception;
                }

                using (var command = connection.CreateCommand())
                {
                    logger.Info($"Поиск файла с ИД {id}...");
                    command.CommandText = "SELECT Data FROM Attachments WHERE AttachId = @AttachId";
                    command.Parameters.AddWithValue("@AttachId", id);
                    try
                    {
                        string data = command.ExecuteScalar() as string;
                        return new Attachment
                        {
                            AttachId = id,
                            Data = data
                        };
                    }
                    catch (SqlException exception)
                    {
                        logger.Error(exception.Message);
                        throw exception;
                    }
                }
            }
        }

        // Удаление файла по ИД. //
        public void DeleteAttachment(Guid id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                logger.Debug($"Поиск файла...");
                try
                {
                    connection.Open();
                }
                catch (SqlException exception)
                {
                    logger.Error($"Не могу подключиться к БД, {exception.Message}");
                    throw exception;
                }

                using (var command = connection.CreateCommand())
                {
                    logger.Info($"Удаление файла с ИД {id}...");
                    command.CommandText = "DELETE FROM Attachments WHERE AttachId = @AttachId";
                    command.Parameters.AddWithValue("@AttachId", id);
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException exception)
                    {
                        logger.Error(exception.Message);
                        throw exception;
                    }
                    logger.Info($"Файл с ИД {id} удален.");
                }
            }
        }  
    }
}
