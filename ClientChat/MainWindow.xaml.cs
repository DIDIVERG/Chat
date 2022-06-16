using System.Windows;
using System.Windows.Input;
using ClientChat.ServiceChat;
using System.Data.SqlClient;
using System.Configuration;

namespace ClientChat
{
    
    public partial class MainWindow : Window, IServiceChatCallback
    {
        bool isConnected = false;
        ServiceChatClient client;
        int ID;
        public MainWindow()
        {
            InitializeComponent();
        }
        void ConnectUser()
        {
            if (!isConnected)
            {
                client = new ServiceChatClient(new System.ServiceModel.InstanceContext(this));
                ID = client.Connect(tbUserName.Text);
                tbUserName.IsEnabled = false;
                bConnDiscon.Content = "Disconnect";
                isConnected = true;
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Database1"].ConnectionString))
                {
                    conn.Open();
                    SqlCommand selectQuery = new SqlCommand("SELECT messages FROM Messages", conn);
                    SqlDataReader reader = selectQuery.ExecuteReader();
                    while (reader.Read())
                    {
                        lbChat.Items.Add(reader.GetString(0));
                    }
                    conn.Close();
                }
            }
        }

        void DisconnectUser()
        {
            if (isConnected)
            {
                client.Disconnect(ID);
                client = null;
                tbUserName.IsEnabled = true;
                bConnDiscon.Content = "Connect";
                isConnected = false;
                lbChat.Items.Clear();
            }

        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (isConnected)
            {
                DisconnectUser();
            }
            else
            {
                ConnectUser();
            }

        }

        public void MsgCallback(string msg)
        {
            lbChat.Items.Add(msg);
            lbChat.ScrollIntoView(lbChat.Items[lbChat.Items.Count - 1]);
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Database1"].ConnectionString))
            {
                conn.Open();
                SqlCommand insertQuery = new SqlCommand("INSERT Messages (messages) VALUES (@answer)", conn);
                insertQuery.Parameters.AddWithValue("@answer", msg);
                insertQuery.ExecuteNonQuery();
                conn.Close();
            }
        }

        private void ChatClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DisconnectUser();
        }

        private void MessageArea_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (client != null)
                {
                    client.SendMsg(tbMessage.Text, ID);
                    tbMessage.Text = string.Empty;
                }
            }
        }
    }
}
