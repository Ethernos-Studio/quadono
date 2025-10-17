using System;
using System.Collections.Generic;
using System.Linq;

namespace Quadono
{
    /// <summary>
    /// 节日类型枚举
    /// </summary>
    public enum HolidayType
    {
        /// <summary>
        /// 国际节日，如劳动节
        /// </summary>
        International,
        
        /// <summary>
        /// 中国节日，如国庆节
        /// </summary>
        Chinese,
        
        /// <summary>
        /// 民间节日，如泼水节
        /// </summary>
        Folk,
        
        /// <summary>
        /// 社区节日，如程序员节
        /// </summary>
        Community
    }

    /// <summary>
    /// 节日数据模型
    /// </summary>
    public class Holiday
    {
        /// <summary>
        /// 节日名称
        /// </summary>
        public string Name { get; set; } = "";
        
        /// <summary>
        /// 节日英文名称
        /// </summary>
        public string EnglishName { get; set; } = "";
        
        /// <summary>
        /// 节日类型
        /// </summary>
        public HolidayType Type { get; set; }
        
        /// <summary>
        /// 公历月份（1-12）
        /// </summary>
        public int Month { get; set; }
        
        /// <summary>
        /// 公历日期（1-31）
        /// </summary>
        public int Day { get; set; }
        
        /// <summary>
        /// 是否为中国农历节日
        /// </summary>
        public bool IsLunar { get; set; }
        
        /// <summary>
        /// 农历月份（如果IsLunar为true）
        /// </summary>
        public int? LunarMonth { get; set; }
        
        /// <summary>
        /// 农历日期（如果IsLunar为true）
        /// </summary>
        public int? LunarDay { get; set; }
        
        /// <summary>
        /// 节日描述
        /// </summary>
        public string Description { get; set; } = "";
        
        /// <summary>
        /// 是否为国家法定节假日
        /// </summary>
        public bool IsPublicHoliday { get; set; }
        
        /// <summary>
        /// 假期天数
        /// </summary>
        public int HolidayDays { get; set; } = 1;
        
        /// <summary>
        /// 获取当前年份的节日日期
        /// </summary>
        public DateTime GetDate(int year)
        {
            if (IsLunar)
            {
                // 简化的农历转换，实际应用中需要更复杂的农历算法
                return LunarCalendarConverter.GetGregorianDate(year, LunarMonth ?? Month, LunarDay ?? Day);
            }
            return new DateTime(year, Month, Day);
        }
        
        /// <summary>
        /// 获取距离节日的天数
        /// </summary>
        public int DaysUntil(int year)
        {
            var holidayDate = GetDate(year);
            var today = DateTime.Today;
            
            if (holidayDate < today)
            {
                holidayDate = GetDate(year + 1);
            }
            
            return (holidayDate - today).Days;
        }
        
        /// <summary>
        /// 获取节日类型显示名称
        /// </summary>
        public string GetTypeDisplayName()
        {
            return Type switch
            {
                HolidayType.International => "国际节日",
                HolidayType.Chinese => "中国节日",
                HolidayType.Folk => "民间节日",
                HolidayType.Community => "社区节日",
                _ => "未知类型"
            };
        }
    }

    /// <summary>
    /// 简化的农历转换器
    /// </summary>
    public static class LunarCalendarConverter
    {
        /// <summary>
        /// 获取指定农历日期对应的公历日期（简化版本）
        /// </summary>
        public static DateTime GetGregorianDate(int year, int lunarMonth, int lunarDay)
        {
            // 这里使用简化的转换逻辑
            // 实际应用中需要使用完整的农历算法
            
            // 春节通常在1月21日-2月20日之间
            if (lunarMonth == 1 && lunarDay == 1)
            {
                // 简化计算：春节在1月21日到2月20日之间循环
                int springFestivalDay = 21 + (year - 2020) % 30;
                if (springFestivalDay > 51) springFestivalDay -= 31;
                int month = springFestivalDay > 31 ? 2 : 1;
                int day = springFestivalDay > 31 ? springFestivalDay - 31 : springFestivalDay;
                return new DateTime(year, month, day);
            }
            
            // 其他农历节日的简化处理
            // 这里返回一个近似日期，实际应用中需要准确的农历算法
            return new DateTime(year, lunarMonth, Math.Min(lunarDay, 28));
        }
    }
}