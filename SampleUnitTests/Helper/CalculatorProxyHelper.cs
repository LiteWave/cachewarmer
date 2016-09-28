using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyCalc;

namespace SampleTests
{
    public class CalculatorProxyHelper
    {
        /// <summary>
        /// InputRequest Object for Add
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        public double AddNumers(int n1, int n2)
        {
            double result = 0;
            CalcServiceClient client = new CalcServiceClient("MyCalc.ICalcService", "http://localhost/MyCalc/MyCalc.CalcService.svc");

            result= client.Add(n1, n2);
            return result;
        }
        
        /// <summary>
        /// InputRequest Object Subtract
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        public double SubtractNumers(int n1, int n2)
        {
            double result = 0;
            CalcServiceClient client = new CalcServiceClient();

            result = client.Subtract(n1, n2);
            return result;
        }
    }
}
