using System;

namespace Celia.io.Core.Utils
{
    public class DateTimeUtils
    {
        /// <summary>
        /// 获取当前long型时间戳（毫秒）
        /// </summary>
        /// <returns></returns>
        public static long GetCurrentTimestamp()
        {
            var dt = DateTime.UtcNow;
            return GetTimestamp(dt);
        }

        /// <summary>
        /// 获取指定时间的时间戳（毫秒）
        /// </summary>
        /// <param name="dt">当地时间</param>
        /// <returns></returns>
        public static long GetTimestamp(DateTime dt)
        {
            var dateTimeOffset = new DateTimeOffset(dt);
            return dateTimeOffset.ToUnixTimeSeconds();
        }

        /// <summary>
        /// 从时间戳（毫秒）转换回当地时间DateTime
        /// </summary>
        /// <param name="timestamp">毫秒时间戳</param>
        /// <returns>当地时间</returns>
        public static DateTime ToDateTimeNow(long timestamp)
        {
            return ToDateTimeUtc(timestamp).ToLocalTime();
        }

        /// <summary>
        /// 从时间戳（毫秒）转换回UTC DateTime
        /// </summary>
        /// <param name="timestamp">毫秒时间戳</param>
        /// <returns>UTC时间</returns>
        public static DateTime ToDateTimeUtc(long timestamp)
        {
            var localDateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(timestamp);
            return localDateTimeOffset.DateTime;
        }
    }
}
