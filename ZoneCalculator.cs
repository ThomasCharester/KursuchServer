using System.Numerics;

namespace KursuchServer;

public class ZoneCalculator
{
    private static ZoneCalculator instance = null;

    public static ZoneCalculator Instance
    {
        get { return instance; }
        private set { instance = value; }
    }

    public void Calculate(string query)
    {
        // Vector2 start = query.Split(DataParsingExtension.QuerySplitter)[0].StringToVector2();
        // Vector2 end = query.Split(DataParsingExtension.QuerySplitter)[1].StringToVector2();

        List<Vector2> points = new List<Vector2>();

        foreach (var point in query
                     .Split(DataParsingExtension.AdditionalQuerySplitter)[0]
                     .Split(DataParsingExtension.QuerySplitter))
            
            points.Add(point.StringToVector2());
        
        
    }
}