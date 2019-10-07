using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace Net2
{
    public partial class Form1 : Form
    {
        Poisson rand = new Poisson();
        double t = 0;
        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            double m = 1/decimal.ToDouble(numericUpDown4.Value);
            t = T(0.1,P(0.1, t));

            timer1.Enabled = true;
            DrawGraph();
        }


        double T(double l, double a)
        {
            return (-1 / l) * Math.Log(1 - a);
        }

        double P(double l, double t)
        {
            return 1 - Math.Exp(-l*t);
        }


        double Pk(double l, double t, double k)
        {
            return (Math.Pow(l * t, k) / Fact(k)) * Math.Exp(-l * t);
        }

        double Fact(double number)
        {
            double result = 1;
            for (double i = 1; i <= number; i++)
            result *= i;

            return result;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
           
            t += rand.Generate(1);
            textBox1.Text = t.ToString();
        }
        class Poisson : Random
        {
            public double Generate(double a)
            {
                double X = 0;
                double Prod = Math.Exp(-a);
                double Sum = Prod;
                double U = base.Sample();
                while (U > Sum)
                {
                    X++;
                    Prod *= a / Convert.ToDouble(X);
                    Sum += Prod;
                }
                return X;
            }
        }


            private void DrawGraph()
        {
            // Получим панель для рисования
            GraphPane pane = Graph1.GraphPane;

            // Очистим список кривых на тот случай, если до этого сигналы уже были нарисованы
            pane.CurveList.Clear();

            // Создадим список точек
            PointPairList list = new PointPairList();

            double xmin = 0;
            int xmax = 10;
            double y = 0;
            // Заполняем список точек
            for (double x = xmin; x <= xmax; x += 1)
            {
                y += rand.Generate(0.9);
                // добавим в список точку
                list.Add(y, 10);
                
                
            }

            // Создадим кривую с названием "Sinc",
            // которая будет рисоваться голубым цветом (Color.Blue),
            // Опорные точки выделяться не будут (SymbolType.None)
            LineItem myCurve = pane.AddCurve("Sinc", list, Color.Blue, SymbolType.Circle);
            myCurve.Line.IsVisible = false;
            // Вызываем метод AxisChange (), чтобы обновить данные об осях.
            // В противном случае на рисунке будет показана только часть графика,
            // которая умещается в интервалы по осям, установленные по умолчанию
             Graph1.AxisChange();
            // Обновляем график
            Graph1.Invalidate();
        }


    }

}
