using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace WcfServer {
    /// <summary>
    /// Класс для работы в БД (singleton)
    /// </summary>
    class DBController {
        private static DBController _instance;
        private DBController() { }
        public static DBController GetInstance(){
            if (_instance == null)
                _instance = new DBController();
            
            return _instance;
        }
        // Строка подключения
        SqlConnection connection = new SqlConnection(
            @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename=|DataDirectory|\UsersDB.mdf;Integrated Security = True; Connect Timeout = 30"
        );
        /// <summary>
        /// Поиск в БД хэша пароля пользователя с указаным именем
        /// </summary>
        /// <param name="user_name">имя пользователя</param>
        /// <returns>хэш пароля</returns>
        public string ReadUserData(string user_name) {
            // если найдено, возвращает хеш пароля пользователя
            // если нет - пустую строку
            string result = "";
            try {
                connection.Open();
                if (connection?.State == System.Data.ConnectionState.Open) {
                    SqlCommand command = new SqlCommand(
                        String.Format(
                            "Select Hash from dbo.Users where Name='{0}'",
                            user_name
                        ),
                        connection);
                    using (SqlDataReader reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            result = reader.GetValue(0).ToString();
                        }
                    }
                    connection.Close();
                }
            } catch (SqlException ex) {
                Console.WriteLine("Error:: database connection error:\n {0}", ex.Message);
            }

            return result;
        }
        /// <summary>
        /// Заполнение БД строками для проверки функционирования
        /// </summary>
        public void PopulateUserData() {
            // Имя-Пароль генерируется по принципу
            // user1 - user11
            // user2 - user22
            // userX - userXX
            // из пароля высчитывается хэш и строка вставляется в БД
            try {
                connection.Open();
                if (connection?.State == System.Data.ConnectionState.Open) {
                    for (int i = 1; i < 5; i++) {
                        var name = "user" + i.ToString();
                        var hash = GetHash(name + i.ToString());
                        var l = hash.Length;
                        SqlCommand command = new SqlCommand(
                            String.Format(
                                "Insert into dbo.Users (Name, Hash) Values ('{0}', '{1}')",
                                name,
                                hash
                            ),
                            connection);
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            } catch (SqlException ex) {
                Console.WriteLine("Error:: database connection error:\n {0}", ex.Message);
            }
        }
        /// <summary>
        /// Получение хеша строки
        /// </summary>
        /// <param name="text">хэшируемый текс</param>
        /// <returns>хэш</returns>
        private static string GetHash(string text) {
            using (HashAlgorithm alg = SHA256.Create()) {
                var bytes = alg.ComputeHash(Encoding.UTF8.GetBytes(text));

                return BytesToString(bytes);
            }
        }
        /// <summary>
        /// Перевод байтового массива в hex-строку
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static string BytesToString(byte[] bytes) {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
    }
}
