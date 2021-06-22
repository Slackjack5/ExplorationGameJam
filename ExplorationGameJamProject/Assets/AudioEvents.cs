using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEvents : MonoBehaviour
{
    private bool eventStarted = false;
    private bool ambienceSilenced = false;
    private int randomizer = 0;
    private float waitTimer = 65;
    private float cooldownTimer = 60;
    private void FixedUpdate()
    {
        //AkSoundEngine.PostEvent("Play_Footsteps", this.gameObject);
        if (eventStarted==false)
        {
            randomizer = Random.Range(0, 10);
            StartCoroutine(RandomEvent());
            eventStarted = true;
        }
    }

    IEnumerator RandomEvent()
    {
        yield return new WaitForSeconds(waitTimer);
        AkSoundEngine.PostEvent("Stop_Playlist", this.gameObject);
        waitTimer = 0;
        
        if(randomizer<=3)
        {
            AkSoundEngine.PostEvent("Fade_AmbienceDown", this.gameObject);
            ambienceSilenced = true;
        }
        else if (randomizer > 3 && randomizer <= 6)
        {
            AkSoundEngine.PostEvent("Play_Spooky", this.gameObject);
        }
        else if (randomizer > 6 && randomizer <= 8)
        {
            if(ambienceSilenced == true)
            {
                AkSoundEngine.PostEvent("Fade_Ambienceup", this.gameObject);
            }
            
        }
        else if (randomizer > 8 && randomizer <= 10)
        {
            AkSoundEngine.PostEvent("Play_RandomMusic", this.gameObject);
            AkSoundEngine.PostEvent("Fade_AmbienceDown", this.gameObject);
            waitTimer += 65;

        }
        waitTimer += 65;

        StartCoroutine(Cooldown());
    }

        IEnumerator Cooldown()
        {
        yield return new WaitForSeconds(cooldownTimer);
        eventStarted = false;
    }
    }
