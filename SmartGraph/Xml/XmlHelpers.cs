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
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SmartGraph.Xml
{
    /// <summary>
    /// Summary description for XmlHelpers.
    /// </summary>
    public static class XmlHelpers
	{
		public static object Deserialize(Type type, Stream xml)
		{
            var s = new XmlSerializer(type);
			return s.Deserialize( xml );
		}

		public static object Deserialize(Type type, Stream xml, string ns)
		{
            var s = new XmlSerializer(type, ns);
			return s.Deserialize( xml );
		}

		public static object Deserialize(Type type, string xml)
		{
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml), false);
			ms.Position = 0;
			return Deserialize( type, ms );
		}

		public static object Deserialize(Type type, string xml, string ns)
		{
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml), false);
			ms.Position = 0;
			return Deserialize( type, ms, ns );
		}

		public static object Deserialize(Type type, XmlReader xml, string ns)
		{
            var s = new XmlSerializer(type, ns);
			return s.Deserialize( xml );
		}
	}
}
