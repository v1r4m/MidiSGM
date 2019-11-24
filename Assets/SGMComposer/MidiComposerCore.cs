using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;

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
            public SerializableTrack() { }
            public SerializableTrack(Track t)
            {
                if (t.midi != null)
                    this.midi = AssetDatabase.GetAssetPath(t.midi).Remove(0, 17);
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
                    midi = (this.midi == null ? null : Resources.Load<TextAsset>(this.midi)),
                    instrument = this.instrument,
                    isplaying = this.isplaying,
                    name = this.name,
                    issolo = this.issolo
                };
            }
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
        static ComposerCore instance = null;
        public static ComposerCore Instance { get { return instance ?? new ComposerCore(); } }
        private ComposerCore() { }
        public Data d = new Data();
        public PlayingData p = new PlayingData();
        public int seqNo = 0;
        

        public bool forced;

        public List<Track> GetNext()
        {
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
                @return.Add(ts.First(q => q.name.Equals(s)));
            return @return;
        }
    }
}