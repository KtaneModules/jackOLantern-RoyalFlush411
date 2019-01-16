using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;

public class jackOLanternScript : MonoBehaviour
{
    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMSelectable[] buttons;

    private KMSelectable correctButton;

    public Material[] pumpkinOptions;
    public Renderer pumpkin;
    public Light pumpkinLight;

    public Material[] eyeOptions;
    public Renderer[] eyes;
    private int eyeIndex = 0;
    public Material[] mouthOptions;
    public Renderer mouth;
    private int mouthIndex = 0;

    private bool nosePresent;
    public Renderer nose;
    private int noseIndex = 0;

    private int solvedModules = 0;
    private int tempSolvedModules = 0;

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;
    private string TwitchHelpMessage = "Type '!{0} trick' or '!{0} treat'";

    void Awake()
    {
        moduleId = moduleIdCounter++;
        foreach (KMSelectable button in buttons)
        {
            KMSelectable pressedButton = button;
            button.OnInteract += delegate () { buttonPress(pressedButton); return false; };
        }
    }

    void Update()
    {
        tempSolvedModules = solvedModules;
        solvedModules = Bomb.GetSolvedModuleNames().Count();
        if(tempSolvedModules != solvedModules)
        {
            Debug.LogFormat("[The Jack-O'-Lantern #{0}] A module has been solved. The answer is being recalculated.", moduleId);
            CalculateAnswer();
        }
    }

    void Start()
    {
        PickFeatures();
        CalculateAnswer();
    }

    void PickFeatures()
    {
        pumpkin.material = pumpkinOptions[0];
        eyeIndex = UnityEngine.Random.Range(0,3);
        foreach(Renderer eye in eyes)
        {
            eye.material = eyeOptions[eyeIndex];
        }

        mouthIndex = UnityEngine.Random.Range(0,4);
        mouth.material = mouthOptions[mouthIndex];

        noseIndex = UnityEngine.Random.Range(0,9);
        if(noseIndex == 0 || noseIndex == 1 || noseIndex == 2)
        {
            nose.gameObject.SetActive(true);
            nose.material = eyeOptions[noseIndex];
            nosePresent = true;
        }
        else
        {
            nose.gameObject.SetActive(false);
            nosePresent = false;
        }
    }

    void CalculateAnswer()
    {
        if(eyeIndex == 0 && mouthIndex == 0)
        {
            if(Bomb.GetOffIndicators().Count() > Bomb.GetOnIndicators().Count())
            {
                correctButton = buttons[0];
            }
            else
            {
                correctButton = buttons[1];
            }
        }
        else if(eyeIndex == 0 && mouthIndex == 1)
        {
            if(Bomb.GetBatteryCount() == 0)
            {
                correctButton = buttons[0];
            }
            else
            {
                correctButton = buttons[1];
            }
        }
        else if(eyeIndex == 0 && mouthIndex == 2)
        {
            if(Bomb.GetPortPlates().All(x => x.Length != 0))
            {
                correctButton = buttons[0];
            }
            else
            {
                correctButton = buttons[1];
            }
        }
        else if(eyeIndex == 0 && mouthIndex == 3)
        {
            if(Bomb.IsIndicatorOn("CAR") || Bomb.IsIndicatorOff("CAR"))
            {
                correctButton = buttons[0];
            }
            else
            {
                correctButton = buttons[1];
            }
        }

        else if(eyeIndex == 1 && mouthIndex == 0)
        {
            if(Bomb.GetPortPlates().Any(x => x.Contains(Port.Parallel.ToString()) && x.Contains(Port.Serial.ToString())))
            {
                correctButton = buttons[0];
            }
            else
            {
                correctButton = buttons[1];
            }
        }
        else if(eyeIndex == 1 && mouthIndex == 1)
        {
            if(Bomb.IsIndicatorOn("SIG") || Bomb.IsIndicatorOff("SIG"))
            {
                correctButton = buttons[0];
            }
            else
            {
                correctButton = buttons[1];
            }
        }
        else if(eyeIndex == 1 && mouthIndex == 2)
        {
            if(solvedModules % 2 == 0)
            {
                correctButton = buttons[0];
            }
            else
            {
                correctButton = buttons[1];
            }
        }
        else if(eyeIndex == 1 && mouthIndex == 3)
        {
            if(Bomb.GetOnIndicators().Count() > Bomb.GetOffIndicators().Count())
            {
                correctButton = buttons[0];
            }
            else
            {
                correctButton = buttons[1];
            }
        }

        else if(eyeIndex == 2 && mouthIndex == 0)
        {
            if(Bomb.GetBatteryCount() > Bomb.GetPortCount())
            {
                correctButton = buttons[0];
            }
            else
            {
                correctButton = buttons[1];
            }
        }
        else if(eyeIndex == 2 && mouthIndex == 1)
        {
            if((Bomb.GetPortCount(Port.StereoRCA) == 0) && (Bomb.GetPortCount(Port.RJ45) == 0))
            {
                correctButton = buttons[0];
            }
            else
            {
                correctButton = buttons[1];
            }
        }
        else if(eyeIndex == 2 && mouthIndex == 2)
        {
            if((Bomb.GetPortCount(Port.PS2) >= 1) && (Bomb.GetPortCount(Port.DVI) >= 1))
            {
                correctButton = buttons[0];
            }
            else
            {
                correctButton = buttons[1];
            }
        }
        else if(eyeIndex == 2 && mouthIndex == 3)
        {
            if(Bomb.IsIndicatorOn("TRN") || Bomb.IsIndicatorOff("TRN"))
            {
                correctButton = buttons[0];
            }
            else
            {
                correctButton = buttons[1];
            }
        }

        Debug.LogFormat("[The Jack-O'-Lantern #{0}] Eye pattern: {1}. Mouth pattern: {2}.", moduleId, eyeIndex + 1, mouthIndex +1);
        if(nosePresent && correctButton == buttons[0])
        {
            correctButton = buttons[1];
            Debug.LogFormat("[The Jack-O'-Lantern #{0}] Nose present.", moduleId);
        }
        else if(nosePresent && correctButton == buttons[1])
        {
            correctButton = buttons[0];
            Debug.LogFormat("[The Jack-O'-Lantern #{0}] Nose present.", moduleId);
        }
        else
        {
            Debug.LogFormat("[The Jack-O'-Lantern #{0}] Nose not present.", moduleId);
        }

        if(correctButton == buttons[0])
        {
            Debug.LogFormat("[The Jack-O'-Lantern #{0}] Press trick.", moduleId);
        }
        else
        {
            Debug.LogFormat("[The Jack-O'-Lantern #{0}] Press treat.", moduleId);
        }
    }

    void buttonPress(KMSelectable button)
    {
        GetComponent<KMSelectable>().AddInteractionPunch();
        if(moduleSolved)
        {
            return;
        }

        if(button == correctButton)
        {
            moduleSolved = true;
            GetComponent<KMBombModule>().HandlePass();
            if(correctButton == buttons[0])
            {
                Debug.LogFormat("[The Jack-O'-Lantern #{0}] You pressed trick. That is correct. Module disarmed.", moduleId);
            }
            else
            {
                Debug.LogFormat("[The Jack-O'-Lantern #{0}] You pressed treat. That is correct. Module disarmed.", moduleId);
            }
            pumpkinLight.enabled = false;
            Audio.PlaySoundAtTransform("solvesound", transform);
            eyeIndex += 3;
            mouthIndex += 4;
            foreach(Renderer eye in eyes)
            {
                eye.material = eyeOptions[eyeIndex];
            }
            mouth.material = mouthOptions[mouthIndex];
            if(nosePresent)
            {
                noseIndex += 3;
                nose.material = eyeOptions[noseIndex];
            }
            pumpkin.material = pumpkinOptions[1];
        }
        else
        {
            GetComponent<KMBombModule>().HandleStrike();
            if(correctButton == buttons[0])
            {
                Debug.LogFormat("[The Jack-O'-Lantern #{0}] Strike! You pressed treat. That is incorrect.", moduleId);
            }
            else
            {
                Debug.LogFormat("[The Jack-O'-Lantern #{0}] Strike! You pressed trick. That is incorrect.", moduleId);
            }
            Start();
        }
    }

    public KMSelectable[] ProcessTwitchCommand(string command)
		{
				if (command.Equals("trick", StringComparison.InvariantCultureIgnoreCase))
				{
						return new KMSelectable[] { buttons[0] };
				}
				else if (command.Equals("treat", StringComparison.InvariantCultureIgnoreCase))
				{
						return new KMSelectable[] { buttons[1] };
				}
				return null;
		}
}
