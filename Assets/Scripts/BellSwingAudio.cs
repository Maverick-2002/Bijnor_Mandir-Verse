using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class BellSwingAudio : MonoBehaviour
{
    public float swingVelocityThreshold = 1.5f;
    public float minTimeBetweenSounds = 0.3f;

    private Rigidbody rb;
    private AudioSource audioSource;
    private XRGrabInteractable grabInteractable;

    private bool isHeld = false;
    private float lastSoundTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        isHeld = true;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        isHeld = false;
        StopSound(); // stop sound on release
    }

    void Update()
    {
        if (!isHeld)
        {
            StopSound();
            return;
        }

        float velocity = rb.velocity.magnitude;
        float angular = rb.angularVelocity.magnitude;

        bool isSwinging = velocity > swingVelocityThreshold || angular > swingVelocityThreshold;

        if (isSwinging)
        {
            if (!audioSource.isPlaying && Time.time - lastSoundTime > minTimeBetweenSounds)
            {
                audioSource.Play();
                lastSoundTime = Time.time;
            }
        }
        else
        {
            StopSound();
        }
    }

    void StopSound()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
