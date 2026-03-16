using UnityEngine;

[RequireComponent(typeof(MicrophoneListener))]
public class MicrophoneRecorder : MonoBehaviour
{
    private MicrophoneListener _microphoneListener;
    private AudioClip _recordingClip;
    
    void Start()
    {
        _microphoneListener = GetComponent<MicrophoneListener>();
    }

    }
