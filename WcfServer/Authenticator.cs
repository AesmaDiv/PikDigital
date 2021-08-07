using System;
using System.ServiceModel;
using Newtonsoft.Json;

namespace WcfServer {
    /// <summary>
    /// Описание интерфейса сервиса
    /// </summary>
    [ServiceContract]
    public interface IAuthenticator {
        [OperationContract]
        bool Authenticate(string json_userdata);
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class AuthenticationService : IAuthenticator {
        /// <summary>
        /// Аутентификационные данные пользователя
        /// </summary>
        public class UserData {
            public string Login;
            public string PasswordHash;
        }
        /// <summary>
        /// Основная функция сервиса: Аутентификация
        /// </summary>
        /// <param name="login">логин</param>
        /// <param name="hash">пароль</param>
        /// <returns>результат проверки соответствия с БД</returns>
        public bool Authenticate(string json_userdata) {
            UserData user_data = JsonConvert.DeserializeObject<UserData>(json_userdata);
            var password_hash  = DBController.GetInstance().ReadUserData(user_data.Login);
            // Вывод информации в консоль, для проверки
            // userdata_pwd - хэш введеного, database_pwd - хэш в БД
            Console.WriteLine("user: {0}", user_data.Login);
            Console.WriteLine("userdata_pwd: {0}", user_data.PasswordHash);
            Console.WriteLine("database_pwd: {0}", password_hash);
            Console.WriteLine("Press Enter to exit...");

            return user_data.PasswordHash == password_hash;
        }
    }
}
