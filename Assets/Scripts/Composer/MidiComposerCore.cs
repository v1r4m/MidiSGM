using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class MidiComposerCore
{
    class Data
    {
        public List<Tracks> globalTracks = new List<Tracks>();
        public List<Tracks> localTracks = new List<Tracks>();

        public List<TracksSet> tracksets = new List<TracksSet>();
        public Dictionary<string, List<Tracks>> presets = new Dictionary<string, List<Tracks>>();
        public Dictionary<string, Dictionary<string, List<Tracks>>> flows = new Dictionary<string, Dictionary<string, List<Tracks>>>();

            
    }
    class PlayingData
    {
        public enum Mode
        {
            preset, flow,
            forced_p, forced_flow
        }
        public int preset;
    }
    public void Init()
    {

    }
}