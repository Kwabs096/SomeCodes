// This code was written by Kwabs
// Implementation of dialogue system with gradual text reveal

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class DialogSystem : MonoBehaviour
{
    [SerializeField] private Animator _dialogAnim;
    private readonly int _openBool = Animator.StringToHash("Open");
    [SerializeField] private InputActionReference _dialogPassInput;

    [SerializeField] private string[] _englishTexts, _portugueseTexts;
    private List<string[]> _allTexts = new List<string[]>();
    [SerializeField] private TextMeshProUGUI _dialogText;
    private string[] _currentTextLanguage;

    [SerializeField] string[] _dictionaryNames;
    Dictionary<string, int> _namesDictionary = new Dictionary<string, int>();
    [SerializeField] string[] _englishNames, _portugueseNames;
    private List<string[]> _allNames = new List<string[]>();
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] Sprite[] _charactersSprites;
    [SerializeField] Image _characterIcon;
    private string[] _currentNameLanguage;

    [SerializeField] private float _delayLetter = 0.05f;
    private bool _nowPrinting = false, _continueMessage = false;
    private int _usedLanguage = 0;
    void Start()
    {
        _allTexts.Add(_englishTexts);
        _allTexts.Add(_portugueseTexts); // able to add other languages
        _allNames.Add(_englishNames);
        _allNames.Add(_portugueseNames);
        _usedLanguage = PlayerPrefs.GetInt("language", _usedLanguage);
        _currentTextLanguage = _allTexts[_usedLanguage];
        _currentNameLanguage = _allNames[_usedLanguage];

        for (int i = 0; i < _dictionaryNames.Length; i++)
        {
            _namesDictionary.Add(_dictionaryNames[i].ToLower(), i);
        }

        _dialogPassInput.action.started += PassDialog;
    }
    public void PrintDialog(List<string> dialogOrder) { if (!_continueMessage) StartCoroutine(ContinuousDialog(DialogList(dialogOrder))); }
    private List<Tuple<string, int>> DialogList(List<string> stringList)
    {
        List<Tuple<string, int>> dialogOrder = new List<Tuple<string, int>>();
        for(int i = 0; i < stringList.Count; i += 2)
        {
            dialogOrder.Add(new Tuple<string, int>(stringList[i].ToLower(), int.Parse(stringList[i + 1])));
        }
        return dialogOrder;
    }
    private IEnumerator ShowText(string name, int indexText)
    {
        if (_namesDictionary.ContainsKey(name))
        {
            int nameId = _namesDictionary[name];
            _nameText.text = _currentNameLanguage[nameId];
            _characterIcon.sprite = _charactersSprites[nameId];
        }
        else
        {
            Debug.Log("Incorrect name");
        }
        string currentText = "";
        string fullText = _allTexts[_usedLanguage][indexText];
        for (int i = 0; i <= fullText.Length; i++)
        {
            currentText = fullText.Substring(0, i);
            _dialogText.text = currentText;
            float currentTime = Time.time;
            while (Time.time - currentTime < _delayLetter && _nowPrinting)
            {
                yield return null;
            }
            if (!_nowPrinting)
            {
                _dialogText.text = fullText;
                break;
            }
        }
        if (_nowPrinting) _nowPrinting = false;
    }
    private IEnumerator ContinuousDialog(List<Tuple<string, int>> dialogOrder)
    {
        SetDialogBools(true);
        for (int i = 0; i < dialogOrder.Count; i++)
        {
            Tuple<string, int> currentMessage = dialogOrder[i];
            StartCoroutine(ShowText(currentMessage.Item1, currentMessage.Item2));
            yield return new WaitUntil(() => !_nowPrinting);
            yield return new WaitUntil(() => _nowPrinting);
        }
        SetDialogBools(false);
    }
    private void PassDialog(InputAction.CallbackContext obj) { if (_continueMessage) _nowPrinting = !_nowPrinting; }
    private void SetDialogBools(bool currentBool) {_continueMessage = currentBool; _nowPrinting = currentBool; _dialogAnim.SetBool(_openBool, currentBool); }
    public void ChangeLanguage(int indexLanguage)
    {
        _usedLanguage = indexLanguage;
        _currentTextLanguage = _allTexts[_usedLanguage];
        _currentNameLanguage = _allNames[_usedLanguage];
        PlayerPrefs.SetInt("language", _usedLanguage);
    }
}
