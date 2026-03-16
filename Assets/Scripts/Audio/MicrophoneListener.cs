using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(MicrophoneSelector))]
[RequireComponent(typeof(AudioSource))]
public class MicrophoneListener : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] [Range(0f, 4f)] private float sensitivity = 1f;
    
    [Header("Debug")]
    [SerializeField] private bool playMicAudio;
    [ProgressBar("Volume Monitor", 1.0f)]
    [SerializeField] private float currentLoudness;
    
    // Required component references
    private MicrophoneSelector _micSelector;
    private AudioSource _audioSource;
    
    private AudioClip _micClip;
    private string _selectedMicrophone;

    void Start()
    {
        _micSelector = GetComponent<MicrophoneSelector>();
        _audioSource = GetComponent<AudioSource>();
        
        // Settings for live mic playback
        _audioSource.playOnAwake = false;
        _audioSource.loop = true;
        
        StartMic();
    }

    void StartMic()
    {
        _selectedMicrophone = _micSelector.selectedMicrophone;
        if (string.IsNullOrEmpty(_selectedMicrophone)) return;

        // Continuously record 1 second microphone audio clips
        _micClip = Microphone.Start(_selectedMicrophone, true, 1, 44100/2);

        _audioSource.clip = _micClip;
        
        StartCoroutine(WaitAndPlay());
    }

    System.Collections.IEnumerator WaitAndPlay()
    {
        while (Microphone.GetPosition(_selectedMicrophone) <= 0)
            yield return null; 
        _audioSource.Play();
    }
    
    void Update()
    {
        currentLoudness = Mathf.Clamp01(GetLoudness() * sensitivity);
        _audioSource.volume = (playMicAudio ? 1 : 0);
    }
    
    void OnAudioFilterRead(float[] data, int channels)
    {
        for (int i = 0; i < data.Length; i++)
        {
            data[i] *= sensitivity;
        
            if (data[i] > 1f) data[i] = 1f;
            if (data[i] < -1f) data[i] = -1f;
        }
    }

    public float GetLoudness()
    {
        if (_micClip == null) return 0f;

        int sampleWindow = 128;
        float[] waveData = new float[sampleWindow];
        int micPosition = Microphone.GetPosition(_selectedMicrophone);

        if (micPosition < sampleWindow) return 0f;

        _micClip.GetData(waveData, micPosition - sampleWindow);

        float sum = 0;
        for (int i = 0; i < sampleWindow; i++)
            sum += waveData[i] * waveData[i];

        return Mathf.Sqrt(sum / sampleWindow);
    }
}