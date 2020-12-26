using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
using Serialhelper;

namespace RTTTLhelper
{
    public struct Note
    {
        public readonly string name;
        public readonly double frequency;
        public readonly float time;
        public readonly int index;

        public Note(string name_, double frequency_, float time_, int index_)
        {
            this.name = name_;
            this.frequency = frequency_;
            this.time = time_;
            this.index = index_;
        }
    }

    public class RTTTL
    {
        public readonly string name;
        public readonly int duration;
        public readonly int octave;
        public readonly int bpm;
        public readonly Note[] notes;

        public RTTTL(string name_, int duration_, int octave_, int bpm_, Note[] notes_)
        {
            this.name = name_;
            this.duration = duration_;
            this.octave = octave_;
            this.bpm = bpm_;
            this.notes = notes_;
        }
    }

    public class RTTTLmanager : MonoBehaviour
    {
        private SerialPort serialPort = null;
        private List<RTTTL> RTTTLlist = new List<RTTTL>();
        private IEnumerator play = null;
        private int startPoint = 0;

        public void Initialize(SerialPort serial)
        {
            this.serialPort = serial;
        }

        public RTTTL RTTTLload(int index)
        {
            if (index >= RTTTLlist.Count)
                return null;
            else
                return RTTTLlist[index];
        }

        public void RTTTLsave(RTTTL rtttl, int index = -1)
        {
            if (index > RTTTLlist.Count)
                return;

            if (index == -1 || index == RTTTLlist.Count)
                RTTTLlist.Add(rtttl);
            else
                RTTTLlist.Insert(index, rtttl);
        }

        public void RTTTLdelete(int index = -1)
        {
            if (index > RTTTLlist.Count)
                return;

            if (index == -1 || index == RTTTLlist.Count)
                RTTTLlist.RemoveAt(RTTTLlist.Count);
            else
                RTTTLlist.RemoveAt(index);
        }

        public RTTTL RTTTLparse(string txt)
        {
            string name = "";
            int duration = 0;
            int octave = 0;
            int bpm = 0;
            List<Note> notes = new List<Note>();

            const char split1 = ':';
            const char split2 = ',';
            const char split3 = '=';

            string rawRTTL = txt;
            string[] token1 = rawRTTL.Split(split1);

            if (token1.Length != 3)
                return null;

            for (int i = 0; i < (int) orderRTTTLparse.orderLength; i++)
            {
                switch ((orderRTTTLparse) i)
                {
                    case orderRTTTLparse.setName:
                        {
                            token1[i] = token1[i].Trim();
                            name = token1[i];
                            break;
                        }

                    case orderRTTTLparse.setting:
                        {
                            token1[i] = token1[i].Trim();
                            string[] token2 = token1[i].Split(split2);

                            if (token2.Length != 3)
                                return null;

                            for (int j = 0; j < 3; j++)
                            {
                                token2[j] = token2[j].Trim();
                                string[] token3 = token2[j].Split(split3);

                                token3[0] = token3[0].Trim();
                                token3[1] = token3[1].Trim();

                                if (token3.Length != 2)
                                    return null;

                                int convert = System.Convert.ToInt32(token3[1]);
                                if (convert == 0 && !token3[1].Equals("0"))
                                    return null;

                                if (token3[0].Equals("d", System.StringComparison.InvariantCultureIgnoreCase))
                                    duration = convert;
                                else if (token3[0].Equals("o", System.StringComparison.InvariantCultureIgnoreCase))
                                    octave = convert;
                                else if (token3[0].Equals("b", System.StringComparison.InvariantCultureIgnoreCase))
                                    bpm = convert;
                                else
                                    return null;
                            }

                            break;
                        }

                    case orderRTTTLparse.setNotes:
                        {
                            token1[i] = token1[i].Trim();
                            string[] token2 = token1[i].Split(split2);

                            for (int j = 0; j < token2.Length; j++)
                            {
                                token2[j] = token2[j].Trim();
                                string stringNOTE = token2[j];

                                string noteName = "";
                                double frequency = 30.8677;
                                float time = 60f / (float) bpm * 4f;
                                int noteOctave = octave;
                                int noteDuration = duration;

                                string[] noteSymbol = new string[13] { "p", "c", "c#", "d", "d#", "e", "f", "f#", "g", "g#", "a", "a#", "b",  };
                                const char sharpSign = '#';
                                const char dotSign = '.';
                                const double noteHzAzeo = 1.059463;

                                int dotIndex = stringNOTE.LastIndexOf(dotSign);
                                bool isDotted = dotIndex != -1;
                                float product = isDotted ? 1.5f : 1f;
                                if (isDotted)
                                    stringNOTE = stringNOTE.Remove(dotIndex);

                                for(int k = 0; k < noteSymbol.Length; k++)
                                {
                                    if (k > 1)
                                        frequency *= noteHzAzeo;

                                    int symbolIndex = stringNOTE.IndexOf(noteSymbol[k], System.StringComparison.InvariantCultureIgnoreCase);

                                    if (symbolIndex != -1 && stringNOTE.Substring(symbolIndex + noteSymbol[k].Length).IndexOf(sharpSign) == -1)
                                    {
                                        if (k == 0)
                                            frequency = 0;

                                        noteName = noteSymbol[k];

                                        int _noteDuration;
                                        int _octave;

                                        if (int.TryParse(stringNOTE.Substring(0, symbolIndex), out _noteDuration))
                                            noteDuration = _noteDuration;

                                        time /= noteDuration;

                                        if (int.TryParse(stringNOTE.Substring(symbolIndex + 1), out _octave))
                                            noteOctave = _octave;

                                        notes.Add(new Note(noteName, frequency * Mathf.Pow(2, noteOctave - 1) * product, time, k + (12 * (noteOctave - 1)) - 1));
                                        goto EXIT;
                                    }
                                }

                                return null;

                                EXIT:
                                    continue;
                            }

                            break;
                        }

                    case orderRTTTLparse.returnRTTL:
                        {
                            return new RTTTL(name, duration, octave, bpm, notes.ToArray());
                        }

                    default:
                        {
                            return null;
                        }
                }
            }

            return null;
        }

        enum orderRTTTLparse
        {
            setName = 0,
            setting,
            setNotes,
            returnRTTL,
            orderLength
        }

        public string PlayNoteMessage(Note note, bool isWriteToSerial)
        {
            string sendMessage = SerialSendHelper.MakeSendMesageData(new string[4] { "name", "frequency", "time", "keyIndex" }, new object[4] { note.name, note.frequency, note.time, note.index });
            if (this.serialPort != null && isWriteToSerial)
                this.serialPort.WriteLine(sendMessage);
            return sendMessage;
        }

        public string EndNoteMessage(bool isWriteToSerial)
        {
            string sendMessage = SerialSendHelper.MakeSendMesageSign(SerialCommunicationSet.endSign);
            if (this.serialPort != null && isWriteToSerial)
                this.serialPort.WriteLine(SerialSendHelper.MakeSendMesageSign(SerialCommunicationSet.endSign));
            return sendMessage;
        }

        public IEnumerator PlayRTTTLsong(RTTTL rtttl, bool debug, Action<int, int, bool> PlayAction = null)
        {
            if (play != null)
                throw new Exception("RTTTL song is didn't stop!");
            if (this.serialPort == null)
                throw new Exception("serial port is didn't assignment");
            if (rtttl == null)
                throw new Exception("RTTTL file is wrong");
            if (startPoint < 0 || startPoint >= rtttl.notes.Length)
                throw new Exception("RTTTL play start point parameter is negative or more than note length");

            if (this.serialPort.IsOpen)
            {
                play = PlayRTTTLsong(rtttl, debug);

                for (int i = startPoint; i < rtttl.notes.Length; i++)
                {
                    startPoint = i;

                    if (PlayAction != null)
                    {
                        if (rtttl.notes[i].index > 0 && rtttl.notes[i].index < 88)
                            PlayAction(rtttl.notes[i].index, i, i == rtttl.notes.Length - 1);
                    }
                    else
                        PlayNoteMessage(rtttl.notes[i], true);

                    if (debug)
                    {
                        Debug.Log("note name: " + rtttl.notes[i].name + "\r\n"
                            + "frequency: " + rtttl.notes[i].frequency + "\r\n"
                            + "time: " + rtttl.notes[i].time);
                        Debug.Log(PlayNoteMessage(rtttl.notes[i], false));
                        Debug.Log(this.serialPort.ReadLine());
                    }

                    yield return new WaitForSeconds(rtttl.notes[i].time);
                }

                if (PlayAction == null)
                    EndNoteMessage(true);
                if (debug)
                    Debug.Log(EndNoteMessage(false));
            }
            else
            {
                throw new Exception("serial port is not opened");
            }
        }

        public IEnumerator PlayRTTTLsongInList(int index, bool debug)
        {
            play = PlayRTTTLsong(RTTTLload(index), debug);
            return play;
        }

        public void PauseRTTTLsong()
        {
            StopCoroutine(play);
            this.serialPort.WriteLine(SerialSendHelper.MakeSendMesageSign(SerialCommunicationSet.endSign));
        }

        public void ReplayRTTTLsong()
        {
            StartCoroutine(play);
        }

        public void StopRTTTLsong()
        {
            StopCoroutine(play);
            this.serialPort.WriteLine(SerialSendHelper.MakeSendMesageSign(SerialCommunicationSet.endSign));
            play = null;
            startPoint = 0;
        }
    }
}
