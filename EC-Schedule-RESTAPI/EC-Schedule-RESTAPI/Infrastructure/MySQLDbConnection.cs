namespace EC_Schedule_RESTAPI.Infrastructure
{
    public class MySQLDbConnection
    {
        public string ConnectionString { get; }

        /// <summary>
        /// This class is presumed to be used as a singleton. Thus its properties, out of best-practices, should be populated through its constructor.
        /// </summary>
        /// <param name="filePath">Assumed to a path to a config file containing information in regards to connecting to a MySQL database.</param>
        public MySQLDbConnection(string filePath)
        {
            ConnectionString = System.IO.File.ReadAllText(filePath).Split(':')[1]; 
        }
    }
}