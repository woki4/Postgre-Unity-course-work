using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TaskCompleteChanger : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputField;
    private ServerSpeaker _serverSpeaker;
    private TasksTable _tasksTable;

    private void Start()
    {
        _serverSpeaker = FindObjectOfType<ServerSpeaker>();
        _tasksTable = GetComponentInParent<TasksTable>();
    }

    public void SetTaskCompleteCondition()
    {
        string id = transform.parent.parent.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text;
        string condition = _inputField.text.ToLower();
       _serverSpeaker.SetTaskCondition(id, condition);
       _tasksTable.UpdateTable();
    }
}
