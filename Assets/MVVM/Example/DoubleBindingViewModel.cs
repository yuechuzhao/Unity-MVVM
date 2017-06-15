using Client.Framework;
using Client.Framework.Example;
using JetBrains.Annotations;

public class DoubleBindingViewModel : ViewModel {
    public BindableProperty<int> MaxValue  = new BindableProperty<int>();
    public BindableProperty<int> CurrentValue = new BindableProperty<int>();
    public BindableProperty<float> Progress = new BindableProperty<float>();//当前进度比例，0~1

    public BindableProperty<DoubleBindingModel> Data = new BindableProperty<DoubleBindingModel>();


    public override void InitProperties() {
        MaxValue.Reset(1000);
        Progress.Reset(0.1f);
        CurrentValue.Reset(100);
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
        //Debuger.LogFormat("OnProgressChanged, old {0} new {1}", oldProgress, newProgress);
        CurrentValue.Value = (int)(MaxValue.Value * newProgress);
    }

    public void OnChanged_MaxValue(float oldMaxValue, float newMaxValue) {
        //Debuger.LogFormat("OnMaxValueChanged, old {0} new {1}", oldMaxValue, newMaxValue);
        CurrentValue.Value = (int)(newMaxValue * Progress.Value);
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
        CurrentValue.Value = (int)(MaxValue.Value * Progress.Value);
        //Debuger.LogFormat("MaxValue.Value {0}, CurrentValue {1}", MaxValue.Value, CurrentValue.Value);
        MaxValue.Value = MaxValue.Value - CurrentValue.Value;
        Progress.Value = 0;
    }

    /// <summary>
    /// 反射调用，相当于以前的OnEventTriggered。这里使用的
    /// </summary>
    /// <param name="param"></param>
    private void OnCommand_SubmitData(object param) {
        var model = Data.Value;
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
    private void OnCommand_ChangeProgress(float param) {
        var model = Data.Value;
        model.Progress = param;
        Data.Reset(model);
    }
}
