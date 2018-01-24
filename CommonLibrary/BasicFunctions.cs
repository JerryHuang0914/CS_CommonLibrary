using System;
using System.Collections.Generic;
using System.Text;

namespace jh.csharp.CommonLibrary
{
    public class BasicFunctions
    {
        public static int GetRandomNumber(int max)
        {
            return GetRandomNumber(max, 0);
        }

        public static int GetRandomNumber(int min, int max)
        {
            int result = GetRandomNumber();
            int diff = max - min + 1;
            if(diff>0)
            {
                result %= diff;
                result += min;
            }
            else
            {
                return 0;
            }
            return result;
        }

        public static int GetRandomNumber()
        {
            int result = 0;
            DateTime dt = DateTime.Now;
            Random random = new Random((int)dt.ToFileTimeUtc());
            result = random.Next();
            return result;
        }
                    
    }
}
