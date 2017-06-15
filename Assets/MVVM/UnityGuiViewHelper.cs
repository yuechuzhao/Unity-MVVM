using System;
using System.Reflection;

namespace Client.Framework {

    /// <summary>
    /// 这个类主要用于处理UnityGuiView当中需要使用反射的方法
    /// </summary>
    public class UnityGuiViewHelper {
        private Type _viewType;
        private Type _viewModelType;

        private FieldInfo[] _viewFields;
        private FieldInfo[] _viewModelFields;

        private MethodInfo[] _viewMethodInfos;
        private MethodInfo[] _viewModelMethodInfos;

        private UnityGuiView _view;
        private ViewModel _oldViewModel;
        private ViewModel _newViewModel;

        /// <summary>
        /// 设置处理的Type
        /// </summary>
        /// <param name="viewType"></param>
        /// <param name="viewModelType"></param>
        public void SetType(Type viewType, Type viewModelType) {

            if (_viewType != viewType) {
                _viewType = viewType;
                _viewFields = _viewType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                _viewMethodInfos = _viewType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic| BindingFlags.Instance);
            }

            if (_viewModelType != viewModelType) {
                _viewModelType = viewModelType;
                _viewModelFields = _viewModelType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                _viewModelMethodInfos = _viewModelType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            }
        }

        /// <summary>
        /// view更换viewModel的时候，绑定所有viewModel的内容
        /// </summary>
        /// <param name="view"></param>
        /// <param name="oldViewModel"></param>
        /// <param name="newViewModel"></param>
        public void ReattatchViewListener(UnityGuiView view, ViewModel oldViewModel, ViewModel newViewModel) {
            if (view == null) {
                return;
            }

            if (newViewModel == null) {
                return;
            }
            _view = view;
            _newViewModel = newViewModel;
            MVVMUtility.DealBindPropertiesListener(_viewModelFields, _viewMethodInfos, OnDelegateFound);
            if (oldViewModel != null) {
                oldViewModel.OnNotificationPush -= OnNotificationReceived;
            }
            _newViewModel.OnNotificationPush += OnNotificationReceived;
        }

        private void OnDelegateFound(FieldInfo fieldInfo, EventInfo eventInfo, Type delegateType, MethodInfo method) {
            Delegate onChangeListener = Delegate.CreateDelegate(delegateType, _view, method);

            if (_oldViewModel != null) {
                // 从旧的viewModel当中移除，添加到新的viewModel当中
                eventInfo.RemoveEventHandler(fieldInfo.GetValue(_oldViewModel), onChangeListener);    
            }
            eventInfo.AddEventHandler(fieldInfo.GetValue(_newViewModel), onChangeListener);
            _oldViewModel = _newViewModel;
        }

        private void OnNotificationReceived(string notification, object paramObject) {
            System.Type paramType = paramObject == null ? null : paramObject.GetType();
            string targetMethodName = string.Format("OnNotification_{0}", notification);
            for (int index = 0; index < _viewMethodInfos.Length; index++) {
                var methodInfo = _viewMethodInfos[index];
                // 依次检查返回类型，函数名，参数
                if (!MVVMUtility.CheckMethodFullMatch(methodInfo,
                    targetMethodName,
                    typeof(void),
                    paramType)) {
                    continue;
                }
                methodInfo.Invoke(_view, new[] {paramObject});
                break;
            }
        }
    }
}