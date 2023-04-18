using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Random;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using TMPro;

public class RoomIntro : MonoBehaviour
{
    public TMP_Text codeGen;

    public GameObject nameCard;
    public string roomID;
    private DatabaseReference dbReference;
    private DatabaseReference userReference;

    [SerializeField]
    private RoomIntro roomIntro;

    [SerializeField]
    private string title;

    [SerializeField]
    private string description;

    [SerializeField]
    private GameObject startButton;

    // Start is called before the first frame update
    public void Start()
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        // userReference = FirebaseDatabase.DefaultInstance.GetReference("Users");
        codeGen.GetComponent<TMP_Text>().text = roomID;
        createRoom();

    }

    public string roomCodeGenerator()
    {

        const string glyphs = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string myString = "";

        int charAmount = 4;
        for (int i = 0; i < charAmount; i++)
        {
            myString += glyphs[Random.Range(0, glyphs.Length)];
        }
        return myString;
    }

    // Update is called once per frame
    public void createRoom()
    {

        roomID = roomCodeGenerator();
        Room newRoom = new Room(roomID);
        // Property baseProp = new Property("Property", null);
        InstanceVariable.roomID = roomID;


        string json = JsonUtility.ToJson(newRoom);
        // string propJson = JsonUtility.ToJson(baseProp);

        dbReference.Child("Room").Child(roomID).SetRawJsonValueAsync(null);
        dbReference.Child("Room").Child(roomID).Child("Properties").Child("waiting_screen").SetValueAsync("true");
        FirebaseDatabase.DefaultInstance.GetReference("Room/" + roomID).ValueChanged += getPlayerNames;
        Debug.Log(FirebaseDatabase.DefaultInstance.GetReference(roomID));
        setFirebaseProperty(true, "logIn");
        codeGen.text = roomID;
    }

    void OnApplicationQuit()
    {
        Debug.Log("Running Quit");
        if (roomID != null)
        {
            deleteRoom(roomID, "Room is closed");
            roomID = null;
        }
    }

    public void createProperty(string propname, string propval)
    {
        Property newProp = new Property(propname, propval);
        string json = JsonUtility.ToJson(newProp);
        Debug.Log(dbReference.Child("Room").Child(roomID).Child("Properties").SetRawJsonValueAsync(json));
    }

    public void generateRandomProperty()
    {
        string propname = roomCodeGenerator();
        string propval = roomCodeGenerator();
        createProperty(propname, propval);
    }

    // public void getPlayerNames(object sender, ChildChangedEventArgs args){
    public void getPlayerNames(object sender, ValueChangedEventArgs args)
    {
        Transform parent = null;
        if(GameObject.Find("NameList").transform != null){
            parent = GameObject.Find("NameList").transform;
        }
        
        if(transform != null){
            DataSnapshot snapshot = args.Snapshot;
            Debug.Log(snapshot.Child("Users").Children);
            foreach (Transform childTemp in parent)
            {
                Destroy(childTemp.gameObject);
            }

            foreach (DataSnapshot childSnapshot in snapshot.Child("Users").Children)
            {
                string childKey = childSnapshot.Key;
                GameObject nameCardInstance = Instantiate(nameCard);
                GameObject child = nameCardInstance.transform.GetChild(0).gameObject;
                child.GetComponent<TMP_Text>().text = childKey;
                nameCardInstance.transform.SetSiblingIndex(parent.childCount);
                nameCardInstance.transform.SetParent(parent);
                enableStartButton();
            }
        }
    }

    public void setFirebaseProperty(bool show, string type)
    {   
            if(show){
                    dbReference.Child("Room").Child(roomID).Child("Properties").Child("screen").Child("title").SetValueAsync(title);
                    dbReference.Child("Room").Child(roomID).Child("Properties").Child("screen").Child("description").SetValueAsync(description);
                    dbReference.Child("Room").Child(roomID).Child("Properties").Child("screen").Child("type").SetValueAsync(type);
                    dbReference.Child("Room").Child(roomID).Child("Properties").Child("screen").Child("show").SetValueAsync(show);
            } else {
                // dbReference.Child("Room").Child(roomID).Child("Properties").Child(variables).SetValueAsync(null);
            }
        
    }

    public void switchScene()
    {
        Debug.Log(roomID);
        Debug.Log(dbReference.Child("Room").Child(roomID));
        FirebaseDatabase.DefaultInstance.GetReference("Room/" + roomID).ValueChanged -= getPlayerNames;
        UnityEngine.SceneManagement.SceneManager.LoadScene("CuttingRoom");
        roomIntro.enabled = false;
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
        dbReference.Child("Room").Child(roomID).SetValueAsync(null);
    }

    public void enableStartButton()
    {
        startButton.GetComponent<Button>().interactable = true;
    }
}


