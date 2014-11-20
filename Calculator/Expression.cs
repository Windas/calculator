#region 引用名称空间
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
#endregion

namespace Calculator
{
	public class Expression : Ioperate
	{
		#region 定义栈
		private Stack<string> IntStack = new Stack<string>();       //数字栈
		private Stack<string> OpStack = new Stack<string>();       //符号栈 
		#endregion

		#region 判断是否为数字
		public bool IsNumber(string ch)
		{
			Regex RegInput = new Regex(@"^[0-9\.]");       //用正则表达式判断
			if (RegInput.IsMatch(ch.ToString()))
			{
				return true;
			}
			else
				return false;
		}
		#endregion

		#region 定义运算符优先级
		public static int priority(String Operator)
		{
			if (Operator == ("+") || Operator == ("-"))
			{
				return 1;
			}
			else if (Operator == ("*") || Operator == ("/"))
			{
				return 2;
			}
			else if (Operator == ("^"))
			{
				return 3;
			}
			else if (Operator == ("S") || Operator == ("C") || Operator == ("T"))
			{
				return 4;
			}
			else
			{
				return 0;
			}
		}
		#endregion

		#region 比较运算符的优先级
		public int CompareOperate(string ch, string stackCh)
		{
			int intCh = priority(ch.ToString());
			int intSCh = priority(stackCh.ToString());
			if (intCh == intSCh)
			{
				return 0;
			}
			else if (intCh < intSCh)
			{
				return -1;
			}
			else if (intCh > intSCh)
			{
				return 1;
			}
			else
				return -2;
		}
		#endregion

		#region 调用getResult将最后结果显示
		public string GetExpression(string InputString)
		{
			bool HasMark = true;    //这个布尔变量用于判断数字之后是否有符号，作用是用来得到9以上数字
			InputString += "#";     //在字符串最后加上#
			#region 入栈
			for (int i = 0; i < InputString.Length; i++)
			{
				string ch = InputString[i].ToString();  //遍历每一个字符
				if (ch == "#")      //判断是否遍历到最后一个字符
				{
					break;      //是则跳出循环
				}
				else
				{
					string chNext = InputString[i + 1].ToString();
					#region "数字的判别与处理"
					if (IsNumber(ch) == true)  //如果是数字的话，要把数字放入数字栈IntStack中
					{
						if (HasMark == true)//如果数字之后有符号，则数字栈顶字符串不变更
						{
							IntStack.Push(ch);
							HasMark = false;    //这个数字之后是什么还未知，所以先将其定义为数字
						}
						else
						{
							IntStack.Push(IntStack.Pop() + ch);//如果数字之后没有符号，则数字栈顶字符串加上新字符串.
						}
					}

					#endregion

					#region "符号的判别与处理"
					else if (ch == "(")
					{
						OpStack.Push(ch);
					}
					else if (ch == ")")
					{
						while (OpStack.Peek() != "(")      //当下一个字符不为左括号
						{
							string mark = OpStack.Pop().ToString();
							string op1 = IntStack.Pop().ToString();
							string op2 = IntStack.Pop().ToString();
							IntStack.Push(getResult(mark, op1, op2));//调用getResult方法，获取计算结果值，并推入数字栈
						}
						OpStack.Pop();      //弹出左括号
					}
					else if (ch == "+" || ch == "-" || ch == "*" || ch == "/" || ch == "^" || ch == "S" || ch == "C" || ch == "T")
					{
						if (chNext == "-")      //处理负号
						{
							OpStack.Push(ch);
							IntStack.Push(chNext);
							i += 1;
							HasMark = false;    //输入了符号，HasMark为false
						}
						else
						{
							if (OpStack.Count == 0)
							{
								OpStack.Push(ch);
							}
							else if (OpStack.Peek() == "(")
							{
								OpStack.Push(ch);
							}
							else if (CompareOperate(ch, OpStack.Peek()) == 1)
							{
								OpStack.Push(ch);
							}
							else if (CompareOperate(ch, OpStack.Peek()) == 0)
							{
								string mark = OpStack.Pop().ToString();
								string op1 = IntStack.Pop().ToString();
								string op2 = IntStack.Pop().ToString();
								IntStack.Push(getResult(mark, op1, op2));//调用getResult方法，获取计算结果值，并推入数字栈
								OpStack.Push(ch);               //把符号推入符号栈
							}
							else if (CompareOperate(ch, OpStack.Peek()) == -1)
							{
								int com = -1;
								while (com == -1 || com == 0)
								{
									string mark = OpStack.Pop().ToString();
									string op1 = IntStack.Pop().ToString();
									string op2 = IntStack.Pop().ToString();
									IntStack.Push(getResult(mark, op1, op2));//调用getResult方法，获取计算结果值，并推入数字栈
									if (OpStack.Count != 0)
									{
										com = CompareOperate(ch, OpStack.Peek());
									}
									else
									{
										break;
									}
								}
								OpStack.Push(ch);//把符号推入符号栈
							}
							HasMark = true;//输入了运算符，HasMark为true
						}
					}
				}
					#endregion
			}
			#endregion
			#region "返回计算结果值"

			for (int i = 0; i < OpStack.Count + 1; i++)      //循环得出结果
			{
				try
				{
					string mark = OpStack.Pop().ToString();
					string op1 = IntStack.Pop().ToString();
					string op2 = IntStack.Pop().ToString();
					IntStack.Push(getResult(mark, op1, op2));//调用getResult方法，获取计算结果值，并推入数字栈
				}
				catch
				{
					return "Error";
				}
			}
			return IntStack.Pop();  //返回最终结果

			#endregion
		}
		#endregion

		#region getResult返回运算结果
		public static string getResult(String Myoperator, String a, String b)
		{
			try
			{
				string op = Myoperator;
				string rs = string.Empty;
				//double x = Double.Parse(b);
				//double y = Double.Parse(a);
				decimal x = System.Convert.ToDecimal(b);
				decimal y = System.Convert.ToDecimal(a);
				decimal z = 0;
				if (op == ("+"))
				{
					z = x + y;
				}
				else if (op == ("-"))
				{
					z = x - y;
				}
				else if (op == ("*"))
				{
					z = x * y;
				}
				else if (op == ("/"))
				{
					z = x / y;
				}
				else if (op == ("^"))
				{
					z = Convert.ToDecimal(Math.Pow(Convert.ToDouble(x), Convert.ToDouble(y)));
				}
				else if (op == ("S"))
				{
					z = Convert.ToDecimal(Math.Sin(Convert.ToDouble(y) * Math.PI / 180));
				}
				else if (op == ("C"))
				{
					z = Convert.ToDecimal(Math.Cos(Convert.ToDouble(y) * Math.PI / 180));
				}
				else if (op == ("T"))
				{
					z = Convert.ToDecimal(Math.Tan(Convert.ToDouble(y) * Math.PI / 180));
				}
				else
				{
					z = 0;
				}
				return rs + z;
			}
			catch (IndexOutOfRangeException)
			{
				return "Error";
			}
		}

		#endregion

	}
}
