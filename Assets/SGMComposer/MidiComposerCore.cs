using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

static public class MidiComposerCore
{
    [System.Serializable]
    public class Track
    {
        [SerializeField]
        public TextAsset midi; // hack: serialize with AssetDatabase.GetAssetPath
        [SerializeField]
        public MidiInstrument instrument = MidiInstrument.Default;
        [SerializeField]
        public bool isplaying = false;
        [SerializeField]
        public string name;
        [SerializeField]
        public bool issolo = false;
    }
    [System.Serializable]
    public class TracksSet
    {
        [SerializeField]
        public string name = "New Track";
        [SerializeField]
        public Dictionary<string, List<string>> presets = new Dictionary<string, List<string>>();
        [SerializeField]
        public Dictionary<string, List<string>> flows = new Dictionary<string, List<string>>();
    }


    public class Data
    {
        public List<Track> tracks = new List<Track>();

        public List<TracksSet> tracksets = new List<TracksSet>();

        #region serializer

        [Serializable]
        class SerializableData
        {
            public List<SerializableTrack> tracks = new List<SerializableTrack>();

            public List<TracksSet> tracksets;
        }
        [Serializable]
        class SerializableTrack
        {
            public string midi;
            public MidiInstrument instrument;
            public bool isplaying;
            public string name;
            public bool issolo;
#if UNITY_EDITOR
            public SerializableTrack() { }
            public SerializableTrack(Track t)
            {
                if (t.midi != null)
                    this.midi = AssetDatabase.GetAssetPath(t.midi);
                else
                    this.midi = null;
                this.instrument = t.instrument;
                this.isplaying = t.isplaying;
                this.name = t.name;
                this.issolo = t.issolo;
            }
            public Track ToTrack()
            {
                return new Track()
                {
                    midi = (this.midi == null ? null : (TextAsset)AssetDatabase.LoadAssetAtPath(this.midi, typeof(TextAsset))),
                    instrument = this.instrument,
                    isplaying = this.isplaying,
                    name = this.name,
                    issolo = this.issolo
                };
            }
#endif
        }
        public string Serialize()
        {
            SerializableData dt = new SerializableData()
            {
                tracks = this.tracks.ConvertAll(t => new SerializableTrack(t)),
                tracksets = this.tracksets
            };
            return JsonConvert.SerializeObject(dt);
        }
        public static Data Desrialize(string serializedData)
        {
            SerializableData dt = JsonConvert.DeserializeObject<SerializableData>(serializedData);
            return new Data()
            {
                tracks = dt.tracks.ConvertAll(d => d?.ToTrack()),
                tracksets = dt.tracksets
            };
        }
        #endregion  
    }
    public class PlayingData
    {
        public enum Mode
        {
            preset, flow,
        }
        public Mode mode;
        public int trackset = 0;
        public string preset_flow_name;
    }
    public class ComposerCore
    {
        const string resourceDir = "Assets\\Resources\\";
        const string savedir = "ComposerSav.sav";
        public ComposerCore()
        {
            if (File.Exists(resourceDir + savedir + ".txt"))
                d = Data.Desrialize(File.ReadAllText(resourceDir + savedir + ".txt"));
            else
            {
                var r = Resources.Load<TextAsset>(savedir);
                if (r != null)
                    d = Data.Desrialize(Resources.Load<TextAsset>(savedir).text);
                else d = new Data();
            }
#if UNITY_EDITOR
            var notAddedTracks =  AssetDatabase.GetAllAssetPaths().Where(a => a.EndsWith(".mid.txt") && !d.tracks.Exists(q => AssetDatabase.GetAssetPath(q.midi).Equals(a)));
            foreach (string natdir in notAddedTracks)
            {
                d.tracks.Add(new Track()
                {
                    name = natdir.Remove(natdir.Length - 8, 8).Split('/').Last(),
                    midi = (TextAsset)AssetDatabase.LoadAssetAtPath(natdir, typeof(TextAsset))
                });
            }
#endif
        }
        public Data d = new Data();
        public PlayingData p = new PlayingData();
        public int seqNo = 0;
        

        public bool forced;

        public List<Track> GetNext()
        {
            if (d.tracksets.Count == 0 || p.preset_flow_name == null) return new List<Track>();
            TracksSet ts = d.tracksets[p.trackset];
            switch (p.mode)
            {
                case PlayingData.Mode.flow:
                    var flow = ts.flows[p.preset_flow_name];
                    return Translate(ts.presets[flow[seqNo++ % flow.Count]], d.tracks);
                case PlayingData.Mode.preset:
                    seqNo = 0;
                    var preset = ts.presets[p.preset_flow_name];
                    return Translate(preset, d.tracks);
                default: throw new ArgumentException();
            }
        }
        public List<Track> Translate(List<string> s, List<Track> ts)
        {
            var @return = new List<Track>();
            foreach (var ss in s)
                @return.Add(ts.First(q => q.name.Equals(ss)));
            return @return;
        }
    }
}