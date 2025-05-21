
namespace App;

public partial class BezierCurveDemo : Form
{
    private readonly Point[] controlPoints = new Point[4];
    private int draggingPointIndex = -1;

    public BezierCurveDemo()
    {
        InitializeComponent();

        // 初始化控制点的位置
        int centerX = ClientRectangle.Width / 2;
        int centerY = ClientRectangle.Height / 2;
        controlPoints[0] = new Point(50, centerY - 50); // 起点
        controlPoints[1] = new Point(centerX - 50, 50);   // 控制点 1
        controlPoints[2] = new Point(centerX + 50, ClientRectangle.Height - 50); // 控制点 2
        controlPoints[3] = new Point(ClientRectangle.Width - 50, centerY + 50); // 终点

        // 订阅 Panel 的绘制、鼠标按下和鼠标移动事件
        panel1.Paint += Panel1_Paint;
        panel1.MouseDown += Panel1_MouseDown;
        panel1.MouseMove += Panel1_MouseMove;
        panel1.MouseUp += Panel1_MouseUp;

        // 设置双缓冲，减少闪烁
        // 启用双缓冲
        //SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
        //UpdateStyles();
        //DoubleBuffered = true;
    }

    private void Panel1_MouseUp(object sender, MouseEventArgs e)
    {
        draggingPointIndex = -1;
    }

    private void Panel1_MouseMove(object sender, MouseEventArgs e)
    {
        if (draggingPointIndex != -1)
        {
            controlPoints[draggingPointIndex] = new Point(e.X, e.Y);
            panel1.Invalidate(); // 强制 Panel 重绘
        }
    }

    private void Panel1_MouseDown(object sender, MouseEventArgs e)
    {
        int hitRadius = 10;
        for (int i = 0; i < controlPoints.Length; i++)
        {
            if (Math.Abs(e.X - controlPoints[i].X) <= hitRadius && Math.Abs(e.Y - controlPoints[i].Y) <= hitRadius)
            {
                draggingPointIndex = i;
                break;
            }
        }
    }

    private void Panel1_Paint(object sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        Pen curvePen = new Pen(Color.Blue, 2);
        Pen controlLinePen = new Pen(Color.Gray, 1);
        SolidBrush controlPointBrush = new SolidBrush(Color.Red);
        int pointRadius = 5;

        // 绘制控制点之间的连线
        g.DrawLine(controlLinePen, controlPoints[0], controlPoints[1]);
        g.DrawLine(controlLinePen, controlPoints[1], controlPoints[2]);
        g.DrawLine(controlLinePen, controlPoints[2], controlPoints[3]);

        // 绘制贝塞尔曲线
        List<Point> bezierPoints = new List<Point>();
        for (float t = 0; t <= 1; t += 0.01f) // 步长越小，曲线越平滑
        {
            bezierPoints.Add(CalculateBezierPoint(t, controlPoints[0], controlPoints[1], controlPoints[2], controlPoints[3]));
        }
        if (bezierPoints.Count > 1)
        {
            g.DrawLines(curvePen, bezierPoints.ToArray());
        }

        // 绘制控制点
        for (int i = 0; i < controlPoints.Length; i++)
        {
            g.FillEllipse(controlPointBrush, controlPoints[i].X - pointRadius, controlPoints[i].Y - pointRadius, 2 * pointRadius, 2 * pointRadius);
        }
    }

    private Point CalculateBezierPoint(float t, Point p0, Point p1, Point p2, Point p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        float x = uuu * p0.X + 3 * uu * t * p1.X + 3 * u * tt * p2.X + ttt * p3.X;
        float y = uuu * p0.Y + 3 * uu * t * p1.Y + 3 * u * tt * p2.Y + ttt * p3.Y;

        return new Point((int)x, (int)y);
    }
}
