// Copyright (c) 2024 GplayJSC (gplayjsc.com)
// 
// Author: Axolotl (lamanh.w@gmail.com)
// 
// Created: 09/09/2024
// 
// File: GLog.cs
// 
// Note:

using System.Collections.Generic;
using System.Linq;
using AppsFlyerSDK;
using Firebase.Analytics;
using GplayAppFlyer.Script;
using UnityEngine;

namespace GplaySDK
{
    public static class Analytics
    {
        public static void LogStartLevel(
            string levelId,
            int showId,
            string mode,
            Dictionary<string, string> extraData = null)
        {
            var localDict = extraData == null
                ? new Dictionary<string, string>()
                : new Dictionary<string, string>(extraData);
            localDict.SetOrAdd(ParamsKeys.Level.Id, levelId);
            localDict.SetOrAdd(ParamsKeys.Level.ShowId, showId.ToString());
            localDict.SetOrAdd(ParamsKeys.Level.Mode, mode);
            localDict.SetOrAdd(ParamsKeys.Level.CompletedLevelCount, AnalyticData.CompletedLevelCount.ToString());
            SendEvent(LogKeys.LevelStart, localDict);
            //CostCenterLog();

            if (AnalyticData.IsFirstTimePlayLevelByShowId(showId, mode))
            {
                LogFirstTimeStartLevelByShowId(localDict);
                AnalyticData.SetFirstTimePlayLevelByShowId(showId, mode);
            }

            if (AnalyticData.IsFirstTimePlayLevelById(levelId))
            {
                LogFirstTimeStartLevelById(localDict);
                AnalyticData.SetFirstTimePlayLevelById(levelId);
            }

            return;

            void CostCenterLog()
            {
                var dict = new Parameter[]
                {
                    new Parameter("level", showId),
                    new Parameter("play_mode", mode),
                };
                SendEvent("level_start", dict);
            }
        }

        private static void LogFirstTimeStartLevelByShowId(Dictionary<string, string> dict)
        {
            var localDict = new Dictionary<string, string>(dict);
            SendEvent(LogKeys.LevelStartOnceByShowId, localDict);
        }


        private static void LogFirstTimeStartLevelById(Dictionary<string, string> dict)
        {
            var localDict = new Dictionary<string, string>(dict);
            SendEvent(LogKeys.LevelStartOnceById, localDict);
        }

        private static void LogCompletedLevelCount(int levelCount)
        {
            // TODO: Update later.
            var dict = new Dictionary<string, string>()
            {
            };
            var eventName = string.Format(LogKeys.CompletedLevelCount, levelCount);
            AppsFlyer.sendEvent(eventName, dict);
            SendEvent(eventName, dict);
        }

        public static void LogEndLevel(
            string levelId,
            int showId,
            string mode,
            bool isSuccess,
            string reason,
            double playtime,
            Dictionary<string, string> extraData = null)
        {
            // Handle when success complete level
            if (isSuccess) AnalyticData.SetCompletedLevelById(levelId);
            if (AnalyticData.HighestCompletedLevelCountWasLog < AnalyticData.CompletedLevelCount)
            {
                LogCompletedLevelCount(AnalyticData.CompletedLevelCount);
                AnalyticData.HighestCompletedLevelCountWasLog = AnalyticData.CompletedLevelCount;
            }

            // Handle default logging
            var localDict = extraData is null
                ? new Dictionary<string, string>()
                : new Dictionary<string, string>(extraData);
            localDict.SetOrAdd(ParamsKeys.Level.Id, levelId);
            localDict.SetOrAdd(ParamsKeys.Level.ShowId, showId.ToString());
            localDict.SetOrAdd(ParamsKeys.Level.Mode, mode);
            localDict.SetOrAdd(ParamsKeys.Level.IsSuccess, isSuccess ? "1" : "0");
            localDict.SetOrAdd(ParamsKeys.Level.LevelEndReason, reason);
            localDict.SetOrAdd(ParamsKeys.Level.LevelEndPlaytime, playtime.ToString("E"));
            localDict.SetOrAdd(ParamsKeys.Level.CompletedLevelCount, AnalyticData.CompletedLevelCount.ToString());
            SendEvent(LogKeys.LevelEnd, localDict);
            //CostCenterLog();

            return;

            void CostCenterLog()
            {
                var dict = new Parameter[]
                {
                    new Parameter("level", showId),
                    new Parameter("play_mode", mode),
                    new Parameter("success", isSuccess.ToString()),
                    new Parameter("reason", reason),
                };
                SendEvent("level_end", dict);
            }
        }

        public static void LogUseBooster(
            string levelId,
            int showId,
            string mode,
            int booster,
            Dictionary<string, string> extraData = null)
        {
            var localDict = extraData is null
                ? new Dictionary<string, string>()
                : new Dictionary<string, string>(extraData);
            localDict.SetOrAdd(ParamsKeys.Level.Id, levelId);
            localDict.SetOrAdd(ParamsKeys.Level.ShowId, showId.ToString());
            localDict.SetOrAdd(ParamsKeys.Level.Mode, mode);
            localDict.SetOrAdd(ParamsKeys.Level.IsSuccess, booster.ToString());
            SendEvent(LogKeys.UseBooster, localDict);
        }

        private static void SendEvent(string key, Dictionary<string, string> dict)
        {
            try
            {
                FirebaseAnalytics.LogEvent(key, dict.Select(x => new Parameter(x.Key, x.Value)).ToArray());
            }
            catch
            {
                Debug.LogError("Send GP Log error");
                // ignored
            }
        }

        private static void SendEvent(string key, IEnumerable<Parameter> dict)
        {
            try
            {
                FirebaseAnalytics.LogEvent(key, dict.ToArray());
            }
            catch
            {
                // ignored
            }
        }

        private static void SetOrAdd(this Dictionary<string, string> dict, string key, string value)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = value;
            }
            else
            {
                dict.Add(key, value);
            }
        }

        private static class LogKeys
        {
            private const string BasePrefix = "gp_";

            public const string LevelStart = BasePrefix + "levelStart";
            public const string LevelStartOnceByShowId = BasePrefix + "levelStartOnceByShowId";
            public const string LevelStartOnceById = BasePrefix + "levelStartOnceById";
            public const string LevelEnd = BasePrefix + "levelEnd";
            public const string CompletedLevelCount = BasePrefix + "completedLevelCount_{0}";
            public const string UseBooster = BasePrefix + "useBooster";
        }

        private static class ParamsKeys
        {
            public static class Level
            {
                public const string Id = "levelId";
                public const string ShowId = "levelShowId";
                public const string Mode = "levelMode";
                public const string IsSuccess = "isSuccess";
                public const string LevelEndReason = "reason";
                public const string LevelEndPlaytime = "playtime";
                public const string CompletedLevelCount = "completedLevelCount";
            }

            public static class Booster
            {
                public const string Id = "boosterId";
            }
        }
    }
}