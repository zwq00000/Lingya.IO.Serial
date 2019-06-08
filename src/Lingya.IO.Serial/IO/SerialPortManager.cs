using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;

namespace Lingya.IO {

    public interface IValueReceiver {
        /// <summary>
        /// 接收到数据
        /// </summary>
        event EventHandler<ValueEventArgs<double>> ReceivedValue;
    }

    /// <summary>
    /// 串口管理器
    /// 串口数据事件转换为数据接收事件<see cref="ReceivedValue"/>
    /// </summary>
    public class SerialPortManager : IValueReceiver, INotifyPropertyChanged, IDisposable {
        private readonly SerialPortSetting _portSetting = new SerialPortSetting();
        private string _newline = "\n";

        /// <summary>
        /// 默认写入超时时间(毫秒)
        /// </summary>
        private const int DefaultWriteTimeout = 1000;

        public SerialPortManager(SerialPortSetting setting) {
            this.PortSetting.AssignFrom(setting.ToString());
            InitSerialPort();
        }

        /// <summary>
        /// 数值解析器
        /// </summary>
        public  Func<string,string> ParserFunc { get; set; }

        /// <summary>
        /// <see cref="SerialPort.NewLine"/>
        /// </summary>
        [DefaultValue("\n")]
        public string NewLine {
            get { return _newline; }
            set {
                if(string.Equals(_newline,value))return;
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
                    Port.DataReceived -= Port_DataReceived;
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
                if (this.Port == null) {
                    Port = _portSetting.CreatePort();
                } else {
                    Port.DataReceived -= Port_DataReceived;
                    Port.Close();
                    _portSetting.ApplySettings(Port);
                }

                Port.NewLine = NewLine;
                Port.Open();
                Port.DataReceived += Port_DataReceived;
            } catch (Exception ex) {
                Trace.TraceWarning("InitSerialPort Error, {0}", ex.Message);
                this.OnPropertyChanged(nameof(Enable));
            }
        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e) {
            try {
                var line = Port.ReadLine();

                if (ParserFunc != null) {
                    var rawValue = ParserFunc(line);
                    if (string.IsNullOrEmpty(rawValue)) {
                        return;
                    }

                    if (double.TryParse(rawValue, out var value)) {
                        OnReceivedValue(rawValue, value);
                    }
                }
            } catch (IOException ioe) {
                Trace.TraceError(ioe.Message);
            }catch (TimeoutException ex) {
                Trace.TraceWarning(ex.Message);
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
        private SerialPort Port { get; set; }

        /// <summary>
        /// 接收到数据
        /// </summary>
        public event EventHandler<ValueEventArgs<double>> ReceivedValue;


        protected virtual void OnReceivedValue(double value) {
            if (!double.IsNaN(value)) {
                ReceivedValue?.Invoke(this, new ValueEventArgs<double>(value));
            }
        }

        protected virtual void OnReceivedValue(string rawValue, double value) {
            if (!double.IsNaN(value)) {
                ReceivedValue?.Invoke(this, new ValueEventArgs<double>(rawValue,value));
            }
        }

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

