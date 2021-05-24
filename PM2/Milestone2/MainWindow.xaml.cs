using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Npgsql;

namespace Milestone1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<Type> MyCollection { get; set; }

        /// <summary>
        /// A business class with business id, name, state and city details
        /// </summary>
        public class Business
        {
            public string bid { get; set; }
            public string name { get; set; }
            public string state { get; set; }
            public string city { get; set; }
            public string postal_code { get; set; }
        }


        /// <summary>
        /// Initialize the win forms app
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            AddState();
            AddColumn2Grid();
        }

        /// <summary>
        /// The connection string for connecting to the database
        /// </summary>
        /// <returns>The connection string</returns>
        private string BuildConnectionString()
        {
            return "Host = localhost; Username = postgres; Database = yelp; password=5829";
        }

        /// <summary>
        /// Adding each distinct state from the database into the win forms app 
        /// </summary>
        private void AddState()
        {
            using (var connection = new NpgsqlConnection(BuildConnectionString()))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand())
                {

                    cmd.Connection = connection;
                    cmd.CommandText = "SELECT distinct state FROM business ORDER BY state";

                    try
                    {
                        var reader = cmd.ExecuteReader(); //executes query and gets results from db
                        while (reader.Read())
                        {
                            stateList.Items.Add(reader.GetString(0));
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
        /// Constructing the data grid objects with the default read values
        /// </summary>
        private void AddColumn2Grid()
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Header = "BusinessName";
            col1.Binding = new Binding("name");
            col1.Width = 255;
            businessGrid.Columns.Add(col1); 
            
            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Binding = new Binding("state");
            col2.Header = "State";
            col2.Width = 60;
            businessGrid.Columns.Add(col2);
            
            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Binding = new Binding("city");
            col3.Header = "City";
            col3.Width = 225;
            businessGrid.Columns.Add(col3); 
            

            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Binding = new Binding("bid");
            col4.Header = "bID";
            col4.Width = 150;
            businessGrid.Columns.Add(col4);
        }

        /// <summary>
        /// Add each city to the city list
        /// </summary>
        /// <param name="R">Data Reader</param>
        private void addCity(NpgsqlDataReader R)
        {
            cityList.Items.Add(R.GetString(0));
        }

        /// <summary>
        /// Add each zip code according to the city selected
        /// </summary>
        /// <param name="R">Data Reader</param>
        private void addZip(NpgsqlDataReader R)
        {
            zipList.Items.Add(R.GetString(0));
        }        
        
        /// <summary>
        /// Add each zip code according to the city selected
        /// </summary>
        /// <param name="R">Data Reader</param>
        private void addCategory(NpgsqlDataReader R)
        {
            categoryList.Items.Add(R.GetString(0));
        }

        /// <summary>
        /// For each state that the user selects, add the corresponding city list to the row
        /// </summary>
        /// <param name="R"></param>
        private void addGridRow(NpgsqlDataReader R)
        {
            businessGrid.Items.Add(new Business()
            {
                name = R.GetString(0),
                state = R.GetString(1),
                city = R.GetString(2),
                bid = R.GetString(3)
            }); ;
        }

        /// <summary>
        /// Function used for selecting each individual state, To which we want to grab all the cities for that state
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event trigger</param>
        private void stateList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cityList.Items.Clear();
            if (stateList.SelectedIndex > -1)
            {
                string txt = "SELECT distinct city FROM business WHERE state = '" + stateList.SelectedItem.ToString() + "' ORDER BY city";
                executeQuery(txt, addCity);
            }
        }

        /// <summary>
        /// We call the addGridRow function when we select a different state
        /// </summary>
        /// <param name="sender">Objest sender</param>
        /// <param name="e">The event trigger</param>
        private void cityList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            zipList.Items.Clear();
            businessGrid.Items.Clear();
            if (cityList.SelectedIndex > -1)
            {
                string txt = "SELECT name, state, city, business_id " +
                    "FROM business " +
                    "WHERE state = '" + stateList.SelectedItem.ToString() + "' " +
                    "AND city = '" + cityList.SelectedItem.ToString() + "' " +
                    "ORDER BY name;";
                string txt2 = "SELECT distinct postal_code " +
                    "FROM business " +
                    "WHERE city = '" + cityList.SelectedItem.ToString() + "' " +
                    "ORDER BY postal_code ASC;";

                executeQuery(txt, addGridRow);
                executeQuery(txt2, addZip);

            }
        }

        /// <summary>
        /// We call the addcategory function when we select a new zip code
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">The event trigger</param>
        private void zipList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            categoryList.Items.Clear();
            businessGrid.Items.Clear();
            if (zipList.SelectedIndex > -1)
            {
                string txt =
                    "SELECT distinct (category) " +
                    "FROM BusinessCategories, " +
                        "(SELECT business_id, postal_code " +
                        "FROM business " +
                        "WHERE postal_code = '" + zipList.SelectedItem.ToString() + "' " +
                        ") " + "AS temp " +
                    "WHERE businesscategories.business_id = temp.business_id " +
                    "ORDER BY category ASC;";

                string txt2 = "SELECT name, state, city, business_id " +
                    "FROM business " +
                    "WHERE postal_code = '" + zipList.SelectedItem.ToString() + "' " +
                    "ORDER BY name;";

                executeQuery(txt, addCategory);
                executeQuery(txt2, addGridRow);
            }
        }


        /// <summary>
        /// Perform the query on the selected bussiness categories
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e"> Route event</param>
        private void searchButton_Click(object sender, RoutedEventArgs e)
        {

            businessGrid.Items.Clear();

            // Check if there is atleast one item in search list
            if (searchList.Items.Count > 0)
            {
                string txt = "SELECT name, state, city, business_id " +
                "FROM business " +
                "WHERE postal_code = '" + zipList.SelectedItem.ToString() + "' " +
                "AND business_id IN (" +
                    "SELECT business_id " +
                    "FROM businesscategories " +
                    "WHERE category IN " + this.formatSearchList() +
                    "GROUP BY business_id " +
                    "HAVING count(distinct category) = " + searchList.Items.Count +
                " );";

                executeQuery(txt, addGridRow);
            }
        }

        /// <summary>
        /// We execute the query for each query string
        /// </summary>
        /// <param name="sqlstr">The string for the query</param>
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
        /// This function is called when the user wants the detail for each city, Opens up a new window
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">Click event trigger</param>
        private void businessGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (businessGrid.SelectedIndex >= 0)
            {
                Business temp = businessGrid.Items[businessGrid.SelectedIndex] as Business;

                nameLabel.Content = temp.name;
                detailButton.IsEnabled = true;
                tipButton.IsEnabled = true;
            }

        }

        /// <summary>
        /// Get the business details if a user clicks on it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void detailButton_Click(object sender, RoutedEventArgs e)
        {
            Business B = businessGrid.Items[businessGrid.SelectedIndex] as Business;
            if ((B.bid != null) && (B.bid.ToString().CompareTo("") != 0))
            {
                BusinessDetails businessWindow = new BusinessDetails(B.bid.ToString());
                businessWindow.Show();
            }
        }

        /// <summary>
        /// Opens the tips window if the user clicks on it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tipButton_Click(object sender, RoutedEventArgs e)
        {
            Business B = businessGrid.Items[businessGrid.SelectedIndex] as Business;
            if ((B.bid != null) && (B.bid.ToString().CompareTo("") != 0))
            {
                TipsDetail tipsWindow = new TipsDetail(B.bid.ToString());
                if (!tipsWindow.IsLoaded)
                {
                    tipsWindow.Show();
                }
            }
        }

        /// <summary>
        /// We want the string to change depending on how many items are in the search list box
        /// </summary>
        /// <returns></returns>
        private string formatSearchList()
        {
            // initializing variables
            var strSearch = new System.Text.StringBuilder();
            string start = "(";
            string quotes = "'";
            string mid = ", ";
            string end = ") ";
            int count = searchList.Items.Count;
            int itr = 0;

            strSearch.Append(start);

            //Scroll through search list and append the search item to final string
            foreach (var item in searchList.Items)
            {
                itr++;
                strSearch.Append(quotes);
                strSearch.Append(item.ToString());
                strSearch.Append(quotes);

                // only append comma if it's not the last item in the list
                if (itr < count)
                {
                    strSearch.Append(mid);
                }
            }
            strSearch.Append(end);

            return strSearch.ToString();
        }

        /// <summary>
        /// Add the selected item to the search list
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">Routed event handler</param>
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            // Loop through all items the ListBox.
            foreach (var selected in categoryList.SelectedItems)
            {
                // Check if item is already in search list
                if (!searchList.Items.Contains(selected))
                {
                    searchList.Items.Add(selected.ToString());
                }
            }
        }

        /// <summary>
        /// Removes selected item from search list
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">Route event</param>
        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if there is atleast one item in search list
            if(searchList.Items.Count > 0)
            {
                searchList.Items.Remove(searchList.SelectedItem);
            }

        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if there is atleast one item in search list
            if (searchList.Items.Count > 0)
            {
                searchList.Items.Clear();
            }
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void searchList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// This function is called when the user selects a categrory from the box list
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">Click event trigger</param>
        private void CategoryBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// Gives user the ability to double click item to add to list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void categoryList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (categoryList.SelectedItem != null)
            {
                // Check if item is already in search list
                if (!searchList.Items.Contains(categoryList.SelectedItem))
                {
                    searchList.Items.Add(categoryList.SelectedItem.ToString());
                }
            }
        }

        /// <summary>
        /// Gives the user the ability to delete search entry by double clicking it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void searchList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (searchList.SelectedItem != null)
            {
                searchList.Items.Remove(searchList.SelectedItem);
            }
        }

        /// <summary>
        /// Change the selection mode to single selection
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">Click event trigger</param>
        private void searchList_Initialized(object sender, EventArgs e)
        {
            searchList.SelectionMode = SelectionMode.Single;
        }

        /// <summary>
        /// Change the selection mode to multiple with extension, allow shift key
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">Click event trigger</param>
        private void categoryList_Initialized(object sender, EventArgs e)
        {
            categoryList.SelectionMode = SelectionMode.Extended;
        }

        /// <summary>
        /// Disable the button until a business is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void detailButton_Initialized(object sender, EventArgs e)
        {
            detailButton.IsEnabled = false;
        }

        /// <summary>
        /// Disable the button until a business is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tipButton_Initialized(object sender, EventArgs e)
        {
            tipButton.IsEnabled = false;
        }

        private void nameLabel_Initialized(object sender, EventArgs e)
        {
            nameLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
            nameLabel.Content = "No Business Selected";
        }
    }
}
