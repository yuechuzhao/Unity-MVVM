using System;
using System.Reflection;

namespace Client.Framework {
    /// <summary>
    /// 这个类主要用于处理ViewModel当中需要使用反射的方法
    /// </summary>
    public class ViewModelHelper {
        private Type _viewModelType;
        private FieldInfo[] _fields;
        private MethodInfo[] _methodInfos;
        private ViewModel _viewModel;

        public ViewModelHelper(Type viewModelType) {
            _viewModelType = viewModelType;
            _fields = _viewModelType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic); 
            _methodInfos = _viewModelType.GetMethods(BindingFlags.Instance  | BindingFlags.Public| BindingFlags.NonPublic); 
        }

        public void BindListeners(ViewModel viewModel) {
            if (viewModel == null) {
                return;
            }
            _viewModel = viewModel;
            MVVMUtility.DealBindPropertiesListener(_fields, _methodInfos, _viewModel, OnMethodFound);
        }

        private void OnMethodFound(BindableProperty bindProperty, MethodInfo method) {
            bindProperty.AddValueChangedHandler(new MethodCaller() {
                Caller = _viewModel,
                Method = method
            });
        }


        /// <summary>
        /// 设置绑定属性的唯一接口，设置后会自动触发数据绑定的委托
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        public void SetBindProperty(ViewModel viewModel, string fieldName, object value) {
            if (viewModel == null) {
                return;
            }
            for (int index = 0; index < _fields.Length; index++) {
                var fieldInfo = _fields[index];
                if (fieldInfo.Name == fieldName) {
                    object field = fieldInfo.GetValue(viewModel);
                    var property = field.GetType()
                        .GetProperty("Value", BindingFlags.Public | BindingFlags.Public | BindingFlags.Instance);
                    if (property != null) {
                        property.SetValue(field, value, null);
                    }
                    break;
                }
            }
        }


        /// <summary>
        /// 分发命令
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="command"></param>
        /// <param name="paramObject"></param>
        public void DispatchCommand(ViewModel viewModel, string command, object paramObject) {
            if (viewModel == null) {
                return;
            }
            // 参数可能为空
            System.Type paramType = paramObject == null ? null : paramObject.GetType();
            string commandMethodName = string.Format("OnCommand_{0}", command);
            for (int index = 0; index < _methodInfos.Length; index++) {
                var methodInfo = _methodInfos[index];
                // 依次检查返回类型，函数名，参数
                if (!MVVMUtility.CheckMethodFullMatch(methodInfo,
                    commandMethodName,
                    typeof(void),
                    paramType)) {
                    continue;
                }
                methodInfo.Invoke(viewModel, new[] {paramObject});
                break;
            }
        }

    }
}