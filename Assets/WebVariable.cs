using UnityEngine;
using static UnityEngine.Random;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using TMPro;

public class WebVariable : MonoBehaviour
{
    public string Name;
    public string ID;
    public TMP_Text codeGen;
    private string roomID;
    private string userID;
    private DatabaseReference dbReference;
    
    // Start is called before the first frame update
    public void Start()
    {
        Name = SystemInfo.deviceUniqueIdentifier;
        const string glyphs= "abcdefghijklmnopqrstuvwxyz0123456789";
        string myString = "";
        Debug.Log(FirebaseDatabase.DefaultInstance.RootReference);
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        

        int charAmount = 4;
        for(int i=0; i<charAmount; i++)
        {
            myString += glyphs[Random.Range(0, glyphs.Length)];
        }
        roomID = myString;
        Debug.Log(roomID);

        codeGen.GetComponent<TMP_Text>().text = roomID;

    }

    // Update is called once per frame
    public void CreateUser()
    {
        User newUser = new User(Name, ID);
        Debug.Log(newUser);
        string json = JsonUtility.ToJson(newUser);
        Debug.Log(dbReference.Child("users").Child(roomID).SetRawJsonValueAsync(json));
    }

    // public void ReadUser()
    // {
    //     dbReference.Child("users").Child(userID).GetValueAsync().ContinueWith(task => {
    //         if (task.IsFaulted)
    //         {
    //             // Handle the error...
    //         }
    //         else if (task.IsCompleted)
    //         {
    //             DataSnapshot snapshot = task.Result;
    //             // Do something with snapshot...
    //             User user = JsonUtility.FromJson<User>(snapshot.GetRawJsonValue());
    //             Name = user.name;
    //             ID = user.id;
    //         }
    //     });
    // }

//     public static string GenerateRandomString(int length)
// {
//     const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
//     var stringChars = new char[length];
//     var random = new Random();

//     for (int i = 0; i < stringChars.Length; i++)
//     {
//         stringChars[i] = chars[random.Next(chars.Length)];
//     }

//     return new String(stringChars);
// }

}


