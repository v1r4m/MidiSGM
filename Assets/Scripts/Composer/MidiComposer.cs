using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum MidiInstrument
{
    ACOUSTIC_GRAND_PIANO = 1,
    BRIGHT_ACOUSTIC_PIANO = 2,
    ELECTRIC_GRAND_PIANO = 3,
    HONKY_TONK_PIANO = 4,
    ELECTRIC_PIANO_1 = 5,
    ELECTRIC_PIANO_2 = 6,
    HARPSICHORD = 7,
    CLAVINET = 8,
    CELESTA = 9,
    GLOCKENSPIEL = 10,
    MUSIC_BOX = 11,
    VIBRAPHONE = 12,
    MARIMBA = 13,
    XYLOPHONE = 14,
    TUBULAR_BELLS = 15,
    DULCIMER = 16,
    DRAWBAR_ORGAN = 17,
    PERCUSSIVE_ORGAN = 18,
    ROCK_ORGAN = 19,
    CHURCH_ORGAN = 20,
    REED_ORGAN = 21,
    ACCORDION = 22,
    HARMONICA = 23,
    TANGO_ACCORDION = 24,
    ACOUSTIC_GUITAR_NYLON = 25,
    ACOUSTIC_GUITAR_STEEL = 26,
    ELECTRIC_GUITAR_JAZZ = 27,
    ELECTRIC_GUITAR_CLEAN = 28,
    ELECTRIC_GUITAR_MUTED = 29,
    OVERDRIVEN_GUITAR = 30,
    DISTORTION_GUITAR = 31,
    GUITAR_HARMONICS = 32,
    ACOUSTIC_BASS = 33,
    ELECTRIC_BASS_FINGER = 34,
    ELECTRIC_BASS_PICK = 35,
    FRETLESS_BASS = 36,
    SLAP_BASS_1 = 37,
    SLAP_BASS_2 = 38,
    SYNTH_BASS_1 = 39,
    SYNTH_BASS_2 = 40,
    VIOLIN = 41,
    VIOLA = 42,
    CELLO = 43,
    CONTRABASS = 44,
    TREMOLO_STRINGS = 45,
    PIZZICATO_STRINGS = 46,
    ORCHESTRAL_HARP = 47,
    TIMPANI = 48,
    STRING_ENSEMBLE_1 = 49,
    STRING_ENSEMBLE_2 = 50,
    SYNTH_STRINGS_1 = 51,
    SYNTH_STRINGS_2 = 52,
    CHOIR_AAHS = 53,
    VOICE_OOHS = 54,
    SYNTH_CHOIR = 55,
    ORCHESTRA_HIT = 56,
    TRUMPET = 57,
    TROMBONE = 58,
    TUBA = 59,
    MUTED_TRUMPET = 60,
    FRENCH_HORN = 61,
    BRASS_SECTION = 62,
    SYNTH_BRASS_1 = 63,
    SYNTH_BRASS_2 = 64,

    SOPRANO_SAX = 65,
    ALTO_SAX = 66,
    TENOR_SAX = 67,
    BARITONE_SAX = 68,
    OBOE = 69,
    ENGLISH_HORN = 70,
    BASSOON = 71,
    CLARINET = 72,

    PICCOLO = 73,
    FLUTE = 74,
    RECORDER = 75,
    PAN_FLUTE = 76,
    BLOWN_BOTTLE = 77,
    SHAKUHACHI = 78,
    WHISTLE = 79,
    OCARINA = 80,

    LEAD_1 = 81,
    LEAD_2 = 82,
    LEAD_3 = 83,
    LEAD_4 = 84,
    LEAD_5 = 85,
    LEAD_6 = 86,
    LEAD_7 = 87,
    LEAD_8 = 88,
    PAD_1 = 89,
    PAD_2 = 90,
    PAD_3 = 91,
    PAD_4 = 92,
    PAD_5 = 93,
    PAD_6 = 94,
    PAD_7 = 95,
    PAD_8 = 96,
    FX_1 = 97,
    FX_2 = 98,
    FX_3 = 99,
    FX_4 = 100,
    FX_5 = 101,
    FX_6 = 102,
    FX_7 = 103,
    FX_8 = 104,
    SITAR = 105,
    BANJO = 106,
    SHAMISEN = 107,
    KOTO = 108,
    KALIMBA = 109,
    BAGPIPE = 110,
    FIDDLE = 111,
    SHANAI = 112,
    TINKLE_BELL = 113,
    AGOGO = 114,
    STEEL_DRUM = 115,
    WOODBLOCK = 116,
    TAIKO_DRUM = 117,
    MELODIC_TOM = 118,
    SYNTH_DRUM = 119,
    REVERSE_CYMBAL = 120,
    GUITAR_FRET = 121,
    BREATH_NOISE = 122,
    SEASHORE = 123,
    BIRD_TWEET = 124,
    TELEPHONE_RING = 125,
    HELICOPTER = 126,
    APPLAUSE = 127,
    GUNSHOT = 128
}

[Serializable]
public struct Tracks
{
    public TextAsset midi;
    public MidiInstrument instrument;
    public bool isplaying;
    public string hash;
    public bool solo;
}


public class MidiComposer : MonoBehaviour
{
    public List<string> presets;
    public List<Tracks> tracks;

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
            trackMidis.Add(new CSharpSynth.Midi.MidiFile(track.midi));
        
        StartCoroutine(ComposeRoutine());
    }
    void Update()
    {
        leftShit = halted.Count;
    }
    public IEnumerator ComposeRoutine()
    {
        ComposeNext();
        yield return new WaitForSeconds(length * 60/2/ bpm);
        while (true)
        {
            ComposeNext();
            yield return new WaitForSeconds(length * 60 / bpm);
        }
    }
    void ComposeNext()
    {
        Debug.Log("ComposeNext start");
        //todo: implement solo
        for (int i = 0; i < tracks.Count; i++)  // compose here!
        {
            if (!tracks[i].isplaying) continue; // compose here!
            var a = tracks[i];
            var q = halted.FirstOrDefault();

            if (q != null) halted.Remove(q);
            else { q = new MIDITrack(); q.Init(); }


            lock (playing)
                playing.Add(new KeyValuePair<long, MIDITrack>( nextstart, q));//todo: force bpm
            q.LoadSong(new CSharpSynth.Midi.MidiFile(a.midi), bpm);
        }
        nextstart += (long)(16 * 60d / bpm * samplerate);
        Debug.Log("ComposeNext end");
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
