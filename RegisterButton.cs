using System.Data.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using TMPro;
using UltimateClean;

public class RegisterButton : MonoBehaviour
{
    public TMP_InputField nameField;
    public TMP_InputField passwordField;
    public TMP_InputField passwordField_2;

    public Notification registerErrorPop;

    public TMP_Text registerText;

    [SerializeField] private bool nameCheckPass = false;
    [SerializeField] private bool passwordCheckPass = false;
    [SerializeField] private bool passwordCheckPass_2 = false;


    private void Start()
    {
        nameField.onEndEdit.AddListener(NameCheck);
        passwordField.onEndEdit.AddListener(PasswordCheck);
        passwordField_2.onEndEdit.AddListener(PasswordCheck_2);
    }

    public void CallRegister()
    {
        if (nameCheckPass && passwordCheckPass && passwordCheckPass_2)
        {
            //流鼻涕了，我操
            StartCoroutine(Register());
        }
        else
        {

        }
    }

    [System.Obsolete]


    IEnumerator Register()
    {
        WWWForm form = new WWWForm();
        form.AddField("user_name", nameField.text);
        form.AddField("user_password", passwordField.text);
        WWW www = new WWW("http://localhost:81/sqlconnect/register.php", form);
        Debug.Log(nameField.text + passwordField.text);
        yield return www;
        if (www.text == "0")
        {
            Debug.Log("register success");
        }
        else
        {
            Debug.Log("error #" + www.text);
        }
    }

    void NameCheck(string _name)
    {
        if (_name.Length > 12 || _name.Length < 2)
        {
            nameCheckPass = false;
            registerErrorPop.Launch(NotificationType.Pop,NotificationPositionType.BottomMiddle,0f,"错误","报错内容");
        }
        else
        {
            nameCheckPass = true;
        }
    }

    void PasswordCheck(string _password)
    {
        if (_password.Length > 16 || _password.Length < 6)
        {
            passwordCheckPass = false;
        }
        else
        {
            passwordCheckPass = true;
        }
    }

    void PasswordCheck_2(string _password)
    {
        if (passwordField.text != passwordField_2.text)
        {
            passwordCheckPass_2 = false;
        }
        else
        {
            passwordCheckPass_2 = true;
        }
    }
}
