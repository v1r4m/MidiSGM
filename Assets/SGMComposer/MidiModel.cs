using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using Midi;

public class MidiModel : MonoBehaviour
{
    OutputDevice outputDevice;
    // Start is called before the first frame update

    public enum Nabi {C4,D4,E4,F4,G4};
    int[] Sheet = {7,4,4,100, 5,2,2,100, 0,2,4, 5,7,7, 7,100};

    void Start()
    {
        outputDevice = OutputDevice.InstalledDevices[0];
        if (outputDevice.IsOpen)
        {
            outputDevice.Close();
        }
        if (!outputDevice.IsOpen)
        {
            outputDevice.Open();
        }


        Debug.Log("Looking for output device...");


        Nabiya(0);
        Nabiya(-3);
//        outputDevice.SendNoteOn(Channel.Channel1, Pitch.C4, 80);
        //       outputDevice.SendPitchBend(Channel.Channel1, 7000);
    }

    public void Nabiya(int adjust)
    {
        Pitch pitch = Pitch.C4;
        for (int i=0;i<16; i++)
        {
            if(Sheet[i]!=100)
            {
                outputDevice.SendNoteOn(Channel.Channel1, pitch + Sheet[i] + adjust, 80);
            }
            Thread.Sleep(500);
        }

  /*      outputDevice.SendNoteOn(Channel.Channel1, Pitch.C4, 80);
        Thread.Sleep(500);*/
        Debug.Log(60);
    }

	
	void OnDestroy()
	{
		if(outputDevice != null && outputDevice.IsOpen)
		{
			outputDevice.Close();
		}
	}
	
	void OnDisable()
	{
		if(outputDevice != null && outputDevice.IsOpen)
		{
			outputDevice.Close();
		}
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
