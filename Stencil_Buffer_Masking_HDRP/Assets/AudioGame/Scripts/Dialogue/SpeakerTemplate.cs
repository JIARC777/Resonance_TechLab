using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeakerTemplate : MonoBehaviour
{
    public TextMeshProUGUI name;
    public Image profilePicture;
    public Image speakingIndicator;

    public void toggleIndicator()
    {
        speakingIndicator.enabled = !speakingIndicator.enabled;
    }
}
