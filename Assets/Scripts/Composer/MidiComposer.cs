﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MidiComposer : MonoBehaviour
{
    public List<string> tracks;
    
    public List<string> presets;
    List<KeyValuePair<long, MIDITrack>> playing = new List<KeyValuePair<long, MIDITrack>>();
    List<MIDITrack> halted = new List<MIDITrack>();
    List<CSharpSynth.Midi.MidiFile> trackMidis;
    const long samplerate = 44100;

    //debug vars, can remove
    float leftShit;

    long nextstart = 0;
    float length = 16;
    

    public int bpm = 140;

    public int preset = 0;

    //debug public, set to private!!!
    public long currentFrame = 0;

    int bufferSize = 1024;

    // Use this for initialization
    void Start()
    {
        trackMidis = new List<CSharpSynth.Midi.MidiFile>();

        for (int i = 0; i < 30; i++)
            halted.Add(new MIDITrack());
        foreach (var a in halted) a.Init();

        foreach (var track in tracks)
            trackMidis.Add(new CSharpSynth.Midi.MidiFile(track));

        ComposeNext();
        StartCoroutine(ComposeRoutine());
    }
    void Update()
    {
        leftShit = halted.Count;
    }
    public IEnumerator ComposeRoutine()
    {
        while (true)
        {
            ComposeNext();
            yield return new WaitForSeconds(16 * 60 / bpm);
        }
    }
    void ComposeNext()
    {
        Debug.LogError("ComposeNext start");
        for (int i = 0; i < tracks.Count; i++)  // compose here!
        {
            if (!presets[preset].Contains(i.ToString())) continue;
            var a = tracks[i];
            var q = halted.FirstOrDefault();

            if (q != null) halted.Remove(q);
            else { q = new MIDITrack(); q.Init(); }


            lock (playing)
                playing.Add(new KeyValuePair<long, MIDITrack>( nextstart, q));//todo: force bpm
            q.LoadSong(new CSharpSynth.Midi.MidiFile(a), bpm);
        }
        nextstart += (long)(16 * 60d / bpm * samplerate);
        Debug.LogError("ComposeNext end");
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        lock (playing)
            for (int i = 0; i < playing.Count; i++)
            {
                if (currentFrame > playing[i].Key + (length*2) * 60d / bpm * samplerate)//make isplaying and remove here
                {
                    var a = playing[i].Value;
                    halted.Add(a);
                    playing.RemoveAt(i);
                    i--;
                }
            }
        for (int i = 0; i < data.Length; i++)
            data[i] = 0;
        lock (playing)
            foreach (var a in playing)
            {
                if (currentFrame < a.Key)
                    continue;
                var track = a.Value;
                var sample = track.Next();
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] += sample[i] * track.gain;
                }
            }
        
        currentFrame += bufferSize;
        
    }
    
}