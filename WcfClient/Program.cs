using System;
using System.Text;
using System.ServiceModel;
using System.Security.Cryptography;

namespace WcfClient {
    class Program {
        /// <summary>
        /// Описание интерфейса сервиса
        /// </summary>
        [ServiceContract]
        public interface IAuthenticator {
            [OperationContract]
            bool Authenticate(string login, string pwd_hash);
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
            Tuple<string, string> login_data = ReadLoginData();
            bool result = authenticator.Authenticate(login_data.Item1, GetHash(login_data.Item2));
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
        private static Tuple<string , string> ReadLoginData() {
            Console.Write("login:");
            var login = Console.ReadLine();

            Console.Write("password:");
            var password = Console.ReadLine();

            return Tuple.Create(login, password);
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
