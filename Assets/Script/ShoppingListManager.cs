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
  *FileName: ShoppingListManager.cs
  *--------------------------------------------------------------------------------------------
  *Function: -For the shopping list. 
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

public class ShoppingListManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform content;
    public Button createButton;
    public GameObject checkListItemPrefab;

    //string filePath=Application.persistentDataPath+"/checkList.txt";
    string filePath;

    public InputField input;

    //public Text ShowText;

    private List<ShoppingListObject> ShoppingListObjects = new List<ShoppingListObject>();

    public class ShoppinglistItem
    {
        public string objName;

        public int index;
        public ShoppinglistItem(string name, int index)
        {
            this.objName = name;
            this.index = index;
        }
    }
    private void Start()
    {
        //filePath = "jar:file://" + Application.dataPath + "!/assets/" + "ShoppingList.json";
        filePath = Application.persistentDataPath + "/ShoppingList.json";

        loadJsonData();

    }


    public void CreateNewItem()
    {
        string temp = input.text;
        CreateCheckListItems(temp);
        Debug.Log(temp);
        input.text="";
    }

    public void CreateCheckListItems(string name, int loadIndex = 0, bool loading = false)
    {
        GameObject item = Instantiate(checkListItemPrefab);

        item.transform.SetParent(content);

        ShoppingListObject itemObject = item.GetComponent<ShoppingListObject>();

        int index = 0;
        if (!loading)
        {
            index = ShoppingListObjects.Count;
        }
        itemObject.setObjectInfo(name, index);
        Debug.Log("itemObject name" + itemObject.objName);
        itemObject.GetComponentInChildren<Text>().text = name;
        ShoppingListObjects.Add(itemObject);
        ShoppingListObject temp = itemObject;
        itemObject.GetComponent<Toggle>().onValueChanged.AddListener((delegate { CheckItem(temp); }));
        if (!loading)
        {
            saveJsonData();
        }
       upload();
        /* if(checkListObjects.Count>10){
            showMessage();
        }*/
    }

    void CheckItem(ShoppingListObject item)
    {
        ShoppingListObjects.Remove(item);
        saveJsonData();
        Destroy(item.gameObject);
        upload();
    }

    void saveJsonData()
    {
        string contents = "";

        for (int i = 0; i < ShoppingListObjects.Count; i++)
        {
            ShoppinglistItem temp = new ShoppinglistItem(ShoppingListObjects[i].objName, ShoppingListObjects[i].index);
            contents += JsonUtility.ToJson(temp) + "\n";
        }
        System.IO.File.WriteAllText(filePath, contents);
    }

    //Upload to the server
    public void upload()
    {
        string contents = "";
        string url = "http://18.191.23.16/jsonServer/UnityUpload.php";
        for (int i = 0; i < ShoppingListObjects.Count; i++)
        {
            ShoppinglistItem temp = new ShoppinglistItem(ShoppingListObjects[i].objName, ShoppingListObjects[i].index);
            contents += JsonUtility.ToJson(temp) + "\n";
        }

        byte[] bytes = Encoding.ASCII.GetBytes(contents);
        Debug.Log(bytes.ToString());

        WWWForm form = new WWWForm();
        form.AddField("Name","ShoppingList");
        form.AddBinaryData("post", bytes);
        WWW www = new WWW(url, form);
    }

    void loadJsonData()
    {
        string dataAsJson = "";
        dataAsJson = System.IO.File.ReadAllText(filePath);
        string[] splitContents = dataAsJson.Split('\n');
        foreach (string content in splitContents)
        {
            if (content.Trim() != "")
            {
                ShoppinglistItem temp = JsonUtility.FromJson<ShoppinglistItem>(content.Trim());
                CreateCheckListItems(temp.objName, temp.index, true);
            }
        }

    }
}