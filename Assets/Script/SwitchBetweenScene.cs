using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchBetweenScene : MonoBehaviour
{
   public void loadShoppingScene(){
       SceneManager.LoadScene("ShoppingList");
   }

   public void loadSocialScene(){
       SceneManager.LoadScene("SocialEventList");
   }

   public void loadAssignmentScene(){
       SceneManager.LoadScene("AssignmentList");
   }
}
