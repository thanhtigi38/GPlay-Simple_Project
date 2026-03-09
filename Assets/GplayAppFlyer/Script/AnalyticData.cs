// Copyright (c) 2024 GplayJSC (gplayjsc.com)
// 
// Author: Axolotl (lamanh.w@gmail.com)
// 
// Created: 19/08/2024
// 
// File: AFGData.cs
// 
// Note:

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace GplayAppFlyer.Script
{
    public static class AnalyticData
    {
        private static HashSet<LevelWithMode> _firstPlayedLevelByShowIds;
        private static HashSet<string> _firstPlayedLevelByIds;

        private static HashSet<string> _completedLevelByIds;


        public static int HighestCompletedLevelCountWasLog
        {
            get => PlayerPrefs.GetInt(Keys.HighestCompletedLevelCountWasLog, 0);
            set
            {
                PlayerPrefs.SetInt(Keys.HighestCompletedLevelCountWasLog, value);
                PlayerPrefs.Save();
            }
        }

        public static int CompletedLevelCount => _completedLevelByIds.Count;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            _firstPlayedLevelByShowIds =
                JsonConvert.DeserializeObject<LevelWithMode[]>(
                    PlayerPrefs.GetString(Keys.FirstTimePlayLevelByShowIds, "[]")).ToHashSet();
            _firstPlayedLevelByIds =
                JsonConvert.DeserializeObject<string[]>(
                    PlayerPrefs.GetString(Keys.FirstTimePlayLevelByIds, "[]")).ToHashSet();

            _completedLevelByIds =
                JsonConvert.DeserializeObject<string[]>(
                    PlayerPrefs.GetString(Keys.CompletedLevelByIds, "[]")).ToHashSet();
        }


        public static bool IsFirstTimePlayLevelByShowId(int levelId, string mode)
        {
            return _firstPlayedLevelByShowIds.Contains(new LevelWithMode(levelId, mode));
        }

        public static bool IsFirstTimePlayLevelById(string levelId)
        {
            return _firstPlayedLevelByIds.Contains(levelId);
        }

        public static void SetFirstTimePlayLevelByShowId(int levelId, string mode)
        {
            _firstPlayedLevelByShowIds.Add(new LevelWithMode(levelId, mode));
            PlayerPrefs.SetString(Keys.FirstTimePlayLevelByShowIds, _firstPlayedLevelByShowIds.ToJson());
            PlayerPrefs.Save();
        }

        public static void SetFirstTimePlayLevelById(string levelId)
        {
            _firstPlayedLevelByIds.Add(levelId);
            PlayerPrefs.SetString(Keys.FirstTimePlayLevelByIds, _firstPlayedLevelByIds.ToJson());
            PlayerPrefs.Save();
        }

        public static void SetCompletedLevelById(string levelId)
        {
            _completedLevelByIds.Add(levelId);
            PlayerPrefs.SetString(Keys.CompletedLevelByIds, _completedLevelByIds.ToJson());
            PlayerPrefs.Save();
        }

        
        public static string ToJson<T>(this T data, bool isPretty = false)
        {
            return JsonConvert.SerializeObject(data, isPretty ? Formatting.Indented : Formatting.None);
        }
        public static T FromJson<T>(this T body, string jsonData)
        {
            return JsonUtility.FromJson<T>(jsonData);
        }

        [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
        private static class Keys
        {
            public const string FirstTimePlayLevelByShowIds = "aly_firstTimePlayLevelByShowIds";
            public const string FirstTimePlayLevelByIds = "aly_firstTimePlayLevelByIds";
            public const string CompletedLevelByIds = "aly_completedLevelByIds";
            public const string HighestCompletedLevelCountWasLog = "aly_highestCompletedLevelWasLog";
        }

        [Serializable]
        private class LevelWithMode
        {
            public int id;
            public string mode;

            public LevelWithMode(int id, string mode)
            {
                this.id = id;
                this.mode = mode;
            }

            /// <summary>
            /// Default constructor needed for serialization
            /// </summary>
            public LevelWithMode()
            {
            }
        }
    }
}