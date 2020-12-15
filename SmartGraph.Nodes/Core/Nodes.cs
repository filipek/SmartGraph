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

using SmartGraph.Common;
using SmartGraph.Core.Interfaces;
using SmartGraph.Xml;
using System;
using System.Timers;
using System.Xml;
using System.Xml.Serialization;

namespace SmartGraph.Nodes.Core
{
	[Serializable]
	[InvariantNode]
	[XmlRoot(Namespace="urn:smartgraph:nodes", IsNullable = false)]
	public sealed class ConstValue : NodeBase
	{
        private const String output = "Value";

        private XmlElement elementValue;
        private Object innerValue;

		[XmlAttribute("type")]
		public String Type = String.Empty;

        [XmlAttribute("value")]
        public String AttributeValue
        {
            get { return innerValue.ToString(); }
            set
            {
                Guard.AssertNotNull(value, "value");

                Type t = System.Type.GetType(Type, true, true);
                innerValue = Convert.ChangeType(value, t);
            }
        }

		[XmlElement("XmlValue")]
		public XmlElement ElementValue
		{
            get { return elementValue; }
			set
			{
                Guard.AssertNotNull(value, "value");

                elementValue = value;

                Type t = System.Type.GetType(Type, true, true);
                innerValue = XmlHelpers.Deserialize(t, elementValue.OuterXml);
			}
		}

        public override void Update()
        {
            Value = innerValue;
        }
	}

    [Serializable]
    [XmlRoot(Namespace = "urn:smartgraph:nodes", IsNullable = false)]
    public sealed class SumOfInputs : NodeBase
    {
        private const String output = "Value";

        public override void Update()
        {
            double sum = 0.0;

            foreach (var input in InputValues.Keys)
            {
                sum += GetInputNode<double>(input);
            }

            Value = sum;
        }
    }

    [Serializable]
    [XmlRoot(Namespace = "urn:smartgraph:nodes", IsNullable = false)]
    public sealed class RandomValue : NodeBase
    {
        private const String output = "Value";

        private readonly Random random = new Random();

        [XmlAttribute("minValue")]
        public String MinValue { get; set; }

        [XmlAttribute("maxValue")]
        public String MaxValue { get; set; }

        public override void Update()
        {
            var min = double.Parse(MinValue);
            var max = double.Parse(MaxValue);

            Value = random.NextBetween(min, max);
        }
    }

	[Serializable]
    [XmlRoot(Namespace = "urn:smartgraph:nodes", IsNullable = false)]
	public sealed class Ticker : ActiveNodeBase
	{
        private const String output = "Ticker";

        private Random random;
        private Timer timer;
        private long sequenceNumber = 0;

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            if (IsRandom)
            {
                var min = double.Parse(MinValue);
                var max = double.Parse(MaxValue);

                Value = random.NextBetween(min, max);
            }
            else
            {
                if (IsCounter)
                {
                    Value = ++sequenceNumber;
                }
                else
                {
                    Value = DateTime.Now;
                }
            }

            Diagnostics.WriteLine(this, String.Format("[{0}] ticked new value '{1}'", Name, Value));

            MarkNodeAsDirty();
        }

        [XmlAttribute("minValue")]
        public String MinValue;

        [XmlAttribute("maxValue")]
        public String MaxValue;

		[XmlAttribute("freq")]
		public double Frequency = 1000;	// in miliseconds

		[XmlAttribute("repeat")]
		public bool IsRepeat = true;

		[XmlAttribute("count")]
		public bool IsCounter = false;

        [XmlAttribute("random")]
        public bool IsRandom = false;

		public override void Activate()
		{
            timer = new Timer(Frequency);
			timer.Elapsed += new ElapsedEventHandler( OnTimedEvent );
            timer.AutoReset = IsRepeat;

            if (IsRandom)
            {
                random = new Random();
            }

			timer.Start();
		}			
	}

    [Serializable]
    [XmlRoot(Namespace = "urn:smartgraph:nodes", IsNullable = false)]
    public sealed class RandomSleeper : NodeBase
    {
        private const String output = "Value";

        private readonly Random random = new Random();

        [XmlAttribute("duration")]
        public double Duration = 100; 	// in miliseconds

        public override void Update()
        {
            var duration = Duration;

            if (duration < 1)
            {
                duration = 1.0;
            }

            System.Threading.Thread.Sleep((int)duration);
            Value = duration;
        }
    }

    [Serializable]
    [XmlRoot(Namespace = "urn:smartgraph:nodes", IsNullable = false)]
    public sealed class SleepJob : NodeBase
    {
        private const String output = "Value";

        public override void Update()
        {
            double sum = 0.0;

            foreach (var input in InputValues.Keys)
            {
                sum += GetInputNode<double>(input);
            }

            System.Threading.Thread.Sleep((int)sum);
            Value = sum;
        }
    }
}
