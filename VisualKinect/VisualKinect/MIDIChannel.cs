using Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualKinect
{
    class MIDIChannel
    {
        static private List<Midi.Instrument> instruments = new List<Midi.Instrument>();
        static private int currentInstrumentChannel1 = 0;
        static private int currentInstrumentChannel2 = 0;
        static OutputDevice oD = null;
        static int[] pitches = { 24, 26, 28, 29, 31, 33, 35 , 37 };
        //static int[] pitches = { 24,25, 26,27, 28, 29,30, 31,32,33,34,35 };
        static int octaves = 6;
        //var t1 = new Thread(() => playPitches(pitches, oD, Channel.Channel1));

        public static void playPitch(Channel chan, int i, Instrument instrument, int velocity)
        {
            oD.SendProgramChange(chan, instrument);
            oD.SendNoteOn(chan, (Pitch)i, velocity);
        }
        public static void playPitch(Channel chan, Pitch pitch, Instrument instrument, int velocity)
        {
            oD.SendProgramChange(chan, instrument);
            oD.SendNoteOn(chan, pitch, velocity);
        }

        public static void playPitch(Channel chan, int i, int octave, Instrument instrument, int velocity)
        {
            /*
             * C2=36, D2=38, F2=41, G2=43, A2=45
             * C3=48, D3=50, F3=53, G3=55, A3=57
             */
            if (oD != null)
            {
                oD.SendProgramChange(chan, instrument);
                //sends the current note +12*octave where 0 = C2, 1=C3, 2=C4
                oD.SendNoteOn(chan, (Pitch)pitches[i] + 12 * octave, velocity);
            }

        }
        internal static void playPitch(Channel channel, int pitch, int octave, int velocity)
        {

            if (oD != null)
            {
                if (channel.Equals(Midi.Channel.Channel1))
                {
                    oD.SendProgramChange(channel, instruments[currentInstrumentChannel1]);
                }
                else if (channel.Equals(Midi.Channel.Channel2))
                {
                    oD.SendProgramChange(channel, instruments[currentInstrumentChannel2]);
                }
                
                //sends the current note +12*octave where 0 = C2, 1=C3, 2=C4
                oD.SendNoteOn(channel, (Pitch)pitches[pitch] + 12 * octave, velocity);
                //oD.SendNoteOff(channel, (Pitch)pitches[pitch] + 12 * octave, velocity);
            }
        }
        internal static void stopPitch(Channel channel, int pitch, int octave, int velocity)
        {

            if (oD != null)
            {
                if (channel.Equals(Midi.Channel.Channel1))
                {
                    oD.SendProgramChange(channel, instruments[currentInstrumentChannel1]);
                }
                else if (channel.Equals(Midi.Channel.Channel2))
                {
                    oD.SendProgramChange(channel, instruments[currentInstrumentChannel2]);
                }
                //sends the current note +12*octave where 0 = C2, 1=C3, 2=C4
                oD.SendNoteOff(channel, (Pitch)pitches[pitch] + 12 * octave, velocity);
                //oD.SendNoteOff(channel, (Pitch)pitches[pitch] + 12 * octave, velocity);
            }
        }
        public static void init()
        {
            initInstruments();
            List<OutputDevice> outputDevices = OutputDevice.InstalledDevices.ToList();
            oD = outputDevices.First();
            oD.Open();
        }
        public static void stopOutputDevice()
        {
            oD.Close();
        }

        internal static int GetPitchesLength()
        {
            return pitches.Length;
        }
        internal static int getNoOctaves()
        {
            return octaves;
        }


        private static void initInstruments()
        {
            instruments.Add(Midi.Instrument.Glockenspiel);
            instruments.Add(Midi.Instrument.SynthBass1);
            instruments.Add(Midi.Instrument.AcousticGrandPiano);
            instruments.Add(Midi.Instrument.FX2Soundtrack);
            instruments.Add(Midi.Instrument.MusicBox);
            instruments.Add(Midi.Instrument.FX3Crystal);
            instruments.Add(Midi.Instrument.Banjo);
            instruments.Add(Midi.Instrument.FX1Rain);
        }
        public static void changeInstrument(int channel)
        {
            if (channel == 0)
            {
                currentInstrumentChannel1 = (currentInstrumentChannel1 + 1) % instruments.Count;
                foreach(int i in pitches)
                {
                    for(int j=0;j<octaves;j++)
                    {
                        oD.SendNoteOff(Midi.Channel.Channel1, (Pitch)(i + 12 * j), 120);
                    }
                }
            }
            else if (channel == 1)
            {
                currentInstrumentChannel2 = (currentInstrumentChannel2 + 1) % instruments.Count;
                foreach (int i in pitches)
                {
                    for (int j = 0; j < octaves; j++)
                    {
                        oD.SendNoteOff(Midi.Channel.Channel2, (Pitch)(i + 12 * j), 120);
                    }
                }
            }
        }

        internal static int[] GetPitches()
        {
            return pitches;
        }
    }
}
