using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(MicrophoneSelector))]
[RequireComponent(typeof(AudioSource))]
public class MicrophoneListener : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool playMicAudio;
    [SerializeField] [Range(0f, 4f)] private float sensitivity = 1f;

    [Header("Live Monitor")]
    [ProgressBar("Volume", 1.0f)]
    [SerializeField] private float currentLoudness;
    
    // Required component references
    private MicrophoneSelector _micSelector;
    private AudioSource _audioSource;
    
    private AudioClip _micClip;
    private string _device;

    void Start()
    {
        _micSelector = GetComponent<MicrophoneSelector>();
        _audioSource = GetComponent<AudioSource>();
        
        // Settings for live mic playback
        _audioSource.playOnAwake = false;
        _audioSource.loop = true;
        
        StartMic();
    }

    void Update()
    {
        currentLoudness = Mathf.Clamp01(GetLoudness() * sensitivity);
        
        if (playMicAudio && _audioSource.isPlaying)
            _audioSource.volume = sensitivity / 10f;
    }

    void StartMic()
    {
        _device = _micSelector.selectedMicrophone;
        if (string.IsNullOrEmpty(_device) || _device == "No Microphones Found") return;

        // Continuously record 1 second microphone audio clips
        _micClip = Microphone.Start(_device, true, 1, 44100/2);

        _audioSource.clip = _micClip;
        
        if (playMicAudio) StartCoroutine(WaitAndPlay());
    }

    System.Collections.IEnumerator WaitAndPlay()
    {
        while (Microphone.GetPosition(_device) <= 0)
            yield return null; 
        _audioSource.Play();
    }

    public float GetLoudness()
    {
        if (_micClip == null) return 0f;

        int sampleWindow = 128;
        float[] waveData = new float[sampleWindow];
        int micPosition = Microphone.GetPosition(_device);

        if (micPosition < sampleWindow) return 0f;

        _micClip.GetData(waveData, micPosition - sampleWindow);

        float sum = 0;
        for (int i = 0; i < sampleWindow; i++)
            sum += waveData[i] * waveData[i];

        return Mathf.Sqrt(sum / sampleWindow);
    }
}