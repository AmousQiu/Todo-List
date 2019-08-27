using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;
using System.Text;

public class AssignmentListManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform content;
    public Button createButton;
    public GameObject checkListItemPrefab;


    public InputField input;


    string filePath;
    //public Text ShowText;
    //string jsonName = "AssignmentList.json";
    string jsonName = "AssignmentList.txt";
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
        /*  #if UNITY_ANDROID
           filePath = "jar:file://" + Application.dataPath + "!/assets/" + "AssignmentList.json";
         #endif

         #if UNITY_EDITOR
            filePath = Application.streamingAssetsPath + "/AssignmentList.json";
         #endif
         */
        filePath = Path.Combine(Application.persistentDataPath, jsonName);

        loadJsonData();

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
        string url = "http://18.191.23.16/jsonServer/UnityUpload.php";
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

