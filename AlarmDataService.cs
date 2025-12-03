using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AdvancedClock
{
    /// <summary>
    /// 闹钟数据持久化服务
    /// </summary>
    public class AlarmDataService
    {
        private readonly string _dataFilePath;
        private readonly JsonSerializerOptions _jsonOptions;

        public AlarmDataService()
        {
            // 获取用户数据目录
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appFolder = Path.Combine(appDataPath, "AdvancedClock");
            
            // 确保目录存在
            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }

            _dataFilePath = Path.Combine(appFolder, "alarms.json");

            // 配置JSON序列化选项
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };
        }

        /// <summary>
        /// 获取数据文件路径
        /// </summary>
        public string DataFilePath => _dataFilePath;

        /// <summary>
        /// 保存闹钟数据
        /// </summary>
        /// <param name="alarms">闹钟列表</param>
        /// <returns>是否保存成功</returns>
        public bool SaveAlarms(IEnumerable<AlarmModel> alarms)
        {
            try
            {
                // 转换为可序列化的DTO对象
                var alarmDtos = new List<AlarmDto>();
                foreach (var alarm in alarms)
                {
                    alarmDtos.Add(new AlarmDto
                    {
                        Id = alarm.Id,
                        Name = alarm.Name,
                        AlarmTime = alarm.AlarmTime,
                        RepeatMode = alarm.RepeatMode,
                        IsEnabled = alarm.IsEnabled,
                        Message = alarm.Message
                    });
                }

                // 序列化为JSON
                string json = JsonSerializer.Serialize(alarmDtos, _jsonOptions);
                
                // 写入文件
                File.WriteAllText(_dataFilePath, json);
                
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"保存闹钟数据失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 加载闹钟数据
        /// </summary>
        /// <returns>闹钟列表</returns>
        public List<AlarmModel> LoadAlarms()
        {
            var alarms = new List<AlarmModel>();

            try
            {
                // 检查文件是否存在
                if (!File.Exists(_dataFilePath))
                {
                    return alarms;
                }

                // 读取文件
                string json = File.ReadAllText(_dataFilePath);
                
                // 反序列化
                var alarmDtos = JsonSerializer.Deserialize<List<AlarmDto>>(json, _jsonOptions);
                
                if (alarmDtos != null)
                {
                    foreach (var dto in alarmDtos)
                    {
                        alarms.Add(new AlarmModel
                        {
                            Id = dto.Id,
                            Name = dto.Name,
                            AlarmTime = dto.AlarmTime,
                            RepeatMode = dto.RepeatMode,
                            IsEnabled = dto.IsEnabled,
                            Message = dto.Message
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"加载闹钟数据失败: {ex.Message}");
            }

            return alarms;
        }

        /// <summary>
        /// 数据传输对象（用于序列化）
        /// </summary>
        private class AlarmDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public DateTime AlarmTime { get; set; }
            public AlarmRepeatMode RepeatMode { get; set; }
            public bool IsEnabled { get; set; }
            public string Message { get; set; } = string.Empty;
        }
    }
}
