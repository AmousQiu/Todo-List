using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;
using System.Text;

public class SocialListManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform content;
    public Button createButton;
    public GameObject checkListItemPrefab;

    //string filePath=Application.persistentDataPath+"/checkList.txt";
    string filePath;

    public InputField input;

    //public Text ShowText;

    private List<SocialListObject> SocialListObjects = new List<SocialListObject>();

    public class SociallistItem
    {
        public string objName;

        public int index;
        public SociallistItem(string name, int index)
        {
            this.objName = name;
            this.index = index;
        }
    }
    private void Start()
    {
        filePath = Application.persistentDataPath + "/SocialList.json";
        //if(!File.Exists(filePath)){
        //    File.Create(filePath);
        //}else{}
        //filePath = Application.streamingAssetsPath + "/SocialList.json";
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

        SocialListObject itemObject = item.GetComponent<SocialListObject>();

        int index = 0;
        if (!loading)
        {
            index = SocialListObjects.Count;
        }
        itemObject.setObjectInfo(name, index);
        Debug.Log("itemObject name" + itemObject.objName);
        itemObject.GetComponentInChildren<Text>().text = name;
        SocialListObjects.Add(itemObject);
        SocialListObject temp = itemObject;
        itemObject.GetComponent<Toggle>().onValueChanged.AddListener((delegate { CheckItem(temp); }));
        if (!loading)
        {
            saveJsonData();
        }
        /* if(checkListObjects.Count>10){
            showMessage();
        }*/
        upload();
    }

    void CheckItem(SocialListObject item)
    {
        SocialListObjects.Remove(item);
        saveJsonData();
        Destroy(item.gameObject);
        upload();
    }

    void saveJsonData()
    {
        string contents = "";

        for (int i = 0; i < SocialListObjects.Count; i++)
        {
            SociallistItem temp = new SociallistItem(SocialListObjects[i].objName, SocialListObjects[i].index);
            contents += JsonUtility.ToJson(temp) + "\n";
        }
        Debug.Log(filePath);

        StreamWriter sw = new StreamWriter(filePath);
        sw.Write("");
        sw.WriteLine(contents);
        sw.Close();
    }

    public void upload()
    {
        string contents = "";
        string url = "http://18.191.23.16/jsonServer/UnityUpload.php";
        for (int i = 0; i < SocialListObjects.Count; i++)
        {
            SociallistItem temp = new SociallistItem(SocialListObjects[i].objName, SocialListObjects[i].index);
            contents += JsonUtility.ToJson(temp) + "\n";
        }

        byte[] bytes = Encoding.ASCII.GetBytes(contents);
        Debug.Log(bytes.ToString());

        WWWForm form = new WWWForm();
        form.AddField("Name","SocialList");
        form.AddBinaryData("post", bytes);
        WWW www = new WWW(url, form);
    }


    void loadJsonData()
    {
        string dataAsJson = "";

        dataAsJson = File.ReadAllText(filePath);
        string[] splitContents = dataAsJson.Split('\n');

        foreach (string content in splitContents)
        {
            if (content.Trim() != "")
            {
                SociallistItem temp = JsonUtility.FromJson<SociallistItem>(content.Trim());
                CreateCheckListItems(temp.objName, temp.index, true);
            }
        }
    }
}

