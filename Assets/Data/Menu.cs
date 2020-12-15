using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    private Button simulationButton;
    private Text simulationButtonText;

    private Button randomizeButton;
    private Text randomizeButtonText;

    private Slider ratioSlider;
    private Text ratioSliderText;

    private Slider simulationSpeedSlider;
    private Text simulationSpeedSliderText;

    private void Start()
    {
        GameObject.Find("SimulationButton").TryGetComponent(out simulationButton);
        simulationButtonText = simulationButton.transform.GetComponentInChildren<Text>();
        simulationButtonText.text = "Start Simulation";
        simulationButton.onClick.AddListener(Simulation);

        GameObject.Find("RandomizeButton").TryGetComponent(out randomizeButton);
        randomizeButtonText = randomizeButton.transform.GetComponentInChildren<Text>();
        randomizeButtonText.text = "Randomize";
        randomizeButton.onClick.AddListener(Randomize);


        GameObject.Find("RatioSlider").TryGetComponent(out ratioSlider);
        ratioSlider.minValue = 0;
        ratioSlider.maxValue = 100;
        ratioSlider.value = 10;
        ratioSlider.onValueChanged.AddListener(AdjustRatio);
        ratioSliderText = ratioSlider.transform.GetComponentInChildren<Text>();
        ratioSliderText.text = "Cell Density : " + ratioSlider.value + "%";

        GameObject.Find("SimulationSpeedSlider").TryGetComponent(out simulationSpeedSlider);
        simulationSpeedSlider.minValue = 1;
        simulationSpeedSlider.maxValue = 5;
        simulationSpeedSlider.value = 5;
        simulationSpeedSlider.wholeNumbers = true;
        simulationSpeedSlider.onValueChanged.AddListener(AdjustSimulatingSpeed);
        simulationSpeedSliderText = simulationSpeedSlider.transform.GetComponentInChildren<Text>();
        simulationSpeedSliderText.text = "Simulation Speed : " + simulationSpeedSlider.value;
    }

    private void Randomize()
    {
        LifeManager.main.RandomizeSeed();
        RandomizeTextSwitch();
    }

    private void RandomizeTextSwitch()
    {
        simulationButtonText.text = "Start Simulation";
        LifeManager.main.StopSimulation();
    }

    private void Simulation()
    {
        SimulationTextSwitch(LifeManager.main.SwitchSimulation());
    }

    private void SimulationTextSwitch(bool value)
    {
        simulationButtonText.text = value ? "Pause" : "Resume";
    }

    private void AdjustRatio(float newValue)
    {
        ratioSliderText.text = "Cell Density : " + Mathf.Round(newValue * 100f) / 100f + "%";
        LifeManager.main.AdjustFillRatio(newValue);
    }

    private void AdjustSimulatingSpeed(float newValue)
    {
        simulationSpeedSliderText.text = "Simulation Speed : " + newValue;
        LifeManager.main.AdjustSimulationSpeed((6 - newValue) / 10);
    }
}
