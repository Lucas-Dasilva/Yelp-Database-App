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
using Npgsql;
namespace Milestone1
{
    /// <summary>
    /// Interaction logic for BusinessDetails.xaml
    /// </summary>
    public partial class BusinessDetails : Window
    {

        /// <summary>
        /// The business Id
        /// </summary>
        string bid = "";

        /// <summary>
        /// Constructor that initializes BusinessDetails
        /// </summary>
        /// <param name="bid"></param>
        public BusinessDetails(string bid)
        {
            InitializeComponent();
            this.bid = String.Copy(bid);
            loadBusinessDetails();
            loadBusinessNumbers();
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
                        reader.Read();
                        myf(reader);
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
        /// This is where we set the details in the forms for each business
        /// </summary>
        /// <param name="R"></param>
        private void setBusinessDetails(NpgsqlDataReader R)
        {
            bName.Text = R.GetString(0);
            state.Text = R.GetString(1);
            city.Text = R.GetString(2);
        }   
        
        /// <summary>
        ///  Set the number of business in each state
        /// </summary>
        /// <param name="R">Data reader</param>
        private void setNumInState(NpgsqlDataReader R)
        {
            numInState.Content = R.GetInt16(0).ToString();
        }        

        /// <summary>
        /// Setting the number of business in each city
        /// </summary>
        /// <param name="R">Data reader</param>
        private void setNumInCity(NpgsqlDataReader R)
        {
            numInCity.Content = R.GetInt16(0).ToString();

        }

        /// <summary>
        /// Load business states and cities for each city
        /// </summary>
        private void loadBusinessNumbers()
        {
            string sqlStr1 = "SELECT count(*) from business WHERE state = (SELECT state FROM business WHERE business_id = '" + this.bid + "');";
            string sqlStr2 = "SELECT count(*) from business WHERE city = (SELECT city FROM business WHERE business_id = '" + this.bid + "');";

            executeQuery(sqlStr1, setNumInState);
            executeQuery(sqlStr2, setNumInCity);

        }

        /// <summary>
        /// We get the details for each business into the form
        /// </summary>
        private void loadBusinessDetails()
        {
            string sqlStr = "SELECT name, state, city FROM business WHERE business_id = '" + this.bid + "' ;";
            executeQuery(sqlStr, setBusinessDetails);
        }        

    }
}
