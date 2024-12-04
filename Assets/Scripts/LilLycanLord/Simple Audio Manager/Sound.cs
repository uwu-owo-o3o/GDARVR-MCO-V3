using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace LilLycanLord_Official
{
    [Serializable]
    [CreateAssetMenu(fileName = "New Sound", menuName = "LilLycanLord/Audio/Sound")]
    //* SUMMARY
    //* All audio will now be handled by the Simple Audio Manager, storing references to the audio clip
    //* and the necessary attributes if this certain audio was played by
    //* a GameObject via the AudioSource component.
    public class Sound : ScriptableObject
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
        [Space(10)]
        [Header("Audio Details")]
        public AudioClip audioClip;
        public float BPM = 0;

        [Space(10)]
        [Header("AudioSource Attributes")]
        public bool loop = false;

        [Range(0.1f, 2.0f)]
        public float pitch = 1.0f;

        [Range(0.1f, 1.0f)]
        public float volume = 1.0f;

        //* ╔════════════╗
        //* ║ Attributes ║
        //* ╚════════════╝

        //* ╔═══════════════╗
        //* ║ Monobehaviour ║
        //* ╚═══════════════╝
        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
    }

    //* SUMMARY
    //* This class then represents as the connection between GameObjects playing a certain Sound.
    [Serializable]
    public class CurrentlyPlayingSound
    {
        //* ╔════════════╗
        //* ║ Components ║
        //* ╚════════════╝

        //* ╔══════════╗
        //* ║ Displays ║
        //* ╚══════════╝
        [Header("Displays")]
        public Sound sound;
        public AudioSource audioSource;

        //* ╔════════╗
        //* ║ Fields ║
        //* ╚════════╝
        // [Space(10)]
        // [Header("Fields")]
        //* ╔════════════╗
        //* ║ Attributes ║
        //* ╚════════════╝

        //* ╔═══════════════╗
        //* ║ Monobehaviour ║
        //* ╚═══════════════╝
        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
    }
}
