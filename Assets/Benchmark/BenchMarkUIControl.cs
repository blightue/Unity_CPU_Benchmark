using System;
using System.Collections;
using System.Collections.Generic;
using Benchmark;
using UnityEngine;
using UnityEngine.UIElements;

public class BenchMarkUIControl : MonoBehaviour
{
    public UIDocument document;
    public BenchMarkMonoBehavior bmNet;
    public BenchMarkBurst bmJob;

    private TextField countInput;
    private ScrollView scrollView;
    private Button dotnetButton;
    private Button jobsystemButton;
    private Button clearButton;

    // Start is called before the first frame update
    void Start()
    {
        countInput = document.rootVisualElement.Query<TextField>("Count");
        scrollView = document.rootVisualElement.Query<ScrollView>("LogScrollView");
        dotnetButton = document.rootVisualElement.Query<Button>("MonoBehavior");
        jobsystemButton = document.rootVisualElement.Query<Button>("Jobsystem");
        clearButton = document.rootVisualElement.Query<Button>("Clear");

        dotnetButton.clicked += OnDotnetClick;
        jobsystemButton.clicked += OnJobClick;
        clearButton.clicked += OnClear;
    }

    private void Update()
    {
        if (bmNet.loop == bmNet.maxLoop && bmJob.loop == bmJob.maxLoop)
        {
            dotnetButton.SetEnabled(true);
            jobsystemButton.SetEnabled(true);
        }
        else
        {
            dotnetButton.SetEnabled(false);
            jobsystemButton.SetEnabled(false);
        }
    }
    
    private void OnJobClick()
    {
        bmJob.count = int.Parse(countInput.text);
        bmJob.loop = 0;
    }
    
    private void OnDotnetClick()
    {
        bmNet.count = int.Parse(countInput.text);
        bmNet.loop = 0;
    }

    private void OnClear()
    {
        scrollView.Clear();
    }
}