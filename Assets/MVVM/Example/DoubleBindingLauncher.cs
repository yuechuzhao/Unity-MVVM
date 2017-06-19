﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleBindingLauncher : MonoBehaviour {
    public bool UsingClassData;
    public DoubleBindingView setupView;
    public DoubleBindingClassView setupClassView;
    private void Awake(){
        Debug.LogFormat("Awake");

        //绑定上下文
        var viewModel = new DoubleBindingViewModel();
        setupClassView.BindingContext = viewModel;
        if (UsingClassData) {
            setupClassView.BindingContext = viewModel;
            setupView.gameObject.SetActive(false);
            setupClassView.gameObject.SetActive(true);
        }
        else {
            setupView.BindingContext = viewModel;
            setupView.gameObject.SetActive(true);
            setupClassView.gameObject.SetActive(false);
        }
    }
}
