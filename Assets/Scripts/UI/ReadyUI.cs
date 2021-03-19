using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReadyUI : MonoBehaviour
{
    public static ReadyUI instance;
    [SerializeField] TextMeshProUGUI textReady;
    float timeReady;
    private void Awake()
    {
        instance = this;
    }

    public void SetReadyText(string text)
    {
        textReady.text =text;
    }
    public void SetTimeReady(float time)
    {
        timeReady = time;
    }
    private void OnEnable()
    {
        StartCoroutine(DisableAfter());
    }
    IEnumerator DisableAfter()
    {
        yield return new WaitForSeconds(timeReady);
        gameObject.SetActive(false);
    }
}
