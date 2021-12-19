using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class ServerSpeaker : MonoBehaviour
{
    [SerializeField] private UIManager _uiManager;

    public void CheckLoginAndPassword(string login, string password)
    {
        string message = $"login;{login};{password}";
        int id;
        
        if (Int32.TryParse(Client.GetDataFromServer(message), out id))
        {
            Debug.Log("User " + login + " id: " + id + " entered in system");
            SetCurrentUser(id, password);
            _uiManager.ShowUserScreen();
        }
    }

    private void SetCurrentUser(int id, string password)
    {
        string[] currentUserData =  GetUserDataById(id);

        CurrentUserData.userLogin = currentUserData[0];
        CurrentUserData.userPassword = password;
        CurrentUserData.userID = id.ToString();
        CurrentUserData.userRole = currentUserData[1];
        CurrentUserData.userName = currentUserData[2];
    }

    private string[] GetUserDataById(int id)
    {
        string message = $"getUserData;{id}";
        string recievedData = Client.GetDataFromServer(message);
        
        string[] userData = recievedData.Split(';');

        return userData;
    }

    public void CreateUser(string login, string password, string role, string name)
    {
        string message = $"register;{login};{password};{role};{name}";
        
        string recievedData = Client.GetDataFromServer(message);
        
        Debug.Log(recievedData);
    }

    public string[] GetAllEmployees()
    {
        string message = "getEmployees";
        string recievedData = Client.GetDataFromServer(message);
        
        string[] employees = recievedData.Split(';');

        foreach (string employee in employees)
        {
            Debug.Log(employee);
        }

        return employees;
    }
    
    public string[] GetUserTasks()
    {
        string message = $"getTasks;{CurrentUserData.userLogin};{CurrentUserData.userPassword}";
        string recievedData = Client.GetDataFromServer(message);
        
        string[] tasks = recievedData.Split(';');

        foreach (string task in tasks)
        {
            Debug.Log(task);
        }

        return tasks;
    }

    public void SetTaskCondition(string id, string condition)
    {
        string message = $"setTaskCondition;{CurrentUserData.userLogin};{CurrentUserData.userPassword};{id};{condition}";
        string recievedData = Client.GetDataFromServer(message);
        
        Debug.Log(recievedData);
    }

    public void CreateTask(string executorID, string authorID, string contractNumber, string equipmentNumber,
        string contactPersonID, string finishDate, string priority, string task)
    {
        string message = $"createTask;{CurrentUserData.userLogin};{CurrentUserData.userPassword};" +
                         $"{executorID};{authorID};{contractNumber};{equipmentNumber};{contactPersonID};" +
                         $"{finishDate};{priority};{task};";
        
        string recievedData = Client.GetDataFromServer(message);
        
        Debug.Log(recievedData);
    }

    public string[] FindContactPerson(string name)
    {
        string message = $"findContactPerson;{name}";
        string recievedData = Client.GetDataFromServer(message);
        
        string[] contactPerson = recievedData.Split(';');

        foreach (string data in contactPerson)
        {
            Debug.Log(data);
        }

        return contactPerson;
    }

    public void ReceiveReport(string id, string startDate, string finishDate)
    {
        string message = $"receiveReport;{id};{startDate};{finishDate}";
        Client.GetDataFromServer(message);
    }
}
