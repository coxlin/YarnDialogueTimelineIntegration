using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public sealed class DialogueImplementation : MonoBehaviour
{
	private int _currentOption;
    [SerializeField]
	private Dialogue _dialogue;
    [SerializeField]
	private UnityEngine.UI.Text _uiText;
    [SerializeField]
	private GameObject[] _optionButtons;
    [SerializeField]
	private TextAsset _defaultDialogue;
	private bool _scrolling;

    public static DialogueImplementation Instance { private set; get; }

	void Awake()
	{
        Instance = this;
		_dialogue = GetComponent<Dialogue>();

		foreach (var gameObject in _optionButtons)
		{
			gameObject.SetActive(false);
		}

		if (_defaultDialogue != null)
		{
			textToRun = _defaultDialogue.text;
		}
	}

    public void Run(string text)
    {
        textToRun = text;
        _dialogue.Run(textToRun);
    }

	public string Parse(string characterName, string line)
	{
		return line;
	}

	public IEnumerator Say(string characterName, string text)
	{
		_uiText.text = "";
		string textToScroll = text;
		//CharacterData characterData = Global.constants.GetCharacterData(characterName);
		//Global.textbox.Say(characterData, text);
		const float timePerChar = .05f;
		float accumTime = 0f;
		int c = 0;
		while (!InputNext() && c < textToScroll.Length)
		{
			yield return null;
			accumTime += Time.deltaTime;
			while (accumTime > timePerChar)
			{
				accumTime -= timePerChar;
				if (c < textToScroll.Length)
					_uiText.text += textToScroll[c];
				c++;
			}
		}
		_uiText.text = textToScroll;

		while (InputNext()) yield return null;

		while (!InputNext()) yield return null;
	}

	public bool InputNext()
	{
		return Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0);
	}

	public IEnumerator EndText()
	{
		_uiText.text = "";
        DialogueManager.Instance.ResumeTimeline();
        yield break;
	}

    public void SelectOption(GameObject optionButton)
    {
        _currentOption = Array.IndexOf(_optionButtons, optionButton);
    }

	public IEnumerator RunOptions(List<Dialogue.Option> options)
	{
		_dialogue.SetCurrentOption(0);

		yield return null;

		int index = 0;
		foreach (var option in options)
		{
			_optionButtons[index].SetActive(true);
			_optionButtons[index].GetComponentInChildren<UnityEngine.UI.Text>().text = option.text;
			index++;
		}

		_currentOption = -1;
		do { yield return null; } while (_currentOption == -1);
		foreach (var gameObject in _optionButtons)
		{
			gameObject.SetActive(false);
		}

		_dialogue.SetCurrentOption(_currentOption);
	}

	public IEnumerator RunCommand(string line)
	{
		string[] tokens = line.Split(' ');
		if (tokens.Length > 0)
		{
			if (IsString(tokens[0], "wait"))
			{
				float timeToWait = (float)Convert.ToDouble(tokens[1]);
				yield return new WaitForSeconds(timeToWait);
			}
			else if (IsString(tokens[0], "tell"))
			{
				GameObject gameObject = GameObject.Find(tokens[1]);
				if (gameObject != null)
				{
					int methodToken = 2;
					if (IsString(tokens[2], "to"))
						methodToken = 3;
					
					string sendData = "";
					if (tokens.Length > methodToken+1)
						sendData = tokens[methodToken+1];
					
					gameObject.SendMessage(tokens[3], sendData, SendMessageOptions.DontRequireReceiver);
				}
			}

		}
		yield break;
	}

	bool ReadBool(string token)
	{
		return IsString(token, "on") || IsString(token, "1");
	}

	bool IsString(string strA, string strB)
	{
		return string.Compare(strA, strB, System.StringComparison.InvariantCultureIgnoreCase) == 0;
	}

	public void SetInteger(string varName, int varValue)
	{
		Continuity.instance.SetVar(varName, varValue);
	}

	public int GetInteger(string varName)
	{
		return Continuity.instance.GetVar(varName);
	}

	public void AddToInteger(string varName, int addAmount)
	{
		Continuity.instance.SetVar(varName, Continuity.instance.GetVar(varName) + addAmount);
	}

	public void SetString(string varName, string varValue)
	{
		// TODO: write this!
	}

	// called when node not found
	public void NodeFail()
	{

	}

	public bool IsPaused()
	{
		return false;
	}

	public bool EvaluateIfChunk(string chunk, ref bool result)
	{
		return false;
	}

	string textToRun = "";
	void OnGUI()
	{
		if (!_dialogue.running)
		{
			textToRun = GUI.TextArea(new Rect(0, 0, 600, 350), textToRun);
			if (GUI.Button(new Rect(610, 0, 100, 50), "Test Run"))
			{
				_dialogue.Run(textToRun);
			}
			if (GUI.Button(new Rect(610, 60, 100, 50), "Clear"))
			{
				textToRun = "";
			}
		}
	}
}
