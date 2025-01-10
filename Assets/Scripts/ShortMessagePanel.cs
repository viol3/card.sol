using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShortMessagePanel : MonoBehaviour
{
    [SerializeField] private float _duration = 2f;
    [Space]
    [SerializeField] private RectTransform _panel;
    [SerializeField] private TMP_Text _textComponent;

    private bool _showing = false;

    public static ShortMessagePanel Instance;
    private void Awake()
    {
        Instance = this;
    }

    public void Show(string text)
    {
        if(_showing)
        {
            return;
        }
        _showing = true;
        StartCoroutine(ShowProcess(text));
    }

    IEnumerator ShowProcess(string text)
    {
        _panel.gameObject.SetActive(true);
        _textComponent.text = text;
        yield return new WaitForSeconds(_duration);
        _panel.gameObject.SetActive(false);
        _showing = false;
    }
}
