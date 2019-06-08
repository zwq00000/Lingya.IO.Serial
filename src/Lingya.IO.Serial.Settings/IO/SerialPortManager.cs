using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;

namespace Lingya.IO {
    /// <summary>
    /// 串口管理器
    /// 串口数据事件转换为数据接收事件<see cref="ReceivedValue"/>
    /// </summary>
    public class SerialPortManager : INotifyPropertyChanged, IDisposable {
        private readonly SerialPortSetting _portSetting = new SerialPortSetting();
        private string _newline = "\n";

        /// <summary>
        /// 默认写入超时时间(毫秒)
        /// </summary>
        private const int DefaultWriteTimeout = 1000;

        public SerialPortManager(SerialPortSetting setting) {
            this.Port = new SerialPort();
            this.PortSetting.AssignFrom(setting.ToString());
            InitSerialPort();
        }

        /// <summary>
        /// <see cref="SerialPort.NewLine"/>
        /// </summary>
        [DefaultValue("\n")]
        public string NewLine {
            get { return _newline; }
            set {
                if (string.Equals(_newline, value)) return;
                _newline = value;
                if (Port != null) {
                    Port.NewLine = value;
                }
                this.OnPropertyChanged(nameof(NewLine));
            }
        }

        /// <summary>
        /// 是否启用接口
        /// </summary>
        public bool Enable {
            get { return Port != null && Port.IsOpen; }
            set {
                if (value) {
                    this.InitSerialPort();
                } else {
                    Port.Close();
                }
                OnPropertyChanged(nameof(Enable));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portParam">转发串口的配置参数</param>
        public SerialPortManager(string portParam) {
            PortSetting.AssignFrom(portParam);
            InitSerialPort();
        }

        public void ApplyPortSetting(SerialPortSetting setting) {
            this.PortSetting = setting;
        }

        /// <summary>
        /// 初始化串口
        /// </summary>
        private void InitSerialPort() {
            try {
                _portSetting.ApplySettings(Port);
                Port.NewLine = NewLine;
                Port.Open();
            } catch (Exception ex) {
                Trace.TraceWarning("InitSerialPort Error, {0}", ex.Message);
                this.OnPropertyChanged(nameof(Enable));
            }
        }

        /// <summary>
        /// 串口设置
        /// </summary>
        public SerialPortSetting PortSetting {
            get { return _portSetting; }
            set {
                if (value == null) {
                    return;
                }
                _portSetting.AssignFrom(value.ToString());
                InitSerialPort();
                this.OnPropertyChanged(nameof(PortSetting));
            }
        }

        /// <summary>
        /// 串口
        /// </summary>
        public SerialPort Port { get; private set; }


        #region IDisposable

        /// <summary>执行与释放或重置非托管资源相关的应用程序定义的任务。</summary>
        public void Dispose() {
            Port?.Dispose();
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

