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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Milestone3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ProjectChart : Window
    {
        // gen the data for the graph
        string bid = "";
        string empty = "";
        //public void ColumnChart(string bid)
        public ProjectChart(string bid)
        {
            InitializeComponent();
            this.bid = String.Copy(bid);

            LoadChart();
        }

        private void LoadChart()
        {
            List<KeyValuePair<string, int>> myChartData = new List<KeyValuePair<string, int>>();

            using (var connection = new NpgsqlConnection("Host = localhost; Username = postgres; Database = yelp; password=soccer"))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    // cmd.CommandText = sqlstr;

                    cmd.CommandText = "SELECT TO_CHAR(TO_DATE(checkin_month, 'MM'), 'Month') AS month, count(*) as numCheckins " +
                                        "from business as b, checkin as c " +
                                        "where b.business_id = '" + this.bid + "' " +
                                        "AND b.business_id = c.business_id " +
                                        "GROUP BY CAST(checkin_month AS INTEGER), month " +
                                        "ORDER BY CAST(checkin_month AS INTEGER);";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            myChartData.Add(new KeyValuePair<string, int>(reader.GetString(0), reader.GetInt32(1)));
                        }
                    }

                    connection.Close();
                }
            }
            monthChart.DataContext = myChartData;
        }

        private string BuildConnectionString()
        {
            return "Host = localhost; Username = postgres; Database = yelp; password=soccer";
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

        private void AddCheckin(NpgsqlDataReader R)
        {
            this.empty = R.GetString(0);
        }

        private void checkinButton_Click(object sender, RoutedEventArgs e)
        {
            string slqstr = "Insert into checkin " +
                            "values('" + this.bid + "', to_char(now(), 'YYYY'), to_char(now(), 'MM'), to_char(now(), 'DD'), to_char(now(), 'HH24:MI:SS'));";

            executeQuery(slqstr, AddCheckin);
            LoadChart();
        }

    }
}
