// using Firebase;
// using System.Collections;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using Firebase.Extensions;
// using UnityEngine;
//
// public class FirebaseInitialize : UnityEngine.MonoBehaviour
// {
//     DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
//     public static FirebaseInitialize Instance;
//     public bool isReady = false;
//     public delegate void OnInit();
//     public static OnInit onInit;
//     private FirebaseApp app;
//     private void Start()
//     {
//         if (Instance == null)
//         {
//             Instance = this;
//             Init();
//         }
//     }
//    
//     private void Init()
//     {
//         if (FirebaseInitialize.Instance.isReady) return;
//         Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
//             var dependencyStatus = task.Result;
//             if (dependencyStatus == Firebase.DependencyStatus.Available) {
//                 // Create and hold a reference to your FirebaseApp,
//                 // where app is a Firebase.FirebaseApp property of your application class.
//                 app = Firebase.FirebaseApp.DefaultInstance;
//                 isReady = true;
//                 onInit.Invoke();
//                 Debug.Log("Firebase is ready to use");
//                 // Set a flag here to indicate whether Firebase is ready to use by your app.
//             } else {
//                 UnityEngine.Debug.LogError(System.String.Format(
//                     "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
//                 // Firebase Unity SDK is not safe to use here.
//             }
//         });
//     }
//
//    
// }