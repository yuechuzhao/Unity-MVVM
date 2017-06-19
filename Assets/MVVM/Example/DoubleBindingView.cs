using System;
using UnityEngine;
using UnityEngine.UI;

public class DoubleBindingView : UnityGuiView {
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
        BindingContext.ReceiveCommand("Submit", null);
    }


    private void OnSliderChanged(float progress) {
        if (BindingContext == null) {
            return;
        }
        //Debuger.LogFormat("progress is {0} ", progress);
        BindingContext.SetProperty("Progress", progress);
    }

    /// <summary>
    /// 反射调用的方法，意味着当ViewModel当中的CurrentValue属性发生变化时，本地视图上相应的元素也要发生变化
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    private void OnChanged_CurrentValue(object oldValue, object newValue ) {
        //Debuger.LogFormat("OnCurrentValueChanged, {0}, {1}", oldValue, newValue);
        CurrentCoins.text = string.Format("贡献金币:{0}", newValue.ToString());
    }

    /// <summary>
    /// 反射调用的方法，意味着当ViewModel当中的CurrentValue属性发生变化时，本地视图上相应的元素也要发生变化
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    private void OnChanged_CurrentValue(int oldValue, int newValue) {
        //Debuger.LogFormat("OnCurrentValueChanged, {0}, {1}", oldValue, newValue);
        CurrentCoins.text = string.Format("贡献金币:{0}", newValue);
    }

    /// <summary>
    /// 反射调用的方法，意味着当ViewModel当中的Progress属性发生变化时，本地视图上相应的元素也要发生变化
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    private void OnChanged_Progress(float oldValue, float newValue) {
        //Debug.LogFormat("OnProgressChanged, {0}, {1}", oldValue, newValue);
        Progress.value = newValue;
    }

    /// <summary>
    /// 反射调用的方法，意味着当ViewModel当中的MaxValue属性发生变化时，本地视图上相应的元素也要发生变化
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    private void OnChanged_MaxValue(int oldValue, int newValue) {
        //Debuger.LogFormat("OnMaxValueChanged, {0}, {1}", oldValue, newValue);
        MaxCoins.text = string.Format("总金币:{0}", newValue);
    }
}
