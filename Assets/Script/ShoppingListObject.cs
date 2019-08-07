using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShoppingListObject : MonoBehaviour
{
   public string objName;
   public int index;

   private Text itemText;

   private void Start(){
       itemText=GetComponentInChildren<Text>();
       itemText.text=objName;
   }


   public void setObjectInfo(string name,int index){
       this.objName=name;
       this.index=index;
   }
}
