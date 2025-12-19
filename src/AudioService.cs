using System;
using System.IO;
using System.Media;
using System.Windows.Media;

namespace AdvancedClock
{
    /// <summary>
    /// 音频播放服务
    /// 支持播放自定义声音文件或系统默认声音
    /// </summary>
    public class AudioService
    {
        private MediaPlayer? _mediaPlayer;
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
        }

        /// <summary>
        /// 播放闹钟声音
        /// </summary>
        /// <param name="customSoundPath">自定义声音文件路径，为空则使用系统默认声音</param>
        /// <param name="isStrongAlert">是否为强提醒（影响默认声音选择）</param>
        public void PlayAlarmSound(string? customSoundPath, bool isStrongAlert = false)
        {
            try
            {
                // 如果指定了自定义声音且文件存在
                if (!string.IsNullOrWhiteSpace(customSoundPath) && File.Exists(customSoundPath))
                {
                    PlayCustomSound(customSoundPath);
                }
                else
                {
                    // 使用系统默认声音
                    PlaySystemSound(isStrongAlert);
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
        private void PlayCustomSound(string filePath)
        {
            try
            {
                string extension = Path.GetExtension(filePath).ToLower();

                if (extension == ".wav")
                {
                    // WAV 文件使用 SoundPlayer 播放（更可靠）
                    using (var player = new SoundPlayer(filePath))
                    {
                        player.Play();
                    }
                }
                else if (extension == ".mp3" || extension == ".wma" || extension == ".m4a")
                {
                    // MP3 等格式使用 MediaPlayer 播放
                    if (_mediaPlayer != null)
                    {
                        _mediaPlayer.Stop();
                        _mediaPlayer.Open(new Uri(filePath, UriKind.Absolute));
                        _mediaPlayer.Play();
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
                    PlayCustomSound(customSoundPath);
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
                _mediaPlayer?.Stop();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"停止播放失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 测试播放声音
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

                PlayCustomSound(filePath);
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
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            try
            {
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
