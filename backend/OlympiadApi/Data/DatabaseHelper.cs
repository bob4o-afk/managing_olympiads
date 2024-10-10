using MySql.Data.MySqlClient;

namespace MySQLRandomNumberApp.Data
{
    public class DatabaseHelper
    {
        private readonly string connectionString;

        public DatabaseHelper(string connectionString)
        {
            this.connectionString = connectionString;
        }

        // Method to insert the random number into the database
        public void InsertRandomNumber(int randomNumber)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO Numbers (random_number) VALUES (@randomNumber)";
                
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@randomNumber", randomNumber);

                    // Execute the query
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
