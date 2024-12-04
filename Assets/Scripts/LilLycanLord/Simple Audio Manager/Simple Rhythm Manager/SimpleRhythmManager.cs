using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using LilLycanLord_Official;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace LilLycanLord_Official
{
    //* SUMMARY
    //* Rhythms execute a UnityEvent at a regular interval, set in BPMs, using the Sound class from
    //* the Simple Audio Manager.
    [Serializable]
    public class Rhythm : HasSignals
    {
        //* ╔════════════╗
        //* ║ Components ║
        //* ╚════════════╝

        //* ╔══════════╗
        //* ║ Displays ║
        //* ╚══════════╝
        [Header("Displays")]
        public string name = "";

        [Tooltip(
            "Signal to be sent to the Simple Game Event Manager. Note that this will send it as follows: \"[signalOnBeat][name]\""
        )]
        public string signalOnBeat = "";

        [Tooltip(
            "To what song will this Rhythm match up to. The rhythm autiomatically gets disabled when this Sound stops playing."
        )]
        public CurrentlyPlayingSound trackedSound;
        public float BPM = -1;

        [Min(0)]
        [Tooltip("The number of intervals per beat this Rhythm is triggered.")]
        public float steps = 1;
        public UnityEvent actions = new UnityEvent();

        //* ╔════════╗
        //* ║ Fields ║
        //* ╚════════╝
        // [Space(10)]
        // [Header("Fields")]
        [Space(10)]
        [Header("Simple Game Event Manager")]
        [SerializedDictionary("Description", "Signal")]
        public SerializedDictionary<string, string> signals = new SerializedDictionary<
            string,
            string
        >
        {
            //? In-going
            //? Out-going
            { "At every beat.", "" }
        };
        public SerializedDictionary<string, string> ownedSignals
        {
            get => signals;
            set => signals = value;
        }

        //* ╔════════════╗
        //* ║ Attributes ║
        //* ╚════════════╝
        private int lastInterval;

        [Tooltip(
            "Setting this to true will remove this Rhythm from the list. This should be done before LateUpdate, via the SimpleBeatManager's StopBeatSync function."
        )]
        public bool removeNextInterval = false;

        //* ╔═══════════════╗
        //* ║ Monobehaviour ║
        //* ╚═══════════════╝
        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
        public float GetBeatMeasure()
        {
            return 60.0f /*seconds in a minute*/
                / (BPM * steps);
        }

        //* SUMMARY
        //* This is the main function which handles the timing to match the AudioClip.
        //: NOTE
        //: This is entirely done mathematically, not by analyzing the specified AudioClip for certain frequencies or such.
        public void CheckForNewBeat()
        {
            VerifyIntegrity();
            float sampledTime = -1.0f;
            if (trackedSound.audioSource.clip != null)
                sampledTime = (
                    trackedSound.audioSource.timeSamples
                    / (trackedSound.audioSource.clip.frequency * GetBeatMeasure())
                );
            else
                Debug.LogWarning("No AudioClip loaded in " + name + "'s tracked AudioSource");

            if (Mathf.FloorToInt(sampledTime) != lastInterval)
            {
                lastInterval = Mathf.FloorToInt(sampledTime);
                if (signalOnBeat != "")
                {
                    signals["At every beat."] = signalOnBeat;
                    SimpleGameEventManager.Instance.SendSignal(signals["At every beat."]);
                }
                actions?.Invoke();
            }
        }

        void VerifyIntegrity()
        {
            if (trackedSound.audioSource == null)
            {
                if (SimpleRhythmManager.Instance.gameObject.GetComponent<AudioSource>() == null)
                    SimpleRhythmManager.Instance.gameObject.AddComponent<AudioSource>();
                Debug.LogWarning(
                    name + " has no set AudioSource; using Simple Beat Manager AudioSource"
                );
                trackedSound.audioSource =
                    SimpleRhythmManager.Instance.gameObject.GetComponent<AudioSource>();
            }
            if (BPM < 0)
            {
                BPM = SimpleRhythmManager.Instance.defaultBPM;
                Debug.LogWarning(name + "'s BPM is invalid; using Default BPM of " + BPM + " BPM");
            }
        }
    }

    [Serializable]
    //* SUMMARY
    //* AutoRhythms are Rhythms which are set to start whenever a certain Sound plays.
    //? Think of it that Rhythms are hard set, which when removed, will have to be manually set again, while
    //? AutoRhythms are automatically set and removed when their soundToWaitFor is played by the Simple Audio Manager.
    public class AutoRhythm : Rhythm
    {
        //* ╔════════════╗
        //* ║ Components ║
        //* ╚════════════╝

        //* ╔══════════╗
        //* ║ Displays ║
        //* ╚══════════╝
        // [Header("Displays")]

        //* ╔════════╗
        //* ║ Fields ║
        //* ╚════════╝
        [Space(10)]
        [Header("Fields")]
        public Sound soundToWaitFor;

        [Tooltip(
            "When this soundToWaitFor is played another time, this replaces already running Rhythms with the same name. Note: if false, Sounds may overlap from multiple AudioSources."
        )]
        public bool singletonRhythm = true;

        //* ╔════════════╗
        //* ║ Attributes ║
        //* ╚════════════╝

        //* ╔═══════════════╗
        //* ║ Monobehaviour ║
        //* ╚═══════════════╝
        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
        public AutoRhythm(Sound syncSound)
        {
            soundToWaitFor = syncSound;
            BPM = syncSound.BPM;
        }
    }

    //* SUMMARY
    //* The Simple Beat Manager handles all Rhythms set in the game. This does so by
    //* having Rhythms added and removed during runtime which do their own actions at a regular interval.
    //* AutoRhythms may be set to have Rhythms anticipate a certain Sound and stop automatically with it,
    //* which is communicated to this class by the SimpleAudioManager.
    public class SimpleRhythmManager : MonoBehaviour
    {
        //! ╔═══════════════════╗
        //! ║ SINGLETON CONTENT ║
        //! ╚═══════════════════╝
        //* SUMMARY
        //* This singleton pattern is used to make singletons which recognize preset singletons
        //* within the Scene, and if there are none,
        //* automatically create instances of themselves when needed.
        static bool logSingleton = false;
        static SimpleRhythmManager instance;
        public static SimpleRhythmManager Instance
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
                typeof(SimpleRhythmManager),
                FindObjectsSortMode.None
            );
            if (existingSingleton.Length > 0)
            {
                instance = existingSingleton[0].GetComponent<SimpleRhythmManager>();
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
                instance = new GameObject("Simple Rhythm Manager").AddComponent<SimpleRhythmManager>();
                Debug.LogWarning("New " + instance.gameObject.name + " created.");
            }
        }

        void Awake()
        {
            CheckForExisitingSingleton(false);
            // transform.parent = null;
            // if (transform.parent == null)
            //     DontDestroyOnLoad(gameObject);
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
        public List<Rhythm> rhythms = new List<Rhythm>();

        //* ╔════════╗
        //* ║ Fields ║
        //* ╚════════╝
        [Space(10)]
        [Header("Fields")]
        public List<AutoRhythm> autoRhythms = new List<AutoRhythm>();
        public float defaultBPM;

        //* ╔════════════╗
        //* ║ Attributes ║
        //* ╚════════════╝

        //* ╔═══════════════╗
        //* ║ Monobehaviour ║
        //* ╚═══════════════╝
        void Start()
        {
            //? Ensures that created Rhythms will follow the Sound they'll follow to.
            if (autoRhythms.Count > 0)
                foreach (AutoRhythm autoRhythm in autoRhythms)
                    autoRhythm.BPM = autoRhythm.soundToWaitFor.BPM;
        }

        //? Done in LateUpdate so scripts with logic that removes certain Rhythms execute first.
        void LateUpdate()
        {
            if (rhythms.Count > 0)
                foreach (Rhythm rhythm in rhythms)
                {
                    rhythm.removeNextInterval =
                        rhythm.trackedSound.audioSource == null
                        || !rhythm.trackedSound.audioSource.isPlaying;

                    if (!rhythm.removeNextInterval)
                        rhythm.CheckForNewBeat();
                }
            rhythms.RemoveAll(rhythm => rhythm.removeNextInterval);
            autoRhythms.RemoveAll(autoRhythm => autoRhythm.removeNextInterval);
        }

        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
        //* SUMMARY
        //* This function starts all AutoRhythms waiting for a certain sound.
        public void StartBeatSync(CurrentlyPlayingSound newlyPlayedSound)
        {
            //? "For each AutoRhythm tracking this newlyPlayedSound's Sound..."
            foreach (
                AutoRhythm autoRhythm in autoRhythms.FindAll(
                    autoRhythm => autoRhythm.soundToWaitFor == newlyPlayedSound.sound
                )
            )
            {
                if (autoRhythm.singletonRhythm && rhythms.Contains(autoRhythm))
                    continue;
                Rhythm newRhythm = new Rhythm();
                newRhythm.name = autoRhythm.name;
                newRhythm.signalOnBeat = autoRhythm.signalOnBeat;
                newRhythm.BPM = autoRhythm.BPM;
                newRhythm.steps = autoRhythm.steps;
                newRhythm.actions = autoRhythm.actions;
                newRhythm.trackedSound = newlyPlayedSound;
                rhythms.Add(newRhythm);
                Debug.Log(
                    autoRhythm.name
                        + " synced to: "
                        + newlyPlayedSound.sound.name
                        + " at "
                        + autoRhythm.BPM
                        + " BPM"
                );
            }
        }

        //* SUMMARY
        //* This function stop all AutoRhythms tracking a CurrentlyPlayingSound.
        //? "All Rhythms relying on this CurrentlyPlayingSound, will be stopped,
        //? but not other CurrentlyPlayingSounds playing the same AudioClip."
        public void StopBeatSync(CurrentlyPlayingSound playedSound)
        {
            foreach (Rhythm rhythm in rhythms.FindAll(rhythm => rhythm.trackedSound == playedSound))
                rhythm.removeNextInterval = true;
        }

        //* SUMMARY
        //* This function stop all AutoRhythms tracking a Sound.
        //? "All Rhythms relying on a Rhythm using with this AudioClip, will be stopped,
        //? whether or not those Rhythms are in sync."
        public void StopBeatSync(Sound sound)
        {
            foreach (Rhythm rhythm in rhythms.FindAll(rhythm => rhythm.trackedSound.sound == sound))
                rhythm.removeNextInterval = true;
        }

        //: NOTE
        //: The following two functions are the same, one simply accepts multiple UnityActions for the UnityEvent,
        //: of the AutoRhythm.
        public Rhythm AddAutoRhythm(
            string rhythmName,
            Sound syncedSound,
            int steps,
            UnityAction action
        )
        {
            AutoRhythm newAutoRhythm = autoRhythms.Find(
                autoRhythm => autoRhythm.name == rhythmName
            );
            if (autoRhythms.Find(autoRhythm => autoRhythm.name == rhythmName) != null)
            {
                Debug.LogWarning(
                    this.name
                        + "has overwritten an AutoRhythm called \""
                        + rhythmName
                        + "\" synced to "
                        + newAutoRhythm.name
                        + " at "
                        + newAutoRhythm.BPM
                        + " BPM"
                );
                autoRhythms.Remove(newAutoRhythm);
            }
            newAutoRhythm = new AutoRhythm(syncedSound);
            newAutoRhythm.name = rhythmName;
            newAutoRhythm.soundToWaitFor = syncedSound;
            newAutoRhythm.BPM = syncedSound.BPM;
            newAutoRhythm.steps = steps;
            newAutoRhythm.actions.AddListener(action);
            autoRhythms.Add(newAutoRhythm);
            return newAutoRhythm;
        }

        public void AddAutoRhythm(
            string rhythmName,
            Sound syncedSound,
            int steps,
            List<UnityAction> actions
        )
        {
            AutoRhythm newAutoRhythm = autoRhythms.Find(
                autoRhythm => autoRhythm.name == rhythmName
            );
            if (autoRhythms.Find(autoRhythm => autoRhythm.name == rhythmName) != null)
            {
                Debug.LogWarning(
                    this.name
                        + "has overwritten an AutoRhythm called \""
                        + rhythmName
                        + "\" synced to "
                        + newAutoRhythm.name
                        + " at "
                        + newAutoRhythm.BPM
                        + " BPM"
                );
                autoRhythms.Remove(newAutoRhythm);
            }
            newAutoRhythm = new AutoRhythm(syncedSound);
            newAutoRhythm.name = rhythmName;
            newAutoRhythm.soundToWaitFor = syncedSound;
            newAutoRhythm.BPM = syncedSound.BPM;
            newAutoRhythm.steps = steps;
            foreach (UnityAction action in actions)
                newAutoRhythm.actions.AddListener(action);
            autoRhythms.Add(newAutoRhythm);
        }

        public AutoRhythm FindAutoRhythmByName(string name)
        {
            foreach (AutoRhythm autoRhythm in autoRhythms)
                if (autoRhythm.name == name)
                {
                    return autoRhythm;
                }
            return null;
        }
    }
}
