using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
//Add MySql Library
using MySql.Data;
using MySql.Data.MySqlClient;


namespace YunZhi
{
    public class MysqlUtils
    {
        private MySqlConnection connection;
        private string server;
        private string db;
        private string uid;
        private string password;
        

        public MysqlUtils()
        {
            // 初始化连接
            Initialize();
        }


        private void Initialize()
        {
            server = "localhost";
            db = "ocr";
            uid = "root";
            password = "zj258025";

            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" + db + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";charset=utf8";

            connection = new MySqlConnection(connectionString);
        }

        // 打开数据库连接
        //open connection to database
        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        MessageBox.Show("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }

        //Close connection
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        //Insert statement
        public void Insert(PlateBO plateBO) 
        {
            string id = plateBO.id;
            string plate = plateBO.plate;
            string department = plateBO.department;
            string name = plateBO.name;
            string phone = plateBO.phone;
            int carWash = plateBO.carWash;
            // INSERT INTO `ocr`.`auto_ocr`(`id`, `plate`, `department`, `name`, `phone`, `car_wash`) VALUES ('1', '苏K225CA', '云智', '进', '18952594881', 10);
            string query = "INSERT INTO `ocr`.`auto_ocr`(`id`, `plate`, `department`, `name`, `phone`, `car_wash`) VALUES ('" + id + "', '" + plate + "', '" + department + "', '" + name + "', '" + phone + "', " + carWash +")";

            //open connection
            if (this.OpenConnection() == true)
            {
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(query, connection);

                //Execute command
                cmd.ExecuteNonQuery();

                //close connection
                this.CloseConnection();
            }
        }

       // INSERT INTO `ocr`.`car_wash_record`(`id`, `plate`) VALUES ('3', '苏K225CA')
        public void InsertIntoWashRecord(WashRecordBO washRecordBO)
        {
            string id = washRecordBO.id;
            string plate = washRecordBO.plate;
            string washTime = washRecordBO.washTime;
            string query = "INSERT INTO `ocr`.`car_wash_record`(`id`, `plate`) VALUES ('" + id + "', '" + plate + "')";

            //open connection
            if (this.OpenConnection() == true)
            {
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(query, connection);

                //Execute command
                cmd.ExecuteNonQuery();

                //close connection
                this.CloseConnection();
            }
        }

        // UPDATE `ocr`.`auto_ocr` SET `name` = '张进', `phone` = '18952' WHERE `id` = 'af20ca5b-b6bd-415b-a9ba-caadedc83df2'
        public void UpdateCarInfo(PlateBO plateBO)
        {

            string query = "UPDATE `ocr`.`auto_ocr` SET `plate` = '" + plateBO.plate + "', `name` = '" + plateBO.name + "', `phone` = '" + plateBO.phone + "', `department` = '" + plateBO.department + "' WHERE `id` = '" + plateBO.id + "'";

            //Open connection
            if (this.OpenConnection() == true)
            {
                //create mysql command
                MySqlCommand cmd = new MySqlCommand();
                //Assign the query using CommandText
                cmd.CommandText = query;
                //Assign the connection using Connection
                cmd.Connection = connection;

                //Execute query
                cmd.ExecuteNonQuery();

                //close connection
                this.CloseConnection();
            }
        }

        //Update statement
        public void Update()
        {
            string query = "UPDATE tableinfo SET id='22', name='Joe', age='22' WHERE name='John Smith'";

            //Open connection
            if (this.OpenConnection() == true)
            {
                //create mysql command
                MySqlCommand cmd = new MySqlCommand();
                //Assign the query using CommandText
                cmd.CommandText = query;
                //Assign the connection using Connection
                cmd.Connection = connection;

                //Execute query
                cmd.ExecuteNonQuery();

                //close connection
                this.CloseConnection();
            }
        }

        // 删除
        public void Delete(string plate)
        {
            // DELETE FROM `ocr`.`auto_ocr` WHERE `id` = '9999'
            string deleteQuery = "DELETE FROM `ocr`.`auto_ocr` WHERE `plate` = '"+ plate +"'";

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(deleteQuery, connection);
                cmd.ExecuteNonQuery();
                this.CloseConnection();
            }
        }

        // 精确查找
        public List<string>[] PriciseSelect(PlateBO plateBO)
        {
            string plate = plateBO.plate;
            // string plate = "苏K225CA";
            string query = "SELECT * FROM `ocr`.`auto_ocr` WHERE plate='" + plate +"'";

            // 创建list存储数据
            List<string>[] list = new List<string>[7];
            list[0] = new List<string>();
            list[1] = new List<string>();
            list[2] = new List<string>();
            list[3] = new List<string>();
            list[4] = new List<string>();
            list[5] = new List<string>();
            list[6] = new List<string>();

            //Open connection
            if (this.OpenConnection() == true)
            {
                // 创建命令
                MySqlCommand cmd = new MySqlCommand(query, connection);
                // 读取数据
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    list[0].Add(dataReader["id"] + "");
                    list[1].Add(dataReader["plate"] + "");
                    list[2].Add(dataReader["department"] + "");
                    list[3].Add(dataReader["name"] + "");
                    list[4].Add(dataReader["phone"] + "");
                    list[5].Add(dataReader["car_wash"] + "");
                    list[6].Add(dataReader["create_time"] + "");
                }

                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
                return list;
            }
            else
            {
                return list;
            }
        }

        public List<string> PriciseSelectOO(PlateBO plateBO)
        {
            string plate = plateBO.plate;
            // string plate = "苏K225CA";
            string query = "SELECT * FROM `ocr`.`auto_ocr` WHERE plate='" + plate + "'";

            // 创建list存储数据
            List<string> list = new List<string>();
           

            //Open connection
            if (this.OpenConnection() == true)
            {
                // 创建命令
                MySqlCommand cmd = new MySqlCommand(query, connection);
                // 读取数据
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    string str = dataReader["id"] + ","
                        + dataReader["plate"] + ","
                        + dataReader["department"] + ","
                        + dataReader["name"] + ","
                        + dataReader["phone"] + ","
                        + dataReader["car_wash"] + ","
                        + dataReader["create_time"] + "";
                    string[] strArray = str.Split(',');
                    list.Add(str);
                }

                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
                return list;
            }
            else
            {
                return list;
            }
        }

        public List<string> FuzzySelectOO(PlateBO plateBO)
        {
            string plate = plateBO.plate;
            // string plate = "苏K225CA";
            string query = "SELECT * FROM `ocr`.`auto_ocr` WHERE plate like '%" + plate + "%'";

            // 创建list存储数据
            List<string> list = new List<string>();


            //Open connection
            if (this.OpenConnection() == true)
            {
                // 创建命令
                MySqlCommand cmd = new MySqlCommand(query, connection);
                // 读取数据
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    string str = dataReader["id"] + ","
                        + dataReader["plate"] + ","
                        + dataReader["department"] + ","
                        + dataReader["name"] + ","
                        + dataReader["phone"] + ","
                        + dataReader["car_wash"] + ","
                        + dataReader["create_time"] + "";
                    string[] strArray = str.Split(',');
                    list.Add(str);
                }

                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
                return list;
            }
            else
            {
                return list;
            }
        }

        public string PriciseSelectByPlate(string plate)
        {
            // string plate = "苏K225CA";
            string query = "SELECT * FROM `ocr`.`auto_ocr` WHERE plate='" + plate + "'";

            string str = "";
            //Open connection
            if (this.OpenConnection() == true)
            {
                // 创建命令
                MySqlCommand cmd = new MySqlCommand(query, connection);
                // 读取数据
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    str = dataReader["id"] + ","
                        + dataReader["plate"] + ","
                        + dataReader["department"] + ","
                        + dataReader["name"] + ","
                        + dataReader["phone"] + ","
                        + dataReader["car_wash"] + ","
                        + dataReader["create_time"] + "";
                    string[] strArray = str.Split(',');
                }

                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
                return str;
            }
            else
            {
                return str;
            }
        }

        /// <summary>
        ///  洗车表中查找洗车记录
        /// </summary>
        /// <param name="plateBO"></param>
        /// <returns></returns>
        public List<string> PriciseSelectCarWashRecord(WashRecordBO washRecordBO)
        {
            string plate = washRecordBO.plate;
            // string plate = "苏K225CA";
            string query = "SELECT * FROM `ocr`.`car_wash_record` WHERE plate='" + plate + "' ORDER BY `wash_time`";

            // 创建list存储数据
            List<string> list = new List<string>();


            //Open connection
            if (this.OpenConnection() == true)
            {
                // 创建命令
                MySqlCommand cmd = new MySqlCommand(query, connection);
                // 读取数据
                MySqlDataReader dataReader = cmd.ExecuteReader();

                // 存储数据
                while (dataReader.Read())
                {
                    string str = dataReader["id"] + ","
                        + dataReader["plate"] + ","
                        + dataReader["wash_time"] + "";
                    string[] strArray = str.Split(',');
                    list.Add(str);
                }

                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
                return list;
            }
            else
            {
                return list;
            }
        }

        // 模糊查找
        public List<string>[] FuzzySelect()
        {
            string query = "SELECT * FROM `ocr`.`auto_ocr` WHERE plate LIKE '%CA%'";

            // 创建list存储数据
            List<string>[] list = new List<string>[7];
            list[0] = new List<string>();
            list[1] = new List<string>();
            list[2] = new List<string>();
            list[3] = new List<string>();
            list[4] = new List<string>();
            list[5] = new List<string>();
            list[6] = new List<string>();

            //Open connection
            if (this.OpenConnection() == true)
            {
                // 创建命令
                MySqlCommand cmd = new MySqlCommand(query, connection);
                // 读取数据
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    list[0].Add(dataReader["id"] + "");
                    list[1].Add(dataReader["plate"] + "");
                    list[2].Add(dataReader["department"] + "");
                    list[3].Add(dataReader["name"] + "");
                    list[4].Add(dataReader["phone"] + "");
                    list[5].Add(dataReader["car_wash"] + "");
                    list[6].Add(dataReader["create_time"] + "");
                }

                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
                return list;
            }
            else
            {
                return list;
            }
        }

        //Count statement
        public int Count()
        {
            string query = "SELECT Count(*) FROM tableinfo";
            int Count = -1;

            //Open Connection
            if (this.OpenConnection() == true)
            {
                //Create Mysql Command
                MySqlCommand cmd = new MySqlCommand(query, connection);

                //ExecuteScalar will return one value
                Count = int.Parse(cmd.ExecuteScalar() + "");

                //close Connection
                this.CloseConnection();

                return Count;
            }
            else
            {
                return Count;
            }
        }

        //Backup
        public void Backup()
        {
            try
            {
                DateTime Time = DateTime.Now;
                int year = Time.Year;
                int month = Time.Month;
                int day = Time.Day;
                int hour = Time.Hour;
                int minute = Time.Minute;
                int second = Time.Second;
                int millisecond = Time.Millisecond;

                //Save file to C:\ with the current date as a filename
                string path;
                path = "C:\\" + year + "-" + month + "-" + day + "-" + hour + "-" + minute + "-" + second + "-" + millisecond + ".sql";
                StreamWriter file = new StreamWriter(path);


                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "mysqldump";
                psi.RedirectStandardInput = false;
                psi.RedirectStandardOutput = true;
                psi.Arguments = string.Format(@"-u{0} -p{1} -h{2} {3}", uid, password, server, db);
                psi.UseShellExecute = false;

                Process process = Process.Start(psi);

                string output;
                output = process.StandardOutput.ReadToEnd();
                file.WriteLine(output);
                process.WaitForExit();
                file.Close();
                process.Close();
            }
            catch (IOException ex)
            {
                MessageBox.Show("Error , unable to backup!");
            }
        }

        //Restore
        public void Restore()
        {
            try
            {
                //Read file from C:\
                string path;
                path = "C:\\MySqlBackup.sql";
                StreamReader file = new StreamReader(path);
                string input = file.ReadToEnd();
                file.Close();


                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "mysql";
                psi.RedirectStandardInput = true;
                psi.RedirectStandardOutput = false;
                psi.Arguments = string.Format(@"-u{0} -p{1} -h{2} {3}", uid, password, server, db);
                psi.UseShellExecute = false;


                Process process = Process.Start(psi);
                process.StandardInput.WriteLine(input);
                process.StandardInput.Close();
                process.WaitForExit();
                process.Close();
            }
            catch (IOException ex)
            {
                MessageBox.Show("Error , unable to Restore!");
            }
        }
    }
}
