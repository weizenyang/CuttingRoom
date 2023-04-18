using CuttingRoom;
using CuttingRoom.VariableSystem.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;

public class WebIntermediateSetter : MonoBehaviour
{

    [SerializeField]
    private NarrativeObject narrativeObject = null;

    [SerializeField]
    private string title = "";

    [SerializeField]
    private string description = "";

    [SerializeField]
    private string className = "";

    private DatabaseReference dbReference;
    private DatabaseReference userReference;

    private string roomID = InstanceVariable.roomID;

    private bool currentBool = false;

    bool initUpdate = false;
    

    void Start()
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        roomID = InstanceVariable.roomID;
        dbReference.Child("Room").Child(roomID).Child("Properties").Child("waiting_screen").SetValueAsync("false");
        // hasPlayedVariable = narrativeObject.VariableStore.GetVariable("hasPlayed") as BoolVariable;
        // currentBool = hasPlayedVariable.Value;
    }

    void Update()
    {
        // if (narrativeObject != null && !initUpdate){
        //     hasPlayedVariable = narrativeObject.VariableStore.GetVariable("hasPlayed") as BoolVariable;
        //     currentBool = hasPlayedVariable.Value;
        //     initUpdate = true;
        // }

        if (narrativeObject != null && narrativeObject.VariableStore != null)
        {
            var hasPlayedVariable = narrativeObject.VariableStore.GetVariable("hasPlayed") as BoolVariable;
            // Debug.Log("hasPlayedVariable");
            // Debug.Log(hasPlayedVariable.Value);
            if (hasPlayedVariable.Value != currentBool)
            {
                if (hasPlayedVariable.Value)
                {
                    setFirebaseProperty(true, "intermediate");
                }
                else
                {
                    setFirebaseProperty(false, "intermediate");
                }
                currentBool = hasPlayedVariable.Value;
            }
        }
    }

    public void setFirebaseProperty(bool show, string type)
    {   
        // Debug.Log("setFirebaseProperty " + type);
            if(show){
                    dbReference.Child("Room").Child(roomID).Child("Properties").Child("screen").Child("title").SetValueAsync(title);
                    dbReference.Child("Room").Child(roomID).Child("Properties").Child("screen").Child("description").SetValueAsync(description);
                    dbReference.Child("Room").Child(roomID).Child("Properties").Child("screen").Child("type").SetValueAsync(type);
                    dbReference.Child("Room").Child(roomID).Child("Properties").Child("screen").Child("show").SetValueAsync(show);
                    dbReference.Child("Room").Child(roomID).Child("Properties").Child("screen").Child("className").SetValueAsync(className);
                    dbReference.Child("Room").Child(roomID).Child("Properties").Child("screen").Child("variable").SetValueAsync(null);
                    
            } else {
                // dbReference.Child("Room").Child(roomID).Child("Properties").Child(variables).SetValueAsync(null);
            }
        
    }

    // public void createRoom()
    // {

    //     roomID = roomCodeGenerator();
    //     Room newRoom = new Room(roomID);
    //     // Property baseProp = new Property("Property", null);


    //     string json = JsonUtility.ToJson(newRoom);
    //     // string propJson = JsonUtility.ToJson(baseProp);

    //     dbReference.Child("Room").Child(roomID).SetRawJsonValueAsync(null);
    //     dbReference.Child("Room").Child(roomID).Child("Properties").Child("default").SetValueAsync("none");
    //     dbReference.Child("Room").Child(roomID).ChildAdded += getPlayerNames;
    //     codeGen.text = roomID;
    // }
}
