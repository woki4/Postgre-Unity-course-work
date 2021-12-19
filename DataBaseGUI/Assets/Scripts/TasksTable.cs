using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TasksTable : MonoBehaviour
{
    [SerializeField] private ServerSpeaker _serverSpeaker;
    [SerializeField] private GameObject _taskPrefab;
    [SerializeField] private Transform _tableContent;
    
    private void OnEnable()
    {
        UpdateTable();
    }

    public void UpdateTable()
    {
        for(int i = _tableContent.childCount - 1; i > 0; i--)
        {
            Destroy(_tableContent.GetChild(i).gameObject);
        }
        
        string[] tasks = _serverSpeaker.GetUserTasks();

        for (int i = 0; i < tasks.Length - 1; i++)
        {
            string[] taskData = tasks[i].Split('|');
            
            GameObject taskPrefab = Instantiate(_taskPrefab, _tableContent);
            
            taskPrefab.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = taskData[0]; //id
            taskPrefab.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = taskData[1]; //executor_id
            taskPrefab.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = taskData[2]; //author_id
            taskPrefab.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = taskData[3]; //contact_number
            taskPrefab.transform.GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>().text = taskData[4]; //equipment_number
            taskPrefab.transform.GetChild(5).GetChild(0).GetComponent<TextMeshProUGUI>().text = taskData[5]; //contact_person
            taskPrefab.transform.GetChild(6).GetChild(0).GetComponent<TextMeshProUGUI>().text = taskData[6]; //task
            taskPrefab.transform.GetChild(7).GetChild(0).GetComponent<TextMeshProUGUI>().text = taskData[7]; //start_date
            taskPrefab.transform.GetChild(8).GetChild(0).GetComponent<TextMeshProUGUI>().text = taskData[8]; //finish_date
            taskPrefab.transform.GetChild(9).GetChild(0).GetComponent<TextMeshProUGUI>().text = taskData[9]; //priority
            taskPrefab.transform.GetChild(10).GetChild(0).GetComponent<TextMeshProUGUI>().text = taskData[10]; //complete_date
            taskPrefab.transform.GetChild(11).GetChild(0).GetComponent<TMP_InputField>().text = taskData[11]; //is_completed
            
            
            if ((CurrentUserData.userRole.ToLower() == "рабочий" || CurrentUserData.userRole.ToLower() == "менеджер") &&
                taskData[11].ToLower() == "true")
            {
                taskPrefab.transform.GetChild(11).GetChild(0).GetComponent<TMP_InputField>().interactable = false;
            }
        }
    }
}
