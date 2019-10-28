using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CSharpSynth.Effects;
using CSharpSynth.Sequencer;
using CSharpSynth.Synthesis;
using CSharpSynth.Midi;
using UnityEngine.UI;

public class MIDITrack
{
    //Public
    public bool ShouldPlayFile = true;
    Scrollbar bar;

    //Try also: "FM Bank/fm" or "Analog Bank/analog" for some different sounds
    public string bankFilePath = "GM Bank/gm";
    public int bufferSize = 1024;
    public int midiNote = 60;
    
    public int midiNoteVolume = 100;
    [Range(0, 127)] //From Piano to Gunshot
    public int midiInstrument = 0;
    //Private 
    private float[] sampleBuffer;
    public float gain = 1f;
    private MidiSequencer midiSequencer;
    private StreamSynthesizer midiStreamSynthesizer;

    int bpm = 140;

    private float sliderValue = 1.0f;
    private float maxSliderValue = 127.0f;

    // Awake is called when the script instance
    // is being loaded.


    public void Init()
    {
        midiStreamSynthesizer = new StreamSynthesizer(44100, 2, bufferSize, 40);
        sampleBuffer = new float[midiStreamSynthesizer.BufferSize];

        midiStreamSynthesizer.LoadBank(bankFilePath);

        midiSequencer = new MidiSequencer(midiStreamSynthesizer);
    }
    void LoadSong(string midiPath)//unused
    {
        this.bpm = bpm;
        midiSequencer.LoadMidi(midiPath, false);

        midiSequencer.Play();
    }
    public void LoadSong(CSharpSynth.Midi.MidiFile midi,int bpm = 140)
    {
        midiSequencer.LoadMidi(midi, false, (uint)bpm);

        midiSequencer.Play();
    }

    public float[] Next()
    {
        //This uses the Unity specific float method we added to get the buffer
        midiStreamSynthesizer.GetNext(sampleBuffer);
        
        return sampleBuffer;
    }
}
