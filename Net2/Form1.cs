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
        double turn = 0;
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
            //t = T(0.1,P(0.1, t));

            //timer1.Enabled = true;
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
           
            t = rand.Generate(0.5);
            textBox1.Text = t.ToString();
        }
        class Poisson : Random
        {
            public double Generate(double l)
            {
                double t = 0;
                double U = base.Sample();
                t = (-1 / l) * Math.Log(U);
                return t;
            }
        }


            private void DrawGraph()
        {
            turn = 0;
            int j = 0;
            double[] mass3y = new double[100];
            double[] mass3dlina = new double[100];
            // Получим панель для рисования
            GraphPane pane = Graph1.GraphPane;
            GraphPane pane2 = Graph2.GraphPane;
            GraphPane pane3 = Graph3.GraphPane;
            // Очистим список кривых на тот случай, если до этого сигналы уже были нарисованы
            pane.CurveList.Clear();
            pane2.CurveList.Clear();
            pane3.CurveList.Clear();
            // Создадим список точек
            PointPairList list1 = new PointPairList();
            PointPairList list2 = new PointPairList();
            PointPairList list2_2 = new PointPairList();
            PointPairList list3 = new PointPairList();
            double y = 0;
            double x = 0;
            double valuePuasson = 0;
            double valueTreatment = 0;
            double valueTreatmentPast = 0;
            list3.Add(0, 0);
            // Заполняем список точек
            for (int i = 0; i <5; i++)
            {
                mass3y[i] = y;
                list2.Add(y, 10); // заполняем значения для второго графика
                list1.Add(y, x);  // заполняем значения для первого графика
               // list3.Add(y, turn);
                valuePuasson = rand.Generate(decimal.ToDouble(numericUpDown4.Value));


                valueTreatment = y + rand.Generate(decimal.ToDouble(numericUpDown8.Value));
                
                // заполняем значения для 2 графика
                if (valueTreatmentPast <= y )
                {
                    list2_2.Add(y, 5);
                    list2_2.Add(valueTreatment, 5);
                    mass3dlina[i] = valueTreatment;
                    //list3.Add(valueTreatmentPast, turn);

                    //if (turn > 0)
                    //{
                    //turn--;
                    // list3.Add(valueTreatmentPast, turn);
                    // }
                }
                else
                {
                    valueTreatment += valueTreatmentPast;
                    mass3dlina[i] = valueTreatment;
                    list2_2.Add(valueTreatmentPast, 5);
                    list2_2.Add(valueTreatment, 5);

                   // list3.Add(y, turn);
                    //turn++;
                    //list3.Add(y, turn);    
                }
                valueTreatmentPast = valueTreatment;
                list2_2.Add(PointPair.Missing, PointPair.Missing);
                y += valuePuasson;
                x++;
            }

            for (int i = 0; i < 4; i++)
            {
                if (mass3y[i + 1] < mass3dlina[i])
                {
                    list3.Add(mass3y[i + 1], turn);
                    turn++;
                    list3.Add(mass3y[i + 1], turn);
                    j++;
                }
                if (turn > 0 && mass3y[i + 1] + mass3dlina[i] < mass3y[i + 1] + mass3y[i + 2])
                {
                    list3.Add(mass3dlina[i], turn);
                    turn--;
                    list3.Add(mass3dlina[i], turn);
                }
            }

                // Создадим кривую с названием "Sinc",
                // которая будет рисоваться голубым цветом (Color.Blue),
                // Опорные точки выделяться не будут (SymbolType.None)
                LineItem myCurve2 = pane2.AddCurve("Sinc", list2, Color.Blue, SymbolType.VDash);
            LineItem myCurve2_2 = pane2.AddCurve("Sinc", list2_2, Color.Blue, SymbolType.VDash);
            LineItem myCurve = pane.AddCurve("Sinc", list1, Color.Blue, SymbolType.None);
            LineItem myCurve3 = pane3.AddCurve("Sinc", list3, Color.Red, SymbolType.None);
            myCurve2.Line.IsVisible = false;
            // Вызываем метод AxisChange (), чтобы обновить данные об осях.
            // В противном случае на рисунке будет показана только часть графика,
            // которая умещается в интервалы по осям, установленные по умолчанию
             Graph2.AxisChange();
            Graph3.AxisChange();
            // Обновляем график
            Graph1.Invalidate();
            Graph2.Invalidate();
            Graph3.Invalidate();
        }

        private void numericUpDown9_ValueChanged(object sender, EventArgs e)
        {
            GraphPane pane = Graph1.GraphPane;
            pane.XAxis.Max = decimal.ToDouble(numericUpDown9.Value);
            GraphPane pane2 = Graph2.GraphPane;
            pane2.XAxis.Max = decimal.ToDouble(numericUpDown9.Value);
            Graph1.AxisChange();
            Graph1.Invalidate();
            Graph2.AxisChange();
            Graph2.Invalidate();

        }

        private void numericUpDown10_ValueChanged(object sender, EventArgs e)
        {
            GraphPane pane = Graph1.GraphPane;
            pane.YAxis.Max = decimal.ToDouble(numericUpDown10.Value);
            Graph1.AxisChange();
            Graph1.Invalidate();
        }
    }
}
