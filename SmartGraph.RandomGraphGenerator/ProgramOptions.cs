﻿#region Copyright (c) 2020 Filip Fodemski
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

using System;

namespace SmartGraph.RandomGraphGenerator
{
	public class ProgramOptions : CommandLineOptions
	{
		[Option(Short = "o", Description = "Output file")]
		public string Output = string.Empty;

		[Option(Short = "f", Description = "Output format, one of: dot or enginexml")]
        public string Format = "dot";

		protected override void InvalidOption(string name)
		{
			Console.WriteLine("Invalid option '{0}'", name);
			Help();
		}
		public override void Help()
		{
			Console.WriteLine("RandomGraphGenerator Command Line Options");

			base.Help();

			Environment.Exit(1);
		}

        public ProgramOptions(string[] args) : base(args)
		{
			if (args.Length == 0)
			{
				Help();
			}
		}
	}
}
