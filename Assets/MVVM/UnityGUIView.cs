using Client.Framework;
using UnityEngine;

public interface IView {
    ViewModel BindingContext { get; set; }
}

public class UnityGuiView : MonoBehaviour, IView {
    public readonly BindableProperty<ViewModel> ViewModelProperty = new BindableProperty<ViewModel>();
    public ViewModel BindingContext {
        get { return ViewModelProperty.Value; }
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

    private void OnBindingContextChanged (ViewModel oldViewModel, ViewModel newViewModel) {
        _unityGuiViewHelper.ReattatchViewListener(this, oldViewModel, newViewModel);
    }

    public UnityGuiView () {
        this.ViewModelProperty.OnValueChanged += OnBindingContextChanged;
    }

    protected void SendCommand(string command, object param) {
        BindingContext.ReceiveCommand(command, param);
    }

}