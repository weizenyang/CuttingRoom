using CuttingRoom;
using CuttingRoom.VariableSystem.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;

public class WebVariableSetter : MonoBehaviour
{
    [SerializeField]
    private VariableSetter variableSetter = null;

    [SerializeField]
    private NarrativeObject narrativeObject = null;

    [SerializeField]
    private string variable;

    [SerializeField]
    private string title;

    [SerializeField]
    private string[] values;

    [SerializeField]
    private string className = "";

    [SerializeField]
    private string tempRoomID = "";

    private DatabaseReference dbReference;
    private DatabaseReference userReference;
    private string roomID = InstanceVariable.roomID;
    private bool currentBool = false;
    bool initUpdate = false;

    Dictionary<string, int> scores = new Dictionary<string, int>();

    string popularAnswer = "";


    void Start()
    {
        Debug.Log("Instance " + InstanceVariable.roomID);
        roomID = InstanceVariable.roomID;
        if(tempRoomID != ""){
            roomID = tempRoomID;
        }

        if(variable != null){
            foreach(string value in values){
                scores.Add(value, 0);
            }
        }

        Debug.Log(values);
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        roomID = InstanceVariable.roomID;
        dbReference.Child("Room").Child(roomID).Child("Properties").Child("waiting_screen").SetValueAsync("false");
        dbReference.Child("Room").Child(roomID).ChildChanged += updateScore;
        Debug.Log(dbReference.Child("Room").Child(roomID));
    }

    void OnApplicationQuit()
    {
        Debug.Log("Running Quit");
        if (roomID != null)
        {
            roomID = null;
            deleteRoom(roomID, "Room is closed");
        }
    }

    // void OnDestroy()
    // {
    //     Debug.Log("Running Quit");
    //     if (roomID != null)
    //     {
    //         roomID = null;
    //         deleteRoom(roomID, "Room is closed");
    //     }
    // }

    void Update()
    {

        // if (narrativeObject != null){
        //     hasPlayedVariable = narrativeObject.VariableStore.GetVariable("hasPlayed") as BoolVariable;
        //     currentBool = hasPlayedVariable.Value;
        // }
        // variableSetter.SetVariable(narrativeObject.VariableStore);
        // var hasPlayedVariable = narrativeObject.VariableStore.GetVariable("hasPlayed") as BoolVariable;

        if (narrativeObject != null && narrativeObject.VariableStore != null)
        {
            var hasPlayedVariable = narrativeObject.VariableStore.GetVariable("hasPlayed") as BoolVariable;
            if (hasPlayedVariable.Value != currentBool)
            {
                if (hasPlayedVariable.Value)
                {
                    setFirebaseProperty(true, "selection");
                }
                else
                {
                    setFirebaseProperty(false, "selection");
                }
                currentBool = hasPlayedVariable.Value;
            }
        }
    }

    
    public void setFirebaseProperty(bool show, string type) //Show Button, Show Text (in browser)
    {
        // Room
        //     -roomID
        //         -Properties
        //             -screen
        //                 -title
        //                 -description
        //                 -type
        //                 -show
        //                 -className
        //                 -variable
        Debug.Log("ROOMID" + roomID);
        Debug.Log("setFirebaseProperty " + type);
        if (show)
        {
            dbReference.Child("Room").Child(InstanceVariable.roomID).Child("Properties").Child("screen").Child("title").SetValueAsync(title);
            dbReference.Child("Room").Child(InstanceVariable.roomID).Child("Properties").Child("screen").Child("description").SetValueAsync(null);
            dbReference.Child("Room").Child(InstanceVariable.roomID).Child("Properties").Child("screen").Child("type").SetValueAsync(type);
            dbReference.Child("Room").Child(InstanceVariable.roomID).Child("Properties").Child("screen").Child("show").SetValueAsync(show);
            dbReference.Child("Room").Child(InstanceVariable.roomID).Child("Properties").Child("screen").Child("className").SetValueAsync(className);
            dbReference.Child("Room").Child(InstanceVariable.roomID).Child("Properties").Child("screen").Child("variable").SetValueAsync(variable);
            
            for (int i = 0; i < values.Length; i++)
            {
                dbReference.Child("Room").Child(InstanceVariable.roomID).Child("Properties").Child("screen").Child("buttons").Child("button" + i).SetValueAsync(values[i]);
            }
            

        }
        else
        {
            dbReference.Child("Room").Child(InstanceVariable.roomID).Child("Properties").Child(variable).SetValueAsync(null);
        }

    }

    public void updateScore(object sender, ChildChangedEventArgs args)
    {
        if(variable != null)
        {
            foreach(string value in values)
            {
                scores[value] = 0;
            }
        }

        DataSnapshot snapshot = args.Snapshot;

        foreach (DataSnapshot childSnapshot in snapshot.Children)
        {
            string childKey = childSnapshot.Key;

            foreach (DataSnapshot name in childSnapshot.Children){

                if (scores.ContainsKey(name.Value as string))
                {
                    scores[name.Value as string] += 1;
                    Debug.Log(name.Value.ToString() + " " + scores[name.Value as string]);
                    
                    int highestValue = 0;
                    foreach (KeyValuePair<string, int> i in scores)
                    {
                        if (i.Value > highestValue)
                        {
                            highestValue = i.Value;
                            popularAnswer = i.Key;
                        }
                    }
                    Debug.Log(popularAnswer + " " + highestValue.ToString() + " " + variable);
                    variableSetter.Set(popularAnswer);
                }
            } 
        }
    }
    
    public void deleteRoom(string roomID, string message) //Delete Room Name, Show Message (in browser)
    {
        // Room
        //     -roomID
        //         -null
        //     -Removed
        //         -roomID
        //             -message

        // setFirebaseProperty(true, "logIn");
        dbReference.Child("Room").Child(InstanceVariable.roomID).SetValueAsync(null);
        Debug.Log("Quit");
    }
}

