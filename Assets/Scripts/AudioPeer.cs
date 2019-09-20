using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent (typeof(AudioSource))]
public class AudioPeer : MonoBehaviour
{

    AudioSource _audioSource;
    public static float[] _samples = new float[512];
    public static float[] _freqBand = new float[8];
    public static float[] _bandbuffer = new float[8];
    public static float[] _bufferDecrease = new float[8];


    public AudioClip audioclip;
    public bool useMicrophone; 
    public string selecteddevice;
    public AudioMixerGroup mixergroupMicrophone, mixergroupMaster;
    


    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>(); 

        if(useMicrophone){
            if(Microphone.devices.Length > 0){
                selecteddevice = Microphone.devices[0].ToString();
                _audioSource.outputAudioMixerGroup = mixergroupMicrophone;
                _audioSource.clip = Microphone.Start(selecteddevice, true, 10, AudioSettings.outputSampleRate);
                
            
            }
            else{
                useMicrophone = false;
            }

        }   
        if(!useMicrophone){
             _audioSource.clip = audioclip;
             _audioSource.outputAudioMixerGroup = mixergroupMaster;
        }


        _audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        GetSpectrumAudioSource();
        MakeFrequencyBands();
        BandBuffer();
    }

    void MakeFrequencyBands(){
        int count = 0;

        for(int i = 0; i <8; i++){
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            if(i ==7){
                sampleCount += 2;
            }

            for(int j = 0; j< sampleCount; j++){
                average += _samples[count] * (count + 1);
                count++;
            }

            average /= count;

            _freqBand[i] = average * 10;
        }
    }

    void GetSpectrumAudioSource(){
        _audioSource.GetSpectrumData (_samples, 0, FFTWindow.Blackman);
    }

    void BandBuffer(){
        for(int g = 0; g < 8; g++){
            if(_freqBand[g] > _bandbuffer[g]){
                _bandbuffer[g] = _freqBand[g];
                _bufferDecrease[g] = 0.005f; 
                
            }

            if(_freqBand[g] < _bandbuffer[g]){
                _bandbuffer[g] -= _bufferDecrease[g];
                _bufferDecrease[g] *= 1.2f;
                
            }
        }
    }
}
