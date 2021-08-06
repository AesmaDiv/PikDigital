using System;
using System.Text;
using System.ServiceModel;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace WcfClient {
    class Program {
        /// <summary>
        /// Описание интерфейса сервиса
        /// </summary>
        [ServiceContract]
        public interface IAuthenticator {
            [OperationContract]
            bool Authenticate(string json_userdata);
        }
        public class UserData {
            public string Login;
            public string PasswordHash;
        }
        /// <summary>
        /// Точка входа
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args) {
            Console.WriteLine("CLIENT started.");

            var channel = CreateChannel<IAuthenticator>(new Uri("http://localhost:5050/Authenticate"));
            bool success = false;
            while (!success) { 
                success = TryToLogin(channel);
                if (success) break;
                Console.WriteLine("Press 'Enter' to try again or 'q' to interrupt.");
                success |= Console.ReadLine() == "q";
            }
            Console.WriteLine("Press 'Enter' to exit.");
            Console.ReadLine();
        }
        /// <summary>
        /// Создание канала для клиента
        /// </summary>
        /// <typeparam name="T">тип интерфейса сервиса</typeparam>
        /// <param name="address">адрес подключения</param>
        /// <returns>канал клиента</returns>
        private static T CreateChannel<T>(Uri address) {
            BasicHttpBinding binding = new BasicHttpBinding();
            EndpointAddress endpoint = new EndpointAddress(address);
            ChannelFactory<T> factory = new ChannelFactory<T>(binding, endpoint);

            return factory.CreateChannel();
        }
        /// <summary>
        /// Попытка аутентификации
        /// </summary>
        /// <param name="authenticator">канал клиента</param>
        /// <returns>результат аутентификации</returns>
        private static bool TryToLogin(IAuthenticator authenticator) {
            UserData user_data = ReadLoginData();
            string json_userdata = JsonConvert.SerializeObject(user_data);
            bool result = authenticator.Authenticate(json_userdata);
            Console.WriteLine(
                result ?
                "SUCCESS:: You are loged in." :
                "FAILED:: Login or password is incorrent."
                );

            return result;
        }
        /// <summary>
        /// Получение пары логин-пароль из консоли
        /// </summary>
        /// <returns>кортеж(логин, пароль)</returns>
        private static UserData ReadLoginData() {
            UserData result = new UserData();
            
            Console.Write("login:");
            result.Login = Console.ReadLine();
            Console.Write("password:");
            result.PasswordHash = GetHash(Console.ReadLine());

            return result;
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
