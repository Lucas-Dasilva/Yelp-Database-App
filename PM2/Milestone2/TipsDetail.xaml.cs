using Npgsql;
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
using System.Windows.Shapes;
using NpgsqlTypes;

namespace Milestone1
{
    /// <summary>
    /// Interaction logic for TipsDetail.xaml
    /// </summary>
    public partial class TipsDetail : Window
    {
        /// <summary>
        /// The business Id
        /// </summary>
        string bid = "";

        /// <summary>
        /// Initialize the component
        /// </summary>
        /// <param name="bid">The bid reference</param>
        public TipsDetail(string bid)
        {
            InitializeComponent();
            this.bid = String.Copy(bid);
            AddColumn2Grid();
            LoadTips();
        }

        /// <summary>
        /// Tips class, that hold tips values
        /// </summary>
        public class Tips
        {
            public DateTime date { get; set; }
            public string user { get; set; }
            public int likes { get; set; }
            public string text { get; set; }
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
        private void addGridRow(NpgsqlDataReader R)
        {
            tipsGrid.Items.Add(new Tips()
            {
                date = R.GetDateTime(0),
                user = R.GetString(1),
                likes = R.GetInt32(2),
                text = R.GetString(3)
            }); ;
        }

        /// <summary>
        /// Load tip data
        /// </summary>
        private void LoadTips()
        {
            tipsGrid.Items.Clear();

            string sqlStr1 =
                "SELECT date, name, likes, text " +
                "FROM usertable, (" +
                    "SELECT * " +
                    "FROM tip " +
                    "WHERE business_id = '" + this.bid + "') " +
                    "AS temp " +
                "WHERE usertable.user_id = temp.user_id " +
                "ORDER BY date DESC;";

            executeQuery(sqlStr1, addGridRow);
        }

        /// <summary>
        /// Constructing the data grid objects with the default read values
        /// </summary>
        private void AddColumn2Grid()
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Header = "Date";
            col1.Binding = new Binding("date");
            col1.Width = 175;
            tipsGrid.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Binding = new Binding("user");
            col2.Header = "User Name";
            col2.Width = 100;
            tipsGrid.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Binding = new Binding("likes");
            col3.Header = "Likes";
            col3.Width = 60;
            tipsGrid.Columns.Add(col3);

            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Binding = new Binding("text");
            col4.Header = "Text";
            col4.Width = 500;
            tipsGrid.Columns.Add(col4);
        }

        /// <summary>
        /// Insert into table upon pressing add tip
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addTipButton_Click(object sender, RoutedEventArgs e)
        {
            string slqstr = "INSERT INTO tip values (" +
                "'jRyO2V1pA4CdVVqCIOPc1Q', '" + this.bid + "', '" + DateTime.Now + "', '" + userInput.Text + "', 0);";

            executeQuery(slqstr, addGridRow);
            LoadTips();
        }

        /// <summary>
        /// Text input from the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void userInput_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

    }
}
