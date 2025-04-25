namespace KursuchServer.Services;

public class GoodsService
{
    
    private static GoodsService instance = null;

    public static GoodsService Instance
    {
        get
        {
            if (instance == null) instance = new();
            return instance;
        }
        private set { instance = value; }
    }
}