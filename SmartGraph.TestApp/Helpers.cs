#region Copyright (c) 2020 Filip Fodemski
// 
// Copyright (c) 2020 Filip Fodemski
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

using SmartGraph.Dag.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace SmartGraph.TestApp
{
	internal class Helpers
	{
		public static string DataDir( string f )
		{
			return Environment.CurrentDirectory + @"\..\..\..\data\" + f;
		}
		public static string LoadExpectedStringFile(string expectedName)
		{
			StreamReader s = File.OpenText( Helpers.DataDir( expectedName + ".txt" ) );
			return s.ReadToEnd();
		}
        public static string DumpVertexList(IList<IVertex> vo)
		{
			int i = 0;
			string res = string.Empty;
			foreach ( IVertex v in vo )
			{
				res += v.Name;
				if ( i++ != ( vo.Count - 1 ) )
					res += ", ";
			}

			return res;
		}
        public static string DumpListOfVertexLists(IList<IList<IVertex>> lvl)
		{
			int i = 0;
			string res = string.Empty;
            foreach (IList<IVertex> vl in lvl)
			{
				res += string.Format( "{0}", DumpVertexList( vl ) );
				if ( i++ != ( lvl.Count - 1 ) )
					res += "\r\n";
			}

			return res;
		}
	}
}
