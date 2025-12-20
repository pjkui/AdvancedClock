using System;
using System.IO;
using System.Media;
using System.Windows.Media;
using System.Windows.Threading;

namespace AdvancedClock
{
    /// <summary>
    /// 音频播放服务
    /// 支持播放自定义声音文件或系统默认声音
    /// 支持循环播放和时长控制
    /// </summary>
    public class AudioService
    {
        private MediaPlayer? _mediaPlayer;
        private DispatcherTimer? _stopTimer;
        private static AudioService? _instance;
        private static readonly object _lock = new object();

        /// <summary>
        /// 获取单例实例
        /// </summary>
        public static AudioService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new AudioService();
                        }
                    }
                }
                return _instance;
            }
        }

        private AudioService()
        {
            _mediaPlayer = new MediaPlayer();
            _stopTimer = new DispatcherTimer();
            _stopTimer.Tick += StopTimer_Tick;
        }

        /// <summary>
        /// 播放闹钟声音
        /// </summary>
        /// <param name="customSoundPath">自定义声音文件路径，为空则使用系统默认声音</param>
        /// <param name="isStrongAlert">是否为强提醒（影响默认声音选择）</param>
        /// <param name="maxDurationSeconds">最大播放时长（秒），默认60秒，0表示不限制</param>
        public void PlayAlarmSound(string? customSoundPath, bool isStrongAlert = false, int maxDurationSeconds = 60)
        {
            try
            {
                // 停止之前的播放
                Stop();

                // 如果指定了自定义声音且文件存在
                if (!string.IsNullOrWhiteSpace(customSoundPath) && File.Exists(customSoundPath))
                {
                    PlayCustomSound(customSoundPath, true, maxDurationSeconds);
                }
                else
                {
                    // 使用系统默认声音
                    PlaySystemSound(isStrongAlert);

                    // 系统默认声音也设置停止定时器
                    if (maxDurationSeconds > 0)
                    {
                        StartStopTimer(maxDurationSeconds);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"播放声音失败: {ex.Message}");
                // 失败时回退到系统默认声音
                PlaySystemSound(isStrongAlert);
            }
        }

        /// <summary>
        /// 播放自定义声音文件
        /// </summary>
        /// <param name="filePath">音频文件路径</param>
        /// <param name="loop">是否循环播放</param>
        /// <param name="maxDurationSeconds">最大播放时长（秒），0表示不限制</param>
        private void PlayCustomSound(string filePath, bool loop = false, int maxDurationSeconds = 0)
        {
            try
            {
                string extension = Path.GetExtension(filePath).ToLower();

                if (extension == ".wav")
                {
                    // WAV 文件使用 SoundPlayer 播放（更可靠）
                    // 注意：SoundPlayer 的 PlayLooping 会阻塞，所以使用 MediaPlayer
                    if (_mediaPlayer != null)
                    {
                        _mediaPlayer.Stop();
                        _mediaPlayer.Open(new Uri(filePath, UriKind.Absolute));

                        if (loop)
                        {
                            _mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
                        }

                        _mediaPlayer.Play();

                        // 设置停止定时器
                        if (maxDurationSeconds > 0)
                        {
                            StartStopTimer(maxDurationSeconds);
                        }
                    }
                }
                else if (extension == ".mp3" || extension == ".wma" || extension == ".m4a")
                {
                    // MP3 等格式使用 MediaPlayer 播放
                    if (_mediaPlayer != null)
                    {
                        _mediaPlayer.Stop();
                        _mediaPlayer.Open(new Uri(filePath, UriKind.Absolute));

                        if (loop)
                        {
                            _mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
                        }

                        _mediaPlayer.Play();

                        // 设置停止定时器
                        if (maxDurationSeconds > 0)
                        {
                            StartStopTimer(maxDurationSeconds);
                        }
                    }
                }
                else
                {
                    // 不支持的格式，使用系统默认声音
                    System.Diagnostics.Debug.WriteLine($"不支持的音频格式: {extension}");
                    SystemSounds.Beep.Play();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"播放自定义声音失败: {ex.Message}");
                SystemSounds.Beep.Play();
            }
        }

        /// <summary>
        /// 播放系统默认声音
        /// </summary>
        /// <param name="isStrongAlert">是否为强提醒</param>
        private void PlaySystemSound(bool isStrongAlert)
        {
            if (isStrongAlert)
            {
                // 强提醒使用更响亮的声音
                SystemSounds.Exclamation.Play();
            }
            else
            {
                // 普通提醒使用标准声音
                SystemSounds.Beep.Play();
            }
        }

        /// <summary>
        /// MediaPlayer 播放结束事件处理（用于循环播放）
        /// </summary>
        private void MediaPlayer_MediaEnded(object? sender, EventArgs e)
        {
            if (_mediaPlayer != null)
            {
                _mediaPlayer.Position = TimeSpan.Zero;
                _mediaPlayer.Play();
            }
        }

        /// <summary>
        /// 启动停止定时器
        /// </summary>
        /// <param name="seconds">秒数</param>
        private void StartStopTimer(int seconds)
        {
            if (_stopTimer != null)
            {
                _stopTimer.Stop();
                _stopTimer.Interval = TimeSpan.FromSeconds(seconds);
                _stopTimer.Start();
                System.Diagnostics.Debug.WriteLine($"启动停止定时器: {seconds}秒后停止播放");
            }
        }

        /// <summary>
        /// 停止定时器触发事件
        /// </summary>
        private void StopTimer_Tick(object? sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("停止定时器触发，停止播放");
            Stop();
        }

        /// <summary>
        /// 播放提前提醒声音（轻柔）
        /// </summary>
        /// <param name="customSoundPath">自定义声音文件路径</param>
        public void PlayAdvanceReminderSound(string? customSoundPath)
        {
            try
            {
                // 如果指定了自定义声音且文件存在
                if (!string.IsNullOrWhiteSpace(customSoundPath) && File.Exists(customSoundPath))
                {
                    PlayCustomSound(customSoundPath, false, 0);
                }
                else
                {
                    // 使用轻柔的系统声音
                    SystemSounds.Asterisk.Play();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"播放提前提醒声音失败: {ex.Message}");
                SystemSounds.Asterisk.Play();
            }
        }

        /// <summary>
        /// 停止播放
        /// </summary>
        public void Stop()
        {
            try
            {
                // 停止定时器
                _stopTimer?.Stop();

                // 移除事件处理
                if (_mediaPlayer != null)
                {
                    _mediaPlayer.MediaEnded -= MediaPlayer_MediaEnded;
                    _mediaPlayer.Stop();
                }

                System.Diagnostics.Debug.WriteLine("音频播放已停止");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"停止播放失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 测试播放声音（不循环，播放5秒）
        /// </summary>
        /// <param name="filePath">音频文件路径</param>
        /// <returns>是否播放成功</returns>
        public bool TestSound(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return false;
                }

                PlayCustomSound(filePath, false, 5); // 测试播放5秒
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 检查音频文件是否有效
        /// </summary>
        /// <param name="filePath">音频文件路径</param>
        /// <returns>是否有效</returns>
        public bool IsValidAudioFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                return false;
            }

            string extension = Path.GetExtension(filePath).ToLower();
            return extension == ".wav" || extension == ".mp3" || extension == ".wma" || extension == ".m4a";
        }

        /// <summary>
        /// 获取支持的音频格式
        /// </summary>
        /// <returns>支持的格式列表</returns>
        public static string GetSupportedFormats()
        {
            return "音频文件 (*.wav;*.mp3;*.wma;*.m4a)|*.wav;*.mp3;*.wma;*.m4a|所有文件 (*.*)|*.*";
        }

        /// <summary>
        /// 获取 sounds/defaults 目录下的所有音频文件
        /// </summary>
        /// <returns>音频文件路径列表</returns>
        public static System.Collections.Generic.List<string> GetDefaultSounds()
        {
            var sounds = new System.Collections.Generic.List<string>();

            try
            {
                string defaultSoundsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sounds", "defaults");

                if (Directory.Exists(defaultSoundsPath))
                {
                    // 获取所有支持的音频文件
                    var extensions = new[] { "*.wav", "*.mp3", "*.wma", "*.m4a" };

                    foreach (var extension in extensions)
                    {
                        var files = Directory.GetFiles(defaultSoundsPath, extension, SearchOption.TopDirectoryOnly);
                        sounds.AddRange(files);
                    }

                    // 按文件名排序
                    sounds.Sort((a, b) => Path.GetFileName(a).CompareTo(Path.GetFileName(b)));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"获取默认声音列表失败: {ex.Message}");
            }

            return sounds;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            try
            {
                Stop();
                _stopTimer = null;
                _mediaPlayer?.Close();
                _mediaPlayer = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"释放音频资源失败: {ex.Message}");
            }
        }
    }
}
