using System;
using System.IO.Ports;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Lingya.IO{
    /// <summary>
    /// 端口配置扩展方法
    /// </summary>
    public static class SerialPortSettingsExtensions{

        /// <summary>
        /// 检查端口是否可用
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static bool Invaliable(this SerialPortSetting setting) {
            if (string.IsNullOrEmpty(setting?.PortName)) {
                return false;
            }
            return SerialPort.GetPortNames().Contains(setting.PortName) && setting.TryOpenPort();
        }

        /// <summary>
        /// 尝试打开端口
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        private static bool TryOpenPort(this SerialPortSetting setting) {
            try {
                using (var port = setting.CreatePort()) {
                    port.Open();
                    return true;
                }
            } catch (Exception) {
                return false;
            }
        }

        /// <summary>
        ///    应用端口配置,不包括端口名称
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="port"></param>
        public static SerialPort ApplySettings(this SerialPortSetting setting,SerialPort port) {
            if (port == null) {
                throw new ArgumentNullException(nameof(port));
            }
            setting.ValidateSettings();
            //保持 端口开关状态,如果已经打开,在结尾处重新打开该端口
            var isOpened = port.IsOpen;
            if (isOpened) {
                port.Close();
            }
            port.PortName = setting.PortName;
            port.BaudRate = setting.BaudRate;
            port.DataBits = setting.DataBits;
            port.Handshake = setting.Handshake;
            port.Parity = setting.Parity;
            port.StopBits = setting.StopBits;
            port.DtrEnable = setting.Dtr;
            port.RtsEnable = setting.Rts;
            //port.DsrHolding = true;
            port.Open();

            return port;
        }

        /// <summary>
        /// 检查端口设置是否合法
        /// </summary>
        /// <param name="setting"></param>
        private static void ValidateSettings(this SerialPortSetting setting) {
            if (string.IsNullOrEmpty(setting.PortName)) {
                throw new ArgumentException("Port Name not is Empty");
            }

            if (!SerialPort.GetPortNames().Contains(setting.PortName)) {
                throw new ArgumentException($"Port Name {setting.PortName} is Invalid");
            }
        }

        /// <summary>
        /// 从串口对象实例获取参数
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="port"></param>
        public static SerialPortSetting AssignFrom(this SerialPortSetting setting, SerialPort port) {
            if (port == null) {
                throw new ArgumentNullException(nameof(port));
            }
            setting.PortName = port.PortName;
            setting.BaudRate = port.BaudRate;
            setting.DataBits = port.DataBits;
            setting.Handshake = port.Handshake;
            setting.StopBits = port.StopBits;
            setting.Parity = port.Parity;
            setting.Dtr = port.DtrEnable;
            setting.Rts = port.RtsEnable;
            return setting;
        }

        /// <summary>
        /// 从CSV 字符串中读取串口配置
        /// "COM1,9600,8,1,1"
        ///  $"{settings.PortName},{settings.BaudRate},{settings.DataBits},{settings.StopBits},{settings.Parity}"
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="settingParam"></param>
        /// <returns></returns>
        public static SerialPortSetting AssignFrom(this SerialPortSetting setting,  string settingParam) {
            if (string.IsNullOrEmpty(settingParam)) {
                //返回默认值
                return setting;
            }

            if (string.Equals(setting.ToString(), settingParam)) {
                //setting 没有变化
                return setting;
            }
            if (settingParam.Contains(",")) {
                var values = settingParam.Split(',');

                for (int i = 0; i < values.Length; i++) {
                    var val = values[i];
                    if (string.IsNullOrEmpty(val)) {
                        continue;
                    }
                    switch (i) {
                        case 0:
                            setting.PortName = val;
                            break;
                        case 1:
                            setting.SetProperty(s=>s.BaudRate ,val);
                            break;
                        case 2:
                            setting.SetProperty(s=> s.DataBits,val);
                            break;
                        case 3:
                            setting.SetProperty(s=> s.StopBits,val);
                            break;
                        case 4:
                            setting.SetProperty(s=> s.Parity ,val);
                            break;
                        case 5:
                            setting.SetProperty(s => s.Handshake ,val);
                            break;
                        default:
                            if (string.Equals(val, "DTR", StringComparison.CurrentCultureIgnoreCase)) {
                                setting.Dtr = true;
                            }

                            if (string.Equals(val, "RTS", StringComparison.CurrentCultureIgnoreCase)) {
                                setting.Rts = true;
                            }
                            break;
                    }
                }
            } else {
                if (SerialPort.GetPortNames().Contains(settingParam)) {
                    setting.PortName = settingParam;
                }
            }
            setting.RasieOnChanged();
            return setting;
        }
    }

    internal static class PropertyExtensions {

        public static void SetProperty<T>(this T owner, Expression<Func<T, Handshake>> expression, string value) {
            if (Enum.TryParse(value, out Handshake handshake)) {
                owner.SetPropertyInternal(expression.Body, handshake);
            }
        }
        

        public static void SetProperty<T>(this T owner, Expression<Func<T, Parity>> expression, string value) {
            if (Enum.TryParse(value, out Parity val)) {
                owner.SetPropertyInternal(expression.Body, val);
            }
        }

        public static void SetProperty<T>(this T owner, Expression<Func<T, StopBits>> expression,string value) {
            if (Enum.TryParse(value, out StopBits val)) {
                owner.SetPropertyInternal(expression.Body,val);
            } 
        }

        public static void SetProperty<T>(this T owner, Expression<Func<T, int>> expression, string value) {
            if (int.TryParse(value, out var val)) {
                owner.SetPropertyInternal(expression.Body, val);
            }
        }

        private static void SetPropertyInternal<T, TV>(this T owner, Expression expression, TV value) {
            switch (expression.NodeType) {
                case ExpressionType.MemberAccess:
                    owner.SetPropertyInternal((MemberExpression)expression, value);
                    break;
                case ExpressionType.Lambda:
                    owner.SetPropertyInternal(((LambdaExpression)expression).Body, value);
                    break;
                default:
                    throw new NotSupportedException("Not Support Expression " + expression.NodeType);
            }
        }

        private static void SetPropertyInternal<T, TV>(this T owner, MemberExpression expression, TV value) {
            var member = expression.Member;
            switch (member.MemberType) {
                case MemberTypes.Property:
                    var property = member as PropertyInfo;
                    property.SetValue(owner,value,new object[]{});
                    break;
                case MemberTypes.Field:
                    var field = member as FieldInfo;
                    field.SetValue(owner, value);
                    break;
                default:
                    throw new NotSupportedException("Not Support MemberType " + member.MemberType);
            }
        }
    }
    
}