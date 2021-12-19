using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreateUserButton : MonoBehaviour
{
    [SerializeField] private ServerSpeaker _serverSpeaker;
    [Space]
    [SerializeField] private TMP_InputField _loginField;
    [SerializeField] private TMP_InputField _passwordField;
    [SerializeField] private TMP_Dropdown _roleDropdown;
    [SerializeField] private TMP_InputField _nameField;

    public void CreateUser()
    {
        string login = _loginField.text;
        string password = _passwordField.text;
        string role = _roleDropdown.options[_roleDropdown.value].text;
        string name = _nameField.text;
        
        _serverSpeaker.CreateUser(login, password, role, name);
    }
}
