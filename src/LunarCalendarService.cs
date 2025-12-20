using System;
using System.Collections.Generic;

namespace AdvancedClock
{
    /// <summary>
    /// 农历日期信息
    /// </summary>
    public class LunarDate
    {
        /// <summary>
        /// 农历年份
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// 农历月份（1-12）
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// 农历日期（1-30）
        /// </summary>
        public int Day { get; set; }

        /// <summary>
        /// 是否闰月
        /// </summary>
        public bool IsLeapMonth { get; set; }

        /// <summary>
        /// 对应的公历日期
        /// </summary>
        public DateTime SolarDate { get; set; }

        /// <summary>
        /// 获取农历日期的显示文本
        /// </summary>
        public string DisplayText
        {
            get
            {
                string monthStr = IsLeapMonth ? $"闰{GetMonthName(Month)}" : GetMonthName(Month);
                string dayStr = GetDayName(Day);
                return $"{Year}年{monthStr}{dayStr}";
            }
        }

        /// <summary>
        /// 获取简短显示文本（不含年份）
        /// </summary>
        public string ShortDisplayText
        {
            get
            {
                string monthStr = IsLeapMonth ? $"闰{GetMonthName(Month)}" : GetMonthName(Month);
                string dayStr = GetDayName(Day);
                return $"{monthStr}{dayStr}";
            }
        }

        private static string GetMonthName(int month)
        {
            string[] monthNames = { "", "正月", "二月", "三月", "四月", "五月", "六月",
                                   "七月", "八月", "九月", "十月", "冬月", "腊月" };
            return month >= 1 && month <= 12 ? monthNames[month] : month.ToString();
        }

        private static string GetDayName(int day)
        {
            string[] dayNames = { "", "初一", "初二", "初三", "初四", "初五", "初六", "初七", "初八", "初九", "初十",
                                 "十一", "十二", "十三", "十四", "十五", "十六", "十七", "十八", "十九", "二十",
                                 "廿一", "廿二", "廿三", "廿四", "廿五", "廿六", "廿七", "廿八", "廿九", "三十" };
            return day >= 1 && day <= 30 ? dayNames[day] : day.ToString();
        }
    }

    /// <summary>
    /// 农历计算服务
    /// 支持1900-2100年的公历与农历转换
    /// </summary>
    public class LunarCalendarService
    {
        // 农历数据表（1900-2100年）
        // 每个元素表示一年的农历信息
        // 前12位表示12个月的大小月（1=大月30天，0=小月29天）
        // 后4位表示闰月月份（0表示无闰月）
        private static readonly int[] LunarInfo = new int[]
        {
            0x04bd8,0x04ae0,0x0a570,0x054d5,0x0d260,0x0d950,0x16554,0x056a0,0x09ad0,0x055d2,
            0x04ae0,0x0a5b6,0x0a4d0,0x0d250,0x1d255,0x0b540,0x0d6a0,0x0ada2,0x095b0,0x14977,
            0x04970,0x0a4b0,0x0b4b5,0x06a50,0x06d40,0x1ab54,0x02b60,0x09570,0x052f2,0x04970,
            0x06566,0x0d4a0,0x0ea50,0x06e95,0x05ad0,0x02b60,0x186e3,0x092e0,0x1c8d7,0x0c950,
            0x0d4a0,0x1d8a6,0x0b550,0x056a0,0x1a5b4,0x025d0,0x092d0,0x0d2b2,0x0a950,0x0b557,
            0x06ca0,0x0b550,0x15355,0x04da0,0x0a5b0,0x14573,0x052b0,0x0a9a8,0x0e950,0x06aa0,
            0x0aea6,0x0ab50,0x04b60,0x0aae4,0x0a570,0x05260,0x0f263,0x0d950,0x05b57,0x056a0,
            0x096d0,0x04dd5,0x04ad0,0x0a4d0,0x0d4d4,0x0d250,0x0d558,0x0b540,0x0b6a0,0x195a6,
            0x095b0,0x049b0,0x0a974,0x0a4b0,0x0b27a,0x06a50,0x06d40,0x0af46,0x0ab60,0x09570,
            0x04af5,0x04970,0x064b0,0x074a3,0x0ea50,0x06b58,0x055c0,0x0ab60,0x096d5,0x092e0,
            0x0c960,0x0d954,0x0d4a0,0x0da50,0x07552,0x056a0,0x0abb7,0x025d0,0x092d0,0x0cab5,
            0x0a950,0x0b4a0,0x0baa4,0x0ad50,0x055d9,0x04ba0,0x0a5b0,0x15176,0x052b0,0x0a930,
            0x07954,0x06aa0,0x0ad50,0x05b52,0x04b60,0x0a6e6,0x0a4e0,0x0d260,0x0ea65,0x0d530,
            0x05aa0,0x076a3,0x096d0,0x04afb,0x04ad0,0x0a4d0,0x1d0b6,0x0d250,0x0d520,0x0dd45,
            0x0b5a0,0x056d0,0x055b2,0x049b0,0x0a577,0x0a4b0,0x0aa50,0x1b255,0x06d20,0x0ada0,
            0x14b63,0x09370,0x049f8,0x04970,0x064b0,0x168a6,0x0ea50,0x06b20,0x1a6c4,0x0aae0,
            0x0a2e0,0x0d2e3,0x0c960,0x0d557,0x0d4a0,0x0da50,0x05d55,0x056a0,0x0a6d0,0x055d4,
            0x052d0,0x0a9b8,0x0a950,0x0b4a0,0x0b6a6,0x0ad50,0x055a0,0x0aba4,0x0a5b0,0x052b0,
            0x0b273,0x06930,0x07337,0x06aa0,0x0ad50,0x14b55,0x04b60,0x0a570,0x054e4,0x0d160,
            0x0e968,0x0d520,0x0daa0,0x16aa6,0x056d0,0x04ae0,0x0a9d4,0x0a2d0,0x0d150,0x0f252,
            0x0d520,0x0dd45
        };

        // 1900年1月31日是农历1900年正月初一
        private static readonly DateTime BaseDate = new DateTime(1900, 1, 31);
        private const int BaseYear = 1900;
        private const int MaxYear = 2100;

        /// <summary>
        /// 获取指定农历年份的闰月月份
        /// </summary>
        /// <param name="year">农历年份</param>
        /// <returns>闰月月份（0表示无闰月）</returns>
        private static int GetLeapMonth(int year)
        {
            if (year < BaseYear || year >= MaxYear)
                return 0;
            return LunarInfo[year - BaseYear] & 0xf;
        }

        /// <summary>
        /// 获取指定农历年份闰月的天数
        /// </summary>
        private static int GetLeapMonthDays(int year)
        {
            if (GetLeapMonth(year) == 0)
                return 0;
            return (LunarInfo[year - BaseYear] & 0x10000) != 0 ? 30 : 29;
        }

        /// <summary>
        /// 获取指定农历年月的天数
        /// </summary>
        private static int GetMonthDays(int year, int month)
        {
            if (year < BaseYear || year >= MaxYear)
                return 29;
            return (LunarInfo[year - BaseYear] & (0x10000 >> month)) != 0 ? 30 : 29;
        }

        /// <summary>
        /// 获取指定农历年份的总天数
        /// </summary>
        private static int GetYearDays(int year)
        {
            int sum = 348; // 12个月，每月29天
            for (int i = 0x8000; i > 0x8; i >>= 1)
            {
                sum += (LunarInfo[year - BaseYear] & i) != 0 ? 1 : 0;
            }
            return sum + GetLeapMonthDays(year);
        }

        /// <summary>
        /// 将公历日期转换为农历日期
        /// </summary>
        /// <param name="solarDate">公历日期</param>
        /// <returns>农历日期信息</returns>
        public static LunarDate SolarToLunar(DateTime solarDate)
        {
            if (solarDate < BaseDate || solarDate.Year >= MaxYear)
            {
                throw new ArgumentOutOfRangeException(nameof(solarDate), 
                    $"日期必须在{BaseDate:yyyy-MM-dd}到{MaxYear - 1}-12-31之间");
            }

            int offset = (int)(solarDate - BaseDate).TotalDays;
            int year = BaseYear;
            int month = 1;
            int day = 1;
            bool isLeapMonth = false;

            // 计算年份
            while (year < MaxYear)
            {
                int yearDays = GetYearDays(year);
                if (offset < yearDays)
                    break;
                offset -= yearDays;
                year++;
            }

            // 计算月份
            int leapMonth = GetLeapMonth(year);
            bool isLeap = false;

            for (int m = 1; m <= 12; m++)
            {
                // 闰月
                if (leapMonth > 0 && m == (leapMonth + 1) && !isLeap)
                {
                    m--;
                    isLeap = true;
                    int leapDays = GetLeapMonthDays(year);
                    if (offset < leapDays)
                    {
                        month = m;
                        isLeapMonth = true;
                        break;
                    }
                    offset -= leapDays;
                    isLeap = false;
                    continue;
                }

                int monthDays = GetMonthDays(year, m);
                if (offset < monthDays)
                {
                    month = m;
                    break;
                }
                offset -= monthDays;
            }

            day = offset + 1;

            return new LunarDate
            {
                Year = year,
                Month = month,
                Day = day,
                IsLeapMonth = isLeapMonth,
                SolarDate = solarDate
            };
        }

        /// <summary>
        /// 将农历日期转换为公历日期
        /// </summary>
        /// <param name="year">农历年份</param>
        /// <param name="month">农历月份</param>
        /// <param name="day">农历日期</param>
        /// <param name="isLeapMonth">是否闰月</param>
        /// <returns>公历日期</returns>
        public static DateTime LunarToSolar(int year, int month, int day, bool isLeapMonth = false)
        {
            if (year < BaseYear || year >= MaxYear)
            {
                throw new ArgumentOutOfRangeException(nameof(year), 
                    $"年份必须在{BaseYear}到{MaxYear - 1}之间");
            }

            if (month < 1 || month > 12)
            {
                throw new ArgumentOutOfRangeException(nameof(month), "月份必须在1到12之间");
            }

            if (day < 1 || day > 30)
            {
                throw new ArgumentOutOfRangeException(nameof(day), "日期必须在1到30之间");
            }

            int offset = 0;

            // 计算从BaseYear到指定年份的天数
            for (int y = BaseYear; y < year; y++)
            {
                offset += GetYearDays(y);
            }

            // 计算从正月到指定月份的天数
            int leapMonth = GetLeapMonth(year);
            bool isLeap = false;

            for (int m = 1; m < month; m++)
            {
                offset += GetMonthDays(year, m);
                
                // 如果有闰月且在指定月份之前，加上闰月天数
                if (leapMonth > 0 && m == leapMonth && !isLeap)
                {
                    offset += GetLeapMonthDays(year);
                }
            }

            // 如果是闰月，需要加上正常月的天数
            if (isLeapMonth && leapMonth == month)
            {
                offset += GetMonthDays(year, month);
            }

            // 加上天数
            offset += day - 1;

            return BaseDate.AddDays(offset);
        }

        /// <summary>
        /// 获取指定农历日期的下一个公历日期（用于循环闹钟）
        /// </summary>
        /// <param name="lunarMonth">农历月份</param>
        /// <param name="lunarDay">农历日期</param>
        /// <param name="isLeapMonth">是否闰月</param>
        /// <param name="currentTime">当前时间</param>
        /// <param name="alarmTime">闹钟时间（用于获取时分秒）</param>
        /// <returns>下一个触发的公历日期</returns>
        public static DateTime GetNextLunarAlarmTime(int lunarMonth, int lunarDay, bool isLeapMonth, 
            DateTime currentTime, DateTime alarmTime)
        {
            // 获取当前的农历日期
            var currentLunar = SolarToLunar(currentTime.Date);
            
            // 从当前年份开始查找
            int searchYear = currentLunar.Year;
            
            // 最多查找3年（考虑闰月可能不是每年都有）
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    // 检查这一年是否有指定的闰月
                    int leapMonth = GetLeapMonth(searchYear);
                    
                    // 如果要找闰月，但这年没有对应的闰月，跳过
                    if (isLeapMonth && leapMonth != lunarMonth)
                    {
                        searchYear++;
                        continue;
                    }

                    // 转换为公历日期
                    DateTime solarDate = LunarToSolar(searchYear, lunarMonth, lunarDay, isLeapMonth);
                    
                    // 组合时分秒
                    DateTime nextAlarmTime = new DateTime(
                        solarDate.Year, solarDate.Month, solarDate.Day,
                        alarmTime.Hour, alarmTime.Minute, alarmTime.Second);

                    // 如果这个时间已经过了，尝试下一年
                    if (nextAlarmTime > currentTime)
                    {
                        return nextAlarmTime;
                    }
                }
                catch
                {
                    // 如果转换失败（比如这年没有这个日期），尝试下一年
                }

                searchYear++;
            }

            // 如果3年内都找不到，返回一个默认值
            throw new InvalidOperationException($"无法找到农历{(isLeapMonth ? "闰" : "")}{lunarMonth}月{lunarDay}日的下一个有效日期");
        }

        /// <summary>
        /// 获取农历月份的显示名称列表
        /// </summary>
        public static List<string> GetMonthNames()
        {
            return new List<string>
            {
                "正月", "二月", "三月", "四月", "五月", "六月",
                "七月", "八月", "九月", "十月", "冬月", "腊月"
            };
        }

        /// <summary>
        /// 获取农历日期的显示名称列表
        /// </summary>
        public static List<string> GetDayNames()
        {
            return new List<string>
            {
                "初一", "初二", "初三", "初四", "初五", "初六", "初七", "初八", "初九", "初十",
                "十一", "十二", "十三", "十四", "十五", "十六", "十七", "十八", "十九", "二十",
                "廿一", "廿二", "廿三", "廿四", "廿五", "廿六", "廿七", "廿八", "廿九", "三十"
            };
        }
    }
}
