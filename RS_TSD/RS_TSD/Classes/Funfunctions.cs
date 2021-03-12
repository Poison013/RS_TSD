using Newtonsoft.Json;
using Plugin.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace RS_TSD.Classes
{
    class Funfunctions
    {
        static string IP = CrossSettings.Current.GetValueOrDefault("BS_IP", null);
        static string Port = CrossSettings.Current.GetValueOrDefault("BS_Port", null);
        static string UserId = CrossSettings.Current.GetValueOrDefault("BS_UserId", null);

        #region Account
        public static (bool Auth, string Reason) Authorization(string Login, string Password)
        {
            try
            {
                TcpClient client = new TcpClient(IP, Convert.ToInt32(Port));
                NetworkStream stream = client.GetStream();
                StreamWriter writer = new StreamWriter(stream);

                string Request = JsonConvert.SerializeObject(new Classes.Request() { Command = "Authorization", Data = Login + "/" + Password });

                writer.WriteLine(Request);
                writer.Flush();

                StreamReader reader = new StreamReader(stream);
                string Result = reader.ReadLine();
                stream.Close();
                writer.Close();
                reader.Close();

                string[] Data = Result.Split('/');
                if (Data[0] == "True")
                {
                    CrossSettings.Current.AddOrUpdateValue("BS_UserId", Data[1]);
                    CrossSettings.Current.AddOrUpdateValue("BS_UserName", Data[2]);
                    return (true, null);
                }
                else return (false, Data[0]);
            }
            catch (SocketException) { return (false, "SocketError"); }
            catch (Exception) { return (false, "Error"); }
        }
        public static (bool Exit, string Reason) Exit()
        {
            try
            {
                TcpClient client = new TcpClient(IP, Convert.ToInt32(Port));
                NetworkStream stream = client.GetStream();
                StreamWriter writer = new StreamWriter(stream);

                string Request = JsonConvert.SerializeObject(new Classes.Request() { Command = "Exit", Data = UserId });

                writer.WriteLine(Request);
                writer.Flush();

                StreamReader reader = new StreamReader(stream);
                string Result = reader.ReadLine();
                stream.Close();
                writer.Close();
                reader.Close();

                string[] Data = Result.Split('/');
                if (Data[0] == "True") return (true, null);
                else return (false, Data[0]);
            }
            catch (SocketException) { return (false, "SocketError"); }
            catch (Exception) { return (false, "Error"); }
        }
        #endregion

        #region Access
        public static (bool Input, int Mode, string Reason) GetAccessRegion(int RegionId)
        {
            try
            {
                TcpClient client = new TcpClient(IP, Convert.ToInt32(Port));
                NetworkStream stream = client.GetStream();
                StreamWriter writer = new StreamWriter(stream);
                StreamReader reader = new StreamReader(stream);

                string Request = JsonConvert.SerializeObject(new Classes.Request() { Command = "GetAccessRegion", Data = RegionId.ToString() + "/" + UserId });

                writer.WriteLine(Request);
                writer.Flush();


                string[] Result = reader.ReadLine().Split('/');
                writer.Close();
                reader.Close();
                stream.Close();

                if (Result[0] == "True") 
                {
                    switch (Result[1])
                    {
                        case "0": return (true, 0, Result[2]);

                        case "1": return (true, 1, Result[2]);

                        case "2": return (true, 2, Result[2]);

                        default: return (false, -1, "Error");
                    }                
                }
                else 
                { 
                    return (false, -1, Result[2]); 
                }
            }
            catch (SocketException) { return (false, -1, "SocketError"); }
            catch (Exception) { return (false, -1, "Error"); }
        }
        public static (bool Set, string Reason) SetAccessRegion(int RegionId, string UserId, int? Access)
        {
            try
            {
                TcpClient client = new TcpClient(IP, Convert.ToInt32(Port));
                NetworkStream stream = client.GetStream();
                StreamWriter writer = new StreamWriter(stream);
                StreamReader reader = new StreamReader(stream);

                string Request = JsonConvert.SerializeObject(new Classes.Request() { Command = "SetAccessRegion", Data = RegionId.ToString() + "/" + UserId + "/" + Access });

                writer.WriteLine(Request);
                writer.Flush();
                string Result = reader.ReadLine();
                writer.Close();
                reader.Close();
                stream.Close();
                if (Result == "True") { return (true, null); }
                else { return (false, null); }
            }
            catch (SocketException) { return (false, "SocketError"); }
            catch (Exception) { return (false, "Error"); }
        }
        #endregion

        #region Settings
        public static (bool Get, Settings settings, string Reason) GetSettingsRegion(int RegionId)
        {
            try
            {
                TcpClient client = new TcpClient(IP, Convert.ToInt32(Port));
                NetworkStream stream = client.GetStream();
                StreamWriter writer = new StreamWriter(stream);
                StreamReader reader = new StreamReader(stream);

                string Request = JsonConvert.SerializeObject(new Classes.Request() { Command = "GetSettingsRegion", Data = RegionId.ToString() });

                writer.WriteLine(Request);
                writer.Flush();
                string Result = reader.ReadLine();

                writer.Close();
                reader.Close();
                stream.Close();

                if (Result != "ServerError") { return (true, JsonConvert.DeserializeObject<Classes.Settings>(Result), null); }
                else { return (false, null, "ServerError"); }
            }
            catch (SocketException) { return (false, null, "SocketError"); }
            catch (Exception) { return (false, null, "Error"); }
        }
        public static (bool Set, string Reason) SetSettingsRegion(int RegionId, Classes.Settings settings)
        {
            try
            {
                TcpClient client = new TcpClient(IP, Convert.ToInt32(Port));
                NetworkStream stream = client.GetStream();
                StreamWriter writer = new StreamWriter(stream);
                StreamReader reader = new StreamReader(stream);

                string Request = JsonConvert.SerializeObject(new Classes.Request() { Command = "SetSettingsRegion", Data = RegionId.ToString() + "/" + JsonConvert.SerializeObject(settings) });

                writer.WriteLine(Request);
                writer.Flush();

                string Result = reader.ReadLine();

                writer.Close();
                reader.Close();
                stream.Close();

                if (Result != "ServerError") { return (true, null); }
                else { return (false, "ServerError"); }
            }
            catch (SocketException) { return (false, "SocketError"); }
            catch (Exception) { return (false, "Error"); }
        }
        #endregion

        #region Workers
        public static (bool Get, string Worker, string Reason) GetWorker(string WorkerId)
        {
            try
            {
                TcpClient client = new TcpClient(IP, Convert.ToInt32(Port));
                NetworkStream stream = client.GetStream();
                // отправляем сообщение
                StreamWriter writer = new StreamWriter(stream);

                string Request = JsonConvert.SerializeObject(new Classes.Request() { Command = "GetWorker", Data = WorkerId });

                writer.WriteLine(Request);
                writer.Flush();
                StreamReader reader = new StreamReader(stream);
                string Result = reader.ReadLine();
                writer.Close();
                reader.Close();
                stream.Close();
                if (Result != null) { return (true, Result, null); }
                else { return (false, "Неизвестный рабочий", null); }
            }
            catch (SocketException) { return (false, null, "SocketError"); }
            catch (Exception) { return (false, null, "Error"); }
        }
        public static (bool Get, string Workers, string Reason) GetWorkersRegion(int RegionId)
        {
            try
            {
                TcpClient client = new TcpClient(IP, Convert.ToInt32(Port));
                NetworkStream stream = client.GetStream();
                // отправляем сообщение
                StreamWriter writer = new StreamWriter(stream);

                string Request = JsonConvert.SerializeObject(new Classes.Request() { Command = "GetWorkersRegion", Data = RegionId.ToString() });

                writer.WriteLine(Request);
                writer.Flush();

                StreamReader reader = new StreamReader(stream);
                string Result = reader.ReadLine();
                writer.Close();
                reader.Close();
                stream.Close();
                if (Result != "ServerError") { return (true, Result, null); }
                else { return (false, null, "ServerError"); }
            }
            catch (SocketException) { return (false, null, "SocketError"); }
            catch (Exception) { return (false, null, "Error"); }
        }
        public static (bool Get, string Workers, string Reason) GetWorkers()
        {
            try
            {
                string NameDevice = CrossSettings.Current.GetValueOrDefault("BS_NameDevice", null);
                string IP = CrossSettings.Current.GetValueOrDefault("BS_IP", null);
                string Port = CrossSettings.Current.GetValueOrDefault("BS_Port", null);
                TcpClient client = new TcpClient(IP, Convert.ToInt32(Port));
                NetworkStream stream = client.GetStream();
                // отправляем сообщение
                StreamWriter writer = new StreamWriter(stream);

                string Request = JsonConvert.SerializeObject(new Classes.Request() { Command = "GetWorkers" });

                writer.WriteLine(Request);
                writer.Flush();
                StreamReader reader = new StreamReader(stream);
                string Result = reader.ReadLine();
                writer.Close();
                reader.Close();
                stream.Close();
                if (Result != "ServerError") { return (true, Result, null); }
                else { return (false, null, "ServerError"); }
            }
            catch (SocketException) { return (false, null, "SocketError"); }
            catch (Exception) { return (false, null, "Error"); }
        }

        #region Select
        public static (bool Set, string Reason) SetWorkerSelect(int JobId, string WorkerName)
        {
            try
            {
                string NameDevice = CrossSettings.Current.GetValueOrDefault("BS_NameDevice", null);
                string IP = CrossSettings.Current.GetValueOrDefault("BS_IP", null);
                string Port = CrossSettings.Current.GetValueOrDefault("BS_Port", null);
                TcpClient client = new TcpClient(IP, Convert.ToInt32(Port));
                NetworkStream stream = client.GetStream();
                // отправляем сообщение
                StreamWriter writer = new StreamWriter(stream);

                string Request = JsonConvert.SerializeObject(new Classes.Request() { Command = "SetWorkerSelect", Data = JobId + "/" + WorkerName});

                writer.WriteLine(Request);
                writer.Flush();
                StreamReader reader = new StreamReader(stream);
                string Result = reader.ReadLine();
                writer.Close();
                reader.Close();
                stream.Close();
                if (Result != "ServerError")
                {
                    if (Result == "True") { return (true, null); }
                    else { return (false, Result); }
                }
                else { return (false, "ServerError"); }
            }
            catch (SocketException) { return (false, "SocketError"); }
            catch (Exception) { return (false, "Error"); }
        }
        #endregion

        #region Edit
        public static (bool Set, string Reason) SetWorkerEdit(int RegionId, int JobId, string WorkerName, string Time)
        {
            try
            {
                string NameDevice = CrossSettings.Current.GetValueOrDefault("BS_NameDevice", null);
                string IP = CrossSettings.Current.GetValueOrDefault("BS_IP", null);
                string Port = CrossSettings.Current.GetValueOrDefault("BS_Port", null);
                TcpClient client = new TcpClient(IP, Convert.ToInt32(Port));
                NetworkStream stream = client.GetStream();
                // отправляем сообщение
                StreamWriter writer = new StreamWriter(stream);

                string Request = JsonConvert.SerializeObject(new Classes.Request() { Command = "SetWorkerEdit", Data = RegionId + "/" + JobId + "/" + WorkerName + "/" + Time });

                writer.WriteLine(Request);
                writer.Flush();
                StreamReader reader = new StreamReader(stream);
                string Result = reader.ReadLine();
                writer.Close();
                reader.Close();
                stream.Close();

                if (Result != "ServerError")
                {
                    if (Result == "True") { return (true, null); }
                    else { return (false, "Error"); }
                }
                else { return (false, "ServerError"); }
            }
            catch (SocketException) { return (false, "SocketError"); }
            catch (Exception) { return (false, "Error"); }
        }
        public static (bool Replace, string Reason) ReplaceWorkerEdit(int RegionId, int JobId, string NewWorkerName, string OldWorkerName, string Time)
        {
            try
            {
                string NameDevice = CrossSettings.Current.GetValueOrDefault("BS_NameDevice", null);
                string IP = CrossSettings.Current.GetValueOrDefault("BS_IP", null);
                string Port = CrossSettings.Current.GetValueOrDefault("BS_Port", null);
                TcpClient client = new TcpClient(IP, Convert.ToInt32(Port));
                NetworkStream stream = client.GetStream();
                // отправляем сообщение
                StreamWriter writer = new StreamWriter(stream);

                string Request = JsonConvert.SerializeObject(new Classes.Request() { Command = "ReplaceWorkerEdit", Data = RegionId + "/" + JobId + "/" + NewWorkerName + "/" + OldWorkerName + "/" + Time });

                writer.WriteLine(Request);
                writer.Flush();
                StreamReader reader = new StreamReader(stream);
                string Result = reader.ReadLine();
                writer.Close();
                reader.Close();
                stream.Close();
                if (Result != "ServerError")
                {
                    if (Result == "True") { return (true, null); }
                    else { return (false, "Error"); }
                }
                else { return (false, "ServerError"); }
            }
            catch (SocketException) { return (false, "SocketError"); }
            catch (Exception) { return (false, "Error"); }
        }
        public static (bool Remove, string Reason) RemoveWorkerEdit(int RegionId, int JobId, string OldWorkerName, string Time)
        {
            /*try
            {*/
                string NameDevice = CrossSettings.Current.GetValueOrDefault("BS_NameDevice", null);
                string IP = CrossSettings.Current.GetValueOrDefault("BS_IP", null);
                string Port = CrossSettings.Current.GetValueOrDefault("BS_Port", null);
                TcpClient client = new TcpClient(IP, Convert.ToInt32(Port));
                NetworkStream stream = client.GetStream();
                // отправляем сообщение
                StreamWriter writer = new StreamWriter(stream);

                string Request = JsonConvert.SerializeObject(new Classes.Request() { Command = "RemoveWorkerEdit", Data = RegionId + "/" + JobId + "/" + OldWorkerName + "/" + Time });

                writer.WriteLine(Request);
                writer.Flush();
                StreamReader reader = new StreamReader(stream);
                string Result = reader.ReadLine();
                writer.Close();
                reader.Close();
                stream.Close();
                if (Result != "ServerError")
                {
                    if (Result == "True") { return (true, null); }
                    else { return (false, "Error"); }
                }
                else { return (false, "ServerError"); }
            /*}
            catch (SocketException) { return (false, "SocketError"); }
            catch (Exception) { return (false, "Error"); }*/
        }

        #endregion
        #endregion

        public static (bool Get, string History, string Reason) GetHistory(int RegionId)
        {
            try
            {
                TcpClient client = new TcpClient(IP, Convert.ToInt32(Port));
                NetworkStream stream = client.GetStream();
                // отправляем сообщение
                StreamWriter writer = new StreamWriter(stream);

                string Request = JsonConvert.SerializeObject(new Classes.Request() { Command = "GetHistory", Data = RegionId.ToString()});

                writer.WriteLine(Request);
                writer.Flush();
                StreamReader reader = new StreamReader(stream);
                string Result = reader.ReadLine();
                writer.Close();
                reader.Close();
                stream.Close();
                if (Result != "ServerError") { return (true, Result, null); }
                else { return (false, null, "ServerError"); }
            }
            catch (SocketException) { return (false, null, "SocketError"); }
            catch (Exception) { return (false, null, "Error"); }
        }

        public static (bool Start, string Reason) StartWork(int RegionId)
        {
           /* try
            {
              */  TcpClient client = new TcpClient(IP, Convert.ToInt32(Port));
                NetworkStream stream = client.GetStream();
                StreamWriter writer = new StreamWriter(stream);
                StreamReader reader = new StreamReader(stream);

                string Request = JsonConvert.SerializeObject(new Classes.Request() { Command = "StartWork", Data = RegionId.ToString() });

                writer.WriteLine(Request);
                writer.Flush();
                string Result = reader.ReadLine();
                writer.Close();
                reader.Close();
                stream.Close();
                if (Result != "ServerError")
                {
                    if (Result == "True") { return (true, null); }
                    else { return (false, "StartNo"); }
                }
                else { return (false, "ServerError"); }
           /* }
            catch (SocketException) { return (false, "SocketError"); }
            catch (Exception) { return (false, "Error"); }*/
        }

        public static (bool End, string Reason) EndWork(int RegionId, string Time)
        {
            try
            {
                TcpClient client = new TcpClient(IP, Convert.ToInt32(Port));
                NetworkStream stream = client.GetStream();
                StreamWriter writer = new StreamWriter(stream);
                StreamReader reader = new StreamReader(stream);

                string Request = JsonConvert.SerializeObject(new Classes.Request() { Command = "EndWork", Data = RegionId + "/" + Time });

                writer.WriteLine(Request);
                writer.Flush();
                string Result = reader.ReadLine();
                writer.Close();
                reader.Close();
                stream.Close();
                if (Result != "ServerError")
                {
                    if (Result == "True") { return (true, null); }
                    else { return (false, "Error"); }
                }
                else { return (false, "ServerError"); }
            }
            catch (SocketException) { return (false, "SocketError"); }
            catch (Exception) { return (false, "Error"); }
        }

        public static (bool End, string Reason) NewWork(int RegionId, string Time)
        {
            try
            {
                TcpClient client = new TcpClient(IP, Convert.ToInt32(Port));
                NetworkStream stream = client.GetStream();
                StreamWriter writer = new StreamWriter(stream);
                StreamReader reader = new StreamReader(stream);

                string Request = JsonConvert.SerializeObject(new Classes.Request() { Command = "NewWork", Data = RegionId + "/" + Time });

                writer.WriteLine(Request);
                writer.Flush();
                string Result = reader.ReadLine();
                writer.Close();
                reader.Close();
                stream.Close();
                if (Result != "ServerError")
                {
                    if (Result == "True") { return (true, null); }
                    else { return (false, "Error"); }
                }
                else { return (false, "ServerError"); }
            }
            catch (SocketException) { return (false, "SocketError"); }
            catch (Exception) { return (false, "Error"); }
        }
    }
}
