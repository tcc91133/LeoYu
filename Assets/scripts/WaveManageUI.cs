using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveManageUI : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI timeText;

    public void UpdateWaveText(string waveSrting) => waveText.text = waveSrting;
    public void UpdateTimerText(string timerSrting) => timeText.text = timerSrting;
}
