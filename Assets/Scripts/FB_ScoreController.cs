using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.UI;

public class FB_ScoreController : MonoBehaviour
{

    DatabaseReference mDatabase;
    string UserId;

    // Start is called before the first frame update
    void Start()
    {
        mDatabase = FirebaseDatabase.DefaultInstance.RootReference;
        UserId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

    }

    public void WriteNewScore(int score)
    {
        UserData data = new UserData();

        data.score = int.Parse( GameObject.Find("InputScore").GetComponent<InputField>().text);
        string json = JsonUtility.ToJson(data);

        mDatabase.Child("users").Child(UserId).SetRawJsonValueAsync(json);

    }

    public void GetUserScore()
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("users/"+UserId)
            .GetValueAsync().ContinueWithOnMainThread(task => {
                 if (task.IsFaulted)
                 {
                    Debug.Log(task.Exception);
                  }
                 else if (task.IsCompleted)
                 {
                     DataSnapshot snapshot = task.Result;

                    Debug.Log(snapshot.Value);

                    var data = ((Dictionary<string, object>)snapshot.Value);
                    Debug.Log("Puntaje: " +data["score"]);

                    //foreach (var userDoc in (Dictionary<string,object>)snapshot.Value)
                    //{
                    //    Debug.Log(userDoc.Key);
                    //    Debug.Log(userDoc.Value);

                    //}
                      // Do something with snapshot...
                  }
     });
    }
    
    public void GetUsersHighestScores()
    {
        FirebaseDatabase.DefaultInstance.GetReference("users").OrderByChild("score").LimitToLast(5)
                .GetValueAsync().ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted)
                    {
                        //Handle the error...
                    }
                    else if (task.IsCompleted)
                    {
                        DataSnapshot snapshot = task.Result;
                        Debug.Log(snapshot);


                        foreach (var userDoc in (Dictionary<string, object>)snapshot.Value)
                        {

                            var userObject = (Dictionary<string, object>)userDoc.Value;

                            Debug.Log(userObject["username"]+":"+userObject["score"]);
                            

                        }
                    }                
                });
    }
}

public class UserData
{
   public int score;
    public string username;
}
