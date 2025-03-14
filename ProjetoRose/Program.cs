using System;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Forms;
using DotNetEnv;
using MySql.Data.MySqlClient;

namespace ProjetoRose
{
    internal static class Program
    {
        public static string ConnectionString { get; private set; }

        [STAThread]
        static void Main()
        {
            // Carrega as variáveis do arquivo .env
            Env.Load();

            // Obtém as variáveis de ambiente
            string server = Environment.GetEnvironmentVariable("DB_SERVER");
            string database = Environment.GetEnvironmentVariable("DB_NAME");
            string user = Environment.GetEnvironmentVariable("DB_USER");
            string password = Environment.GetEnvironmentVariable("DB_PASSWORD");
            string port = Environment.GetEnvironmentVariable("DB_PORT");
            string sslMode = Environment.GetEnvironmentVariable("DB_SSL_MODE");

            Debug.WriteLine($"DB_SERVER: {server}");
            Debug.WriteLine($"DB_NAME: {database}");
            Debug.WriteLine($"DB_USER: {user}");
            Debug.WriteLine($"DB_PASSWORD: {password}");
            Debug.WriteLine($"DB_PORT: {port}");
            Debug.WriteLine($"DB_SSL_MODE: {sslMode}");

            // Monta a connection string
            ConnectionString = $"server={server};database={database};user={user};password={password};port={port};SslMode={sslMode};";

            // Exibe a connection string no console (apenas para depuração)
            Console.WriteLine("Connection String: " + ConnectionString);

            // Atualiza o connectionString no App.config
            UpdateConnectionStringInConfig(ConnectionString);

            // Inicializa a aplicação Windows Forms
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormPrincipal());
        }

        private static void UpdateConnectionStringInConfig(string connectionString)
        {
            try
            {
                // Abre o arquivo de configuração
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                // Atualiza a connection string
                config.ConnectionStrings.ConnectionStrings["sql"].ConnectionString = connectionString;

                // Salva as alterações
                config.Save(ConfigurationSaveMode.Modified);

                // Recarrega a seção de connection strings
                ConfigurationManager.RefreshSection("connectionStrings");

                Console.WriteLine("Connection string atualizada no App.config com sucesso.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao atualizar a connection string no App.config: " + ex.Message);
            }
        }
    }
}