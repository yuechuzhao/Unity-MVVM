using System;
using Client.Framework;
using Client.Framework.Example;
using JetBrains.Annotations;
using UnityEngine;

public class DoubleBindingViewModel : ViewModel {
    public BindableProperty MaxValue  = new BindableProperty(typeof(int));
    public BindableProperty CurrentValue = new BindableProperty(typeof(int));
    public BindableProperty Progress = new BindableProperty(typeof(float));//当前进度比例，0~1

    public BindableProperty Data = new BindableProperty(typeof(DoubleBindingModel));


    public override void InitProperties() {
        MaxValue.Reset(10000);
        Progress.Reset(0.1f);
        CurrentValue.Reset(1000);
        Data.Reset(new DoubleBindingModel() {
            Coins = 1000,
            MaxCoins = 10000,
            Progress = 0.1f
        });
    }

    /// <summary>
    /// 由反射调用，相当于以前的OnPropertyChanged
    /// </summary>
    /// <param name="oldProgress"></param>
    /// <param name="newProgress"></param>
    public void OnChanged_Progress(float oldProgress, float newProgress) {
        //Debug.LogFormat("OnProgressChanged, old {0} new {1}", oldProgress, newProgress);
        CurrentValue.Value = (int)(MaxValue.GetInt() * newProgress);
    }

    public void OnChanged_MaxValue(float oldMaxValue, float newMaxValue) {
        //Debuger.LogFormat("OnMaxValueChanged, old {0} new {1}", oldMaxValue, newMaxValue);
        CurrentValue.Value = (int)(newMaxValue * Progress.GetFloat());
    }

    public void OnChanged_Data(DoubleBindingModel old, DoubleBindingModel current) {
        current.Coins = (int)(current.MaxCoins * current.Progress); 
    }

    /// <summary>
    /// 反射调用，相当于以前的OnEventTriggered
    /// </summary>
    /// <param name="param"></param>
    private void OnCommand_Submit(object param) {
        //Debuger.LogFormat("OnCommand_Submit, {0}, Progress.Value {1}", param, Progress.Value);
        CurrentValue.Value = (int)(MaxValue.GetInt() * Progress.GetFloat());
        //Debuger.LogFormat("MaxValue.Value {0}, CurrentValue {1}", MaxValue.Value, CurrentValue.Value);
        MaxValue.Value = MaxValue.GetInt() - CurrentValue.GetInt();
        Progress.Value = 0;
    }

    /// <summary>
    /// 反射调用，相当于以前的OnEventTriggered。这里使用的
    /// </summary>
    /// <param name="param"></param>
    private void OnCommand_SubmitData(object param) {
        var model = Data.Value as DoubleBindingModel;
        Debug.Assert(model != null, "model 's type error");
        model.Coins = (int) (model.MaxCoins * model.Progress);
        model.MaxCoins = model.MaxCoins - model.Coins;
        model.Coins = 0;
        model.Progress = 0;
        Data.Reset(model);
        SendNotification("SubmitSuccess", null);
    }


    /// <summary>
    /// 反射调用，相当于以前的OnEventTriggered。这里使用的
    /// </summary>
    /// <param name="param"></param>
    private void OnCommand_ChangeDataProgress(float param) {
        var model = Data.Value as DoubleBindingModel;
        model.Progress = param;
        Data.Reset(model);
    }
}
