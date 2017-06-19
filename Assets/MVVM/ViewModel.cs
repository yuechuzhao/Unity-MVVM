
namespace Client.Framework {
    public class ViewModel {
        private ViewModelHelper _helper;

        public System.Action<string, object> OnNotificationPush;
        public ViewModel() {
            _helper = new ViewModelHelper(GetType());
            _helper.BindListeners(this);
        }

        /// <summary>
        /// 设置属性。将会触发对应类的OnChanged_xxx方法
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        public void SetProperty(string fieldName, object value) {
            _helper.SetBindProperty(this, fieldName, value);
        }

        /// <summary>
        /// 接受信号。触发类中OnCommand_xxx(param)方法
        /// </summary>
        /// <param name="command"></param>
        /// <param name="param"></param>
        public void ReceiveCommand(string command, object param) {
            _helper.DispatchCommand(this, command, param);
        }

        /// <summary>
        /// 发送通知。触发View中OnNotifaction_xxx(param)方法
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="param"></param>
        public void SendNotification(string notification, object param) {
            if (OnNotificationPush != null) {
                OnNotificationPush(notification, param);
            }
        }

        /// <summary>
        /// 必须主动调用才能调用在viewmodel当中的初始化
        /// </summary>
        public virtual void InitProperties() {
        }

    }
}
