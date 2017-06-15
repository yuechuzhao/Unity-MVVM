namespace Client.Framework {
    /// <summary>
    /// 数据绑定
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BindableProperty<T> {
        public delegate void ValueChangedHandler (T oldValue, T newValue);

        public event ValueChangedHandler OnValueChanged;

        public void Reset(T value) {
            _value = value;
            ValueChanged(_value, value); 
        }


        private T _value;
        public T Value {
            get {
                return _value;
            }
            set {
                if (!object.Equals(_value, value)) {
                    T old = _value;
                    _value = value;
                    ValueChanged(old, _value);
                }
            }
        }

        private void ValueChanged (T oldValue, T newValue) {
            if (OnValueChanged != null) {
                OnValueChanged(oldValue, newValue);
            }
        }

        public override string ToString () {
            return (Value != null ? Value.ToString() : "null");
        }
    }

}