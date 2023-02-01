using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.SharedKernel
{
    public static class DateUtil
    {
        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long ToUnixTime(this DateTime datetime)
        {
            return (long)(datetime - epoch).TotalMilliseconds;
        }
    }
}
