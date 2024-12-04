using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using LilLycanLord_Official;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

namespace LilLycanLord_Official
{
    interface HasProgession
    {
        public void ProgressionUpdate();
    }

    [Serializable]
    public class ProgressionCheck
    {
        //* ╔════════════╗
        //* ║ Components ║
        //* ╚════════════╝
        //* ╔══════════╗
        //* ║ Displays ║
        //* ╚══════════╝
        //* ╔════════╗
        //* ║ Fields ║
        //* ╚════════╝
        public string flag;
        public string condition;
        public int value = 0;

        //* ╔═══════════════╗
        //* ║ Monobehaviour ║
        //* ╚═══════════════╝
        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
        public ProgressionCheck(string newFlag, string newCondition, int newValue = 0)
        {
            this.flag = newFlag;
            this.condition = newCondition;
            this.value = newValue;
        }
    }

    public class SimpleProgressionManager : MonoBehaviour
    {
        //! ╔═══════════════════╗
        //! ║ SINGLETON CONTENT ║
        //! ╚═══════════════════╝
        //* SUMMARY
        //* This singleton pattern is used to make singletons which recognize preset singletons
        //* within the Scene, and if there are none,
        //* automatically create instances of themselves when needed.
        static bool logSingleton = false;
        static SimpleProgressionManager instance;
        public static SimpleProgressionManager Instance
        {
            get
            {
                if (instance == null)
                    CheckForExisitingSingleton(true);
                return instance;
            }
            private set { }
        }

        static void CheckForExisitingSingleton(bool fromGet)
        {
            if (instance != null)
                return;
            UnityEngine.Object[] existingSingleton = FindObjectsByType(
                typeof(SimpleProgressionManager),
                FindObjectsSortMode.None
            );
            if (existingSingleton.Length > 0)
            {
                instance = existingSingleton[0].GetComponent<SimpleProgressionManager>();
                if (!logSingleton)
                    return;
                string logMessage;
                if (fromGet)
                    logMessage = "(Get)";
                else
                    logMessage = "(Awake)";
                Debug.Log(
                    logMessage
                        + " Preset singleton found; "
                        + instance.name
                        + " set as singleton instance."
                );
            }
            else
            {
                instance = new GameObject(
                    "Simple Progression Manager"
                ).AddComponent<SimpleProgressionManager>();
                Debug.LogWarning("New " + instance.gameObject.name + " created.");
            }
        }

        void Awake()
        {
            CheckForExisitingSingleton(false);
            transform.parent = null;
            if (transform.parent == null)
                DontDestroyOnLoad(gameObject);
            if (instance == this)
            {
                NonSingletonAwake();
                return;
            }
            if (instance != null)
            {
                if (logSingleton)
                    Debug.Log(
                        name + " was overwritten by existing singleton: " + instance.gameObject.name
                    );
                Destroy(gameObject);
                return;
            }
            if (logSingleton)
                Debug.Log(name + " set as singleton instance.");
            instance = this;
            NonSingletonAwake();
        }

        void NonSingletonAwake() { }

        //! - - - - - - - - - - -

        //* ╔════════════╗
        //* ║ Components ║
        //* ╚════════════╝

        //* ╔══════════╗
        //* ║ Displays ║
        //* ╚══════════╝
        [Header("Displays")]
        [SerializedDictionary("Flag", "Default Value")]
        public SerializedDictionary<string, int> defaultValues =
            new SerializedDictionary<string, int>();

        //* ╔════════╗
        //* ║ Fields ║
        //* ╚════════╝
        [Space(10)]
        [Header("Fields")]
        [SerializedDictionary("Flag", "Value")]
        public SerializedDictionary<string, int> progressionFlags =
            new SerializedDictionary<string, int>();

        [SerializeField]
        List<string> validConditions = new List<string> { ">", ">=", "==", "!=", "<", "<=" };

        //* ╔════════════╗
        //* ║ Attributes ║
        //* ╚════════════╝

        //* ╔═══════════════╗
        //* ║ Monobehaviour ║
        //* ╚═══════════════╝
        void Start()
        {
            if (progressionFlags.Count > 0)
            {
                defaultValues.Clear();
                foreach (KeyValuePair<string, int> progressionFlag in progressionFlags)
                    defaultValues.Add(progressionFlag.Key, progressionFlag.Value);
            }
        }

        void Update() { }

        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
        public int GetFlagValue(string flag)
        {
            if (progressionFlags.Keys.Contains(flag))
                return progressionFlags[flag];
            Debug.LogWarning(name + " has no flag called \"" + flag + "\"");
            return -1;
        }

        public bool CheckForFlag(ProgressionCheck progressionCheck)
        {
            return CheckForFlagsAND(new List<ProgressionCheck> { progressionCheck });
        }

        public bool CheckForFlagsAND(List<ProgressionCheck> progressionChecks)
        {
            List<string> flags = new List<string>();
            foreach (ProgressionCheck progressionCheck in progressionChecks)
                flags.Add(progressionCheck.flag);
            List<string> conditions = new List<string>();
            foreach (ProgressionCheck progressionCheck in progressionChecks)
                conditions.Add(progressionCheck.condition);
            List<int> values = new List<int>();
            foreach (ProgressionCheck progressionCheck in progressionChecks)
                values.Add(progressionCheck.value);
            return CheckForFlagsAND(flags, conditions, values);
        }

        public bool CheckForFlagsOR(List<ProgressionCheck> progressionChecks, int atLeast = 1)
        {
            List<string> flags = new List<string>();
            foreach (ProgressionCheck progressionCheck in progressionChecks)
                flags.Add(progressionCheck.flag);
            List<string> conditions = new List<string>();
            foreach (ProgressionCheck progressionCheck in progressionChecks)
                conditions.Add(progressionCheck.condition);
            List<int> values = new List<int>();
            foreach (ProgressionCheck progressionCheck in progressionChecks)
                values.Add(progressionCheck.value);
            return CheckForFlagsOR(flags, conditions, values, atLeast);
        }

        bool CheckForFlagsAND(List<string> flags, List<string> conditions, List<int> values)
        {
            if (!VerifyProgressionCheck(flags, conditions, values))
                return false;

            int index = 0;
            foreach (string flag in flags)
            {
                switch (conditions[index])
                {
                    case ">":
                        if (progressionFlags[flag] <= values[index])
                            return false;
                        break;
                    case ">=":
                        if (progressionFlags[flag] < values[index])
                            return false;
                        break;
                    case "==":
                        if (progressionFlags[flag] != values[index])
                            return false;
                        break;
                    case "!=":
                        if (progressionFlags[flag] == values[index])
                            return false;
                        break;
                    case "<":
                        if (progressionFlags[flag] >= values[index])
                            return false;
                        break;
                    case "<=":
                        if (progressionFlags[flag] > values[index])
                            return false;
                        break;
                }
                index++;
            }
            return true;
        }

        bool CheckForFlagsOR(
            List<string> flags,
            List<string> conditions,
            List<int> values,
            int atLeast = 1
        )
        {
            if (!VerifyProgressionCheck(flags, conditions, values))
                return false;

            int index = 0;
            int satisfiedConditions = 0;
            foreach (string flag in flags)
            {
                if (satisfiedConditions >= atLeast)
                    return true;
                switch (conditions[index])
                {
                    case ">":
                        if (progressionFlags[flag] > values[index])
                            satisfiedConditions++;
                        break;
                    case ">=":
                        if (progressionFlags[flag] >= values[index])
                            satisfiedConditions++;
                        break;
                    case "==":
                        if (progressionFlags[flag] == values[index])
                            satisfiedConditions++;
                        break;
                    case "!=":
                        if (progressionFlags[flag] != values[index])
                            satisfiedConditions++;
                        break;
                    case "<":
                        if (progressionFlags[flag] < values[index])
                            satisfiedConditions++;
                        break;
                    case "<=":
                        if (progressionFlags[flag] <= values[index])
                            satisfiedConditions++;
                        break;
                }
                index++;
            }
            return satisfiedConditions >= atLeast;
        }

        public int SetFlag(string flag, int value = 0)
        {
            Debug.Log("Set flag called! value: " + value);
            if (!progressionFlags.ContainsKey(flag))
            {
                Debug.LogWarning(
                    name + " has no Progression Flag called \"" + flag + "\"; Creating..."
                );
                AddProgressionFlag(flag, value);
            }
            progressionFlags[flag] = value;
            return progressionFlags[flag];
        }

        public void AddProgressionFlag(string flag, int defaultValue = 0)
        {
            if (progressionFlags.ContainsKey(flag))
            {
                Debug.LogWarning(
                    name
                        + " already has a Progression Flag called \""
                        + flag
                        + "\", with a default value of "
                        + defaultValues[flag]
                        + ", and a current value of "
                        + progressionFlags[flag]
                );
                return;
            }
            progressionFlags.Add(flag, defaultValue);
            defaultValues.Add(flag, defaultValue);
        }

        public void RemoveFlag(string flag)
        {
            if (!progressionFlags.ContainsKey(flag))
            {
                Debug.LogWarning(name + " has no Progression Flag called \"" + flag + "\"");
                return;
            }
            progressionFlags.Remove(flag);
            defaultValues.Remove(flag);
        }

        public void ResetFlag(string flag)
        {
            if (!progressionFlags.ContainsKey(flag))
            {
                Debug.LogWarning(name + " has no Progression Flag called \"" + flag + "\"");
                return;
            }
            Debug.Log("\"" + flag + "\" has been reset to " + defaultValues[flag]);
            progressionFlags[flag] = defaultValues[flag];
        }

        bool VerifyProgressionCheck(List<string> flags, List<string> conditions, List<int> values)
        {
            //? These should be proper triplets of data
            if (
                flags.Count != conditions.Count
                || conditions.Count != values.Count
                || values.Count != flags.Count
            )
            {
                Debug.LogWarning(
                    name
                        + " had an uneven count of flags, conditions, and/or values; returning false..."
                );
                return false;
            }

            //? Verify each flag exists
            foreach (string flag in flags)
            {
                if (!progressionFlags.ContainsKey(flag))
                {
                    Debug.Log(
                        name
                            + " has no Progression Flag called \""
                            + flag
                            + "\"; flag added but returning false..."
                    );
                    AddProgressionFlag(flag);
                    return false;
                }
            }

            //? Verify each condition exists
            foreach (string condition in conditions)
            {
                bool valid = false;
                foreach (string validCondition in validConditions)
                {
                    if (condition == validCondition)
                    {
                        valid = true;
                        continue;
                    }
                }
                if (valid)
                    continue;
                Debug.LogWarning(
                    name + " had an invalid condition (" + condition + "); returning false..."
                );
                return false;
            }

            return true;
        }
    }
}
