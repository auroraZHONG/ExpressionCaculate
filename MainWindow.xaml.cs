using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
/*******************************/
//为什么有时会出现堆栈为空的问题

namespace ExpressionCaculate
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            min.Content = X.Minimum;
            max.Content = X.Maximum;
        }
        /*表达式的输入*/
        Undefine_variable[] vary;   //存放未声明的变量
        int undefine=1;   //未声明变量的个数
        double result;   //求值后的结果
        private void Button_Click(object sender, RoutedEventArgs e) //清空输入输出区，以便重新输入一个表达式
        {
            Expression.Clear();   //清空文本框
            output.Inlines.Clear();   //清空文本区
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)   //确认表达式的输入是否结束
        {
            vary = new Undefine_variable[10];   //申请一些空间用于存放未声明的变量
            undefine = 1;   //重置undefine的值
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)//修改x的值
        {
            output.Inlines.Clear();   //清空文本区
            /*vary[0]用于存放变量x*/
            vary[0].variable = 'x';
            vary[0].value = X.Value;
            string input = Expression.Text;
            /*表达式的标准化与拆分*/
            Standard(input,vary,ref undefine, out string[] items, out int sum);
            /*表达式的计算*/
            result = Caculate(items, sum);
            /*输出结果*/
            //输出计算结果
            output.Inlines.Add(new Run(Convert.ToString(result)));
            output.Inlines.Add(new LineBreak());
            //如若有未声明的变量，则输出其值
            for (int i = 0; i < undefine; i++)
            {
                output.Inlines.Add(new Run(Convert.ToString(vary[i].variable) + ":" + Convert.ToString(vary[i].value)));
                output.Inlines.Add(new LineBreak());
            }
        }
        /*表达式的标准化与拆分*/
        static void Standard(string input,Undefine_variable[] vary,ref int undefine, out string[] items, out int sum)
        {
            string expression = "";   //存放标准化后的表达式
            items = new string[10]; //存放拆分后的各个表达式
            sum = 0;    //item的个数
            char temp;  //用于记录当前字符
            Boolean isExist = false;    //判断变量是否已声明
            Random r = new Random();    //用于生成随机数
            input += "#";   //自定义一个结束符
            int couples = 0;    //记录一个item中当前未成对的括号数
            for (int i = 0; input[i] != '#'; i++)
            {
                temp = input[i];
                if (temp >= '0' && temp <= '9' && input[i + 1] >= 'a' && input[i + 1] <= 'z') //数字与字母相乘
                    expression = expression + temp + '*';
                /*连续字母*/
                else if (temp >= 'a' && temp <= 'z' && input[i + 1] >= 'a' && input[i + 1] <= 'z')
                {
                    string alpha = "";
                    alpha += temp;
                    /*获取连续字符*/
                    while (input[i + 1] >= 'a' && input[i + 1] <= 'z')
                    {
                        alpha += input[i + 1];
                        i++;
                    }
                    /*连续字母检查是否存在运算符*/
                    while (alpha.Length != 0)   //当字符串不为空时
                    {
                        switch (alpha)
                        {   /*先判断是否为运算符，若不是，则长度减1后继续判断，直至字符串长度为空*/
                            case "sin":
                                expression += "sin";   //字符串为sin运算符
                                alpha = "";  //将alpha置空
                                break;
                            case "cos":
                                expression += "cos";   //字符串为cos运算符
                                alpha = "";  //将alpha置空
                                break;
                            case "tan":
                                expression += "tan";   //字符串为tan运算符
                                alpha = "";  //将alpha置空
                                break;
                            case "abs":
                                expression += "abs";   //字符串为abs运算符
                                alpha = "";  //将alpha置空
                                break;
                            case "log":
                                expression += "log";   //字符串为log运算符
                                alpha = "";  //将alpha置空
                                break;
                            case "sec":
                                expression += "sec";   //字符串为sec运算符
                                alpha = "";  //将alpha置空
                                break;
                            case "floor":
                                expression += "floor";   //字符串为floor运算符
                                alpha = "";  //将alpha置空
                                break;
                            default:   //存在一个变量
                                temp = Convert.ToChar(alpha.Substring(0, 1));   //变量
                                alpha = alpha.Substring(1, alpha.Length - 1);   //运算符
                                foreach (Undefine_variable v in vary)   //判断变量是否已声明
                                {
                                    if (v.variable == temp)
                                    {
                                        isExist = true;
                                        break;
                                    }
                                }
                                if (!isExist)   //变量未声明
                                {
                                    vary[undefine].variable = temp;
                                    vary[undefine].value = r.Next(0, 10);
                                    if(vary[undefine].value<0)//对值为负数的数据作处理
                                        expression = expression + "(0"+Convert.ToString(vary[undefine].value)+")";
                                    else
                                        expression = expression + Convert.ToString(vary[undefine].value);
                                    undefine++;
                                }
                                else   //变量已声明
                                {
                                    for (int j = 0; j < undefine; j++)
                                    {
                                        if (vary[j].variable == temp)
                                        {
                                            if (vary[j].value < 0)//对值为负数的数据作处理
                                                expression = expression + "(0" + Convert.ToString(vary[j].value) + ")";
                                            else
                                                expression = expression + Convert.ToString(vary[j].value);
                                            break;
                                        }
                                    }
                                }
                                if (alpha.Length != 0)  //不是最后一个字符，添加一个'*'
                                    expression += '*';
                                isExist = false;    //将标记isExist初始化
                                break;
                        }
                    }

                }
                else if (temp >= 'a' && temp <= 'z')   //单字母，即变量
                {
                    foreach (Undefine_variable v in vary)   //判断变量是否已声明
                    {
                        if (v.variable == temp)
                        {
                            isExist = true;
                            break;
                        }
                    }
                    if (!isExist)  //变量未声明
                    {
                        vary[undefine].variable = temp;
                        vary[undefine].value = r.Next(0, 10);
                        if (vary[undefine].value < 0)//对值为负数的数据作处理
                            expression = expression + "(0" + Convert.ToString(vary[undefine].value) + ")";
                        else
                            expression = expression + Convert.ToString(vary[undefine].value);
                        undefine++;
                    }
                    else  //变量已声明
                    {
                        for (int j = 0; j < undefine; j++)
                        {
                            if (vary[j].variable == temp)
                            {
                                if (vary[j].value < 0)//对值为负数的数据作处理
                                    expression = expression + "(0" + Convert.ToString(vary[j].value) + ")";
                                else
                                    expression = expression + Convert.ToString(vary[j].value);
                                break;
                            }
                        }
                    }
                    isExist = false;    //将标记isExist初始化
                }
                else if (temp == '(')
                {
                    expression += temp;
                    if (input[i + 1] == '-')//考虑括号后面是否为负数
                        expression += "0";
                    couples++;  //item中未成对的括号数
                }
                else if (temp == ')')
                {
                    expression += temp;
                    couples--;
                    if (input[i + 1] == '(')    //解决括号表达式*括号表达式的情况
                        expression += "*";
                }
                else//单个数字、+、-、*、/
                {
                    if (couples == 0 && (temp == '+' || temp == '-'))
                    {
                        items[sum] = expression;
                        sum++;
                        expression = Convert.ToString(temp);
                    }
                    else
                        expression += temp;
                }
            }
            items[sum] = expression;    //表达式中的最后一个item
            sum++;
        }
        /*表达式的计算*/
        static double Caculate(string[] items, int sum)
        {
            /*存放数据栈*/
            Stack<double> Sd = new Stack<double>();
            /*存放运算符栈*/
            Stack<string> Ss = new Stack<string>();
            double result = 0;
            char temp;
            string alpha;
            /*自定义字符串结束符*/
            sum--;
            while (sum >= 0)
            {
                alpha = ""; //初始化alpha
                /*自定义字符串结束符*/
                items[sum] += '#';
                Sd.Clear(); //初始化sd
                Ss.Clear(); //初始化ss
                Sd.Push(0); //假设栈底元素的值为0，为了处理数值为负数的情况
                for (int i = 0; items[sum][i] != '#'; i++)
                {
                    temp = items[sum][i];
                    alpha += temp;
                    if (temp >= '0' && temp <= '9')   //将数字，包括小数压入栈中
                    {
                        while (items[sum][i] != '#')
                        {
                            if (items[sum][i + 1] >= '0' && items[sum][i + 1] <= '9' || items[sum][i + 1] == '.')
                            {
                                alpha += items[sum][i + 1];
                                i++;
                            }
                            else
                                break;
                        }
                        Sd.Push(Convert.ToDouble(alpha));
                    }
                    else if (temp >= 'a' && temp <= 'z')   //将运算函数压入栈中
                    {
                        while (items[sum][i + 1] != '#')
                        {
                            if (items[sum][i + 1] >= 'a' && items[sum][i + 1] <= 'z')
                            {
                                alpha += items[sum][i + 1];
                                i++;
                            }
                            else
                                break;
                        }
                        Ss.Push(alpha);
                    }
                    else if (temp == ')')   //遇到字符‘(’,将字符栈中的元素出栈,直到‘(’
                    {
                        string tempS;
                        while (!Ss.Peek().Equals("("))
                        {
                            tempS = Ss.Pop();
                            if (tempS.Equals("+"))
                                Sd.Push(Sd.Pop() + Sd.Pop());
                            else if (tempS.Equals("-"))
                                Sd.Push(-Sd.Pop() + Sd.Pop());
                            else if (tempS.Equals("*"))
                                Sd.Push(Sd.Pop() * Sd.Pop());
                            else if (tempS.Equals("/"))
                                Sd.Push(1 / Sd.Pop() * Sd.Pop());
                            else if (tempS.Equals("sin"))
                                Sd.Push(Math.Sin(Sd.Pop()));
                            else if (tempS.Equals("cos"))
                                Sd.Push(Math.Cos(Sd.Pop()));
                            else if (tempS.Equals("tan"))
                                Sd.Push(Math.Tan(Sd.Pop()));
                            else if (tempS.Equals("sec"))
                                Sd.Push(1 / Math.Cos(Sd.Pop()));
                            else if (tempS.Equals("^"))//有错误
                            {
                                double top = Sd.Pop();
                                Sd.Push(Math.Pow(Sd.Pop(), top));
                            }
                            else if (tempS.Equals("log"))
                                Sd.Push(1 / Math.Log10(Sd.Pop()) * Math.Log10(Sd.Pop()));
                            else if (tempS.Equals("abs"))
                                Sd.Push(Math.Abs(Sd.Pop()));
                            else if (tempS.Equals("floor"))
                                Sd.Push(Math.Floor(Sd.Pop())); ;
                        }
                        Ss.Pop();   //将字符'('出栈
                    }
                    else
                    {
                        switch (alpha)
                        {
                            /*将运算符+、-压入字符栈中*/
                            case "+":
                            case "-":
                                while (Ss.Count != 0 && !Ss.Peek().Equals("("))
                                {
                                    string tempS;
                                    tempS = Ss.Pop();
                                    if (tempS.Equals("+"))
                                        Sd.Push(Sd.Pop() + Sd.Pop());
                                    else if (tempS.Equals("-"))
                                        Sd.Push(-Sd.Pop() + Sd.Pop());
                                    else if (tempS.Equals("*"))
                                        Sd.Push(Sd.Pop() * Sd.Pop());
                                    else if (tempS.Equals("/"))
                                        Sd.Push(1 / Sd.Pop() * Sd.Pop());
                                    else if (tempS.Equals("sin"))
                                        Sd.Push(Math.Sin(Sd.Pop()));
                                    else if (tempS.Equals("cos"))
                                        Sd.Push(Math.Cos(Sd.Pop()));
                                    else if (tempS.Equals("tan"))
                                        Sd.Push(Math.Tan(Sd.Pop()));
                                    else if (tempS.Equals("sec"))
                                        Sd.Push(1 / Math.Cos(Sd.Pop()));
                                    else if (tempS.Equals("^"))
                                    {
                                        double top = Sd.Pop();
                                        Sd.Push(Math.Pow(Sd.Pop(), top));
                                    }
                                    else if (tempS.Equals("log"))
                                        Sd.Push(1 / Math.Log10(Sd.Pop()) * Math.Log10(Sd.Pop()));
                                    else if (tempS.Equals("abs"))
                                        Sd.Push(Math.Abs(Sd.Pop()));
                                    else if (tempS.Equals("floor"))
                                        Sd.Push(Math.Floor(Sd.Pop()));
                                }
                                Ss.Push(alpha);
                                break;
                            /*将运算符*、/压入字符栈中*/
                            case "*":
                            case "/":
                                while (Ss.Count != 0 && !Ss.Peek().Equals("("))
                                {
                                    string tempS;
                                    tempS = Ss.Pop();
                                    if (tempS.Equals("*"))
                                        Sd.Push(Sd.Pop() * Sd.Pop());
                                    else if (tempS.Equals("/"))
                                        Sd.Push(1 / Sd.Pop() * Sd.Pop());
                                    else if (tempS.Equals("sin"))
                                        Sd.Push(Math.Sin(Sd.Pop()));
                                    else if (tempS.Equals("cos"))
                                        Sd.Push(Math.Cos(Sd.Pop()));
                                    else if (tempS.Equals("tan"))
                                        Sd.Push(Math.Tan(Sd.Pop()));
                                    else if (tempS.Equals("sec"))
                                        Sd.Push(1 / Math.Cos(Sd.Pop()));
                                    else if (tempS.Equals("^"))
                                    {
                                        double top = Sd.Pop();
                                        Sd.Push(Math.Pow(Sd.Pop(), top));
                                    }
                                    else if (tempS.Equals("log"))
                                        Sd.Push(1 / Math.Log10(Sd.Pop()) * Math.Log10(Sd.Pop()));
                                    else if (tempS.Equals("abs"))
                                        Sd.Push(Math.Abs(Sd.Pop()));
                                    else if (tempS.Equals("floor"))
                                        Sd.Push(Math.Floor(Sd.Pop()));
                                    else
                                    {
                                        Ss.Push(tempS);
                                        break;
                                    }
                                }
                                Ss.Push(alpha);
                                break;
                            case "^":
                                Ss.Push(alpha);
                                break;
                            /*将'('压入占中*/
                            case "(":
                                Ss.Push(alpha);
                                break;
                        }
                    }
                    alpha = "";
                }
                /*字符串检测完毕，运算符栈的元素依次退栈*/
                while (Ss.Count != 0)
                {
                    alpha = Ss.Pop();
                    if (alpha.Equals("+"))
                        Sd.Push(Sd.Pop() + Sd.Pop());
                    else if (alpha.Equals("-"))
                        Sd.Push(-Sd.Pop() + Sd.Pop());
                    else if (alpha.Equals("*"))
                        Sd.Push(Sd.Pop() * Sd.Pop());
                    else if (alpha.Equals("/"))
                        Sd.Push(1 / Sd.Pop() * Sd.Pop());
                    else if (alpha.Equals("sin"))
                        Sd.Push(Math.Sin(Sd.Pop()));
                    else if (alpha.Equals("cos"))
                        Sd.Push(Math.Cos(Sd.Pop()));
                    else if (alpha.Equals("tan"))
                        Sd.Push(Math.Tan(Sd.Pop()));
                    else if (alpha.Equals("sec"))
                        Sd.Push(1 / Math.Cos(Sd.Pop()));
                    else if (alpha.Equals("^"))
                    {
                        double top = Sd.Pop();
                        Sd.Push(Math.Pow(Sd.Pop(), top));
                    }
                    else if (alpha.Equals("log"))
                        Sd.Push(1 / Math.Log10(Sd.Pop()) * Math.Log10(Sd.Pop()));
                    else if (alpha.Equals("abs"))
                        Sd.Push(Math.Abs(Sd.Pop()));
                    else if (alpha.Equals("floor"))
                        Sd.Push(Math.Floor(Sd.Pop()));
                }
                result += Sd.Peek();   //item最终计算的结果即为数字栈的顶端元素
                sum--;
            }
            return result;  //返回最终结果
        }

    }
    //定义一个结构体来存放未声明的变量
    struct Undefine_variable
    {
        public char variable;   //字符
        public double value;    //字符的值
    }

}

