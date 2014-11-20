#region 引用名称空间
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Calculator
{
	interface Ioperate
	{
		#region 定义接口
		bool IsNumber(string ch);       //判断是否为数字
		int CompareOperate(string ch, string stackCh);      //比较运算符的优先级
		string GetExpression(string InputString);       //进行入栈操作，调用getResult得出结果
		#endregion
	}
}
