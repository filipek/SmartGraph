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

namespace SmartGraph.Core
{
    /// <summary>
    /// Represents performance counters used by the engine.
    /// </summary>
    /// <remarks>
    /// It seems necessary to run VS as administrator to allow the 'creation' of
    /// performance counters (see PerformanceCounterCategory.Delete and Create below).
    /// Also, it may be necessary to repair the registry location where performance
    /// counters are specified. The following article explains how to do this:
    /// http://stackoverflow.com/questions/17980178/cannot-load-counter-name-data-because-an-invalid-index-exception
    /// The steps are basically as follows:
    /// Open a cmd window as the administrator and execute at the prompt:
    /// cmd> lodctr /r
    /// </remarks>
    internal static class EngineCounters
    {
        internal const string engineCategory = "SmartGraph";
        internal const string dirtyNodeEventCount = "dirtyNodeEventCount";
        internal const string calculatedNodeCount = "calculatedNodeCount";
        internal const string taskExecutionTime = "taskExecutionTime";

        internal static PerformanceCounter dirtyNodeEventCounter;
        internal static PerformanceCounter calculatedNodeCounter;
        internal static PerformanceCounter taskExecutionTimeCounter;

        internal static void Create()
        {
            if (PerformanceCounterCategory.Exists(engineCategory))
            {
                PerformanceCounterCategory.Delete(engineCategory);
            }

            var counterList = new CounterCreationDataCollection();

            counterList.Add(new CounterCreationData(
                dirtyNodeEventCount,
                "Describes the number of dirty node messages on the engine's event queue.",
                PerformanceCounterType.NumberOfItems32));

            counterList.Add(new CounterCreationData(
                calculatedNodeCount,
                "Describes the number of items scheduled for calculation by an engine task.",
                PerformanceCounterType.NumberOfItems32));

            counterList.Add(new CounterCreationData(
                taskExecutionTime,
                @"Describes the time in milliseconds to process an engine task.",
                PerformanceCounterType.NumberOfItems32));

            PerformanceCounterCategory.Create(
                engineCategory,
                "Engine counters",
                PerformanceCounterCategoryType.SingleInstance,
                counterList);
        }

        internal static void Dispose()
        {
            Reset();

            dirtyNodeEventCounter.Dispose();
            calculatedNodeCounter.Dispose();
            taskExecutionTimeCounter.Dispose();
        }

        internal static void Initialize()
        {
            dirtyNodeEventCounter = new PerformanceCounter(engineCategory, dirtyNodeEventCount, false);
            calculatedNodeCounter = new PerformanceCounter(engineCategory, calculatedNodeCount, false);
            taskExecutionTimeCounter = new PerformanceCounter(engineCategory, taskExecutionTime, false);
        }

        internal static void Reset()
        {
            dirtyNodeEventCounter.RawValue = 0;
            calculatedNodeCounter.RawValue = 0;
            taskExecutionTimeCounter.RawValue = 0;
        }

        internal static void AddDirtyNodeEvent()
        {
            dirtyNodeEventCounter.Increment();
        }

        internal static void RemoveDirtyNodeEvent()
        {
            dirtyNodeEventCounter.Decrement();
        }

        internal static void UpdateCalculatedNodeCount(int count)
        {
            calculatedNodeCounter.RawValue = count;
        }

        internal static void UpdateTaskTime(long milliseconds)
        {
            taskExecutionTimeCounter.RawValue = milliseconds;
        }
    }
}
