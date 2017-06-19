using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Client.Framework {
    /// <summary>
    /// 数据绑定，原本为泛型类，为了xlua等调用修改为使用object
    /// </summary>
    public struct MethodCaller : IEquatable<MethodCaller> {
        public MethodInfo Method;
        public object Caller;

        public void Invoke(params object[] objects) {
            if (Method != null) {
                Method.Invoke(Caller, objects);
            }
        }

        public bool Equals(MethodCaller other) {
            return Equals(Method, other.Method) && Equals(Caller, other.Caller);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MethodCaller) obj);
        }

        public override int GetHashCode() {
            {
                return ((Method != null ? Method.GetHashCode() : 0) * 397) ^ (Caller != null ? Caller.GetHashCode() : 0);
            }
        }
    }

    public class BindableProperty {
        public delegate void ValueChangedHandler (object oldValue, object newValue);

        public event ValueChangedHandler OnValueChanged;


        
        private List<MethodCaller> _invokeMethods = new List<MethodCaller>();

        public BindableProperty(Type systemType) {
            _valueType = systemType;
        }


        private object _value;
        public object Value {
            get {
                return _value;
            }
            set {
                // 检查赋值类型是否正确。必须是类或者子类
                Type newValueType = value.GetType();
                bool isClassAndTypeError = newValueType != _valueType && !newValueType.IsSubclassOf(_valueType);
                bool isValueType = newValueType.IsValueType && _valueType.IsValueType;
                if (!isValueType && isClassAndTypeError) {
                    Debug.LogErrorFormat("赋值类型错误！将{0}类型的值赋给了{1}", value.GetType(), _valueType);
                }
                if (!object.Equals(_value, value)) {
                    object old = _value;
                    _value = value;
                    ValueChanged(old, _value);
                }
            }
        }

        private readonly Type _valueType;
        public Type ValueType {
            get { return _valueType; }
        }


        
        public void AddValueChangedHandler(MethodCaller method) {
            if (!_invokeMethods.Contains(method)) {
                _invokeMethods.Add(method);
            }
        }

        public void RemoveValueChangedHandler(MethodCaller method) {
            if (_invokeMethods.Contains(method)) {
                _invokeMethods.Remove(method);
            }
        }

        public void Reset(object value) {
            _value = value;
            ValueChanged(_value, value); 
        }

        public int GetInt() {
            if (_value == null || _valueType.IsClass) {
                return default(int);
            }
            return (int) _value;
        }

        public long GetLong() {
            if (_value == null || _valueType.IsClass) {
                return default(long);
            }
            return (long) _value;
        }

        public float GetFloat() {
            if (_value == null || _valueType.IsClass) {
                return default(float);
            }
            return (float) _value;
        }

        public string GetString() {
            return _value as string;
        }
 
 
        private void ValueChanged (object oldValue, object newValue) {
            //if (OnValueChanged != null) {
            //    OnValueChanged(oldValue, newValue);
            //}
            for (int mIndex = 0; mIndex < _invokeMethods.Count; mIndex++) {
                var methodInfo = _invokeMethods[mIndex];
                methodInfo.Invoke(oldValue, newValue);
            }
        }

        public override string ToString () {
            return (Value != null ? Value.ToString() : "null");
        }
    }

}