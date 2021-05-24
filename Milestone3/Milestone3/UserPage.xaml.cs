using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace Milestone3
{
    /// <summary>
    /// Interaction logic for UserPage.xaml
    /// </summary>
    public partial class UserPage : Page 
    {
        public User mainUser;
        public string userId;
        public string userName;

        ObservableCollection<Friend> friends = new ObservableCollection<Friend>();
        ObservableCollection<Tip> tips = new ObservableCollection<Tip>();
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public UserPage()
        {
            InitializeComponent();
            AddColumn2TipsGrid();
            AddColumn2FriendsGrid();
            this.latBlock.IsReadOnly = true;
            this.longBlock.IsReadOnly = true;
            this.mainUser = new User();
        }

        public partial class User : INotifyPropertyChanged
        {
            private string userId;

            public string UserId
            {
                get
                {
                    return this.userId;
                }
                set
                {
                    userId = value;
                    this.OnPropertyChanged("UserId");
                }
            }
            public string name { get; set; }
            public double avgStars { get; set; }
            public DateTime dateJoined { get; set; }
            public int fans { get; set; }
            public int coolVotes { get; set; }
            public int funnyVotes { get; set; }
            public int usefulVotes { get; set; }
            public int tipCount { get; set; }
            public int totalLikes { get; set; }
            public double latitude { get; set; }
            public double longitude { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;

            // The calling member's name will be used as the parameter.
            protected void OnPropertyChanged(string name)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
                }
            }
        }
        class Friend
        {
            public string name { get; set; }
            public double avgStars { get; set; }
            public DateTime date { get; set; }
            public int totalLikes { get; set; }
            public Tip tip { get; set; }
        }

        /// <summary>
        /// Tips class, that hold tips values
        /// </summary>
        public class Tip
        {
            public string user { get; set; }
            public string business { get; set; }
            public string city { get; set; }
            public string text { get; set; }
            public DateTime date { get; set; }
        }

        /// <summary>
        /// Establishes connection with sql database
        /// </summary>
        /// <returns></returns>
        private string BuildConnectionString()
        {
            return "Host = localhost; Username = postgres; Database = yelp; password=5829";
        }

        /// <summary>
        /// Executing each sql statement
        /// </summary>
        /// <param name="sqlstr"> The string with the statement</param>
        /// <param name="myf">Data reader</param>
        private void executeQuery(string sqlstr, Action<NpgsqlDataReader> myf)
        {
            using (var connection = new NpgsqlConnection(BuildConnectionString()))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand())
                {

                    cmd.Connection = connection;
                    cmd.CommandText = sqlstr;

                    try
                    {
                        var reader = cmd.ExecuteReader(); //executes query and gets results from db
                        while (reader.Read())
                        {
                            myf(reader);
                        }
                    }
                    catch (NpgsqlException ex)
                    {
                        Console.WriteLine(ex.Message.ToString());
                        MessageBox.Show("SQL Error - " + ex.Message.ToString());
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Add the row for each tip
        /// </summary>
        /// <param name="R"></param>
        private void addTipGridRow(NpgsqlDataReader R)
        {
            tipsGrid.Items.Add(new Tip()
            {
                user = R.GetString(1),
                business = R.GetString(2),
                city = R.GetString(3),
                text = R.GetString(4),
                date = R.GetDateTime(5),
            });
        }

        /// <summary>
        /// Add the row for each tip
        /// </summary>
        /// <param name="R"></param>
        private void addFriendGridRow(NpgsqlDataReader R)
        {
            friendsGrid.Items.Add(new Friend()
            {
                name = R.GetString(1),
                totalLikes = R.GetInt32(2),
                avgStars = Math.Round(R.GetDouble(3), 2),
                date = R.GetDateTime(4),
                //tip = R.Get(3)
            }); ;
        }

        /// <summary>
        /// Add each zip code according to the city selected
        /// </summary>
        /// <param name="R">Data Reader</param>
        private void AddUser(NpgsqlDataReader R)
        {
            this.userList.Items.Add(R.GetString(0));
        }

        /// <summary>
        /// Set the data for user block values
        /// </summary>
        /// <param name="R"></param>
        private void SetUser(NpgsqlDataReader R)
        {
            this.mainUser.name = R.GetString(1);
            this.mainUser.avgStars = Math.Round(R.GetDouble(2), 2);
            try
            {
                this.mainUser.latitude = R.GetDouble(3);
                this.mainUser.longitude = R.GetDouble(4);

            }
            catch
            {
                this.mainUser.latitude = 0.0;
                this.mainUser.longitude = 0.0;
            }

            this.mainUser.coolVotes = R.GetInt32(5);
            this.mainUser.fans = R.GetInt32(6);
            this.mainUser.funnyVotes = R.GetInt32(7);
            this.mainUser.tipCount = R.GetInt32(8);
            this.mainUser.totalLikes = R.GetInt32(9);
            this.mainUser.usefulVotes = R.GetInt32(10);
            this.mainUser.dateJoined = R.GetDateTime(11);
        }

        /// <summary>
        /// Load the user block data
        /// </summary>
        private void SetUserData()
        {
            this.nameBlock.Text = this.mainUser.name;
            this.starsBlock.Text = this.mainUser.avgStars.ToString();
            this.fansBlock.Text = this.mainUser.fans.ToString();
            this.dateBlock.Text = this.mainUser.dateJoined.ToString();
            this.funnyBlock.Text = this.mainUser.funnyVotes.ToString();
            this.coolBlock.Text = this.mainUser.coolVotes.ToString();
            this.usefulBlock.Text = this.mainUser.usefulVotes.ToString();
            this.tipcountBlock.Text = this.mainUser.tipCount.ToString();
            this.totalTipsBlock.Text = this.mainUser.totalLikes.ToString();
            this.latBlock.Text = this.mainUser.latitude.ToString();
            this.longBlock.Text = this.mainUser.longitude.ToString();
        }

        /// <summary>
        /// Updates this.user db with new location
        /// </summary>
        /// <param name="obj"></param>
        private void SetNewLocation(NpgsqlDataReader R)
        {
            this.mainUser.latitude = R.GetDouble(0);
        }

        /// <summary>
        /// Load tip data
        /// </summary>
        private void LoadTips()
        {
            tipsGrid.Items.Clear();

            string sqlStr1 =
                "SELECT DISTINCT ON (u.user_id) u.user_id, u.name, b.name as business_name, city, text, date " +
                "FROM tip, usertable as u, business as b, (" +
                    "SELECT second_user_id as user_id FROM friendship " +
                    "WHERE first_user_id = '" + this.userList.SelectedItem.ToString() + "') as f " +
                    "WHERE tip.user_id = f.user_id AND u.user_id = tip.user_id AND tip.business_id = b.business_id " +
                    "ORDER BY u.user_id, date desc;";

            executeQuery(sqlStr1, addTipGridRow);
        }

        /// <summary>
        /// Load friends grid data
        /// </summary>
        private void LoadFriends()
        {
            friendsGrid.Items.Clear();

            string sqlStr1 =
                "SELECT u.user_id, name, total_likes, average_stars, yelping_since " +
                "FROM usertable as u, (" +
                    "SELECT second_user_id as user_id from friendship " +
                    "WHERE first_user_id = '" + this.userList.SelectedItem.ToString() + "') as f " +
                "WHERE u.user_id = f.user_id;";

            executeQuery(sqlStr1, addFriendGridRow);
        }

        /// <summary>
        /// Search for a user, and set it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtNameToSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.userList.Items.Clear();

            string name = txtNameToSearch.Text;

            string sqlStr1 =
                "SELECT user_id, name " +
                "FROM usertable " +
                "WHERE LOWER(name) LIKE LOWER('%" + name + "%');";

            executeQuery(sqlStr1, AddUser);
        }

        /// <summary>
        /// Set the current main user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetCurrentUserSelectionChange(object sender, SelectionChangedEventArgs e)
        {
            if (this.userList.SelectedIndex > -1)
            {
                this.userId = this.userList.SelectedItem.ToString();
                string txt = "SELECT * " +
                    "FROM usertable " +
                    "WHERE user_id ='" + this.userList.SelectedItem.ToString() + "';";
                executeQuery(txt, SetUser);
                SetUserData();
                LoadFriends();
                LoadTips();
            }

        }

        /// <summary>
        /// Update database with new latitude and longitude coordinates
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.userList.SelectedIndex > -1)
            {
                string txt = "UPDATE usertable as u " +
                    "SET latitude ='" + this.latBlock.Text + "', " + "longitude = '" + this.longBlock.Text +"' " +
                    "WHERE user_id ='" + this.userList.SelectedItem.ToString() + "';";
                executeQuery(txt, SetNewLocation);
            }
        }

        /// <summary>
        /// Edit user coordinates on button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditCoordinatesButtonClick(object sender, RoutedEventArgs e)
        {
            this.latBlock.IsReadOnly = false;
            this.longBlock.IsReadOnly = false;

            this.latBlock.Focus();
            this.latBlock.SelectAll();
        }

        /// <summary>
        /// On pressing enter inside the latitude text block
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLatKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.latBlock.Text = this.latBlock.Text;
                this.latBlock.IsReadOnly = true;
                this.longBlock.Focus();
                this.longBlock.SelectAll();
            }

        }

        /// <summary>
        /// On pressing enter while in longitude text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLongKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.longBlock.Text = this.longBlock.Text;
                this.longBlock.IsReadOnly = true;
            }
        }

        /// <summary>
        /// Constructing the data grid objects with the default read values
        /// </summary>
        private void AddColumn2TipsGrid()
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Binding = new Binding("user");
            col1.Header = "User Name";
            col1.Width = 125;
            tipsGrid.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Binding = new Binding("business");
            col2.Header = "Business";
            col2.Width = 165;
            tipsGrid.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Binding = new Binding("city");
            col3.Header = "City";
            col3.Width = 120;
            tipsGrid.Columns.Add(col3);

            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Binding = new Binding("text");
            col4.Header = "Text";
            col4.Width = 250;
            tipsGrid.Columns.Add(col4);

            DataGridTextColumn col5 = new DataGridTextColumn();
            col5.Binding = new Binding("date");
            col5.Header = "Date";
            col5.Width = 175;
            tipsGrid.Columns.Add(col5);
        }

        private void AddColumn2FriendsGrid()
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Binding = new Binding("name");
            col1.Header = "Name";
            col1.Width = 80;
            friendsGrid.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Binding = new Binding("totalLikes");
            col2.Header = "Total Likes";
            col2.Width = 100;
            friendsGrid.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Binding = new Binding("avgStars");
            col3.Header = "Avg Stars";
            col3.Width = 90;
            friendsGrid.Columns.Add(col3);

            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Binding = new Binding("date");
            col4.Header = "Yelping Since";
            col4.Width = 150;
            friendsGrid.Columns.Add(col4);
        }

        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
