using System;
using System.IO.Ports;
using System.Linq.Expressions;
using System.Reflection;

namespace Lingya.IO.Serial.Tests {
    internal static class ExpressionTest {
        public static void SetProperty<T>(this T owner, Expression<Func<T, Handshake>> expression, string value) {
            if(Enum.TryParse(value,out Handshake handshake)){
                owner.SetPropertyInternal(expression.Body, handshake);
            }
        }

        public static void SetProperty<T>(this T owner, Expression<Func<T, int>> expression, string value) {
            if (int.TryParse(value, out var val)) {
                owner.SetPropertyInternal(expression.Body, val);
            }
        }

        private static void SetPropertyInternal<T,TV>(this T owner, Expression expression, TV value) {
            switch (expression.NodeType) {
                case ExpressionType.MemberAccess:
                    owner.SetPropertyInternal((MemberExpression) expression,value);
                    break;
                case ExpressionType.Lambda:
                    owner.SetPropertyInternal(((LambdaExpression) expression).Body, value);
                    break;
                default:
                    Console.WriteLine(expression.NodeType);
                    break;
            }
        }

        private static void SetPropertyInternal<T,TV>(this T owner, MemberExpression expression, TV value) {
            var member = expression.Member;
            switch (member.MemberType) {
                case MemberTypes.Property:
                    var property = member as PropertyInfo;
                    property.SetValue(owner,value);
                    break;
                case MemberTypes.Field:
                    var field = member as FieldInfo;
                    field.SetValue(owner, value);
                    break;
                default:
                    throw new NotSupportedException("Not Support MemberType "+ member.MemberType);
            }
        }
    }
}
