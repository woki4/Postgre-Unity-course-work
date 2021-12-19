using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 3228);
            
            Socket sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            
            try
            {
                sListener.Bind(ipEndPoint);
                sListener.Listen(10);
                
                while (true)
                {
                    Console.WriteLine("Ожидаем соединение через порт " + ipEndPoint);
                    
                    Socket handler = sListener.Accept();
                    string recievedData;

                    byte[] bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);
                    
                    recievedData = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                    string[] allData = recievedData.Split(';');
                    Console.WriteLine("Запрос: " + allData[0]);
                    string[] data = new string[allData.Length - 1];

                    Console.WriteLine("Данные:");
                    for (int i = 0; i < allData.Length - 1; i++)
                    {
                        data[i] = allData[i + 1];
                        Console.WriteLine(data[i]);
                    }

                    Console.WriteLine("-------------------------------");
                    
                    HandleClientRequest(allData[0], data, handler);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.ReadLine();
            }
        }

        private static void HandleClientRequest(string request, string[] data, Socket handler)
        {
            switch (request)
            {
                case "login":
                    HandleLogin(data, handler);
                    break;
                
                case "getUserData":
                    GetUserData(data, handler);
                    break;
                
                case "register":
                    CreateUser(data, handler);
                    break;
                
                case "getEmployees":
                    GetAllEmployees(handler);
                    break;
                
                case "getTasks":
                    GetUserTasks(data, handler);
                    break;
                
                case "findContactPerson":
                    FindContactPerson(data, handler);
                    break;
                
                case "setTaskCondition":
                    SetTaskCondition(data, handler);
                    break;
                
                case "createTask":
                    CreateTask(data, handler);
                    break;
                
                case "receiveReport":
                    SendReport(data, handler);
                    break;
                    
                
                default:
                    PrintError(request, handler);
                    break;
            }
        }
        
        private static void HandleLogin(string[] data, Socket handler)
        {
            string connectionString =
                "Port = 1234;"+
                "Server=localhost;" +
                "Database=company;" +
                "User ID=postgres;" +
                "Password=12345;";
          
            NpgsqlConnection dbcon;

            dbcon = new NpgsqlConnection(connectionString);
            dbcon.Open();
                    
            NpgsqlCommand dbcmd = dbcon.CreateCommand();
                    
            string sql = $"SELECT id FROM employees WHERE login = '{data[0]}' AND password = '{GetSha256Hash(data[1])}'";

            dbcmd.CommandText = sql;

            string reply = "";
            byte[] msg;
            
            try
            {
                NpgsqlDataReader reader = dbcmd.ExecuteReader();
                
                while (reader.Read())
                {
                    reply += reader.GetInt32(0);
                }

                if (reply == "")
                {
                    reply = "Пользователя c такими логином и паролем не существует!";
                }

                msg = Encoding.UTF8.GetBytes(reply);
                handler.Send(msg);
                    
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
                  
                reader.Close();
                  
                dbcmd.Dispose();
                   
                dbcon.Close();
            }
            catch (NpgsqlException ex)
            {
                reply = "ERROR (code: " + ex.ErrorCode + ")";
                msg = Encoding.UTF8.GetBytes(reply);
                handler.Send(msg);
                    
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

                dbcmd.Dispose();
                   
                dbcon.Close();
            }
        }

        private static void GetUserData(string[] data, Socket handler)
        {
            string connectionString =
                "Port = 1234;"+
                "Server=localhost;" +
                "Database=company;" +
                "User ID=postgres;" +
                "Password=12345;";
          
            NpgsqlConnection dbcon;

            dbcon = new NpgsqlConnection(connectionString);
            dbcon.Open();
                    
            NpgsqlCommand dbcmd = dbcon.CreateCommand();
                    
            string sql = $"SELECT login, role, name FROM employees WHERE id = {data[0]}";

            dbcmd.CommandText = sql;

            string reply = "";
            byte[] msg;
            
            try
            {
                NpgsqlDataReader reader = dbcmd.ExecuteReader();
                
                while (reader.Read())
                {
                    reply += reader.GetString(0) + ";";
                    reply += reader.GetString(1) + ";";
                    reply += reader.GetString(2);
                }

                msg = Encoding.UTF8.GetBytes(reply);
                handler.Send(msg);
                    
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
                  
                reader.Close();
                  
                dbcmd.Dispose();
                   
                dbcon.Close();
            }
            catch (NpgsqlException ex)
            {
                reply = "ERROR (code: " + ex.ErrorCode + ")";
                msg = Encoding.UTF8.GetBytes(reply);
                handler.Send(msg);
                    
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

                dbcmd.Dispose();
                   
                dbcon.Close();
            }
        }

        private static void CreateUser(string[] data, Socket handler)
        {
            string connectionString =
                "Port = 1234;"+
                "Server=localhost;" +
                "Database=company;" +
                "User ID=postgres;" +
                "Password=12345;";
          
            NpgsqlConnection dbcon;

            dbcon = new NpgsqlConnection(connectionString);
            dbcon.Open();
                    
            NpgsqlCommand dbcmd = dbcon.CreateCommand();

            string sql = $"INSERT INTO employees (login, password, role, name) VALUES ('{data[0]}', '{GetSha256Hash(data[1])}', '{data[2]}', '{data[3]}');";
            sql += $"CREATE USER {data[0]} WITH PASSWORD '{data[1]}';";
            
            string role = data[2].ToLower();
            
            switch (role)
            {
                case "рабочий":
                    sql += $"GRANT workers TO {data[0]};";
                    break;
                
                case "менеджер":
                    sql += $"GRANT managers TO {data[0]};";
                    break;
                
                case "администратор":
                    sql += $"GRANT admins TO {data[0]};";
                    break;
            }

            dbcmd.CommandText = sql;

            string reply = "";
            byte[] msg;
            
            try
            {
                dbcmd.ExecuteReader();
                reply = "Пользователь с логином " + data[0] + " был создан";
                
                msg = Encoding.UTF8.GetBytes(reply);
                handler.Send(msg);
                    
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

                dbcmd.Dispose();
                   
                dbcon.Close();
            }
            catch (NpgsqlException ex)
            {
                reply = "ERROR (code: " + ex.ErrorCode + ")";
                msg = Encoding.UTF8.GetBytes(reply);
                handler.Send(msg);
                    
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

                dbcmd.Dispose();
                   
                dbcon.Close();
            }
        }

        private static void GetAllEmployees(Socket handler)
        {
            string connectionString =
                "Port = 1234;"+
                "Server=localhost;" +
                "Database=company;" +
                "User ID=postgres;" +
                "Password=12345;";
          
            NpgsqlConnection dbcon;

            dbcon = new NpgsqlConnection(connectionString);
            dbcon.Open();
                    
            NpgsqlCommand dbcmd = dbcon.CreateCommand();

            string sql = "SELECT id, login, role, name FROM employees";

            dbcmd.CommandText = sql;

            string reply = "";
            byte[] msg;

            
            try
            {
                NpgsqlDataReader reader = dbcmd.ExecuteReader();
                
                while (reader.Read())
                {
                    reply += reader.GetInt32(0) + "|";
                    reply += reader.GetString(1) + "|";
                    reply += reader.GetString(2) + "|";
                    reply += reader.GetString(3) + ";";
                }

                Console.WriteLine(reply);
                
                msg = Encoding.UTF8.GetBytes(reply);
                handler.Send(msg);
                    
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
                
                reader.Close();

                dbcmd.Dispose();
                   
                dbcon.Close();
            }
            catch (NpgsqlException ex)
            {
                reply = "ERROR (code: " + ex.ErrorCode + ")";
                msg = Encoding.UTF8.GetBytes(reply);
                handler.Send(msg);
                    
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

                dbcmd.Dispose();
                   
                dbcon.Close();
            }
        }

        private static void GetUserTasks(string[] data, Socket handler)
        {
            string connectionString =
                "Port = 1234;"+
                "Server=localhost;" +
                "Database=company;" +
                $"User ID={data[0].ToLower()};" +
                $"Password={data[1]};";
          
            NpgsqlConnection dbcon;

            dbcon = new NpgsqlConnection(connectionString);
            dbcon.Open();
                    
            NpgsqlCommand dbcmd = dbcon.CreateCommand();

            string sql = "SELECT * FROM tasks";

            dbcmd.CommandText = sql;

            string reply = "";
            byte[] msg;
            
            NpgsqlDataReader reader = dbcmd.ExecuteReader();
                
            while (reader.Read())
            {
                reply += reader.IsDBNull(0) ? "Null" + "|" : reader.GetInt32(0) + "|";
                reply += reader.IsDBNull(1) ? "Null" + "|" : reader.GetInt32(1) + "|";
                reply += reader.IsDBNull(2) ? "Null" + "|" : reader.GetInt32(2) + "|";
                reply += reader.IsDBNull(3) ? "Null" + "|" : reader.GetInt32(3) + "|";
                reply += reader.IsDBNull(4) ? "Null" + "|" : reader.GetInt32(4) + "|";
                reply += reader.IsDBNull(5) ? "Null" + "|" : reader.GetInt32(5) + "|";
                reply += reader.IsDBNull(6) ? "Null" + "|" : reader.GetString(6) + "|";
                reply += reader.IsDBNull(7) ? "Null" + "|" : reader.GetDateTime(7) + "|";
                reply += reader.IsDBNull(8) ? "Null" + "|" : reader.GetDateTime(8) + "|";
                reply += reader.IsDBNull(9) ? "Null" + "|" : reader.GetInt32(9) + "|";
                reply += reader.IsDBNull(10) ? "Null" + "|" : reader.GetDateTime(10) + "|";
                reply += reader.IsDBNull(11) ? "Null" + ";" : reader.GetBoolean(11) + ";";
            }

            Console.WriteLine(reply);
                
            msg = Encoding.UTF8.GetBytes(reply);
            handler.Send(msg);
                    
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
                
            reader.Close();

            dbcmd.Dispose();
                   
            dbcon.Close();

            try
            {
                
            }
            catch (NpgsqlException ex)
            {
                reply = "ERROR (code: " + ex.ErrorCode + ")";
                msg = Encoding.UTF8.GetBytes(reply);
                handler.Send(msg);
                    
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

                dbcmd.Dispose();
                   
                dbcon.Close();
            }
        }
        
        private static void SetTaskCondition(string[] data, Socket handler)
        {
             string connectionString =
                "Port = 1234;"+
                "Server=localhost;" +
                "Database=company;" +
                $"User ID={data[0].ToLower()};" +
                $"Password={data[1]};";
          
            NpgsqlConnection dbcon;

            dbcon = new NpgsqlConnection(connectionString);
            dbcon.Open();
                    
            NpgsqlCommand dbcmd = dbcon.CreateCommand();

            string sql = $"UPDATE tasks SET is_completed={data[3]} WHERE id={data[2]};";

            dbcmd.CommandText = sql;

            string reply = "";
            byte[] msg;
            
            try
            {
                dbcmd.ExecuteReader();
                reply = "Задание с id " + data[3] + " обновлено";
                
                msg = Encoding.UTF8.GetBytes(reply);
                handler.Send(msg);
                    
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

                dbcmd.Dispose();
                   
                dbcon.Close();
            }
            catch (NpgsqlException ex)
            {
                reply = "ERROR (code: " + ex.ErrorCode + ")";
                msg = Encoding.UTF8.GetBytes(reply);
                handler.Send(msg);
                    
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

                dbcmd.Dispose();
                   
                dbcon.Close();
            }
        }

        private static void CreateTask(string[] data, Socket handler)
        {
            string connectionString =
                "Port = 1234;"+
                "Server=localhost;" +
                "Database=company;" +
                $"User ID={data[0].ToLower()};" +
                $"Password={data[1]};";
          
            NpgsqlConnection dbcon;

            dbcon = new NpgsqlConnection(connectionString);
            dbcon.Open();
                    
            NpgsqlCommand dbcmd = dbcon.CreateCommand();

            string sql = $"INSERT INTO tasks (executor_id, author_id, contract_number, equipment_number, contact_person_id, " +
                         $"finish_date, priority, task) VALUES ({data[2]}, {data[3]}, {data[4]}, {data[5]}, {data[6]}," +
                         $" '{data[7]}', {data[8]}, '{data[9]}');";

            dbcmd.CommandText = sql;

            string reply = "";
            byte[] msg;
            
            try
            {
                dbcmd.ExecuteReader();
                reply = "Задание добавлено";
                
                msg = Encoding.UTF8.GetBytes(reply);
                handler.Send(msg);
                    
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

                dbcmd.Dispose();
                   
                dbcon.Close();
            }
            catch (NpgsqlException ex)
            {
                reply = "ERROR (code: " + ex.ErrorCode + ")";
                msg = Encoding.UTF8.GetBytes(reply);
                handler.Send(msg);
                    
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

                dbcmd.Dispose();
                   
                dbcon.Close();
            }
        }

        private static void FindContactPerson(string[] data, Socket handler)
        {
            string connectionString =
                "Port = 1234;"+
                "Server=localhost;" +
                "Database=company;" +
                "User ID=postgres;" +
                "Password=12345;";
          
            NpgsqlConnection dbcon;

            dbcon = new NpgsqlConnection(connectionString);
            dbcon.Open();
                    
            NpgsqlCommand dbcmd = dbcon.CreateCommand();
                    
            string sql = $"SELECT * FROM find_contact_person_by_name('{data[0]}')";

            dbcmd.CommandText = sql;

            string reply = "";
            byte[] msg;
            
            try
            {
                NpgsqlDataReader reader = dbcmd.ExecuteReader();
                
                while (reader.Read())
                {
                    reply += reader.IsDBNull(0) ? "Null" + ";" : reader.GetInt32(0) + ";";
                    reply += reader.IsDBNull(1) ? "Null" + ";" : reader.GetInt32(1) + ";";
                    
                    reply += reader.IsDBNull(2) ? "Null" + ";" : reader.GetString(2) + ";";
                    reply += reader.IsDBNull(3) ? "Null" + ";" : reader.GetString(3) + ";";
                    reply += reader.IsDBNull(4) ? "Null" + ";" : reader.GetString(4) + ";";
                    reply += reader.IsDBNull(5) ? "Null" : reader.GetString(5);
                }
                
                msg = Encoding.UTF8.GetBytes(reply);
                handler.Send(msg);
                    
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
                  
                reader.Close();
                  
                dbcmd.Dispose();
                   
                dbcon.Close();
            }
            catch (NpgsqlException ex)
            {
                reply = "ERROR (code: " + ex.ErrorCode + ")";
                msg = Encoding.UTF8.GetBytes(reply);
                handler.Send(msg);
                    
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

                dbcmd.Dispose();
                   
                dbcon.Close();
            }
        }

        private static void SendReport(string[] data, Socket handler)
        {
            string connectionString =
                "Port = 1234;"+
                "Server=localhost;" +
                "Database=company;" +
                "User ID=postgres;" +
                "Password=12345;";
          
            NpgsqlConnection dbcon;

            dbcon = new NpgsqlConnection(connectionString);
            dbcon.Open();
                    
            NpgsqlCommand dbcmd = dbcon.CreateCommand();
                    
            string sql = $"CALL generate_report({data[0]}, '{data[1]}', '{data[2]}')";

            dbcmd.CommandText = sql;

            string reply = "";
            byte[] msg;
            
            try
            {
                dbcmd.ExecuteReader();

                string filePath = "D:\\Report.csv";
                
                msg = File.ReadAllBytes(filePath);

                Console.WriteLine(msg.ToString());
                
                using (FileStream stream = File.OpenRead(filePath))
                    
                {
                    msg = new byte[stream.Length];
                    stream.Read(msg, 0, msg.Length);
                }
                
                handler.Send(msg);
                    
                handler.Shutdown(SocketShutdown.Both);
                
                handler.Close();

                dbcmd.Dispose();
                   
                dbcon.Close();
            }
            catch (NpgsqlException ex)
            {
                
            }
        }
        
        private static void PrintError(string request, Socket handler)
        {
            Byte[] msg = Encoding.UTF8.GetBytes("Такого запроса нет");
            handler.Send(msg);
                    
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
                    
            Console.WriteLine("Полученный запрос: " + request);
        }

        private static string GetSha256Hash(String value) 
        {
            StringBuilder Sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create()) 
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }
    }
}