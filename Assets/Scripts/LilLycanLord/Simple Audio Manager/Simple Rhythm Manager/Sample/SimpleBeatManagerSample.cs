using LilLycanLord_Official;
using UnityEngine;
using UnityEngine.Assertions;

namespace LilLycanLord_Official
{
    public class SimpleRhythmManagerSample : MonoBehaviour
    {
        //* ╔════════════╗
        //* ║ Components ║
        //* ╚════════════╝

        //* ╔══════════╗
        //* ║ Displays ║
        //* ╚══════════╝
        [Header("Displays")]
        [SerializeField]
        bool pulse = false;

        //* ╔════════╗
        //* ║ Fields ║
        //* ╚════════╝
        [Space(10)]
        [Header("Fields")]
        [SerializeField]
        bool manuallyAddRhythm = false;

        [SerializeField]
        Sound syncedSound;

        [SerializeField]
        float pulseSize = 1.2f;

        [SerializeField]
        float pulseDuration = 0.1f;

        //* ╔════════════╗
        //* ║ Attributes ║
        //* ╚════════════╝

        //* ╔═══════════════╗
        //* ║ Monobehaviour ║
        //* ╚═══════════════╝
        void Start()
        {
            if (manuallyAddRhythm)
                return;
            SimpleRhythmManager.Instance.AddAutoRhythm("Half Beat", syncedSound, 2, Pulse);
            SimpleAudioManager.Instance.Play(syncedSound.name, gameObject);
        }

        void Update()
        {
            if (pulse)
                Grow();
            else
                Shrink();
        }

        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
        public void Pulse()
        {
            pulse = true;
            Invoke("ResetScale", pulseDuration);
        }

        private void ResetScale()
        {
            pulse = false;
        }

        private void Grow()
        {
            transform.localScale = new Vector3(
                Mathf.Lerp(transform.localScale.x, pulseSize, pulseDuration / 2),
                Mathf.Lerp(transform.localScale.y, pulseSize, pulseDuration / 2),
                Mathf.Lerp(transform.localScale.z, pulseSize, pulseDuration / 2)
            );
        }

        private void Shrink()
        {
            transform.localScale = new Vector3(
                Mathf.Lerp(transform.localScale.x, 1.0f, pulseDuration / 2),
                Mathf.Lerp(transform.localScale.y, 1.0f, pulseDuration / 2),
                Mathf.Lerp(transform.localScale.z, 1.0f, pulseDuration / 2)
            );
        }
    }
}
