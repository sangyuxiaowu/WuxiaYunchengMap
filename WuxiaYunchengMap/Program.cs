using CommandLine;
using System.Drawing;
using System.Drawing.Imaging;


internal class Program
{
    private static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       DrawMap(o.pointsFile, o.saveMap, o.PointFix_X, o.PointFix_Y, o.loadMap);
                   });
    }

    /// <summary>
    /// 绘制地图信息
    /// </summary>
    /// <param name="pointsFile">坐标点文件</param>
    /// <param name="saveMap">生成地图保存文件名</param>
    /// <param name="PointFix">地图坐标点修正值</param>
    /// <param name="loadMap">加载的原始地图</param>
    private static void DrawMap(string pointsFile, string saveMap, int PointFix_X, int PointFix_Y, string loadMap)
    {
        // 存储坐标点的列表
        List<MapPoint> points = new List<MapPoint>();

        // 修正坐标点，因为是地图中的一部分，所以减去一个固定的数值
        //int PointFix = 2000;

        // 读取csv文件的每一行
        foreach (string line in File.ReadAllLines(pointsFile))
        {
            // 将行解析为x、y和注释的值
            string[] values = line.Split(',');
            int x = int.Parse(values[0]);
            int y = int.Parse(values[1]);
            string annotation = values[2];

            // 创建坐标点对象
            MapPoint point = new MapPoint(x, y, annotation);

            // 将坐标点添加到列表中
            points.Add(point);
        }

        // 读取图像文件，如果为指定为空，则创建一个可以将坐标点绘制到上面的新图像
        Image image = loadMap == "" ? new Bitmap(4000, 4000) : Image.FromFile(loadMap);

        // 创建Graphics对象
        using (Graphics g = Graphics.FromImage(image))
        {
            // 设置绘图颜色
            Pen pen = new Pen(Color.Red, 3);
            SolidBrush brush = new SolidBrush(Color.Blue);

            // 遍历坐标点
            foreach (var point in points)
            {

                // 绘制坐标点
                g.DrawEllipse(pen, point.X - PointFix_X, image.Height - (point.Y - PointFix_Y), 10, 10);
                // 绘制坐标点的注释
                g.DrawString($"({point.X},{point.Y}){point.Annotation}", new Font("Arial", 8), brush, point.X + 15 - PointFix_X, image.Height - (point.Y - PointFix_Y) - 8);
            }
        }

        // 保存图像
        image.Save(saveMap, ImageFormat.Png);
    }
}

class MapPoint
{
    public int X { get; set; }
    public int Y { get; set; }
    public string Annotation { get; set; }

    public MapPoint(int x, int y, string annotation)
    {
        this.X = x;
        this.Y = y;
        this.Annotation = annotation;
    }
}


public class Options
{
    [Option('p', "points", Required = true, HelpText = "坐标点文件.")]
    public string pointsFile { get; set; }

    [Option('s', "save", Required = false, Default = "output_map.png", HelpText = "生成的地图文件.")]
    public string saveMap { get; set; }

    [Option('x', "xfix", Required = false, Default = 0, HelpText = "地图坐标点修正值.")]
    public int PointFix_X { get; set; }

    [Option('y', "yfix", Required = false, Default = 0, HelpText = "地图坐标点修正值.")]
    public int PointFix_Y { get; set; }

    [Option('l', "load", Required = false, Default = "", HelpText = "加载的原始地图.")]
    public string loadMap { get; set; }

}