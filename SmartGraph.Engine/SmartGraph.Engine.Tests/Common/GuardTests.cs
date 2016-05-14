#region Copyright (C) 2016 Filip Fodemski
// 
// Copyright (c) 2016 Filip Fodemski
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartGraph.Engine.Common;
using System;

namespace SmartGraph.Engine.Common.Tests
{
    /// <summary>
    /// Summary description for ExtensionsTests
    /// </summary>
    [TestClass]
    public class ExtensionsTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Extensions_Guard_AssertSmallerThan_Bigger()
        {
            Guard.AssertSmallerThan(1, 0, "integer");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Extensions_Guard_AssertSmallerThan_Equal()
        {
            Guard.AssertSmallerThan(1, 1, "integer");
        }

        [TestMethod]
        public void Extensions_Guard_AssertSmallerThan_Smaller()
        {
            Guard.AssertSmallerThan(1, 2, "integer");
        }

        [TestMethod]
        public void Extensions_Guard_AssertBiggerThan_Bigger()
        {
            Guard.AssertBiggerThan(1, 0, "integer");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Extensions_Guard_AssertBiggerThan_Equal()
        {
            Guard.AssertBiggerThan(1, 1, "integer");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Extensions_Guard_AssertBiggerThan_Smaller()
        {
            Guard.AssertBiggerThan(1, 2, "integer");
        }
    }
}
