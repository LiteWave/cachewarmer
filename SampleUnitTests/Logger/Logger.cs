using System;
using System.Diagnostics;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SampleTests
{
    public class Logger
    {
        public static string currentTestCaseName = String.Empty;
        const int ribbonLength = 60;
        
        public static void LogDebugMessages(string logMessage)
        {
            string logString = string.Format(
            "{0}\t{1}",
            DateTime.Now.ToString("hh:mm:ss.fffffff"),
            logMessage);
            LogRaw(logString);
        }
        
        public static void Log(string format, params object[] args)
        {
            string logString = string.Format(
            "{0}\t{1}",
            DateTime.Now.ToString("hh:mm:ss.fffffff"),
            format);
            LogRaw(logString, args);
        }
        
        public static void Initialize(TestContext testContext)
        {
            if (testContext != null)
            {
                currentTestCaseName = testContext.TestName;
                BeginTestCase();
            }
            //StartTimingTestCase();
        }

        public static void Cleanup(UnitTestOutcome outcome)
        {
            EndTestCase(outcome);
            //StopTimingTestCase();
            //OutputTestCaseTimes();
        }

        public static void LogRaw(string format, params object[] args)
        {
            string logString = string.Format(format, args);
            Debug.WriteLine(logString);
        }

        public static void LogMethod(string methodName)
        {
            LogMethod(methodName, false);
        }

        public static void LogMethod(string methodName, bool isEnd)
        {
            Log("{0} method {1}", isEnd ? "End" : "Begin", methodName);
        }

        public static void LogThrowException(Exception exception, string format, params object[] args)
        {
            if (!string.IsNullOrEmpty(format))
            {
                Log(format, args);
            }
            Log(exception.ToString());
            throw exception;
        }

        public static void LogThrowException(string format, params object[] args)
        {
            LogThrowException(new Exception(string.Format(format, args)), string.Empty, new object[] { });
        }

        public static void LogThrowException(Exception exception, string methodName)
        {
            LogThrowException(exception, "Method '{0}' threw the following exception :", methodName);
        }

        public static Exception LogException(Exception exception, string methodName)
        {
            if (!string.IsNullOrEmpty(methodName))
            {
                Log("Method '{0}' threw the following exception :", methodName);
            }
            Log(exception.ToString());
            return exception;
        }

        public static Exception LogException(string format, params object[] args)
        {
            return LogException(new Exception(string.Format(format, args)), string.Empty);
        }

        public static void LogThrowExceptionIfTrue(bool condition, string format, params object[] args)
        {
            if (condition)
            {
                LogThrowException(format, args);
            }
        }

        public static void LogThrowExceptionIfFalse(bool condition, string format, params object[] args)
        {
            LogThrowExceptionIfTrue(!condition, format, args);
        }

        public static void LogThrowExceptionIfNull(object obj, string format, params object[] args)
        {
            LogThrowExceptionIfTrue(obj == null, format, args);
        }

        public static void LogThrowExceptionIfNotNull(object obj, string format, params object[] args)
        {
            LogThrowExceptionIfTrue(obj != null, format, args);
        }

        public static void LogThrowExceptionIfNotEqual(object expected, object actual, string format)
        {
            if (expected != null && actual != null)
            {
                LogThrowExceptionIfFalse(expected.Equals(actual), "Values for {2} doesn't match. Expected '{0}' but actual value '{1}'", expected, actual, format);
            }
            else if (expected != null && actual == null)
            {
                LogThrowExceptionIfNull(actual, "actual value for {0} is null and expected value is {1}", format, expected);
            }
            else if (expected == null && actual != null)
            {
                LogThrowExceptionIfNull(expected, "expected value for {0} is null and actual value is {1}", format, actual);
            }
            else
            {
                Log("Values for {0} are null", format);
            }
        }
        public static void IsInstanceOfType(Type obj, Type expectedType)
        {
            LogThrowExceptionIfTrue(
            obj == null || obj != expectedType,
            "The object was expected to be of type {0} but it was {1}.",
            expectedType,
            (obj == null) ? null : obj.GetType());
        }

        public static void BeginTestCase()
        {
            //StartTimingTestCase();
            LogRibbon(
            "*",
            string.Format("Begin test case '{0}' ", currentTestCaseName));
        }

        static void EndTestCase(UnitTestOutcome outcome)
        {
            LogRibbon(
            "*",
                //string.Format("Test case's result was '{0}' and took {1}", outcome, StopTimingTestCase()), 
            string.Format("Test case's result was '{0}'", outcome),
            string.Format("End test case '{0}' ", currentTestCaseName));
        }

        static void LogRibbon(string ribbonText, params string[] lines)
        {
            string ribbon = GetString(ribbonLength, ribbonText);
            Log(ribbon);
            int minTagLength = 5;
            foreach (string line in lines)
            {
                int minLength = (minTagLength * 4) + line.Length;
                int extraBefore = 0;
                int extraAfter = 0;
                if (ribbonLength > minLength)
                {
                    int difference = ribbonLength - minLength;
                    extraBefore = difference / 2;
                    extraAfter = difference - extraBefore;
                }
                StringBuilder returnedString = new StringBuilder();
                returnedString.Append(GetString(minTagLength, ribbonText));
                returnedString.Append(GetString(minTagLength + extraBefore, " "));
                returnedString.Append(line);
                returnedString.Append(GetString(minTagLength + extraAfter, " "));
                returnedString.Append(GetString(minTagLength, ribbonText));
                Log(returnedString.ToString());
            }
            Log(ribbon);
        }

        public static String GetString(int length, String template)
        {
            ValidateLength(length);
            StringBuilder returnedString = new StringBuilder();
            if (string.IsNullOrEmpty(template))
            {
                template = new String('#', 30);
            }
            int iterations = length / template.Length;
            for (int i = 0; i < iterations; ++i)
            {
                returnedString.Append(template);
            }
            if (returnedString.Length < length)
            {
                returnedString.Append(template.Substring(0, length - returnedString.Length));
            }
            if (returnedString.Length != length)
                throw new InvalidOperationException(
                "Unable to construct string of specified length");
            return returnedString.ToString();
        }

        static void ValidateLength(long length)
        {
            if (length <= 0)
                throw new ArgumentException("Length cannot be zero or negative");
        }
    }

}
