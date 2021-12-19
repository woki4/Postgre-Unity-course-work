using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GenerateReportButton : MonoBehaviour
{
    [SerializeField] private ServerSpeaker _serverSpeaker;
    [Space]
    [SerializeField] private TMP_InputField _idField;
    [SerializeField] private TMP_InputField _startDateField;
    [SerializeField] private TMP_InputField _endDateField;
   
    public void GenerateReport()
    {
        string id = _idField.text;
        string startDate = _startDateField.text;
        string endDate = _endDateField.text;
     
        _serverSpeaker.ReceiveReport(id, startDate, endDate);
    }
}
