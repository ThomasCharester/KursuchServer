namespace KursuchServer.Services;

public class StaticDataService
{
    private static String configFileName = "conf.lox";
    private static StaticDataService instance = null;

    public static StaticDataService? Instance
    {
        get
        {
            if (instance == null) instance = new();
            return instance;
        }
        private set { instance = value; }
    }

    public void LoadServicesConfigs() //
    {
        if (File.Exists(configFileName)) { 
            string[] lines = File.ReadAllLines(configFileName);

            var tcpService = new TCPConnectorService(lines[0], lines[1]);
            var databaseService = new DatabaseService(lines[2]);
            
        } 
    }
}