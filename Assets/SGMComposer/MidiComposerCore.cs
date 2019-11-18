using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

static public class MidiComposerCore
{
    public class Tracks
    {
        public TextAsset midi; // hack: serialize with AssetDatabase.GetAssetPath
        public MidiInstrument instrument;
        public bool isplaying;
        public string hash;
        public bool issolo;
    }
    public class TracksSet
    {
        public string name;

        public Dictionary<string, List<Tracks>> presets;
        public Dictionary<string, List<string>> flows;
    }


    public class Data
    {
        public List<Tracks> globalTracks = new List<Tracks>();

        public List<TracksSet> tracksets = new List<TracksSet>();

        #region serializer

        [Serializable]
        class SerializedData
        {
            public List<Tracks> globalTracks = new List<Tracks>();
            public List<Tracks> localTracks = new List<Tracks>();

            public List<STracksSet> tracksets = new List<STracksSet>();
        }

        [Serializable]
        class STracksSet
        {
            public string name;

            public Dictionary<string, List<int>> presets = new Dictionary<string, List<int>>();
            public Dictionary<string, int> flows = new Dictionary<string, int>();
        }
        /*
        public string Serialize()
        {
            SerializedData dt = new SerializedData();
            dt.globalTracks = globalTracks;
            dt.localTracks = localTracks;
            foreach (var a in tracksets)
            {
                dt.tracksets
            }
        }*/
        #endregion  
    }
    public class PlayingData
    {
        public enum Mode
        {
            preset, flow,
            forced_p, forced_flow
        }
        public Mode mode;
        public int trackset = 0;
        public string preset_flow_name;
    }
    public class ComposerCore
    {
        public Data d;
        public PlayingData p = new PlayingData();
        public int seqNo = 0;
        List<Tracks> forcedTrack;
        List<Tracks> GetNext()
        {
            TracksSet ts = d.tracksets[p.trackset];
            switch (p.mode)
            {
                case PlayingData.Mode.flow:
                    var flow = ts.flows[p.preset_flow_name];
                    return ts.presets[flow[seqNo++ % flow.Count]];
                case PlayingData.Mode.preset:
                    var preset = ts.presets[p.preset_flow_name];
                    return preset;
                case PlayingData.Mode.forced_flow:
                    p.mode = PlayingData.Mode.flow;
                    return forcedTrack;
                case PlayingData.Mode.forced_p:
                    p.mode = PlayingData.Mode.flow;
                    return forcedTrack;
                default: throw new ArgumentException();
            }
        }
    }
}