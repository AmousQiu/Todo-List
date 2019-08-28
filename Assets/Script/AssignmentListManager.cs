/* 
 * ------------------------------------------------------------------------------------------
                                         _____ _                     _                _    _ 
      /\                                / ____| |                   | |              | |  (_)
     /  \   _ __ ___   ___  _   _ ___  | |    | |__   ___   ___ ___ | | _____   _____| | ___ 
    / /\ \ | '_ ` _ \ / _ \| | | / __| | |    | '_ \ / _ \ / __/ _ \| |/ _ \ \ / / __| |/ / |
   / ____ \| | | | | | (_) | |_| \__ \ | |____| | | | (_) | (_| (_) | | (_) \ V /\__ \   <| |
  /_/    \_\_| |_| |_|\___/ \__,_|___/  \_____|_| |_|\___/ \___\___/|_|\___/ \_/ |___/_|\_\_|           
                                                                                                                                                                          
 * <AmousQiu@dal.ca> wrote this file. As long as you retain this notice you
 * can do whatever you want with this stuff. If we meet some day, and you think this stuff is
 * worth it, you can buy me a beer in return (Personal prefer Garrison Raspberry).
 *                                                                        @Copyright Ziyu Qiu
 * ------------------------------------------------------------------------------------------
 */

 /*-------------------------------------------------------------------------------------------
  *FILE INTRODUTION PART 
  *-------------------------------------------------------------------------------------------
  *FileName: AssignmentListManager.cs
  *--------------------------------------------------------------------------------------------
  *Function: -For the assignment list. 
  *          -save, upload, load from storage
  *          -save, upload, load from web server
  *          -using jsonData
  *---------------------------------------------------------------------------------------------
  *Json Data File: 
  *          - {"objName":"1","index":0}
  *          - {"objName":"2","index":1}
  *          - objName: the tuple user insert
  *          - index: used for order
  *----------------------------------------------------------------------------------------------
  */ 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;
using System.Text;

public class AssignmentListManager : MonoBehaviour
{
    public Transform content;
    public Button createButton;
    public GameObject checkListItemPrefab;
    public InputField input;

    string filePath;
 
    string jsonName = "AssignmentList.json";
    private List<AssignmentListObject> AssignmentListObjects = new List<AssignmentListObject>();

    public class AssignmentlistItem
    {
        public string objName;
        public int index;
        public AssignmentlistItem(string name, int index)
        {
            this.objName = name;
            this.index = index;
        }
    }

    private void Start()
    {
        //This is for Android device
        filePath = Path.Combine(Application.persistentDataPath, jsonName);
        loadJsonData();
    }

    public void CreateNewItem()
    {
        string temp = input.text;
        CreateCheckListItems(temp);
        Debug.Log(temp);
        input.text = "";
    }

    public void CreateCheckListItems(string name, int loadIndex = 0, bool loading = false)
    {
        GameObject item = Instantiate(checkListItemPrefab);
        item.transform.SetParent(content);
        AssignmentListObject itemObject = item.GetComponent<AssignmentListObject>();
        int index = 0;
        if (!loading)
        {
            index = AssignmentListObjects.Count;
        }
        itemObject.setObjectInfo(name, index);
        Debug.Log("itemObject name" + itemObject.objName);
        itemObject.GetComponentInChildren<Text>().text = name;
        AssignmentListObjects.Add(itemObject);
        AssignmentListObject temp = itemObject;
        itemObject.GetComponent<Toggle>().onValueChanged.AddListener((delegate { CheckItem(temp); }));
        if (!loading)
        {
            saveJsonData();
        }
        upload();
    }

    void CheckItem(AssignmentListObject item)
    {
        AssignmentListObjects.Remove(item);
        saveJsonData();
        Destroy(item.gameObject);
        upload();
    }

    void saveJsonData()
    {
        string contents = "";

        for (int i = 0; i < AssignmentListObjects.Count; i++)
        {
            AssignmentlistItem temp = new AssignmentlistItem(AssignmentListObjects[i].objName, AssignmentListObjects[i].index);
            contents += JsonUtility.ToJson(temp) + "\n";
        }
        Debug.Log(filePath);

        System.IO.File.WriteAllText(filePath, contents);

    }


    public void upload()
    {
        string contents = "";
        string url = "https://web.cs.dal.ca/~zqiu/UnityAPI/jsonServer/UnityUpload.php";
        for (int i = 0; i < AssignmentListObjects.Count; i++)
        {
            AssignmentlistItem temp = new AssignmentlistItem(AssignmentListObjects[i].objName, AssignmentListObjects[i].index);
            contents += JsonUtility.ToJson(temp) + "\n";
        }

        byte[] bytes = Encoding.ASCII.GetBytes(contents);
        Debug.Log(bytes.ToString());

        WWWForm form = new WWWForm();
        form.AddField("Name","AssignmentList");
        form.AddBinaryData("post", bytes);
        WWW www = new WWW(url, form);
    }


    //load json data from storage
    void loadJsonData()
    {
        string dataAsJson = "";

        //dataAsJson = System.IO.File.ReadAllText(filePath);
        UnityWebRequest www = UnityWebRequest.Get(filePath);
        www.SendWebRequest();
        while (!www.isDone) { }
        if (string.IsNullOrEmpty(www.error))
        {
            dataAsJson = www.downloadHandler.text;
        }
        string[] splitContents = dataAsJson.Split('\n');

        foreach (string content in splitContents)
        {
            if (content.Trim() != "")
            {
                AssignmentlistItem temp = JsonUtility.FromJson<AssignmentlistItem>(content.Trim());
                CreateCheckListItems(temp.objName, temp.index, true);
            }
        }

    }
}

