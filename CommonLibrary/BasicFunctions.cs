using System;
using System.Collections.Generic;
using System.Text;

namespace jh.csharp.CommonLibrary
{
    public class BasicFunctions
    {
        public static int GetRandomNumber(int max=Int32.MaxValue, int min=0)
        {
            DateTime dt = DateTime.Now;
            Random random = new Random((int)dt.ToFileTimeUtc());
            return random.Next(min,max);
        }                    
    }
}
