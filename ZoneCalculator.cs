using System.Globalization;
using System.Numerics;
using System.Text;

namespace KursuchServer;

class Zone(float radius, Vector2 center)
{
    public Vector2 Center = center;
    public float Radius = radius;
    public float liters = 0;
}

public class ZoneCalculator
{
    private static ZoneCalculator _instance = null;

    public static ZoneCalculator Instance
    {
        get
        {
            if (_instance == null) _instance = new();
            return _instance;
        }
        private set { _instance = value; }
    }

    public void Calculate(Command command)
    {
        // Vector2 start = query.Split(DataParsingExtension.QuerySplitter)[0].StringToVector2();
        // Vector2 end = query.Split(DataParsingExtension.QuerySplitter)[1].StringToVector2();

        List<Vector2> points = new List<Vector2>();
        List<Zone> zones = new List<Zone>();
        float dosage = float.Parse(command.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[2],
            CultureInfo.InvariantCulture);
        float workRadius = float.Parse(command.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[3],
            CultureInfo.InvariantCulture);

        foreach (var point in command.Query
                     .Split(DataParsingExtension.AdditionalQuerySplitter)[1]
                     .Split(DataParsingExtension.QuerySplitter))
        {
            points.Add(point.StringToVector2());
            zones.Add(new Zone(workRadius, points.Last()));
        }

        for (int i = 0; i < zones.Count; i++)
        for (int j = 0; j < zones.Count; j++)
            if (zones[i].Center == zones[j].Center) continue;
            else
            {
                if (Vector2.Distance(zones[i].Center, zones[j].Center) <= zones[i].Radius + zones[j].Radius)
                {
                    zones[i].Center.X = (zones[i].Center.X + zones[j].Center.X) / 2;
                    zones[i].Center.Y = (zones[i].Center.Y + zones[j].Center.Y) / 2;

                    zones[i].Radius += zones[j].Radius;

                    zones.Remove(zones[j]);

                    i = 0;
                    j = 0;
                }
            }

        StringBuilder output = new();
        output.Append("zd" + DataParsingExtension.AdditionalQuerySplitter + zones.Count.ToString() + DataParsingExtension.AdditionalQuerySplitter);
        foreach (var zone in zones)
        {
            zone.liters = zone.Radius * zone.Radius * (float)Math.PI * dosage;
            output.Append(zone.Center.X.ToString(CultureInfo.InvariantCulture)
                          + DataParsingExtension.ValueSplitter
                          + zone.Center.Y.ToString(CultureInfo.InvariantCulture)
                          + DataParsingExtension.QuerySplitter
                          + zone.Radius.ToString(CultureInfo.InvariantCulture)
                          + DataParsingExtension.QuerySplitter
                          + zone.liters.ToString(CultureInfo.InvariantCulture)
                          + DataParsingExtension.AdditionalQuerySplitter);
        }
        output.Remove(output.Length - 1, 1);
        output.Append(DataParsingExtension.AdditionalQuerySplitter + dosage.ToString(CultureInfo.InvariantCulture) );
        output.Append(DataParsingExtension.AdditionalQuerySplitter + workRadius.ToString(CultureInfo.InvariantCulture));
        output.Append(DataParsingExtension.AdditionalQuerySplitter + command.Query.Split(DataParsingExtension.AdditionalQuerySplitter)[4]);
        ServerApp.Instance.AddCommand(new TCPCommand(command.Client, output.ToString(),
            TCPCommandType.SendSingleValue));
    }
}