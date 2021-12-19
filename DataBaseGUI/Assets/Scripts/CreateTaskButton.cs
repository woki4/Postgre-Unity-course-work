using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreateTaskButton : MonoBehaviour
{
    [SerializeField] private ServerSpeaker _serverSpeaker;
    [Space]
    [SerializeField] private TMP_InputField _executorIdInput;
    [SerializeField] private TMP_InputField _authorIdInput;
    [SerializeField] private TMP_InputField _contractNumberInpt;
    [SerializeField] private TMP_InputField _equipmentNumberInput;
    [SerializeField] private TMP_InputField _contactPersonIdInput;
    [SerializeField] private TMP_InputField _finishDateInput;
    [SerializeField] private TMP_InputField _priorityInput;
    [SerializeField] private TMP_InputField _taskInput;

    public void CreateTask()
    {
        string executorId = _executorIdInput.text.Length == 0 ? "null" : _executorIdInput.text;
        string authorId = _authorIdInput.text.Length == 0 ? "null" : _authorIdInput.text;
        string contractNumber = _contractNumberInpt.text.Length == 0 ? "null" : _contractNumberInpt.text;
        string equipmentNumber = _equipmentNumberInput.text.Length == 0 ? "null" : _equipmentNumberInput.text;
        string contactPersonId = _contactPersonIdInput.text.Length == 0 ? "null" : _contactPersonIdInput.text;
        string finishDate = _finishDateInput.text.Length == 0 ? "null" : _finishDateInput.text;
        string priority = _priorityInput.text.Length == 0 ? "null" : _priorityInput.text;
        string task = _taskInput.text.Length == 0 ? "null" : _taskInput.text;
        
        _serverSpeaker.CreateTask(executorId, authorId, contractNumber, equipmentNumber, contactPersonId, finishDate,
            priority, task);
    }
}
