using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;

namespace Lingya.IO {
    /// <summary>
    ///     串口配置
    /// </summary>
    public class SerialPortSetting: INotifyPropertyChanged {

        public SerialPortSetting() {
            ResetToDefault();
        }

        /// <summary>
        /// 通过配置参数构造 <see cref="SerialPortSetting"/>
        /// 参数可以是 端口名称,也可以是 "COM1,9600,1,1,2" 这样的配置参数字符串
        /// </summary>
        /// <param name="param"></param>
        public SerialPortSetting(string param) : this() {
            //串口配置字符串
            this.AssignFrom(param);
        }

        public SerialPortSetting(SerialPort port) : this() {
            this.AssignFrom(port);
        }

        /// <summary>
        ///     重置为默认值
        /// </summary>
        private void ResetToDefault() {
            _baudRate = DefaultBaudRate;
            _dataBits = DefaultDataBits;
            Handshake = DefaultHandshake;
            Parity = DefaultParity;
            StopBits = DefaultStopBits;
        }

        /// <summary>
        ///     根据 设置内容 创建端口实例（包括端口名称）
        /// </summary>
        /// <returns></returns>
        public SerialPort CreatePort() {
            var port = new SerialPort();
            this.ApplySettings(port);
            return port;
        }


        /// <summary>
        /// 返回串口配置信息
        /// $"{PortName},{BaudRate},{DataBits},{StopBits},{Parity}"
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            if (Dtr || Rts) {
                var dtrEnable = Dtr?"DTR":string.Empty;
                var rtsEnable = Rts? "RTS":string.Empty;
                return $"{PortName},{BaudRate},{DataBits},{StopBits},{Parity},{Handshake},{dtrEnable},{rtsEnable}";
            }
            if (Handshake == DefaultHandshake) {
                if (Parity == DefaultParity) {
                    return $"{PortName},{BaudRate},{DataBits},{StopBits}";
                }
                return $"{PortName},{BaudRate},{DataBits},{StopBits},{Parity}";
            }

            return $"{PortName},{BaudRate},{DataBits},{StopBits},{Parity},{Handshake}";
        }

        #region const values

        /// <summary>
        ///     默认波特率
        /// </summary>
        private const int DefaultBaudRate = 115200;

        /// <summary>
        ///     默认数据位
        /// </summary>
        private const int DefaultDataBits = 8;

        /// <summary>
        ///     默认流控制方法
        /// </summary>
        private const Handshake DefaultHandshake = Handshake.None;

        /// <summary>
        ///     默认奇偶校验方法
        /// </summary>
        private const Parity DefaultParity = Parity.None;

        /// <summary>
        ///     默认停止位
        /// </summary>
        private const StopBits DefaultStopBits = StopBits.One;

        private const int MaxDataBits = 8;
        private const int MinDataBits = 5;


        /// <summary>
        ///     默认端口名称
        /// </summary>
        private static readonly string DefaultPortName = SerialPort.GetPortNames()[0];

        #endregion const values

        #region fields

        /// <summary>
        ///     波特率
        /// </summary>
        private int _baudRate;

        /// <summary>
        ///     数据位
        /// </summary>
        private int _dataBits;

        /// <summary>
        ///     端口名称
        /// </summary>
        private string _portName;

        private Parity _parity;
        private Handshake _handshake;
        private StopBits _stopBits;
        private bool _dtr;
        private bool _rts;

        #endregion fields

        #region 公共属性

        /// <summary>
        ///     波特率
        /// </summary>
        [DisplayName("波特率")]
        public int BaudRate {
            get { return _baudRate; }
            set {
                if (value <= 0) {
                    throw new ArgumentOutOfRangeException(nameof(value), Resources.Error_Message_BaudRate_Param_Error);
                }

                _baudRate = value;
                OnPropertyChanged(nameof(BaudRate));
            }
        }

        /// <summary>
        ///     数据位
        /// </summary>
        [DisplayName("数据位")]
        public int DataBits {
            get { return _dataBits; }
            set {
                if ((value < MinDataBits) || (value > MaxDataBits)) {
                    throw new ArgumentOutOfRangeException(nameof(value), Resources.Error_Message_DataBits_Range);
                }

                _dataBits = value;
                OnPropertyChanged(nameof(DataBits));
            }
        }

        /// <summary>
        ///     停止位
        /// </summary>
        [DisplayName("停止位")]
        public StopBits StopBits {
            get => _stopBits;
            set {
                if (value == _stopBits) return;
                _stopBits = value;
                OnPropertyChanged(nameof(StopBits));
            }
        }

        /// <summary>
        ///     数据流控制
        /// </summary>
        [DefaultValue(DefaultHandshake), Browsable(true), MonitoringDescription("Handshake")]
        [DisplayName("数据流控制")]
        public Handshake Handshake {
            get => _handshake;
            set {
                if (value == _handshake) return;
                _handshake = value;
                OnPropertyChanged(nameof(Handshake));
            }
        }

        /// <summary>
        ///     奇偶校验
        /// </summary>
        [DefaultValue(DefaultParity)]
        [DisplayName("奇偶校验")]
        public Parity Parity {
            get => _parity;
            set {
                if (value == _parity) return;
                _parity = value;
                OnPropertyChanged(nameof(Parity));
            }
        }

        /// <summary>
        ///     端口名称
        /// </summary>
        [DisplayName("端口名称")]
        public string PortName {
            get { return _portName; }
            set {
                if (string.IsNullOrEmpty(value)) {
                    throw new ArgumentNullException(nameof(value), Resources.Error_Message_PortName_NotEmpty);
                }

                _portName = value;
                OnPropertyChanged(nameof(PortName));
            }
        }

        /// <summary>
        /// 获取或设置一个值，该值在串行通信过程中启用数据终端就绪 (DTR) 信号
        /// </summary>
        public bool Dtr {
            get => _dtr;
            set {
                if(_dtr==value)return;
                _dtr = value;
                this.OnPropertyChanged(nameof(Dtr));
            }
        }

        /// <summary>
        /// 获取或设置一个值，该值指示在串行通信中是否启用请求发送 (RTS) 信号
        /// <see cref="SerialPort.RtsEnable"/>
        /// </summary>
        public bool Rts {
            get => _rts;
            set {
                if(_rts == value)return;
                _rts = value;
                this.OnPropertyChanged(nameof(Rts));
            }
        }

        #endregion 公共属性


        #region 静态方法

        /// <summary>
        /// 查找可用端口
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetAvailablePorts() {
            var names = SerialPort.GetPortNames();
            return names.Where(IsAvaliable);
        }

        /// <summary>
        /// 查找可用端口,包括当前端口
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> AvailablePorts() {
            if (string.IsNullOrEmpty(this.PortName)) {
                return GetAvailablePorts();
            }

            var ports = GetAvailablePorts().ToList();
            if (!ports.Contains(this.PortName)) {
                ports.Add(this.PortName);
            }

            return ports;
        }

        /// <summary>
        /// 尝试打开端口判断端口是否可用
        /// </summary>
        /// <param name="portName"></param>
        /// <returns></returns>
        private static bool IsAvaliable(string portName) {
            using (var port = new SerialPort(portName)) {
                try {
                    port.Open();
                    return true;
                } catch (UnauthorizedAccessException) {
                    return false;
                } catch (Exception) {
                    return false;
                }
            }
        }

        #endregion

        /// <summary>
        /// 检查端口设置是否正确
        /// </summary>
        /// <param name="settingParam"></param>
        /// <returns></returns>
        public static bool CheckSettings(string settingParam) {
            if (string.IsNullOrEmpty(settingParam)) {
                return false;
            }

            if (settingParam.Contains(",")) {
                var values = settingParam.Split(',');
                var portName = values.FirstOrDefault();
                if (string.IsNullOrEmpty(portName)) {
                    return false;
                }

                return SerialPort.GetPortNames().Contains(portName.Trim());

            }

            return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}