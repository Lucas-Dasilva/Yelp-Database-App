using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
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
    /// Interaction logic for BusinessPage.xaml
    /// </summary>
    public partial class BusinessPage : Page
    {
        string userId;
        ObservableCollection<CheckBox> MoneyCheckboxes;
        ObservableCollection<CheckBox> AttributeCheckboxes;
        string sortedBy;

        public BusinessPage()
        {
            InitializeComponent();
            AddState();
            AddColumn2Grid();
        }
        public BusinessPage(string userId)
        {
            this.MoneyCheckboxes = new ObservableCollection<CheckBox>();
            this.AttributeCheckboxes = new ObservableCollection<CheckBox>();
            this.sortedBy = "b.name";
            InitializeComponent();
            AddState();
            AddColumn2Grid();
            PopulateComboBox();
            this.userId = userId;
            this.AddCheckBoxList();
        }

        /// <summary>
        /// A business class with business id, name, state and city details
        /// </summary>
        public class Business
        {
            public string bid { get; set; }
            public string name { get; set; }
            public string address { get; set; }
            public double stars { get; set; }
            public int tips { get; set; }
            public double distance { get; set; }
            public string state { get; set; }
            public int checkins { get; set; }
            public string city { get; set; }
            public string postal_code { get; set; }
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
            col1.Header = "Business Name";
            col1.Binding = new Binding("name");
            col1.Width = 170;
            businessGrid.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Binding = new Binding("address");
            col2.Header = "Address";
            col2.Width = 180;
            businessGrid.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Binding = new Binding("city");
            col3.Header = "City";
            col3.Width = 125;
            businessGrid.Columns.Add(col3);

            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Binding = new Binding("state");
            col4.Header = "State";
            col4.Width = 100;
            businessGrid.Columns.Add(col4);

            DataGridTextColumn col5 = new DataGridTextColumn();
            col5.Binding = new Binding("distance");
            col5.Header = "Distance\n(Miles)";
            col5.Width = 100;
            businessGrid.Columns.Add(col5);

            DataGridTextColumn col6 = new DataGridTextColumn();
            col6.Binding = new Binding("stars");
            col6.Header = "Stars";
            col6.Width = 100;
            businessGrid.Columns.Add(col6);

            DataGridTextColumn col7 = new DataGridTextColumn();
            col7.Binding = new Binding("tips");
            col7.Header = "# of Tips";
            col7.Width = 100;
            businessGrid.Columns.Add(col7);

            DataGridTextColumn col8 = new DataGridTextColumn();
            col8.Binding = new Binding("checkins");
            col8.Header = "Total\nCheckins";
            col8.Width = 100;
            businessGrid.Columns.Add(col8);
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
                bid = R.GetString(0),
                name = R.GetString(1),
                address = R.GetString(2),
                city = R.GetString(3),
                state = R.GetString(4),
                stars = R.GetDouble(5),
                tips = R.GetInt32(6),
                checkins = R.GetInt32(7),
                distance = R.GetDouble(8)
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
                string txt2 = "SELECT distinct postal_code " +
                    "FROM business " +
                    "WHERE city = '" + cityList.SelectedItem.ToString() + "' " +
                    "ORDER BY postal_code ASC;";

                //executeQuery(txt, addGridRow);
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
                        "and city = '" + cityList.SelectedItem.ToString() + "' " +
                ") " + "AS temp " +
                    "WHERE businesscategories.business_id = temp.business_id " +
                    "ORDER BY category ASC;";

                //string txt2 = "SELECT name, state, city, business_id " +
                //    "FROM business " +
                //    "WHERE postal_code = '" + zipList.SelectedItem.ToString() + "' " +
                //    "ORDER BY name;";

                executeQuery(txt, addCategory);
                //executeQuery(txt2, addGridRow);
            }
        }

        /// <summary>
        /// Function to check how many boxes are checked in checklist
        /// </summary>
        /// <param name="checklist"></param>
        /// <returns>amount of items checked</returns>
        private int CheckCount(ObservableCollection<CheckBox> checklist)
        {
            int counter = 0;
            foreach(var item in checklist)
            {
                if ((bool)item.IsChecked)
                {
                    counter++;
                }
            }
            return counter;
        }

        /// <summary>
        /// Perform the query on the selected bussiness categories
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e"> Route event</param>
        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            searchFill();
        }

        private void searchFill()
        {
            businessGrid.Items.Clear();
            StringBuilder searchString = new StringBuilder();
            StringBuilder moneyString = new StringBuilder();
            StringBuilder attributeString = new StringBuilder();
            ChangeSort();

            //The main search string, which we append to in case we require more attributes added
            searchString.Append("SELECT business_id, b.name, address, city, state, stars, num_tips, num_checkins, " +
                "calcDistance(b.latitude, b.longitude, u.latitude, u.longitude) as distance " +
                "from business as b, " +
                "(select * from usertable " +
                "where user_id = '" + this.userId + "') as u " +
                "where postal_code = '" + zipList.SelectedItem.ToString() + "' " +
                "and city = '" + cityList.SelectedItem.ToString() + "' ");

            // If they want to filder by money
            if (this.CheckCount(this.MoneyCheckboxes) != 0)
            {
                moneyString.Append("AND business_id IN " +
                "(SELECT business_id " +
                "FROM additional_attribute " +
                "WHERE attribute_name = 'RestaurantsPriceRange2' " +
                "and attribute_value::int4 IN(");

                foreach (var item in this.MoneyCheckboxes)
                {
                    if ((bool)item.IsChecked)
                    {
                        moneyString.Append(item.Name.Last());
                        moneyString.Append(", ");
                    }
                }
                moneyString.Remove(moneyString.Length - 2, 2);
                moneyString.Append(")) ");
                moneyString.ToString();
                searchString.Append(moneyString);
            }
            int checkCount = this.CheckCount(this.AttributeCheckboxes);
            // If user has include atleast one attribute filter
            if (checkCount != 0)
            {
                attributeString.Append("AND business_id IN " +
                "(SELECT business_id " +
                "FROM additional_attribute " +
                "WHERE attribute_name IN(");
                foreach (var item in this.AttributeCheckboxes)
                {
                    if ((bool)item.IsChecked)
                    {
                        attributeString.Append("'" + item.Name + "'");
                        attributeString.Append(", ");
                    }
                }
                attributeString.Remove(attributeString.Length - 2, 2);
                attributeString.Append(") ");

                attributeString.Append("AND attribute_value = 'True' " +
                "GROUP BY business_id " +
                "HAVING count(distinct attribute_name) = " + checkCount + ") ");

                searchString.Append(attributeString);
            }

            int checkCatCount = searchList.Items.Count;
            // If user has include atleast one attribute filter
            if (checkCatCount != 0)
            {
                searchString.Append("AND business_id IN " +
                "(SELECT business_id " +
                    "FROM businesscategories " +
                    "WHERE category IN " + this.formatSearchList() +
                    "GROUP BY business_id " +
                    "HAVING count(distinct category) = " + searchList.Items.Count +
                " )");
            }

            searchString.Append(" ORDER BY " + this.sortedBy + "; ");

            string fullstring = searchString.ToString();


            executeQuery(fullstring, addGridRow);
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
        /// This function clicks on a business name in the main business list grid
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
                chartButton.IsEnabled = true;
                this.detailButton.Background = new SolidColorBrush(Colors.RoyalBlue);
                this.detailButton.BorderBrush = new SolidColorBrush(Colors.RoyalBlue);
                this.chartButton.Background = new SolidColorBrush(Colors.RoyalBlue);
                this.chartButton.BorderBrush = new SolidColorBrush(Colors.RoyalBlue);
                this.tipButton.Background = new SolidColorBrush(Colors.RoyalBlue);
                this.tipButton.BorderBrush = new SolidColorBrush(Colors.RoyalBlue);
            }
            else
            {
                detailButton.IsEnabled = false;
                tipButton.IsEnabled = false;
                chartButton.IsEnabled = false;
                this.detailButton.Background = new SolidColorBrush(Colors.Gray);
                this.detailButton.BorderBrush = new SolidColorBrush(Colors.Gray);
                this.chartButton.Background = new SolidColorBrush(Colors.Gray);
                this.chartButton.BorderBrush = new SolidColorBrush(Colors.Gray);
                this.tipButton.Background = new SolidColorBrush(Colors.Gray);
                this.tipButton.BorderBrush = new SolidColorBrush(Colors.Gray);
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
                if (!businessWindow.IsLoaded)
                {
                    Application.Current.MainWindow.Hide();
                    businessWindow.Show();
                }
            }
        }

        // chart
        private void chartButton_Click(object sender, RoutedEventArgs e)
        {
            Business B = businessGrid.Items[businessGrid.SelectedIndex] as Business;
            if ((B.bid != null) && (B.bid.ToString().CompareTo("") != 0))
            {
                Chart chartWindow = new Chart();
                //if (!chartWindow.IsLoaded)
                //{
                //    chartWindow.
                //}

                
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
                TipsDetail tipsWindow = new TipsDetail(B.bid.ToString(), this.userId.ToString());
                if (!tipsWindow.IsLoaded)
                {
                    Application.Current.MainWindow.Hide();
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
            if (this.searchList.Items.Count == 0)
            {
                this.searchButton.Background = new SolidColorBrush(Colors.RoyalBlue);
                this.searchButton.BorderBrush = new SolidColorBrush(Colors.RoyalBlue);
                this.searchButton.IsEnabled = true;

            }
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
            if (searchList.Items.Count > 0)
            {
                searchList.Items.Remove(searchList.SelectedItem);
            }
            if (searchList.Items.Count == 0)
            {
                this.searchButton.Background = new SolidColorBrush(Colors.Gray);
                this.searchButton.BorderBrush = new SolidColorBrush(Colors.Gray);
                this.searchButton.IsEnabled = true;
            }

        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if there is atleast one item in search list
            if (searchList.Items.Count > 0)
            {
                searchList.Items.Clear();
                this.searchButton.Background = new SolidColorBrush(Colors.Gray);
                this.searchButton.BorderBrush = new SolidColorBrush(Colors.Gray);
                this.searchButton.IsEnabled = true;

            }
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
            if (this.searchList.Items.Count == 0)
            {
                this.searchButton.Background = new SolidColorBrush(Colors.RoyalBlue);
                this.searchButton.BorderBrush = new SolidColorBrush(Colors.RoyalBlue);
                this.searchButton.IsEnabled = true;
            }
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
            if (this.searchList.Items.Count == 0)
            {
                this.searchButton.Background = new SolidColorBrush(Colors.Gray);
                this.searchButton.BorderBrush = new SolidColorBrush(Colors.Gray);
                this.searchButton.IsEnabled = true;

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
            this.detailButton.Background = new SolidColorBrush(Colors.Gray);
            this.detailButton.BorderBrush = new SolidColorBrush(Colors.Gray);
        }

        private void chartButton_Initialized(object sender, EventArgs e)
        {
            chartButton.IsEnabled = false;
            this.chartButton.Background = new SolidColorBrush(Colors.Gray);
            this.chartButton.BorderBrush = new SolidColorBrush(Colors.Gray);
        }

        /// <summary>
        /// Disable the button until a business is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tipButton_Initialized(object sender, EventArgs e)
        {
            tipButton.IsEnabled = false;
            this.tipButton.Background = new SolidColorBrush(Colors.Gray);
            this.tipButton.BorderBrush = new SolidColorBrush(Colors.Gray);
        }


        /// <summary>
        /// Initialize the name of selected business to No business selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nameLabel_Initialized(object sender, EventArgs e)
        {
            nameLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
            nameLabel.Content = "No Business Selected";
        }

        private void SortSelectionChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Add every check box to observable collection to keep track of whats checked and whats not checked
        /// </summary>
        public void AddCheckBoxList()
        {
            this.MoneyCheckboxes.Add(this.money1);
            this.MoneyCheckboxes.Add(this.money2);
            this.MoneyCheckboxes.Add(this.money3);
            this.MoneyCheckboxes.Add(this.money4);
            this.AttributeCheckboxes.Add(this.WheelchairAccessible);
            this.AttributeCheckboxes.Add(this.BusinessAcceptsCreditCards);
            this.AttributeCheckboxes.Add(this.OutdoorSeating);
            this.AttributeCheckboxes.Add(this.RestaurantsReservations);
            this.AttributeCheckboxes.Add(this.RestaurantsGoodForGroups);
            this.AttributeCheckboxes.Add(this.GoodForKids);
            this.AttributeCheckboxes.Add(this.FreeWiFi);
            this.AttributeCheckboxes.Add(this.RestaurantsTakeOut);
            this.AttributeCheckboxes.Add(this.RestaurantsDelivery);
            this.AttributeCheckboxes.Add(this.GoodForMeal_breakfast);
            this.AttributeCheckboxes.Add(this.GoodForMeal_latenight);
            this.AttributeCheckboxes.Add(this.GoodForMeal_dessert);
            this.AttributeCheckboxes.Add(this.GoodForMeal_dinner);
            this.AttributeCheckboxes.Add(this.GoodForMeal_brunch);
            this.AttributeCheckboxes.Add(this.GoodForMeal_lunch);
        }

        private void PopulateComboBox()
        {
            sortList.ItemsSource = new List<string> { "Business Name", "Highest Rating", "Most Number of Tips", "Most Check-Ins", "Nearest" };
        }

        private void ChangeSort()
        {
            if (sortList.Text == "Business Name")
            {
                this.sortedBy = "b.name";
            }
            else if (sortList.Text == "Highest Rating")
            {
                this.sortedBy = "stars DESC";
            }
            else if (sortList.Text == "Most Number of Tips")
            {
                this.sortedBy = "num_tips DESC";
            }
            else if (sortList.Text == "Most Check-Ins")
            {
                this.sortedBy = "num_checkins DESC";
            }
            else if (sortList.Text == "Nearest")
            {
                this.sortedBy = "distance ASC";
            }
        }
    }
}
