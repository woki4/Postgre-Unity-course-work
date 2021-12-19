using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoginButton : MonoBehaviour
{
   [SerializeField] private ServerSpeaker _serverSpeaker;
   [Space]
   [SerializeField] private TMP_InputField _loginField;
   [SerializeField] private TMP_InputField _passwordField;
   
   public void CheckPassword()
   {
      string login = _loginField.text;
      string password = _passwordField.text;
     
      _serverSpeaker.CheckLoginAndPassword(login, password);
   }
}
