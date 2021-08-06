using System;
using System.ServiceModel;

namespace WcfServer {
    /// <summary>
    /// Описание интерфейса сервиса
    /// </summary>
    [ServiceContract]
    public interface IAuthenticator {
        [OperationContract]
        bool Authenticate(string login, string pwd_hash);
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class AuthenticationService : IAuthenticator {
        /// <summary>
        /// Основная функция сервиса: Аутентификация
        /// </summary>
        /// <param name="login">логин</param>
        /// <param name="hash">пароль</param>
        /// <returns>результат проверки соответствия с БД</returns>
        public bool Authenticate(string login, string hash) {
            return CheckLoginData(login, hash);
        }
        /// <summary>
        /// Проверяет соответствие хэша введенного пароля и хэша из БД
        /// </summary>
        /// <param name="login">логин</param>
        /// <param name="hash">хэш пароль</param>
        /// <returns>результат соответствия с БД</returns>
        private static bool CheckLoginData(string login, string hash) {
            var db_hash  = DBController.GetInstance().ReadUserData(login);
            // Вывод информации в консоль, для проверки
            // pw - хэш введеного, db - хэш в БД
            Console.WriteLine("user: {0}", login);
            Console.WriteLine("pw: {0}", hash);
            Console.WriteLine("db: {0}", db_hash);
            Console.WriteLine("Press Enter to exit...");

            return (db_hash != null) && (hash == db_hash);
        }
    }
}
