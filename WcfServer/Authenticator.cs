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
            public string Password_Hash;
        }
        /// <summary>
        /// Основная функция сервиса: Аутентификация
        /// </summary>
        /// <param name="login">логин</param>
        /// <param name="hash">пароль</param>
        /// <returns>результат проверки соответствия с БД</returns>
        public bool Authenticate(string json_userdata) {
            UserData ud = JsonConvert.DeserializeObject<UserData>(json_userdata);
            var db_hash  = DBController.GetInstance().ReadUserData(ud.Login);
            // Вывод информации в консоль, для проверки
            // pw - хэш введеного, db - хэш в БД
            Console.WriteLine("user: {0}", ud.Login);
            Console.WriteLine("pw: {0}", ud.Password_Hash);
            Console.WriteLine("db: {0}", db_hash);
            Console.WriteLine("Press Enter to exit...");

            return (db_hash != null) && (ud.Password_Hash == db_hash);
        }
    }
}
