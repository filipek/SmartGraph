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

using QLNet;
using System;
using System.Collections.Generic;

namespace SmartGraph.Engine.Nodes.QLNet
{
    internal static class QLHelpers
    {
        internal readonly static int SettlementDays = 1;
        internal readonly static Period Tenor = new Period(1, TimeUnit.Years);
        internal readonly static Calendar Calendar = new UnitedKingdom();
        internal readonly static DayCounter DayCounter = new ActualActual(ActualActual.Convention.Bond);

        internal class SimpleDiscountCurve : InterpolatedDiscountCurve<LogLinear>
        {
            private static double GetDiscountFactor(double interestRate, int n)
            {
                // Discount factor = 1 / (1 + r)^n
                return 1 / Math.Pow(1 + interestRate, n);
            }

            private static List<double> CreateCurve(double interestRate, int curvePoints)
            {
                var discountCurve = new List<double>(curvePoints);
                for (int i = 0; i < curvePoints; ++i)
                {
                    discountCurve.Add(GetDiscountFactor(interestRate, i));
                }

                return discountCurve;
            }

            public SimpleDiscountCurve(List<Date> dates, List<double> discounts) :
                base(dates, discounts, DayCounter, Calendar, null, null, new LogLinear()) { }

            public SimpleDiscountCurve(List<Date> dates, double interestRate, int curvePoints) :
                this(dates, CreateCurve(interestRate, curvePoints)) { }

            public override String ToString()
            {
                return String.Format("DiscountCurve({0})", this.data_.Count);
            }
        }

        internal static Schedule CreateSchedule(DateTime issueDate, DateTime maturityDate)
        {
            return new Schedule(
                issueDate,
                maturityDate,
                Tenor,
                Calendar,
                BusinessDayConvention.Unadjusted,
                BusinessDayConvention.Unadjusted,
                DateGeneration.Rule.Backward,
                false);
        }

        internal static YieldTermStructure CreateDiscountCurve(
            List<Date> dates,
            double interestRate,
            int paymentCount)
        {
            return new SimpleDiscountCurve(dates, interestRate, paymentCount);
        }
    }
}
