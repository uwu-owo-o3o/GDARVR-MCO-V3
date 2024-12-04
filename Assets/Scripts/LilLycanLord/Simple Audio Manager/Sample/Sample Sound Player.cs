using LilLycanLord_Official;
using UnityEngine;
using UnityEngine.Assertions;

namespace LilLycanLord_Official
{
    enum SampleType
    {
        PlaySound,
        FadeIn,
        FadeOut,
    }

    public class SampleSoundPlayer : MonoBehaviour
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
        [SerializeField]
        Sound sampleSound;

        [SerializeField]
        SampleType sampleType = SampleType.PlaySound;

        [SerializeField]
        float fadeInOrOutAmount = 3.0f;

        //* ╔════════════╗
        //* ║ Attributes ║
        //* ╚════════════╝

        //* ╔═══════════════╗
        //* ║ Monobehaviour ║
        //* ╚═══════════════╝
        void Start()
        {
            switch (sampleType)
            {
                case SampleType.PlaySound:
                    SimpleAudioManager.Instance.Play(sampleSound.name, gameObject);
                    break;
                case SampleType.FadeIn:
                    SimpleAudioManager.Instance.Play(
                        sampleSound.name,
                        gameObject,
                        fadeInOrOutAmount
                    );
                    break;
                case SampleType.FadeOut:
                    SimpleAudioManager.Instance.Play(
                        "SampleSound",
                        gameObject,
                        0.0f,
                        fadeInOrOutAmount
                    );
                    break;
            }
        }

        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
    }
}
