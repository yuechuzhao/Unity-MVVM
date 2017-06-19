using Client.Framework;
using UnityEngine;

public interface IView {
    ViewModel BindingContext { get; set; }
}

public class UnityGuiView : MonoBehaviour, IView {
    public readonly BindableProperty ViewModelProperty = new BindableProperty();
    public ViewModel BindingContext {
        get {
            ViewModel value = ViewModelProperty.Value as ViewModel;
            if (value == null) {
                Debug.LogErrorFormat("设值错误，ViewModelProperty容器内不是ViewModel");
            }
            return (value);
        }
        set {
            // 有顺序，必须先setType，不然会报空异常
            if (value == null) {
                Debug.LogErrorFormat("BindContext不允许为空!");
                return;
            }
            _unityGuiViewHelper.SetType(GetType(), value.GetType());
            ViewModelProperty.Value = value;
            BindingContext.InitProperties();
        }
    }

    private readonly UnityGuiViewHelper _unityGuiViewHelper = new UnityGuiViewHelper();

    private void OnBindingContextChanged (object oldViewModelObj, object newViewModelObj) {
        ViewModel oldViewModel = oldViewModelObj as ViewModel;
        ViewModel newViewModel = newViewModelObj as ViewModel;
        _unityGuiViewHelper.ReattatchViewListener(this, oldViewModel, newViewModel);
    }

    public UnityGuiView () {
        this.ViewModelProperty.OnValueChanged += OnBindingContextChanged;
    }

    protected void SendCommand(string command, object param) {
        BindingContext.ReceiveCommand(command, param);
    }

}