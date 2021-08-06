using System;
using System.ServiceModel;

namespace WcfServer {
    public class Program {
        /// <summary>
        /// Точка входа
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args) {
            // Вызов функции заполнения БД для примера
            //db.PopulateUserData();
            //return;

            using (var host = CreateHost(new Uri("http://localhost:5050/Authenticate"))) {
                // Запуск работы хоста
                host.Open();
                Console.WriteLine("AUTHENTICATOR SERVICE is started.");
                Console.WriteLine("Press Enter to exit...");
                Console.ReadLine();
                host.Close();
            }
        }
        /// <summary>
        /// Создание хоста
        /// </summary>
        /// <param name="address">адрес подключения</param>
        /// <returns>хост</returns>
        private static ServiceHost CreateHost(Uri address) {
            BasicHttpBinding binding = new BasicHttpBinding();
            ServiceHost host = new ServiceHost(typeof(AuthenticationService));
            host.AddServiceEndpoint(typeof(IAuthenticator), binding, address);

            return host;
        }
    }
}
