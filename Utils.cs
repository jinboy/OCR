using System;
using System.Collections.Generic;
using System.Text;

namespace YunZhi
{
    class Utils
    {

        public static  int  ADaySeconds = 86400;

        /// <summary>
        ///  生成唯一键
        /// </summary>
        /// <returns></returns>
        public static string sid()
        {
            System.Guid guid = new Guid();
            guid = Guid.NewGuid();
            string str = guid.ToString();
            return str;
        }

        /// <summary>
        ///  少于一周，true；超过一周，false
        /// </summary>
        /// <param name="time1"></param>
        /// <param name="time2"></param>
        /// <returns></returns>
        public static bool isLessThanAWeek(string time1, string time2)
        {
            DateTime oldTime = Convert.ToDateTime(time1);
            DateTime newTime = Convert.ToDateTime(time2);
            TimeSpan ts = newTime - oldTime;
            double secondInterval = ts.TotalSeconds;
            if (secondInterval > 604800){// 超过一周，一天86400秒，一周604800
                return false;
            }
            else
            {// 不足一周
                return true;
            }
        }

        /// <summary>
        /// 少于自定义天数，true；超过自定义天数，false
        /// </summary>
        /// <param name="time1"></param>
        /// <param name="time2"></param>
        /// <param name="customDay"></param>
        /// <returns></returns>
        public static bool isLessThanCustomDays(string time1, string time2, int customDay)
        {
            DateTime oldTime = Convert.ToDateTime(time1);
            DateTime newTime = Convert.ToDateTime(time2);
            TimeSpan ts = newTime - oldTime;
            double secondInterval = ts.TotalSeconds;
            if (secondInterval > ADaySeconds * customDay)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
