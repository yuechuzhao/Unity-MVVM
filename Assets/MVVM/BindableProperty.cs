using UnityEngine;

namespace Client.Framework {
    /// <summary>
    /// 数据绑定，原本为泛型类，为了xlua等调用修改为使用object
    /// </summary>
    public class BindableProperty {
        public delegate void ValueChangedHandler (object oldValue, object newValue);

        public event ValueChangedHandler OnValueChanged;


        private object _value;
        public object Value {
            get {
                return _value;
            }
            set {
                if (_value!= null && value.GetType() != _value .GetType()) {
                    Debug.LogErrorFormat("赋值类型错误！将{0}类型的值赋给了{1}", value.GetType(), _value.GetType());
                }
                if (!object.Equals(_value, value)) {
                    object old = _value;
                    _value = value;
                    ValueChanged(old, _value);
                }
            }
        }

        public void Reset(object value) {
            _value = value;
            ValueChanged(_value, value); 
        }

        public int GetInt() {
            if (!(_value is int)) {
                return default(int);
            }
            return (int) _value;
        }

        public long GetLong() {
            if (!(_value is long)) {
                return default(long);
            }
            return (long) _value;
        }

        public float GetFloat() {
            if (!(_value is float)) {
                return default(float);
            }
            return (float) _value;
        }

        public string GetString() {
            return _value as string;
        }
 
 
        private void ValueChanged (object oldValue, object newValue) {
            if (OnValueChanged != null) {
                OnValueChanged(oldValue, newValue);
            }
        }

        public override string ToString () {
            return (Value != null ? Value.ToString() : "null");
        }
    }

}