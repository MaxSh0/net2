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

        private void button1_Click(object sender, EventArgs e)
        {
            double m = 1/decimal.ToDouble(numericUpDown4.Value);
            //t = T(0.1,P(0.1, t));

            //timer1.Enabled = true;
            DrawGraph();
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
            double rejected = 0;
            double accepted = 0;
            
            double[] mass3y = new double[decimal.ToInt64(numericUpDown1.Value)];
            double[] mass3dlina = new double[decimal.ToInt64(numericUpDown1.Value)];
            // Получим панель для рисования
            GraphPane pane = Graph1.GraphPane;
            GraphPane pane2 = Graph2.GraphPane;
            GraphPane pane3 = Graph3.GraphPane;
            // Очистим список кривых на тот случай, если до этого сигналы уже были нарисованы
            pane.CurveList.Clear();
            pane2.CurveList.Clear();
            pane2.GraphObjList.Clear();
            pane3.CurveList.Clear();
            // Создадим список точек
            PointPairList list1 = new PointPairList();
            PointPairList list1_1 = new PointPairList();
            PointPairList list1_2 = new PointPairList();
            PointPairList list2 = new PointPairList();
            PointPairList list2_2 = new PointPairList();
            PointPairList list3 = new PointPairList();
            double y = 0;
            double x = 0;
            double valuePuasson = 0;
            double valueTreatment = 0;
            double valueTreatmentPast = 0;
            list3.Add(0, 0);
            list1_2.Add(0, 0);
            list1_1.Add(0, 0);

            numericUpDown9.Value = 100;
            pane.YAxis.Title.Text = "Количество заявок";
            pane.XAxis.Title.Text = "Время";

            pane2.YAxis.Title.Text = "Серверы";
            pane2.XAxis.Title.Text = "Время";

            pane3.YAxis.Title.Text = "Очередь";
            pane3.XAxis.Title.Text = "Время";
            // Заполняем список точек
            for (int i = 0; i < decimal.ToDouble(numericUpDown1.Value); i++)
            {
                mass3y[i] = y;
                list2.Add(y, 3); // заполняем значения для второго графика
                
                //подписываем графики
                TextObj text2 = new TextObj("t" + i.ToString(), y, 2.5);
                pane2.GraphObjList.Add(text2);
                text2.FontSpec.Border.IsVisible = false;
                text2.FontSpec.Size = 20;

                list1.Add(y, x);  // заполняем значения для первого графика
                x++;
                list1.Add(y, x);

                valuePuasson = rand.Generate(decimal.ToDouble(numericUpDown4.Value));


                valueTreatment = y + rand.Generate(decimal.ToDouble(numericUpDown8.Value));
                
                // заполняем значения для 2 графика
                if (valueTreatmentPast <= y )
                {
                    list2_2.Add(y, 1);
                    list2_2.Add(valueTreatment, 1);


                    //подписываем графики
                    TextObj text = new TextObj("m"+i.ToString(),(y + valueTreatment)/2, 0.75);
                    pane2.GraphObjList.Add(text);
                    text.FontSpec.Size = 15;

                    mass3dlina[i] = valueTreatment;
                }
                else
                {
                    valueTreatment += valueTreatmentPast;
                    mass3dlina[i] = valueTreatment;
                    list2_2.Add(valueTreatmentPast, 1);
                    list2_2.Add(valueTreatment, 1);

                    //подписываем графики
                    TextObj text = new TextObj("m" + i.ToString(), (valueTreatmentPast + valueTreatment) / 2, 0.75);
                    pane2.GraphObjList.Add(text);
                    text.FontSpec.Size = 15;
                }
                valueTreatmentPast = valueTreatment;
                list2_2.Add(PointPair.Missing, PointPair.Missing);
                y += valuePuasson;
                
            }
            //заполняем точки для графика очереди
            for (int i = 0; i < decimal.ToDouble(numericUpDown1.Value) - 1; i++)
            {
                //пока есть точки концов продолжительности между двумя входными запросами уменьшаем график на 1
                while (turn > 0 && mass3y[i] < mass3dlina[j] && mass3y[i + 1] > mass3dlina[j])
                {
                        list3.Add(mass3dlina[j], turn);
                        turn--;
                        list3.Add(mass3dlina[j], turn);
                        j++;
                }
                //если точка входящего запроса меньше точки длины предыдущего то увеличиваем график на 1
                if (mass3y[i + 1] < mass3dlina[i])
                {
                    if (turn != decimal.ToDouble(numericUpDown3.Value))
                    {
                        list3.Add(mass3y[i + 1], turn);
                        turn++;
                        list3.Add(mass3y[i + 1], turn);
                        //заполняем график обработанных запросов точками
                        list1_2.Add(mass3dlina[i], accepted);
                        accepted++;
                        list1_2.Add(mass3dlina[i], accepted);
                    }
                    else //заполняем график отказов точками
                    {
                        list1_1.Add(mass3y[i+1], rejected);
                        rejected++;
                        list1_1.Add(mass3y[i+1], rejected);
                    }
                }
                else
                {
                    j++;//показатель того сколько отрезков продолжительности прошли
                    //заполняем график обработанных запросов точками
                    list1_2.Add(mass3dlina[i], accepted);
                    accepted++;
                    list1_2.Add(mass3dlina[i], accepted);
                }
            }
            //дорисовываем концовку графика
            while (turn > 0)
            {
                list3.Add(mass3dlina[j], turn);
                turn--;
                list3.Add(mass3dlina[j], turn);
                j++;
            }
            list1_2.Add(mass3dlina[decimal.ToInt64(numericUpDown1.Value) - 1], accepted);
            list1_2.Add(mass3dlina[decimal.ToInt64(numericUpDown1.Value) - 1], accepted+1);
            // Создадим кривые
            LineItem myCurve2 = pane2.AddCurve("Заявка", list2, Color.Blue, SymbolType.VDash);
            LineItem myCurve2_2 = pane2.AddCurve("Процесс обработки заявки", list2_2, Color.Blue, SymbolType.VDash);  
            LineItem myCurve1 = pane.AddCurve("Всего", list1, Color.Blue, SymbolType.None);
            LineItem myCurve1_1 = pane.AddCurve("Отказ", list1_1, Color.Red, SymbolType.None);
            LineItem myCurve1_2 = pane.AddCurve("Обработаны", list1_2, Color.Green, SymbolType.None);
            LineItem myCurve3 = pane3.AddCurve("Очередь", list3, Color.Red, SymbolType.None);
            //2 кривую делаем невидимой
            myCurve2.Line.IsVisible = false;
            //толщина кривых
            myCurve1.Line.Width = 2.0f;
            myCurve1_1.Line.Width = 2.0f;
            myCurve1_2.Line.Width = 2.0f;
            myCurve2.Line.Width = 2.0f;
            myCurve2_2.Line.Width = 2.0f;
            myCurve3.Line.Width = 2.0f;


            //myCurve2_2.Line.Fill = new ZedGraph.Fill( Color.Blue);
            //размеры шрифтов
            pane.XAxis.Scale.FontSpec.Size = 20;
            pane.YAxis.Scale.FontSpec.Size = 20;

            pane2.XAxis.Scale.FontSpec.Size = 20;
            pane2.YAxis.Scale.FontSpec.Size = 20;

            pane3.XAxis.Scale.FontSpec.Size = 20;
            pane3.YAxis.Scale.FontSpec.Size = 20;

            pane.Legend.FontSpec.Size = 20;
            pane2.Legend.FontSpec.Size = 20;
            pane3.Legend.FontSpec.Size = 20;

            pane.XAxis.Title.FontSpec.Size = 20;
            pane.YAxis.Title.FontSpec.Size = 20;

            pane2.XAxis.Title.FontSpec.Size = 20;
            pane2.YAxis.Title.FontSpec.Size = 20;

            pane3.XAxis.Title.FontSpec.Size = 20;
            pane3.YAxis.Title.FontSpec.Size = 20;

            //Задаем начальные приближения графика
            pane.YAxis.Scale.MinAuto = true;
            pane.YAxis.Scale.MaxAuto = true;
            pane.XAxis.Scale.Min = 0;
            pane.XAxis.Scale.Max = 100;
            pane.IsBoundedRanges = true;


            pane2.YAxis.Scale.MinAuto = true;
            pane2.YAxis.Scale.MaxAuto = true;
            pane2.XAxis.Scale.Min = 0;
            pane2.XAxis.Scale.Max = 100;
            pane2.IsBoundedRanges = true;

            pane3.YAxis.Scale.Min = 0;
            pane3.YAxis.Scale.MaxAuto = true;
            pane3.XAxis.Scale.Min = 0;
            pane3.XAxis.Scale.Max = 100;
            pane3.IsBoundedRanges = true;

            // Вызываем метод AxisChange (), чтобы обновить данные об осях.
            Graph1.AxisChange();
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
            pane.XAxis.Scale.Max = decimal.ToDouble(numericUpDown9.Value);

            GraphPane pane2 = Graph2.GraphPane;
            pane2.XAxis.Scale.Max = decimal.ToDouble(numericUpDown9.Value);

            GraphPane pane3 = Graph3.GraphPane;
            pane3.XAxis.Scale.Max = decimal.ToDouble(numericUpDown9.Value);


            pane.YAxis.Scale.MinAuto = true;
            pane.YAxis.Scale.MaxAuto = true;
            pane.IsBoundedRanges = true;

            pane3.YAxis.Scale.MinAuto = true;
            pane3.YAxis.Scale.MaxAuto = true;
            pane3.IsBoundedRanges = true;


            Graph1.AxisChange();
            Graph1.Invalidate();
            Graph2.AxisChange();
            Graph2.Invalidate();
            Graph3.AxisChange();
            Graph3.Invalidate();


        }

        private void numericUpDown10_ValueChanged(object sender, EventArgs e)
        {
            if (decimal.ToDouble(numericUpDown9.Value) > decimal.ToDouble(numericUpDown10.Value))
            {
                GraphPane pane = Graph1.GraphPane;
                GraphPane pane2 = Graph2.GraphPane;
                GraphPane pane3 = Graph3.GraphPane;
                pane.XAxis.Scale.Min = decimal.ToDouble(numericUpDown10.Value);
                pane2.XAxis.Scale.Min = decimal.ToDouble(numericUpDown10.Value);
                pane3.XAxis.Scale.Min = decimal.ToDouble(numericUpDown10.Value);
                Graph1.AxisChange();
                Graph1.Invalidate();
                Graph2.AxisChange();
                Graph2.Invalidate();
                Graph3.AxisChange();
                Graph3.Invalidate();
            }
            else
            {
                numericUpDown10.Value = numericUpDown10.Value - 100;
            }
        }
    }
}
