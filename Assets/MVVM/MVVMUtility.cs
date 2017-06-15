using System;
using System.Reflection;

namespace Client.Framework {

    /// <summary>
    /// 根据的属性XXX, 获取Methods集合中当中对应的OnXXXChanged的Method
    /// 判断条件：
    /// 1.名字
    /// 2.参数列表相同
    /// 3.返回值为void
    /// </summary>
    /// <returns></returns>
    public static class MVVMUtility {
        public static bool TryGetListenerMethod(MethodInfo[] methodInfos, string propertyName, Type fieldType, out MethodInfo methodInfo) {
            methodInfo = null;
            if (methodInfos.Length == 0) {
                return false;
            }
            string targetMethodName = string.Format("OnChanged_{0}", propertyName);
            for (int index = 0; index < methodInfos.Length; index++) {
                var method = methodInfos[index];
                if (!CheckMethodFullMatch(method, targetMethodName, typeof(void), fieldType, fieldType)) {
                   continue;
                }
                methodInfo = method;
                //Debuger.LogFormat("找到了目标methodInfo, 目标是{0}", methodInfo);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 遍历所有bindProperties，做添加或者删除
        /// </summary>
        /// <param name="viewModelFields"></param>
        /// <param name="listenClassMethods"></param>
        /// <param name="onDelegateFound"></param>
        public static void DealBindPropertiesListener(FieldInfo[] viewModelFields, MethodInfo[] listenClassMethods,
            System.Action<FieldInfo, EventInfo, Type, MethodInfo> onDelegateFound) {
            // 找到泛型定义类
            Type binderDefType = typeof(BindableProperty<>);
            Type delegateDefType = typeof(BindableProperty<>.ValueChangedHandler);
            // 遍历所有的field，当field是BinderProperty<T>时，自动进行BinderProperty的委托操作
            for (int index = 0; index < viewModelFields.Length; index++) {
                var field = viewModelFields[index];
                System.Type fieldType = field.FieldType;
                // 确认是不是bindableProperty<T>
                if (fieldType.GetGenericTypeDefinition() != binderDefType) continue;
                Type genericParamType = fieldType.GetGenericArguments()[0];//如int,float之类的

                // 检查本类当中有没有对应的OnXXXChanged方法
                MethodInfo listenerMethodInfo = null;
                if (!TryGetListenerMethod(listenClassMethods, field.Name, genericParamType, out listenerMethodInfo)) {
                    continue;
                }

                Type changeHandlerDeleType = delegateDefType.MakeGenericType(genericParamType);//创建onValueChange代理的实际类型
                // 得到onvaluechanged，确认其类型正是我们得到的这个泛型代理
                EventInfo onValueChanged = fieldType.GetEvent("OnValueChanged");
                Type tDelegate = onValueChanged.EventHandlerType;
                if (tDelegate != changeHandlerDeleType) continue;

                if (onDelegateFound != null) {
                    //// 添加到viewModel当中
                    onDelegateFound(field, onValueChanged, tDelegate, listenerMethodInfo);
                }
               
            }
        }

        /// <summary>
        /// 检查函数的名字，返回值，参数列表是否完全相等
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="targetMethodName"></param>
        /// <param name="returnType"></param>
        /// <param name="matchParamTypes"></param>
        /// <returns></returns>
        public static bool CheckMethodFullMatch(MethodInfo methodInfo, 
            string targetMethodName, 
            Type returnType, 
            params Type[] matchParamTypes) {
            if (methodInfo.ReturnType != returnType) return false;
            if (methodInfo.Name != targetMethodName) return false;

            var paramInfo = methodInfo.GetParameters();

            if (!IsParamTypeMatch(paramInfo, matchParamTypes)) return false;
            return true;
        }

        private static bool IsParamTypeMatch(ParameterInfo[] paramInfo, Type[] matchParamTypes) {
            // 如果传递了{null}这样的匹配类型数组，直接返回空
            if (paramInfo.Length == 0 && matchParamTypes.Length == 0) {
                return true;
            }

            if (paramInfo.Length != matchParamTypes.Length) {
                return false;
            }

            for (int pIndex = 0; pIndex < paramInfo.Length; pIndex++) {
                //如果指定参数类型为null，表示这个参数类型不做检查
                System.Type matchParamType = matchParamTypes[pIndex];
                if (matchParamType == null) {
                    continue;
                }
                //Debuger.LogFormat("paramType {0}, fieldType {1}", paramInfo[pIndex].ParameterType, fieldType);
                if (paramInfo[pIndex].ParameterType != matchParamTypes[pIndex]) {
                    return false;
                }
            }
            return true;
        }
    }
}