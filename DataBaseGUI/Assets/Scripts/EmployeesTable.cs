using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EmployeesTable : MonoBehaviour
{
    [SerializeField] private ServerSpeaker _serverSpeaker;
    [SerializeField] private GameObject _employeePrefab;
    [SerializeField] private Transform _tableContent;
    
    private void OnEnable()
    {
       UpdateTable();
    }

    private void UpdateTable()
    {
        for(int i = _tableContent.childCount - 1; i > 0; i--)
        {
            Destroy(_tableContent.GetChild(i).gameObject);
        }
        
        string[] employees = _serverSpeaker.GetAllEmployees();

        for (int i = 0; i < employees.Length - 1; i++)
        {
            string[] employeeData = employees[i].Split('|');
            
            GameObject employeePrefab = Instantiate(_employeePrefab, _tableContent);
            
            employeePrefab.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = employeeData[0]; //id
            employeePrefab.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = employeeData[1]; //login
            employeePrefab.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = employeeData[2]; //role
            employeePrefab.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = employeeData[3]; //name
        }
    }
}
