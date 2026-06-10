namespace FpsHorrorKit
{
    using UnityEngine;
    using UnityEngine.Audio; // Mikser işlemleri için eklendi

    [RequireComponent(typeof(CharacterController))]
    public class PlayerAudioManager : MonoBehaviour
    {
        [Header("Ses Kaynakları (Inspector'dan Atanacak)")]
        public AudioSource actionSource; // Ayak ve Düşme sesleri için
        public AudioSource breathSource; // Nefes sesleri için

        [Header("Mikser Ayarları")]
        public AudioMixerGroup playerMixerGroup;

        [Header("Ses Seviyeleri (Volume Ayarları)")]
        [Range(0f, 1f)] public float footstepVolume = 0.25f; 
        [Range(0f, 1f)] public float breathVolume = 0.9f;    
        [Range(0f, 1f)] public float landVolume = 1.0f;      

        [Header("Ayak Sesleri")]
        public AudioClip[] walkSounds;
        public AudioClip[] sprintSounds;
        public float walkInterval = 0.5f;
        public float sprintInterval = 0.3f;

        [Header("Zıplama & Düşme")]
        public AudioClip landSound;
        private float airTimeTimer = 0f;

        [Header("Nefes Alma")]
        public AudioClip normalBreathSound;
        public AudioClip heavyBreathSound;
        public float timeToHeavyBreath = 5.0f; 
        private float currentSprintTime = 0f;

        private CharacterController controller;
        private FpsAssetsInputs input;

        private float stepTimer;
        private bool wasGrounded;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            input = GetComponent<FpsAssetsInputs>();

            // Artık hoparlörleri otomatik yaratmıyoruz. Atanmamışsa hata verecek.
            if (actionSource == null || breathSource == null)
            {
                Debug.LogError("PlayerAudioManager: Lütfen Inspector üzerinden Action ve Breath AudioSource yuvalarını doldurun!");
            }
            else if (playerMixerGroup != null)
            {
                // Sahnede senin atadığın o iki hoparlörün çıkış kablosunu, kodla miksere bağlıyoruz
                actionSource.outputAudioMixerGroup = playerMixerGroup;
                breathSource.outputAudioMixerGroup = playerMixerGroup;
            }
        }

        private void Update()
        {
            if (breathSource != null) 
            {
                breathSource.volume = breathVolume;
            }

            HandleFootsteps();
            HandleJumpAndLand();
            HandleBreathing();
        }

        private void HandleFootsteps()
        {
            if (controller.isGrounded && input.move.magnitude > 0.1f)
            {
                stepTimer += Time.deltaTime;
                float currentInterval = input.sprint ? sprintInterval : walkInterval;

                if (stepTimer >= currentInterval)
                {
                    PlaySoundFromArray(input.sprint ? sprintSounds : walkSounds);
                    stepTimer = 0f;
                }
            }
            else
            {
                stepTimer = 0f;
            }
        }

        private void HandleJumpAndLand()
        {
            if (!controller.isGrounded)
            {
                airTimeTimer += Time.deltaTime;
            }

            if (!wasGrounded && controller.isGrounded)
            {
                if (airTimeTimer > 0.2f)
                {
                    if (landSound != null) actionSource.PlayOneShot(landSound, landVolume);
                }
                
                airTimeTimer = 0f;
            }

            wasGrounded = controller.isGrounded; 
        }

        private void HandleBreathing()
        {
            bool isSprinting = input.sprint && input.move.magnitude > 0.1f;

            if (isSprinting)
            {
                currentSprintTime += Time.deltaTime;

                if (currentSprintTime >= timeToHeavyBreath)
                {
                    if (heavyBreathSound != null)
                    {
                        if (breathSource.clip != heavyBreathSound)
                        {
                            breathSource.clip = heavyBreathSound;
                            breathSource.Play();
                        }
                        else if (!breathSource.isPlaying)
                        {
                            breathSource.Play();
                        }
                    }
                }
                else
                {
                    PlayNormalBreath();
                }
            }
            else
            {
                if (currentSprintTime > 0)
                {
                    currentSprintTime -= Time.deltaTime;
                }

                if (breathSource.clip == heavyBreathSound && breathSource.isPlaying)
                {
                    return; 
                }

                PlayNormalBreath();
            }
        }

        private void PlayNormalBreath()
        {
            if (normalBreathSound != null)
            {
                if (breathSource.clip != normalBreathSound)
                {
                    breathSource.clip = normalBreathSound;
                    breathSource.Play();
                }
                else if (!breathSource.isPlaying)
                {
                    breathSource.Play();
                }
            }
        }

        private void PlaySoundFromArray(AudioClip[] clips)
        {
            if (clips != null && clips.Length > 0 && clips[0] != null)
            {
                int index = Random.Range(0, clips.Length);
                actionSource.PlayOneShot(clips[index], footstepVolume);
            }
        }
    }
}