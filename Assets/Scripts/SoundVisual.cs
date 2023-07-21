using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundVisual : MonoBehaviour
{
    private const int SAMPLE_SIZE = 1024;

    public float maxVisualScale = 10.0f;
    public float rmsValue;
    public float dbValue;
    public float pitchValue;
    public float visualModifier = 250.0f;
    public float smoothSpeed = 10.0f;
    public float keepPercentage = 0.25f;

    private AudioSource source;
    private float[] samples;
    private float[] spectrum;
    private float sampleRate;

    private Transform[] visualList;
    private float[] visualScale;
    public int amnVisual = 250;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        samples = new float[SAMPLE_SIZE];
        spectrum = new float[SAMPLE_SIZE];
        sampleRate = AudioSettings.outputSampleRate;
        SpawnCircle();
        //SpawnLine();
    }

    private void SpawnCircle(){
        visualScale = new float[amnVisual];
        visualList = new Transform[amnVisual];
        
        Vector3 center = Vector3.zero;
        float radius = 10.0f;

        for (int i = 0; i < amnVisual; i++)
        {
            float ang = i*1.0f / amnVisual;
            ang = ang * Mathf.PI *2;

            float x = center.x + Mathf.Cos(ang) * radius;
            float y = center.y + Mathf.Sin(ang) * radius;

            Vector3 pos = center + new Vector3(x,y,0);
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;

            go.transform.position= pos;
            go.transform.rotation = Quaternion.LookRotation(Vector3.forward, pos);
            visualList[i] = go.transform;
        }
    }

    private void SpawnLine()
    {
        visualScale = new float[amnVisual];
        visualList = new Transform[amnVisual];

        for (int i = 0; i < amnVisual; i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
            visualList[i] = go.transform;
            visualList[i].position = Vector3.right * i;
        } 
    }
    // Update is called once per frame
    void Update()
    {
        AnalyzeSound();
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        int visualIndex = 0;
        int spectrumIndex = 0;
        int averageSize = (int)(SAMPLE_SIZE * keepPercentage) / amnVisual;

        while(visualIndex < amnVisual){
            int j = 0;
            float sum = 0;
            while(j < averageSize)
            {
                sum += spectrum[spectrumIndex];
                spectrumIndex++;
                j++;
            }

            float scaleY = sum / averageSize * visualModifier;
            visualScale[visualIndex] -= Time.deltaTime * smoothSpeed;
            if(visualScale[visualIndex] < scaleY)
                visualScale[visualIndex] = scaleY;


            if(visualScale[visualIndex] > maxVisualScale)
                visualScale[visualIndex] = maxVisualScale;

            visualList[visualIndex].localScale = Vector3.one + Vector3.up * visualScale[visualIndex];
            visualIndex++;
        }
    }

    private void AnalyzeSound()
    {
        source.GetOutputData(samples, 0);

        //Get the RMS value
        int i = 0;
        float sum = 0;
        for(;i < SAMPLE_SIZE;i++){
            sum = samples[i] * samples[i];
        }
        rmsValue = Mathf.Sqrt(sum/SAMPLE_SIZE);

        //Get the DB value
        dbValue = 20 * Mathf.Log10(rmsValue / 0.1f);

        // Get sound spectrum
        source.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        // Find pitch
        float maxV = 0;
        var maxN = 0;
        for (i = 0; i < SAMPLE_SIZE; i++)
        {
            if(!(spectrum[i] > maxV) || !(spectrum[i] > 0.0f))
                continue;

            maxV = spectrum[i];
            maxN = i;
        }

        float freqN = maxN;
        if(maxN > 0 && maxN < SAMPLE_SIZE - 1){
            var dL = spectrum[maxN - 1] / spectrum[maxN];
            var dR = spectrum[maxN + 1]/spectrum[maxN];
            freqN += 0.5f * (dR*dR-dL*dL);
        }
        pitchValue = freqN * (sampleRate/2)/SAMPLE_SIZE;
    }
}
