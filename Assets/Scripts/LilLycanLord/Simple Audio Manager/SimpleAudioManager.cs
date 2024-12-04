using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using LilLycanLord_Official;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

namespace LilLycanLord_Official
{
    //* SUMMARY
    //* This class simplifies the implementation of SFXs & BGMs by having them all in one place,
    //* with convenient function calls, to easily have an object use audio.
    public class SimpleAudioManager : MonoBehaviour
    {
        //! ╔═══════════════════╗
        //! ║ SINGLETON CONTENT ║
        //! ╚═══════════════════╝
        //* SUMMARY
        //* This singleton pattern is used to make singletons which recognize preset singletons
        //* within the Scene, and if there are none,
        //* automatically create instances of themselves when needed.
        static bool logSingleton = false;
        static SimpleAudioManager instance;
        public static SimpleAudioManager Instance
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
                typeof(SimpleAudioManager),
                FindObjectsSortMode.None
            );
            if (existingSingleton.Length > 0)
            {
                instance = existingSingleton[0].GetComponent<SimpleAudioManager>();
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
                    "Simple Audio Manager"
                ).AddComponent<SimpleAudioManager>();
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
        [Tooltip(
            "A dictionary of all objects currently playing audio in the scene. Sounds are automatically removed once they stop playing."
        )]
        [SerializedDictionary("GameObject", "CurrentlyPlayingSound")]
        public SerializedDictionary<
            GameObject,
            List<CurrentlyPlayingSound>
        > currentlyPlayingSounds =
            new SerializedDictionary<GameObject, List<CurrentlyPlayingSound>>();

        //* ╔════════╗
        //* ║ Fields ║
        //* ╚════════╝
        [Space(10)]
        [Header("Fields")]
        [SerializeField]
        [Tooltip(
            "Manually play a sound from the Sounds list via their name. Note that the name is its actual name in the Assets folder."
        )]
        string manualSoundName;

        [Tooltip("A list of all audios found within the game.")]
        public List<Sound> sounds = new List<Sound>();

        //* ╔════════════╗
        //* ║ Attributes ║
        //* ╚════════════╝

        //* ╔═══════════════╗
        //* ║ Monobehaviour ║
        //* ╚═══════════════╝
        void Start() { }

        void Update()
        {
            //? Store all GameObjects to remove from the dictionary by batches.
            List<GameObject> toRemove = new List<GameObject>();
            foreach (
                KeyValuePair<
                    GameObject,
                    List<CurrentlyPlayingSound>
                > soundPlayer in currentlyPlayingSounds
            )
            {
                foreach (CurrentlyPlayingSound playedSound in soundPlayer.Value)
                {
                    //? Remove unused audiosources from GameObjects.
                    if (!playedSound.audioSource.isPlaying)
                    {
                        SimpleRhythmManager.Instance.StopBeatSync(playedSound);
                        Destroy(playedSound.audioSource);
                        playedSound.audioSource = null;
                    }
                }

                //? Remove empty references from the list.
                soundPlayer.Value.RemoveAll(sound => sound.audioSource == null);

                //? Remove GameObjects that aren't currently playing audio.
                if (soundPlayer.Value.Count <= 0)
                    toRemove.Add(soundPlayer.Key);
            }
            foreach (GameObject keyToRemove in toRemove)
                currentlyPlayingSounds.Remove(keyToRemove);
        }

        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
        [ContextMenu("Manual Play")]
        public void ManualPlay()
        {
            Play(manualSoundName, gameObject);
        }

        [ContextMenu("Manual Stop")]
        public void ManualStop()
        {
            StopAllWithName(manualSoundName);
        }

        //* SUMMARY
        //* Play an audio by attaching an AudioSource component to the soundPlayer, and setting it's necessary
        //* attributes from the Sound class. The connection of the soundPlayer playing a certain sound are represented
        //* as the PlayingSound class.
        public CurrentlyPlayingSound Play(
            string soundName,
            GameObject soundPlayer,
            float fadeInDuration = 0.0f,
            float fadeOutDuration = 0.0f
        )
        {
            if (GetSoundByName(soundName) == null)
                return null;

            CurrentlyPlayingSound newSound = new CurrentlyPlayingSound();
            newSound.sound = sounds.Find(sound => sound.name == soundName);
            newSound.audioSource = soundPlayer.AddComponent<AudioSource>();
            newSound.audioSource.loop = newSound.sound.loop;
            newSound.audioSource.clip = newSound.sound.audioClip;
            newSound.audioSource.pitch = newSound.sound.pitch;
            newSound.audioSource.volume = newSound.sound.volume;

            //? If this is the first Sound a GameObject will make, add a Key for it.
            if (!currentlyPlayingSounds.ContainsKey(soundPlayer))
                currentlyPlayingSounds.Add(soundPlayer, new List<CurrentlyPlayingSound>());
            currentlyPlayingSounds[soundPlayer].Add(newSound);

            if (fadeInDuration > 0.0f)
                StartCoroutine(FadeIn(newSound, soundPlayer, fadeInDuration));
            else
                newSound.audioSource.Play();
            if (fadeOutDuration > 0.0f)
                StartCoroutine(FadeOut(newSound, soundPlayer, fadeOutDuration));

            SimpleRhythmManager.Instance.StartBeatSync(newSound);
            return newSound;
        }

        //* SUMMARY
        //* Stops a specified Instance of a PlayingSound within a specific soundPlayer.
        public void Stop(
            CurrentlyPlayingSound playedSound,
            GameObject soundPlayer,
            float fadeOutDuration = 0.0f
        )
        {
            if (!currentlyPlayingSounds.ContainsKey(soundPlayer))
            {
                Debug.LogWarning(soundPlayer.name + " is not playing any Sounds.");
                return;
            }
            if (
                currentlyPlayingSounds[soundPlayer].Find(
                    playingSound => playingSound == playedSound
                ) != null
            )
                if (fadeOutDuration > 0.0f)
                    StartCoroutine(FadeOut(playedSound, soundPlayer, fadeOutDuration));
                else
                    playedSound.audioSource.Stop();
        }

        //* SUMMARY
        //* Stops all PlayingSounds
        [ContextMenu("Stop All")]
        public void StopAll(float fadeOutDuration = 0.0f)
        {
            foreach (GameObject soundPlayer in currentlyPlayingSounds.Keys)
            {
                foreach (CurrentlyPlayingSound playingSound in currentlyPlayingSounds[soundPlayer])
                    if (fadeOutDuration > 0.0f)
                        StartCoroutine(FadeOut(playingSound, soundPlayer, fadeOutDuration));
                    else
                        playingSound.audioSource.Stop();
            }
        }

        //* SUMMARY
        //* Stops a specific PlayingSound from a soundPlayer.
        public void StopAllPlayedByPlayerWithName(
            string soundName,
            GameObject soundPlayer,
            float fadeOutDuration = 0.0f
        )
        {
            if (!currentlyPlayingSounds.ContainsKey(soundPlayer))
            {
                Debug.LogWarning(soundPlayer.name + " is not playing any Sounds.");
                return;
            }
            foreach (
                CurrentlyPlayingSound playingSound in currentlyPlayingSounds[soundPlayer].FindAll(
                    playingSound => playingSound.sound.name == soundName
                )
            )
                if (fadeOutDuration > 0.0f)
                    StartCoroutine(FadeOut(playingSound, soundPlayer, fadeOutDuration));
                else
                    playingSound.audioSource.Stop();
        }

        //* SUMMARY
        //* Stops a specific PlayingSound in all soundPlayers.
        public void StopAllWithName(string soundName, float fadeOutDuration = 0.0f)
        {
            foreach (GameObject soundPlayer in currentlyPlayingSounds.Keys)
            {
                foreach (
                    CurrentlyPlayingSound playingSound in currentlyPlayingSounds[
                        soundPlayer
                    ].FindAll(playingSound => playingSound.sound.name == soundName)
                )
                    if (fadeOutDuration > 0.0f)
                        StartCoroutine(FadeOut(playingSound, soundPlayer, fadeOutDuration));
                    else
                        playingSound.audioSource.Stop();
            }
        }

        //* SUMMARY
        //* Stops all PlayingSounds from a soundPlayer.
        public void StopAllPlayedByPlayer(GameObject soundPlayer, float fadeOutDuration = 0.0f)
        {
            if (!currentlyPlayingSounds.ContainsKey(soundPlayer))
            {
                Debug.LogWarning(soundPlayer.name + " is not playing any Sounds.");
                return;
            }
            foreach (CurrentlyPlayingSound playingSound in currentlyPlayingSounds[soundPlayer])
                playingSound.audioSource.Stop();
        }

        IEnumerator FadeIn(
            CurrentlyPlayingSound currentlyPlayingSound,
            GameObject soundPlayer,
            float fadeInDuration
        )
        {
            float timeElapsed = 0;
            currentlyPlayingSound.audioSource.volume = 0.0f;
            currentlyPlayingSound.audioSource.Play();
            while (timeElapsed < fadeInDuration)
            {
                currentlyPlayingSound.audioSource.volume = Mathf.Lerp(
                    0.0f,
                    1.0f,
                    timeElapsed / fadeInDuration
                );
                timeElapsed += Time.deltaTime;
                yield return null;
            }
        }

        IEnumerator FadeOut(
            CurrentlyPlayingSound currentlyPlayingSound,
            GameObject soundPlayer,
            float fadeOutDuration
        )
        {
            float timeElapsed = 0;
            yield return new WaitForSeconds(
                currentlyPlayingSound.audioSource.clip.length - fadeOutDuration
            );
            while (timeElapsed < fadeOutDuration && currentlyPlayingSound.audioSource != null)
            {
                currentlyPlayingSound.audioSource.volume = Mathf.Lerp(
                    1.0f,
                    0.0f,
                    Mathf.SmoothStep(0.0f, 1.0f, timeElapsed / fadeOutDuration)
                );
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            if (currentlyPlayingSound.audioSource != null)
                currentlyPlayingSound.audioSource.Stop();
        }

        public Sound GetSoundByName(string soundName)
        {
            foreach (Sound sound in sounds)
                if (sound.name == soundName)
                    return sound;

            Debug.LogWarning(name + " has no Sound called \"" + soundName + "\"");
            return null;
        }

        public Sound AddSoundIfNotExisting(Sound newSound)
        {
            foreach (Sound sound in sounds)
                if (sound.name == newSound.name)
                    return sound;

            sounds.Add(newSound);
            return sounds.Find(sound => sound == newSound);
        }

        public Sound GetSoundPlayedByObject(string soundName, GameObject soundPlayer)
        {
            return VerifyIfPlayerIsPlayingSound(soundName, soundPlayer).sound;
        }

        CurrentlyPlayingSound VerifyIfPlayerIsPlayingSound(string soundName, GameObject soundPlayer)
        {
            if (!currentlyPlayingSounds.ContainsKey(soundPlayer))
            {
                Debug.LogWarning(soundPlayer.name + " is not playing any Sounds.");
                return null;
            }
            if (
                currentlyPlayingSounds[soundPlayer].Find(
                    playedSound => playedSound.sound.name == soundName
                ) == null
            )
            {
                Debug.LogWarning(
                    soundPlayer.name + " is not playing a Sound called \"" + soundName + "\""
                );
                return null;
            }
            return currentlyPlayingSounds[soundPlayer].Find(
                playedSound => playedSound.sound.name == soundName
            );
        }
    }
}
