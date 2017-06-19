using Client.Framework.Example;
using UnityEngine;
using UnityEngine.UI;

public class DoubleBindingClassView : UnityGuiView {
    public Slider Progress;
    public Text MaxCoins;
    public Text CurrentCoins;

    public Button SubmitButton;

    /// <summary>
    /// 控制或输入模块
    /// </summary>
    private void Awake() {
        Progress.onValueChanged.AddListener(OnSliderChanged);
        SubmitButton.onClick.AddListener(OnClickSubmit);
    }

    private void OnClickSubmit() {
        SendCommand("SubmitData", null);
    }

    private void OnSliderChanged(float progress) {
        //Debuger.LogFormat("progress is {0} ", progress);
        //BindingContext.SetProperty("Progress", progress);
        SendCommand("ChangeProgress", progress);
    }

    /// <summary>
    /// 反射调用的方法，意味着当ViewModel当中的CurrentValue属性发生变化时，本地视图上相应的元素也要发生变化
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    private void OnChanged_Data(object oldValue, object newValueObj) {
        var newValue = newValueObj as DoubleBindingModel;
        //Debuger.LogFormat("OnCurrentValueChanged, {0}, {1}", oldValue, newValue);
        CurrentCoins.text = string.Format("贡献金币:{0}", newValue.Coins);
        Progress.value = newValue.Progress;
        MaxCoins.text = string.Format("总金币:{0}", newValue.MaxCoins);
    }

    private void OnNotification_SubmitSuccess(object param) {
        Debug.LogFormat("提交成功！{0}", Time.frameCount);
    }

}
