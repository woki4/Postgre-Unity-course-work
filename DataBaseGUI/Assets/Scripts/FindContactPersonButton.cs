using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FindContactPersonButton : MonoBehaviour
{
    [SerializeField] private ServerSpeaker _serverSpeaker;
    [Space]
    [SerializeField] private TMP_InputField _nameField;
    [SerializeField] private GameObject _table;
    [SerializeField] private GameObject _head;

    private void OnEnable()
    {
        _table.SetActive(false);
        _head.SetActive(false);
    }

    public void FindContactPerson()
    {
        string[] contactPerson = _serverSpeaker.FindContactPerson(_nameField.text);

        _table.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = contactPerson[0];
        _table.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = contactPerson[1];
        
        _table.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = contactPerson[2];
        _table.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = contactPerson[3];
        _table.transform.GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>().text = contactPerson[4];
        _table.transform.GetChild(5).GetChild(0).GetComponent<TextMeshProUGUI>().text = contactPerson[5];
        
        _table.SetActive(true);
        _head.SetActive(true);
    }
}
