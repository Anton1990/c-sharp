
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HelloApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //Microsoft Identification
            //string connectionString = @"Data Source=NTB-054\SQLEXPRESS;Database=adonetdb;" +
            // "Integrated Security=SSPI;Pooling=False";

            ///SQL Identification
            string connectionString = @"Data Source=NTB-054\SQLEXPRESS;Database=adonetdb; User Id = Anton; Password = 777rs4ka1;";
            string sqlExpression = "INSERT INTO Users (Name, Age) VALUES ('Tom', 18)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                int number = command.ExecuteNonQuery();
                Console.WriteLine("Добавлено объектов: {0}", number);
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                sqlExpression = "SELECT * FROM Users";
                await connection.OpenAsync();

                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = await command.ExecuteReaderAsync();

                if (reader.HasRows) // если есть данные
                {
                    // выводим названия столбцов
                    string columnName1 = reader.GetName(0);
                    string columnName2 = reader.GetName(1);
                    string columnName3 = reader.GetName(2);

                    Console.WriteLine($"{columnName1}\t{columnName3}\t{columnName2}");

                    while (await reader.ReadAsync()) // построчно считываем данные
                    {
                        object id = reader.GetValue(0);
                        object name = reader.GetValue(2);
                        object age = reader.GetValue(1);

                        Console.WriteLine($"{id} \t{name} \t{age}");
                    }
                }

                await reader.CloseAsync();

            }
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                SqlCommand command = new SqlCommand();
                command.CommandText = "CREATE TABLE Cars (Id INT PRIMARY KEY IDENTITY, Age INT NOT NULL, Name NVARCHAR(100) NOT NULL)";
                command.Connection = connection;
                await command.ExecuteNonQueryAsync();
                 
                Console.WriteLine("Таблица Users создана");
            }
            Console.Read();
        }
    }
}

                
            




