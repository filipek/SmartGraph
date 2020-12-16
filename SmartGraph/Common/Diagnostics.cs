#region Copyright (C) 2015 Filip Fodemski
// 
// Copyright (c) 2015 Filip Fodemski
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files
// (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR
// ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE
//
#endregion

using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace SmartGraph.Common
{
	/// <summary>
	/// Summary description for Diagnostics.
	/// </summary>
	public class Diagnostics
	{
		private Diagnostics() {}

		public static string ThisMethod()
		{
			StackFrame sf = new StackTrace().GetFrame(1);
			MethodBase method = sf.GetMethod();
			string sep = ( method.IsStatic ? "::" : "." );
			return string.Format( "{0}{1}{2}$ ", method.DeclaringType.Name, sep, method.Name);
		}
		public static string MethodBeforeThisMethod()
		{
			StackFrame sf = new StackTrace().GetFrame(2);
			MethodBase method = sf.GetMethod();
			string sep = ( method.IsStatic ? "::" : "." );
			return string.Format( "{0}{1}{2}$ ", method.DeclaringType.Name, sep, method.Name);
		}
		public static void WriteLine(object o, string msg)
		{
			StackTrace st = new StackTrace();
			StackFrame sf = st.GetFrame(1);
			string type = o.GetType().ToString();

			string tid = Thread.CurrentThread.Name;
			if ( tid == null || tid.Length == 0 )
			{
				tid = Thread.CurrentThread.GetHashCode().ToString();
			}

			Debug.WriteLine( string.Format(
				"<dbg-msg\n\t\ttid='{0}'\n\t\ttype='{1}'\n\t\tmethod='{2}'\n>{3}</dbg-msg>\n",
                tid, type, sf.GetMethod().Name, msg));
		}
		public static void DebugException(Exception e, string msg, string src)
		{
			Debug.WriteLine( string.Format(
				"[{0}] thread {1} caught exception:\n{2}\n--------\n{3}{4}",
				src,
				Thread.CurrentThread.GetHashCode(),
				msg,
				e.Message,
				e.InnerException == null ? string.Empty : "\n" + e.InnerException.Message));
		}
		public static void DebugException(Exception e, string msg)
		{
			Debug.WriteLine( string.Format(
				"[{0}] thread {1} caught exception:\n{2}\n--------\n{3}{4}",
				MethodBeforeThisMethod(),
				Thread.CurrentThread.GetHashCode(),
				msg,
				e.Message,
				e.InnerException == null ? string.Empty : "\n" + e.InnerException.Message));
		}
	}
}
