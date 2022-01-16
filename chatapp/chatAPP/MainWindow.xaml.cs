using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SQLite;
using System.Security.Cryptography;

namespace chatAPP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            initDB();  //connect to database
            
        }
        // a reference to databse connection
        private SQLiteConnection dbConnection;
        private void initDB()
        {
            // connect to databse
            String path = @"data\database.db";
            dbConnection = new SQLiteConnection("Data Source=" + path + ";Version=3;");
            dbConnection.Open();


        }
        public string getHash(string pass)
        {
            //converts a password to a hash value
            var data = Encoding.Unicode.GetBytes(pass);
            var sha1 = new SHA1CryptoServiceProvider();
            var sha1data = sha1.ComputeHash(data);
            return Convert.ToBase64String(sha1data);
        }
        private void btnLogin_clicked(object sender, RoutedEventArgs e)
        {
            if(txtUsername.Text == "" || txtPassword.Text == "" || txtServer.Text == "")
            {
                MessageBox.Show("please fill all the fields", "empty field error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else
            {
                string hashedPass = getHash(txtPassword.Text);
                string sql = "SELECT password AS pass FROM users WHERE username='" + txtUsername.Text + "'";
                SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                reader.Read();

                // check if username exists within database
                if (reader.HasRows)
                {
                    //if the entered password is correct
                    if ((reader["pass"] + "").Equals(getHash(txtPassword.Text)))
                    {
                        Client client = new Client(txtUsername.Text, txtServer.Text);
                        dbConnection.Close();
                        client.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("wrong password", "incorrect input error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("wrong username", "incorrect input error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

            }
        }
        private void btnSignup_clicked(object sender, RoutedEventArgs e)
        {
            if (txtUsername.Text == "" || txtPassword.Text == "" || txtServer.Text == "")
            {
                MessageBox.Show("please fill all the fields", "empty field error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else
            {
                string sql = "INSERT INTO users(username, password) VALUES('"+ txtUsername.Text +"', '" + getHash(txtPassword.Text) +"')";
                try
                {
                    SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
                    SQLiteDataReader reader = command.ExecuteReader();
                    MessageBox.Show("you have been successfully registered", "signed up", MessageBoxButton.OK, MessageBoxImage.Information);
                    reader.Close();
                }
                catch(Exception exp)
                {
                    MessageBox.Show("you may have an empty filed or your username is already taken!", "taken username", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }
}
