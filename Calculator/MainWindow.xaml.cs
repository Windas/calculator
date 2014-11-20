#region 引用名称空间
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
using System.Text.RegularExpressions;
using IronRuby;                                  //引入ironRuby的dll库，用于进行高精度阶乘计算
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System.Reflection;
#endregion

namespace Calculator
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{
		#region 主窗口函数
		public MainWindow()
		{
			InitializeComponent();

			//为按钮添加鼠标左击事件响应，否则Button控件无法响应MouseLeftButtonDown等事件
			Store1.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(Store1_MouseLeftButtonDown), true);           
			Store1.AddHandler(Button.MouseRightButtonDownEvent, new MouseButtonEventHandler(Store1_MouseRightButtonDown), true);

			Store2.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(Store2_MouseLeftButtonDown), true);
			Store2.AddHandler(Button.MouseRightButtonDownEvent, new MouseButtonEventHandler(Store2_MouseRightButtonDown), true);

			Store3.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(Store3_MouseLeftButtonDown), true);
			Store3.AddHandler(Button.MouseRightButtonDownEvent, new MouseButtonEventHandler(Store3_MouseRightButtonDown), true);

			Store4.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(Store4_MouseLeftButtonDown), true);
			Store4.AddHandler(Button.MouseRightButtonDownEvent, new MouseButtonEventHandler(Store4_MouseRightButtonDown), true);

			Store5.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(Store5_MouseLeftButtonDown), true);
			Store5.AddHandler(Button.MouseRightButtonDownEvent, new MouseButtonEventHandler(Store5_MouseRightButtonDown), true);

			Store6.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(Store6_MouseLeftButtonDown), true);
			Store6.AddHandler(Button.MouseRightButtonDownEvent, new MouseButtonEventHandler(Store6_MouseRightButtonDown), true);

			btnX.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(btnX_MouseLeftButtonDown), true);
			btnX.AddHandler(Button.MouseRightButtonDownEvent, new MouseButtonEventHandler(btnX_MouseRightButtonDown), true);

			btnY.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(btnY_MouseLeftButtonDown), true);
			btnY.AddHandler(Button.MouseRightButtonDownEvent, new MouseButtonEventHandler(btnY_MouseRightButtonDown), true);

			btnZ.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(btnZ_MouseLeftButtonDown), true);
			btnZ.AddHandler(Button.MouseRightButtonDownEvent, new MouseButtonEventHandler(btnZ_MouseRightButtonDown), true);

			btnFun.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(btnFun_MouseLeftButtonDown), true);

			btnEFun.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(btnEFun_MouseLeftButtonDown), true);
			btnSFun.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(btnSFun_MouseLeftButtonDown), true);
		}
		#endregion

		#region 定义变量
		Regex RegNum = new Regex(@"^[0-9\.]*$");    //数字的正则表达式
		Regex RegBegin = new Regex(@"^[0-9\-(]");   //第一个字符的正则表达式，必须是数字、负号和左括号
		Regex RegMark = new Regex(@"[-+*/^\()=.]");     //运算符的正则表达式
		Regex RegAll = new Regex(@"[0-9\-(\-+*/^\()=.SCT]");//所有运算符、数字的正则表达式
		bool Start = true;      //判断是否为第一个字符
		bool IsMark = true;     //判断是否可以输入数字，为真，允许输入数字
		bool IsRight = false;   //判断最后一个字符是否为右括号，避免出现 )5的情况
		bool IsDot = false;     //判断是否可以输入小数点
		int BeforeLen = 0;       //判断输入运算符之前的字符串长度
		List<bool> LeftList = new List<bool>(); //存储左括号个数
		string RealStr = string.Empty;
		string FinalResult = string.Empty;          //存储最终结果

		string S1 = string.Empty;           //存储当前结果的变量，由用户选择存储
		string S2 = string.Empty;
		string S3 = string.Empty;
		string S4 = string.Empty;
		string S5 = string.Empty;
		string S6 = string.Empty;
		string X = string.Empty;
		string Y = string.Empty;
		string Z = string.Empty;

		int system = 10;                    //用于记录当前数据的进制

		bool IsFunction = false;            //用于判断函数模式是否打开
		string Function = string.Empty;      //存储用户定义的函数表达式
		#endregion

		#region 数字按钮
		private void btn7_Click(object sender, RoutedEventArgs e)
		{
			PressNumBtn("7");
		}

		private void btn8_Click(object sender, RoutedEventArgs e)
		{
			PressNumBtn("8");
		}

		private void btn9_Click(object sender, RoutedEventArgs e)
		{
			PressNumBtn("9");
		}

		private void btn5_Click(object sender, RoutedEventArgs e)
		{
			PressNumBtn("5");
		}

		private void btn2_Click(object sender, RoutedEventArgs e)
		{
			PressNumBtn("2");
		}

		private void btn0_Click(object sender, RoutedEventArgs e)
		{
			PressNumBtn("0");
		}

		private void btn6_Click(object sender, RoutedEventArgs e)
		{
			PressNumBtn("6");
		}

		private void btn3_Click(object sender, RoutedEventArgs e)
		{
			PressNumBtn("3");
		}
		private void btn4_Click(object sender, RoutedEventArgs e)
		{
			PressNumBtn("4");
		}

		private void btn1_Click(object sender, RoutedEventArgs e)
		{
			PressNumBtn("1");
		}
		#endregion

		#region 科学运算按钮
		private void btnPi_Click(object sender, RoutedEventArgs e)
		{
			this.InputText.Text += Math.PI.ToString();
			IsDot = true;
		}

		private void btnLn_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				double a = Convert.ToDouble(this.InputText.Text);
				this.InputText.Text = (Math.Log(a)).ToString();
			}
			catch
			{
				this.InputText.Text = "无效输入";
			}
		}

		private void btnSqrt_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				double a = Convert.ToDouble(this.InputText.Text);
				this.InputText.Text = (Math.Sqrt(a)).ToString();
			}
			catch
			{
				this.InputText.Text = "无效输入";
			}
		}

		private void btnE_Click(object sender, RoutedEventArgs e)
		{
			this.InputText.Text += Math.E.ToString();
			IsDot = true;
		}

		private void btnN_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (this.InputText.Text.Contains("."))
				{
					this.InputText.Text = "Error";
				}
				else if (this.InputText.Text.Contains("-"))
				{
					this.InputText.Text = "Error";
				}
				else
				{
					int a = int.Parse(this.InputText.Text);

					//嵌入ruby脚本进行阶乘运算
					const string Rubycode = @"
class Jiecheng
	def test(a)
		if a==1
			1
		else
			a*test(a-1)
		end
	end
end";
					ScriptRuntime runtime = Ruby.CreateRuntime();
					ScriptEngine engine = Ruby.GetEngine(runtime);
					ScriptScope scope = engine.CreateScope();

					runtime.LoadAssembly(Assembly.GetExecutingAssembly());
					engine.Execute(Rubycode, scope);

					dynamic globals = engine.Runtime.Globals;
					dynamic house = globals.Jiecheng.@new();
					this.InputText.Text = System.Convert.ToString(house.test(a));
				}
			}
			//catch (StackOverflowException ex)                         //该异常在堆栈消耗完的情况下不会被捕获，由于StackOverflow发生在CLR内部，且内存已被耗尽，因此finally程序块不会被执行，StackOverflow无法被任何异常处理块捕获
			//{
			//	this.InputText.Text = "StackOverFlow";
			//}
			catch
			{
				this.InputText.Text = "Error";
			}
		}

		private void btnSqu_Click(object sender, RoutedEventArgs e)
		{
			PressMathBtn("^");
		}


		private void btnDS_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (this.InputText.Text != "0")
				{
					double a = Convert.ToDouble(this.InputText.Text);
					this.InputText.Text = (1 / a).ToString();
				}
			}
			catch
			{
				this.InputText.Text = "Error";
			}
		}

		private void btnFu_Click(object sender, RoutedEventArgs e)
		{
			this.InputText.Text += "-";
		}

		private void btnSin_Click(object sender, RoutedEventArgs e)
		{
			string Txt = this.InputText.Text;
			if (Txt.Trim() == string.Empty || (!RegNum.IsMatch(Txt.Substring(Txt.Length - 1, 1)) && Txt.Substring(Txt.Length - 1, 1) != ")" && IsDot == false && Txt.Substring(Txt.Length - 1, 1) != "n" && Txt.Substring(Txt.Length - 1, 1) != "s"))
			{
				this.InputText.Text += "sin";
				IsMark = true;      //输入了sin，最后一个字符就是运算符了
				Start = false;
			}
			this.GetFocus();
		}

		private void btnCos_Click(object sender, RoutedEventArgs e)
		{
			string Txt = this.InputText.Text;
			if (Txt.Trim() == string.Empty || (!RegNum.IsMatch(Txt.Substring(Txt.Length - 1, 1)) && Txt.Substring(Txt.Length - 1, 1) != ")" && IsDot == false && Txt.Substring(Txt.Length - 1, 1) != "n" && Txt.Substring(Txt.Length - 1, 1) != "s"))
			{
				this.InputText.Text += "cos";
				IsMark = true;//输入了cos，最后一个字符就是运算符了
				Start = false;
			}
			this.GetFocus();
		}

		private void btnTan_Click(object sender, RoutedEventArgs e)
		{
			string Txt = this.InputText.Text;
			if (Txt.Trim() == string.Empty || (!RegNum.IsMatch(Txt.Substring(Txt.Length - 1, 1)) && Txt.Substring(Txt.Length - 1, 1) != ")" && IsDot == false && Txt.Substring(Txt.Length - 1, 1) != "n" && Txt.Substring(Txt.Length - 1, 1) != "s"))
			{
				this.InputText.Text += "tan";
				IsMark = true;//输入了tan，最后一个字符就是运算符了
				Start = false;
			}
			this.GetFocus();
		}
		#endregion

		#region 基本运算按钮
		private void btnMinus_Click(object sender, RoutedEventArgs e)
		{
			PressMathBtn("-");
		}

		private void btnPlus_Click(object sender, RoutedEventArgs e)
		{
			PressMathBtn("+");
		}
		private void btnTimes_Click(object sender, RoutedEventArgs e)
		{
			PressMathBtn("*");
		}

		private void btnDiv_Click(object sender, RoutedEventArgs e)
		{
			PressMathBtn("/");
		}

		private void btnEqual_Click(object sender, RoutedEventArgs e)
		{
			this.Equal();
		}


		private void btnLeft_Click(object sender, RoutedEventArgs e)
		{
			string Txt = this.InputText.Text;
			if (Txt.Trim() == string.Empty || (!RegNum.IsMatch(Txt.Substring(Txt.Length - 1, 1)) && Txt.Substring(Txt.Length - 1, 1) != ")" && IsDot == false) || Txt.Trim().EndsWith("n") || Txt.Trim().EndsWith("s"))
			{
				this.InputText.Text = this.InputText.Text + "(";
				IsMark = true;//输入了左括号，最后一个字符就是运算符了
				this.LeftList.Add(true);
			}
			this.GetFocus();
		}

		private void btnRight_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				string Txt = this.InputText.Text;
				if (LeftList.Contains(true) && (RegNum.IsMatch(Txt.Substring(Txt.Length - 1, 1)) || Txt.Substring(Txt.Length - 1, 1) == ")"))
				{
					this.PressMathBtn(")");
					this.LeftList.Remove(true);
					IsMark = false;//输入了右括号，最后一个字符不是运算符了
					IsRight = true;//输入了右括号，最后一个字符是右括号
				}
				this.GetFocus();
			}
			catch
			{
				this.InputText.Text = "Error";
			}
		}

		private void btnDot_Click(object sender, RoutedEventArgs e)
		{
			string Txt = this.InputText.Text;
			if (IsDot == false && !Txt.EndsWith("n") && !Txt.EndsWith("s") && Txt != "")
			{
				this.PressMathBtn(".");
				IsDot = true;
			}
			this.GetFocus();
		}

		private void btnC_Click(object sender, RoutedEventArgs e)
		{
			this.InputText.Text = string.Empty.Trim();
			Start = true;
			IsMark = true;
			IsRight = false;
			IsDot = false;
			BeforeLen = 0;
			LeftList.Clear();
			RealStr = string.Empty;
			FinalResult = string.Empty;

			system = 10;
			Sys.Content = system;
		}

		private void btnDel_Click(object sender, RoutedEventArgs e)
		{
			Del();
		}
		#endregion

		#region 功能函数
		private void PressNumBtn(string NumTxt)     //数字键处理
		{
			if (IsRight == false)                                         //判断之前有没有右括号，有的话不允许输入数字
			{
				if (!this.InputText.Text.Contains("="))
				{
					this.InputText.Text += NumTxt;
					IsMark = false;                                       //输入数字后，则最后一个字符不是运算符
				}
				else
				{
					this.InputText.Text = string.Empty;
					this.InputText.Text = this.FinalResult + NumTxt;
					IsMark = false;
				}
			}
			this.GetFocus();
		}

		private void PressMathBtn(string MarkTxt)       //符号键处理
		{
			if (MarkTxt != "." && IsMark == false)
			{
				if (FinalResult == string.Empty)
				{
					this.InputText.Text += MarkTxt;
				}
				else
				{
					if (this.InputText.Text.Contains("="))
					{
						this.InputText.Text = FinalResult;
					}
					else
					{
						this.InputText.Text += MarkTxt;
					}
				}
				IsMark = true;                                     //输入了运算符，最后一个字符就是运算符了
				IsDot = false;                                     //输入了运算符，最后一个字符就不是小数点了
				IsRight = false;                                   //输入了运算符，最后一个字符就不是右括号了
				Start = false;
				this.ZeroTwoChk(this.InputText.Text.Trim(), BeforeLen);
			}
			else
			{
				if (IsMark == false)
				{
					if (FinalResult == string.Empty)
					{
						this.InputText.Text += MarkTxt;
					}
					else
					{
						if (this.InputText.Text.Contains("="))
						{
							this.InputText.Text = FinalResult;
						}
						else
						{
							this.InputText.Text += MarkTxt;
						}
					}
					IsMark = true;                                 //输入了运算符，最后一个字符就是运算符了
					IsDot = false;                                 //输入了运算符，最后一个字符就不是小数点了
					IsRight = false;                               //输入了运算符，最后一个字符就不是右括号了
					Start = false;
				}
			}
			this.GetFocus();
		}

		private void Equal()        //等于号处理
		{
			try
			{
				if (this.InputText.Text.Contains("/0"))   //除数不能为0
				{
					if (this.InputText.Text.Contains("/0.")) { }     //除号后为小数的情况
					else
						this.InputText.Text = "除数不能为零";
				}
				if (this.InputText.Text.Contains("tan90"))   //判断tan90为无效输入
				{
					this.InputText.Text = "无效输入";
				}
				else if (this.InputText.Text.Contains("="))
				{
					this.InputText.Text = FinalResult;
				}
				else if (IsMark == false)
				{
					RealStr = this.InputText.Text.Trim() + "=";
					this.ZeroTwoChk(RealStr, BeforeLen);
					RealStr = this.InputText.Text.Trim();
					if (RealStr.StartsWith("-"))        //开始为负号时
					{
						RealStr = "0" + RealStr;
					}
					this.Calculate();
					IsRight = false;
				}
				this.GetFocus();
			}
			catch
			{ }
		}

		private void Del()     //退格键处理
		{
			string Txt = this.InputText.Text;
			if (Txt.Length >= 1)
			{
				string L = Txt.Substring(Txt.Length - 1, 1);    //取前一个字符
				if (L == "(")
				{
					this.LeftList.Remove(true);     //左括号列表的记录删除一个
					this.InputText.Text = Txt.Remove(Txt.Length - 1, 1);
				}
				else if (L == ")")
				{
					this.LeftList.Add(true);    //左括号列表的记录增加一个
					this.InputText.Text = Txt.Remove(Txt.Length - 1, 1);
					IsRight = false;
				}
				else if (L == "n" || L == "s")
				{
					this.InputText.Text = Txt.Remove(Txt.Length - 3, 3);
				}
				else
					this.InputText.Text = Txt.Remove(Txt.Length - 1, 1); //删除最后一个字符
				IsMark = false;
			}
		}

		private string CheckLegal(string StrInput, bool IsStart, List<bool> List)
		{
			if (RegAll.IsMatch(StrInput))
			{
				if (IsStart == true)
				{
					if (RegBegin.IsMatch(StrInput))
					{
						Start = false;
						return "GoOn";      //输入的第一个字符合格，继续
					}
					else
					{
						return "RollBack";      //输入的第一个字符不合格则，输入无效
					}

				}
				else
				{
					if (StrInput.Contains("="))
					{
						if (List.Contains(true))
						{
							return "LeftProblem";   //括号没有完全，则返回“左括号问题”，应用于等号按键的按下
						}
						else
						{
							return "Over";      //算式合符规范，返回计算结束
						}
					}
					else
					{
						return "GoOn";      //不是等号，则继续
					}
				}
			}
			else
			{
				return "RollBack";      //其他情况，输入无效
			}
		}

		private void Calculate()
		{
			if (this.CheckLegal(RealStr, Start, LeftList) == "Over")
			{
				Expression Exp = new Expression();
				FinalResult = Exp.GetExpression(RealStr.Replace("=", "").Replace("sin", "1S").Replace("cos", "1C").Replace("tan", "1T"));
				if (FinalResult == "Error")
				{
					FinalResult = "Error";
				}
				else
				{
					this.InputText.Text = FinalResult;           //显示最后结果
					if (FinalResult.Contains("."))
					{
						IsDot = true;
					}
					if (RealStr.StartsWith("0-"))
					{
						this.InputText.Text = this.InputText.Text.TrimStart('0');
					}
				}
			}
			else if (this.CheckLegal(RealStr, Start, LeftList) == "RollBack")
			{
				FinalResult = "Error";
			}
			else if (this.CheckLegal(RealStr, Start, LeftList) == "LeftProblem")
			{
				FinalResult = "Error";
			}
		}

		private void ZeroTwoChk(string inPutTxt, int beforeLen)    //处理0的问题，如两个0和4.30等
		{
			try
			{
				string lastMark = inPutTxt.Trim().Substring(inPutTxt.Trim().Length - 1, 1);
				if (beforeLen != 0)
				{
					string beforeStr = inPutTxt.Trim().Substring(0, beforeLen);
					string nowStr = inPutTxt.Trim().Substring(beforeLen, inPutTxt.Length - beforeLen - 1);
					if (nowStr.Contains("."))
					{
						nowStr = nowStr.Trim('0');
						if (nowStr.StartsWith(".") && !nowStr.EndsWith("."))
						{
							nowStr = "0" + nowStr;
						}
						if (nowStr == ".")
						{
							nowStr = "0";
						}
						if (nowStr.EndsWith("."))
						{
							nowStr = nowStr.TrimEnd('.');
						}
					}
					else
					{
						if (nowStr != string.Empty)
						{
							nowStr = nowStr.TrimStart('0');
							if (nowStr == string.Empty)
							{
								nowStr = "0";
							}
						}
					}
					this.InputText.Text = beforeStr + nowStr + lastMark;
				}
				else
				{
					inPutTxt = inPutTxt.Trim().Substring(0, inPutTxt.Length - 1);
					if (inPutTxt.Contains("."))
					{
						inPutTxt = inPutTxt.Trim('0');
						if (inPutTxt.StartsWith(".") && !inPutTxt.EndsWith("."))
						{
							inPutTxt = "0" + inPutTxt;
						}
						if (inPutTxt == ".")
						{
							inPutTxt = "0";
						}
						if (inPutTxt.EndsWith("."))
						{
							inPutTxt = inPutTxt.TrimEnd('.');
						}
					}
					else
					{
						inPutTxt = inPutTxt.TrimStart('0');
						if (inPutTxt == string.Empty)
						{
							inPutTxt = "0";
						}
					}
					this.InputText.Text = inPutTxt + lastMark;
				}
			}
			catch { }
			BeforeLen = this.InputText.Text.Trim().Length;
		}

		private void GetFocus()         //将焦点定位在最后一个字符之后
		{
			if (this.InputText.IsFocused == false)
			{
				this.InputText.Focus();
				this.InputText.ScrollToEnd();
				this.InputText.SelectionStart = this.InputText.GetLineLength(0);
			}
		}
		#endregion

		#region 存储功能按钮
		private void Store1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;                       //激活事件处理程序

			if (S1 != string.Empty)                 //如果存储变量不为空则打印到显示口,且不允许存储非十进制数据
			{
				InputText.Text += S1;
				IsMark = false;                     //最后输入的不是运算符，以便让之后可以继续输入运算符
			}
		}

		private void Store1_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;                        //激活事件处理程序
			try
			{
				if (system != 10)
				{
					const string Rubycode = @"
class HexChange
	def fHex(a)
		a.hex
	end
	
	def fOct(a)
		a.oct
	end

	def fBin(a)
		Integer('0b'+a)
	end
end";
					ScriptRuntime runtime = Ruby.CreateRuntime();
					ScriptEngine engine = Ruby.GetEngine(runtime);
					ScriptScope scope = engine.CreateScope();

					runtime.LoadAssembly(Assembly.GetExecutingAssembly());
					engine.Execute(Rubycode, scope);

					dynamic globals = engine.Runtime.Globals;
					dynamic house = globals.HexChange.@new();
					if (system == 16)
						S1 = System.Convert.ToString(house.fHex(InputText.Text));
					else if (system == 8)
						S1 = System.Convert.ToString(house.fOct(InputText.Text));
					else if (system == 2)
						S1 = System.Convert.ToString(house.fBin(InputText.Text));
					system = 10;

					Sys.Content = system;
					InputText.Text = S1;
					return;
				}
				double a = Convert.ToDouble(this.InputText.Text);
				S1 = InputText.Text;
			}
			catch
			{
				this.InputText.Text = "Error";
			}
		}

		private void Store2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;

			if (S2 != string.Empty)
			{
				InputText.Text += S2;
				IsMark = false;
			}
		}

		private void Store2_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;

			try
			{
				if (system != 10)
				{
					const string Rubycode = @"
class HexChange
	def fHex(a)
		a.hex
	end
	
	def fOct(a)
		a.oct
	end

	def fBin(a)
		Integer('0b'+a)
	end
end";
					ScriptRuntime runtime = Ruby.CreateRuntime();
					ScriptEngine engine = Ruby.GetEngine(runtime);
					ScriptScope scope = engine.CreateScope();

					runtime.LoadAssembly(Assembly.GetExecutingAssembly());
					engine.Execute(Rubycode, scope);

					dynamic globals = engine.Runtime.Globals;
					dynamic house = globals.HexChange.@new();
					if (system == 16)
						S2 = System.Convert.ToString(house.fHex(InputText.Text));
					else if (system == 8)
						S2 = System.Convert.ToString(house.fOct(InputText.Text));
					else if (system == 2)
						S2 = System.Convert.ToString(house.fBin(InputText.Text));
					system = 10;

					Sys.Content = system;
					InputText.Text = S2;
					return;
				}
				double a = Convert.ToDouble(this.InputText.Text);
				S2 = InputText.Text;
			}
			catch
			{
				this.InputText.Text = "Error";
			}
		}

		private void Store3_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;

			if (S3 != string.Empty)
			{
				InputText.Text += S3;
				IsMark = false;
			}
		}

		private void Store3_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;

			try
			{
				if (system != 10)
				{
					const string Rubycode = @"
class HexChange
	def fHex(a)
		a.hex
	end
	
	def fOct(a)
		a.oct
	end

	def fBin(a)
		Integer('0b'+a)
	end
end";
					ScriptRuntime runtime = Ruby.CreateRuntime();
					ScriptEngine engine = Ruby.GetEngine(runtime);
					ScriptScope scope = engine.CreateScope();

					runtime.LoadAssembly(Assembly.GetExecutingAssembly());
					engine.Execute(Rubycode, scope);

					dynamic globals = engine.Runtime.Globals;
					dynamic house = globals.HexChange.@new();
					if (system == 16)
						S3 = System.Convert.ToString(house.fHex(InputText.Text));
					else if (system == 8)
						S3 = System.Convert.ToString(house.fOct(InputText.Text));
					else if (system == 2)
						S3 = System.Convert.ToString(house.fBin(InputText.Text));
					system = 10;

					Sys.Content = system;
					InputText.Text = S3;
					return;
				}
				double a = Convert.ToDouble(this.InputText.Text);
				S3 = InputText.Text;
			}
			catch
			{
				this.InputText.Text = "Error";
			}
		}
		private void Store6_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;

			if (S6 != string.Empty)
			{
				InputText.Text += S6;
				IsMark = false;
			}
		}

		private void Store6_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			try
			{
				if (system != 10)
				{
					const string Rubycode = @"
class HexChange
	def fHex(a)
		a.hex
	end
	
	def fOct(a)
		a.oct
	end

	def fBin(a)
		Integer('0b'+a)
	end
end";
					ScriptRuntime runtime = Ruby.CreateRuntime();
					ScriptEngine engine = Ruby.GetEngine(runtime);
					ScriptScope scope = engine.CreateScope();

					runtime.LoadAssembly(Assembly.GetExecutingAssembly());
					engine.Execute(Rubycode, scope);

					dynamic globals = engine.Runtime.Globals;
					dynamic house = globals.HexChange.@new();
					if (system == 16)
						S6 = System.Convert.ToString(house.fHex(InputText.Text));
					else if (system == 8)
						S6 = System.Convert.ToString(house.fOct(InputText.Text));
					else if (system == 2)
						S6 = System.Convert.ToString(house.fBin(InputText.Text));
					system = 10;

					Sys.Content = system;
					InputText.Text = S6;
					return;
				}
				double a = Convert.ToDouble(this.InputText.Text);
				S6 = InputText.Text;
			}
			catch
			{
				this.InputText.Text = "Error";
			}
		}

		private void Store5_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;

			if (S5 != string.Empty)
			{
				InputText.Text += S5;
				IsMark = false;
			}
		}

		private void Store5_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			try
			{
				if (system != 10)
				{
					const string Rubycode = @"
class HexChange
	def fHex(a)
		a.hex
	end
	
	def fOct(a)
		a.oct
	end

	def fBin(a)
		Integer('0b'+a)
	end
end";
					ScriptRuntime runtime = Ruby.CreateRuntime();
					ScriptEngine engine = Ruby.GetEngine(runtime);
					ScriptScope scope = engine.CreateScope();

					runtime.LoadAssembly(Assembly.GetExecutingAssembly());
					engine.Execute(Rubycode, scope);

					dynamic globals = engine.Runtime.Globals;
					dynamic house = globals.HexChange.@new();
					if (system == 16)
						S5 = System.Convert.ToString(house.fHex(InputText.Text));
					else if (system == 8)
						S5 = System.Convert.ToString(house.fOct(InputText.Text));
					else if (system == 2)
						S5 = System.Convert.ToString(house.fBin(InputText.Text));
					system = 10;

					Sys.Content = system;
					InputText.Text = S5;
					return;
				}
				double a = Convert.ToDouble(this.InputText.Text);
				S5 = InputText.Text;
			}
			catch
			{
				this.InputText.Text = "Error";
			}
		}

		private void Store4_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;

			if (S4 != string.Empty)
			{
				InputText.Text += S4;
				IsMark = false;
			}
		}

		private void Store4_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			try
			{
				if (system != 10)
				{
					const string Rubycode = @"
class HexChange
	def fHex(a)
		a.hex
	end
	
	def fOct(a)
		a.oct
	end

	def fBin(a)
		Integer('0b'+a)
	end
end";
					ScriptRuntime runtime = Ruby.CreateRuntime();
					ScriptEngine engine = Ruby.GetEngine(runtime);
					ScriptScope scope = engine.CreateScope();

					runtime.LoadAssembly(Assembly.GetExecutingAssembly());
					engine.Execute(Rubycode, scope);

					dynamic globals = engine.Runtime.Globals;
					dynamic house = globals.HexChange.@new();
					if (system == 16)
						S4 = System.Convert.ToString(house.fHex(InputText.Text));
					else if (system == 8)
						S4 = System.Convert.ToString(house.fOct(InputText.Text));
					else if (system == 2)
						S4 = System.Convert.ToString(house.fBin(InputText.Text));
					system = 10;

					Sys.Content = system;
					InputText.Text = S4;
					return;
				}
				double a = Convert.ToDouble(this.InputText.Text);
				S4 = InputText.Text;
			}
			catch
			{
				this.InputText.Text = "Error";
			}
		}
		#endregion

		#region 函数功能
		private void btnX_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;

			if (X != string.Empty)               
			{
				if (IsFunction == false)
				{
					InputText.Text += X;
					IsMark = false;
				}
			}

			if(IsFunction == true)                //函数模式下点击按钮则显示变量名
			{
				InputText.Text += "X";
				IsMark = false;
			}
		}

		private void btnX_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;

			try
			{
				double a = Convert.ToDouble(this.InputText.Text);
				X = InputText.Text;
			}
			catch
			{
				this.InputText.Text = "Error";
			}
		}

		private void btnY_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;

			if (Y != string.Empty)
			{
				if (IsFunction == false)
				{
					InputText.Text += Y;
					IsMark = false;
				}
			}

			if (IsFunction == true)
			{
				InputText.Text += "Y";
				IsMark = false;
			}
		}

		private void btnY_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;

			try
			{
				double a = Convert.ToDouble(this.InputText.Text);
				Y = InputText.Text;
			}
			catch
			{
				this.InputText.Text = "Error";
			}
		}

		private void btnZ_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;

			if (Z != string.Empty)
			{
				if (IsFunction == false)
				{
					InputText.Text += Z;
					IsMark = false;
				}
			}

			if (IsFunction == true)
			{
				InputText.Text += "Z";
				IsMark = false;
			}
		}

		private void btnZ_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;

			try
			{
				double a = Convert.ToDouble(this.InputText.Text);
				Z = InputText.Text;
			}
			catch
			{
				this.InputText.Text = "Error";
			}
		}
		private void btnSFun_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;

			IsFunction = true;                      //将函数功能打开，同时关闭一些在函数功能当中无法使用的功能
			btnEqual.IsEnabled = false;
			btnLn.IsEnabled = false;
			btnSqrt.IsEnabled = false;
			btnDS.IsEnabled = false;
			btnEFun.IsEnabled = true;

			btnSFun.IsEnabled = false;
			Mode.Content = "Function";
		}
		private void btnEFun_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;

			IsFunction = false;                   //将函数功能关闭，同时恢复关闭的按钮功能
			btnEqual.IsEnabled = true;
			btnLn.IsEnabled = true;
			btnSqrt.IsEnabled = true;
			btnDS.IsEnabled = true;
			btnSFun.IsEnabled = true;

			btnEFun.IsEnabled = false;

			Function = InputText.Text;
			InputText.Clear();
			Mode.Content = "Compute";
		}

		private void btnFun_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			InputText.Clear();

			if (Function != string.Empty)                              //左键单击Fun按钮输出函数表达式以及最终结果
			{
				InputText.Text = Function;
				if (Function.Contains("X") && X == string.Empty)
				{
					InputText.Text = "X Not Found";
					return;
				}
				else if (Function.Contains("Y") && Y == string.Empty)
				{
					InputText.Text = "Y Not Found";
					return;
				}
				else if (Function.Contains("Z") && Z == string.Empty)
				{
					InputText.Text = "Z Not Found";
					return;
				}
				InputText.Text = InputText.Text.Replace("X", X).Replace("Y", Y).Replace("Z", Z);
				IsMark = false;
				Start = false;
				this.Equal();
				InputText.Text = Function + "=" + FinalResult;
			}
		}
		#endregion

		#region 进制转换功能
		private void btnDec_Click(object sender, RoutedEventArgs e)                  //用于转换为十进制
		{
			try
			{
				if (this.InputText.Text.Contains("."))
				{
					this.InputText.Text = "Error";
				}
				else if (this.InputText.Text.Contains("-") && !this.InputText.Text.StartsWith("-"))
				{
					this.InputText.Text = "Error";
				}
				else
				{
					const string Rubycode = @"
class HexChange
	def fHex(a)
		a.hex
	end
	
	def fOct(a)
		a.oct
	end

	def fBin(a)
		Integer('0b'+a)
	end
end";
					ScriptRuntime runtime = Ruby.CreateRuntime();
					ScriptEngine engine = Ruby.GetEngine(runtime);
					ScriptScope scope = engine.CreateScope();

					runtime.LoadAssembly(Assembly.GetExecutingAssembly());
					engine.Execute(Rubycode, scope);

					dynamic globals = engine.Runtime.Globals;
					dynamic house = globals.HexChange.@new();
					if (system == 16)
						InputText.Text = System.Convert.ToString(house.fHex(InputText.Text));
					else if (system == 8)
						InputText.Text = System.Convert.ToString(house.fOct(InputText.Text));
					else if (system == 2)
						InputText.Text = System.Convert.ToString(house.fBin(InputText.Text));
					system = 10;

					Sys.Content = system;
				}
			}
			catch
			{
				this.InputText.Text = "Error";
			}
		}

		private void btnHex_Click(object sender, RoutedEventArgs e)                //用于转换为十六进制
		{
			try
			{
				if(this.InputText.Text.Contains("."))
				{
					this.InputText.Text = "Error";
				}
				else if (this.InputText.Text.Contains("-") && !this.InputText.Text.StartsWith("-"))
				{
					this.InputText.Text = "Error";
				}
				else
				{

					const string Rubycode = @"
class HexChange
	def fDec(a)
		Integer(a).to_s(16)
	end

	def fOct(a)
		a.oct.to_s(16)
	end

	def fBin(a)
		Integer('0b'+a).to_s(16)
	end
end";
					ScriptRuntime runtime = Ruby.CreateRuntime();
					ScriptEngine engine = Ruby.GetEngine(runtime);
					ScriptScope scope = engine.CreateScope();

					runtime.LoadAssembly(Assembly.GetExecutingAssembly());
					engine.Execute(Rubycode, scope);

					dynamic globals = engine.Runtime.Globals;
					dynamic house = globals.HexChange.@new();
					if (system == 10)
					{
						InputText.Text = System.Convert.ToString(house.fDec(InputText.Text));
					}
					else if (system == 8)
						InputText.Text = System.Convert.ToString(house.fOct(InputText.Text));
					else if (system == 2)
						InputText.Text = System.Convert.ToString(house.fBin(InputText.Text));
					system = 16;

					Sys.Content = system;
				}
			}
			catch
			{
				this.InputText.Text = "Error";
			}
		}

		private void btnOct_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (this.InputText.Text.Contains("."))
				{
					this.InputText.Text = "Error";
				}
				else if (this.InputText.Text.Contains("-") && !this.InputText.Text.StartsWith("-"))
				{
					this.InputText.Text = "Error";
				}
				else
				{
					const string Rubycode = @"
class HexChange
	def fDec(a)
		Integer(a).to_s(8)
	end

	def fHex(a)
		a.hex.to_s(8)
	end

	def fBin(a)
		Integer('0b'+a).to_s(8)
	end
end";
					ScriptRuntime runtime = Ruby.CreateRuntime();
					ScriptEngine engine = Ruby.GetEngine(runtime);
					ScriptScope scope = engine.CreateScope();

					runtime.LoadAssembly(Assembly.GetExecutingAssembly());
					engine.Execute(Rubycode, scope);

					dynamic globals = engine.Runtime.Globals;
					dynamic house = globals.HexChange.@new();
					if(system == 10)
					{
						InputText.Text = System.Convert.ToString(house.fDec(InputText.Text));
					}
					else if(system == 16)
					{
						InputText.Text = System.Convert.ToString(house.fHex(InputText.Text));
					}
					else if(system == 2)
					{
						InputText.Text = System.Convert.ToString(house.fBin(InputText.Text));
					}
					system = 8;

					Sys.Content = system;
				}
			}
			catch
			{
				this.InputText.Text = "Error";
			}
		}

		private void btnBin_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (this.InputText.Text.Contains("."))
				{
					this.InputText.Text = "Error";
				}
				else if (this.InputText.Text.Contains("-") && !this.InputText.Text.StartsWith("-"))
				{
					this.InputText.Text = "Error";
				}
				else
				{
					const string Rubycode = @"
class HexChange
	def fDec(a)
		Integer(a).to_s(2)
	end

	def fOct(a)
		a.oct.to_s(2)
	end

	def fHex(a)
		a.hex.to_s(2)
	end
end";
					ScriptRuntime runtime = Ruby.CreateRuntime();
					ScriptEngine engine = Ruby.GetEngine(runtime);
					ScriptScope scope = engine.CreateScope();

					runtime.LoadAssembly(Assembly.GetExecutingAssembly());
					engine.Execute(Rubycode, scope);

					dynamic globals = engine.Runtime.Globals;
					dynamic house = globals.HexChange.@new();
					if(system == 10)
					{
						InputText.Text = System.Convert.ToString(house.fDec(InputText.Text));
					}
					else if(system == 8)
					{
						int a = int.Parse(this.InputText.Text);
						InputText.Text = System.Convert.ToString(house.fOct(InputText.Text));
					}
					else if(system == 16)
						InputText.Text = System.Convert.ToString(house.fHex(InputText.Text));
					system = 2;

					Sys.Content = system;
				}
			}
			catch
			{
				this.InputText.Text = "Error";
			}
		}
		#endregion
	}
}
