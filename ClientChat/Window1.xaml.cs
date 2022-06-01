using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace ClientChat
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private string GetHash(string password)
        {
            var md5 = MD5.Create();
            var pass = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(pass);
        }
        private void registerButton_Click(object sender, RoutedEventArgs e)
        {
            loginArea_LostFocus(null, null);
            passworArea_LostFocus(null, null);
            mailArea_LostFocus(null, null);
            if ((string)loginArea.ToolTip != "" || (string)passworArea.ToolTip != "" || (string)mailArea.ToolTip != "")
            {
                return;
            }
            string login = loginArea.Text.Trim();
            string password = passworArea.Password.Trim();
            string mail = mailArea.Text.Trim();
            password = GetHash(password);

            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Database1"].ConnectionString);

            int loginCounter = 0;
            int emailCounter = 0;
            connection.Open();
            SqlCommand selectCommand = new SqlCommand("SELECT COUNT(*) FROM Users WHERE login = @login", connection);
            selectCommand.Parameters.AddWithValue("@login", login);
            loginCounter = Convert.ToInt32(selectCommand.ExecuteScalar());
            selectCommand.CommandText = "SELECT COUNT(*) FROM Users WHERE email = @mail";
            selectCommand.Parameters.AddWithValue("@mail", mail);
            emailCounter = Convert.ToInt32(selectCommand.ExecuteScalar());
            if (loginCounter >= 1)
            {
                MessageBox.Show("User with this login already exist");

            }
            else
            {
                if (emailCounter >= 1)
                {
                    MessageBox.Show("User with this e-mail already exist");

                }
                else
                {

                    SqlCommand insertCommand = new SqlCommand("INSERT Users (login,password,email) VALUES (@login,@password,@mail)", connection);
                    insertCommand.Parameters.AddWithValue("@login", login);
                    insertCommand.Parameters.AddWithValue("@password", password);
                    insertCommand.Parameters.AddWithValue("@mail", mail);
                    insertCommand.ExecuteNonQuery();
                }
            }

            connection.Close();

        }
        private void autorhizationButton_Click(object sender, RoutedEventArgs e)
        {
            loginArea_LostFocus(null, null);
            passworArea_LostFocus(null, null);
            mailArea_LostFocus(null, null);
            if ((string)loginArea.ToolTip != "" || (string)passworArea.ToolTip != "")
            {
                return;
            }
            String login = loginArea.Text;
            string password = passworArea.Password.Trim();
            int loginCounter = 0;
            int passCounter = 0;
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Database1"].ConnectionString))
            {
                connection.Open();
                using (SqlCommand selectCommand = new SqlCommand("SELECT COUNT(*) FROM Users WHERE login = @login", connection))
                {
                    selectCommand.Parameters.AddWithValue("@login", login);
                    loginCounter = Convert.ToInt32(selectCommand.ExecuteScalar());
                    if (loginCounter == 0)
                    {
                        MessageBox.Show("User with this login do not exist");
                    }
                    else
                    {
                        selectCommand.CommandText = "SELECT COUNT(*) FROM Users WHERE password = @password";
                        selectCommand.Parameters.AddWithValue("@password", GetHash(password));
                        passCounter = Convert.ToInt32(selectCommand.ExecuteScalar());
                        if (passCounter == 0)
                        {
                            MessageBox.Show("Wrong password wrote");
                        }
                        else
                        {
                            ClientChat.MainWindow mainWindow = new ClientChat.MainWindow();
                            Hide();
                            mainWindow.Show();
                        }
                    }
                }
                connection.Close();

            }
        }

        private void ChangeAuthorizationButton_Click(object sender, RoutedEventArgs e)
        {
            loginArea.Background = Brushes.Transparent;
            loginArea.ToolTip = "";
            passworArea.Background = Brushes.Transparent;
            passworArea.ToolTip = "";
            mailArea.Background = Brushes.Transparent;
            mailArea.ToolTip = "";
            authorizationChanger.Style = (Style)FindResource("MaterialDesignFlatMidBgButton");
            authorizationChanger.Foreground = new SolidColorBrush(Color.FromRgb(189, 208, 228));
            registrationChanger.Style = (Style)FindResource("MaterialDesignOutlinedDarkButton");
            registrationChanger.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            mailArea.Visibility = Visibility.Collapsed;
            registerButton.Visibility = Visibility.Collapsed;
            authorizationButton.Visibility = Visibility.Visible;
            textAboveForm.Text = "Ну ты это, авторизируйся что-ли";

        }
        private void ChangeRegisterButton_Click(object sender, RoutedEventArgs e)
        {
            loginArea.Background = Brushes.Transparent;
            loginArea.ToolTip = "";
            passworArea.Background = Brushes.Transparent;
            passworArea.ToolTip = "";
            mailArea.Background = Brushes.Transparent;
            mailArea.ToolTip = "";
            authorizationChanger.Style = (Style)FindResource("MaterialDesignOutlinedDarkButton");
            authorizationChanger.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            registrationChanger.Style = (Style)FindResource("MaterialDesignFlatMidBgButton");
            registrationChanger.Foreground = new SolidColorBrush(Color.FromRgb(189, 208, 228));
            mailArea.Visibility = Visibility.Visible;
            registerButton.Visibility = Visibility.Visible;
            authorizationButton.Visibility = Visibility.Collapsed;
            textAboveForm.Text = "Ну ты это, зарегистрируйся что-ли";
        }
        private void loginArea_LostFocus(object sender, RoutedEventArgs e)
        {
            string login = loginArea.Text.Trim();
            if (login.Length < 5)
            {
                loginArea.Background = new SolidColorBrush(Color.FromRgb(204, 51, 51));
                loginArea.ToolTip = "Длинна строки меньше 5";
                return;
            }
            else
            {
                loginArea.Background = Brushes.Transparent;
                loginArea.ToolTip = "";
            }

        }
        private void passworArea_LostFocus(object sender, RoutedEventArgs e)
        {
            string password = passworArea.Password.Trim();
            const string numbers = "0123456789";
            int tempInt = 0;
            if (password.Length < 5)
            {
                passworArea.Background = new SolidColorBrush(Color.FromRgb(204, 51, 51)); ;
                passworArea.ToolTip = "Длинна пароля меньше 5";
                return;
            }
            else if (!password.Any(q => numbers.Contains(q)))
            {
                passworArea.Background = new SolidColorBrush(Color.FromRgb(204, 51, 51)); ;
                passworArea.Background.Opacity = 0.35;
                passworArea.ToolTip = "Пароль должен содержать цифры";
                return;
            }
            else
           if (int.TryParse(password, out tempInt))
            {
                passworArea.Background = new SolidColorBrush(Color.FromRgb(204, 51, 51)); ;
                passworArea.ToolTip = "Пароль должен содержать буквы";
                return;
            }
            else
            {
                passworArea.Background = Brushes.Transparent;
                passworArea.ToolTip = "";
            }

        }
        private void mailArea_LostFocus(object sender, RoutedEventArgs e)
        {
            string mail = mailArea.Text.Trim();
            string emailRegularExpression = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
            if (!Regex.IsMatch(mail, emailRegularExpression))
            {
                mailArea.Background = new SolidColorBrush(Color.FromRgb(204, 51, 51)); ;
                mailArea.ToolTip = "Неправильный e-mail адрес. Проверьте наличие символов @ и .";
                return;
            }
            else
            {
                mailArea.Background = Brushes.Transparent;
                mailArea.ToolTip = "";
            }

        }


    }
}

