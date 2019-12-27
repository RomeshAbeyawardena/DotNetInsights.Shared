using System;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.App
{
    public class Startup
    {
        public async Task<int> Begin()
        {
            Console.WriteLine("I was run");
            throw new NullReferenceException();
            return 0;
        }
    }
}