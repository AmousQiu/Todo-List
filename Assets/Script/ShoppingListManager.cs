using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;

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
        //if(!File.Exists(filePath)){
        //    File.Create(filePath);
        // }else{}
        loadJsonData();

        //input = this.GetComponent<InputField>();
        //createButton.onClick.AddListener(delegate { CreateCheckListItems(input.text); });
    }


    /* IEnumerator showMessage(){

        ShowText.enabled=true;
        ShowText.text="This is tooooo much";
        yield return new WaitForSecondsRealtime(1f);
        ShowText.enabled=false;
        ShowText.text="";
    }*/
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
        /* if(checkListObjects.Count>10){
            showMessage();
        }*/
    }

    void CheckItem(ShoppingListObject item)
    {
        ShoppingListObjects.Remove(item);
        saveJsonData();
        Destroy(item.gameObject);

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
