using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _loginScreen;
    [SerializeField] private GameObject _adminScreen;
    [SerializeField] private GameObject _workerScreen;
    [SerializeField] private GameObject _managerScreen;
    
    public void ShowUserScreen()
    {
        _loginScreen.SetActive(false);
        
        switch (CurrentUserData.userRole.ToLower())
        {
            case "администратор":
                _adminScreen.SetActive(true);
                break;
            
            case "рабочий":
                _workerScreen.SetActive(true);
                break;
            
            case "менеджер":
                _managerScreen.SetActive(true);
                break;
        }
    }
}
