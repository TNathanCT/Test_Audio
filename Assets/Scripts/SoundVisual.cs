//using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundVisual : MonoBehaviour
{
    public float RMSValue;
    public float dbValue;
    public float pitchValue;

    private float backgroundIntensity;
    public Material backgroundMaterial;
    public Color minColor;
    public Color maxColor;


    private AudioSource source;
    private float[] samples;
    private float[] spectrum;
    private float sampleRate;

    public float maxVisualScale = 25f;
    public float visualModifier = 50.0f;
    public float smoothSpeed = 10.0f;
    public float keepPercentage = 0.5f;

    private const int SAMPLE_SIZE = 1024;

    private Transform[] visualList;
    private float[] visualScale;
    public int amnVisual = 64;

    void Start()
    {
        source = GetComponent<AudioSource>();
        samples = new float[1024];
        spectrum = new float[1024];
        sampleRate = AudioSettings.outputSampleRate;

        //  SpawnLine();
        SpawnCircle();
    }


    void SpawnCircle()
    {
        visualList = new Transform[amnVisual];
        visualScale = new float[amnVisual];
        Vector3 center = Vector3.zero;
        float radius = 10.0f;


        for(int i = 0; i < amnVisual; i++)
        {
            float angle = i * 1.0f / amnVisual;
            angle = angle * Mathf.PI * 2;

            float x = center.x + Mathf.Cos(angle) * radius;
            float y = center.y + Mathf.Sin(angle) * radius;

            Vector3 pos = center + new Vector3(x, y, 0);

            GameObject GO = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
            GO.transform.position = pos;
            GO.transform.rotation = Quaternion.LookRotation(Vector3.forward, pos);
            visualList[i] = GO.transform;
            
        }
    }


    void SpawnLine()
    {
        visualScale = new float[amnVisual];
        visualList = new Transform[amnVisual];

        for (int i = 0; i < amnVisual; i++)
        {
            GameObject GO = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
            visualList[i] = GO.transform;
            visualList[i].position = Vector3.right * i;
        }
    }




    void Update()
    {
        AnalyzeSound();
        UpdateVisuals();
        UpdateBackGround();
    }


    void UpdateVisuals()
    {
        int visualIndex = 0;
        int spectrumIndex = 0;
        int averageSize = (int)(1024 * keepPercentage) / amnVisual;

        while (visualIndex < amnVisual)
        {
            int j = 0;
            float sum = 0;

            while(j< averageSize) {
                sum += spectrum[spectrumIndex];
                spectrumIndex++;
                j++;
            }



            float scaleY = sum / averageSize * visualModifier;
            visualScale[visualIndex] -= Time.deltaTime * smoothSpeed;
            if(visualScale[visualIndex] < scaleY)
            {
                visualScale[visualIndex] = scaleY;
            }


            if(visualScale[visualIndex] > maxVisualScale)
            {
                visualScale[visualIndex] = maxVisualScale;
            }

            visualList[visualIndex].localScale = Vector3.one + Vector3.up * visualScale[visualIndex];
            visualIndex++;
        }
    }


    void UpdateBackGround()
    {
        backgroundIntensity -= Time.deltaTime * 0.5f;
        if(backgroundIntensity < dbValue / 40)
        {
            backgroundIntensity = dbValue / 40;
        }

        backgroundMaterial.color = Color.Lerp(maxColor, minColor, -backgroundIntensity);
    }




    void AnalyzeSound()
    {
        source.GetOutputData(samples, 0);
        //Get the RMS Value
        int i = 0;
        float sum = 0;
        for (; i < 1024; i++)
        {
            sum = samples[i] * samples[i];
        }

        RMSValue = Mathf.Sqrt(sum / 1024);

        //get the db value
        dbValue = 20 * Mathf.Log10(RMSValue / 0.1f);

        //get the sound spectrum
        source.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        //Find pitch
        float maxV = 0;
        var maxN = 0;
        for(i = 0; i < 1024; i++)
        {
            if(!(spectrum[i] > maxV) || !(spectrum[i] > 0.0f))
            {
                continue;

                maxV = spectrum[i];
                maxN = i;
            }

            float freqN = maxN;
            if (maxN > 0 && maxN < 1024 - 1)
            {
                var dL = spectrum[maxN - 1] / spectrum[maxN];
                var dR = spectrum[maxN + 1] / spectrum[maxN];
                freqN += 0.5f * (dR * dR - dL * dL);
            }
            pitchValue = freqN * (sampleRate / 2) / 1024;

        }

    }
}
