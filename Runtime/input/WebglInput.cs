using UnityEngine.UI;


public class WebglInput : WebglInputNode
{
    int _inputID;
    protected override void Awake()
    {
        base.Awake();
        _inputID = _id;

        //Debug.Log(_inputID);
    }
         
    InputField _input;
    int _characterLimit; 
    // Start is called before the first frame update
    void Start()
    {
        _input = GetComponent<InputField>();
        _characterLimit = _input.characterLimit > 0 ? _input.characterLimit : 65535;
    }

    bool hasKeyBoard = false;
    // Update is called once per frame
    void Update()
    {
        if (_input == null)
        {
            return;
        }

        if (_input.isFocused)
        {
            if (!hasKeyBoard)
            {
                WebGLInputPlugin.WebGLInputBeginEditing(_inputID,_input.text, _characterLimit, _input.multiLine, true);
            }

            hasKeyBoard = true;
        }
        else
        {
            hasKeyBoard = false;
        }
    }

    protected override void OnText(int inputid, int eventType, string text)
    {
        if (_input.isFocused)
        {
            //Debug.Log("OnText " + inputid + "  " + _inputID + "   " + text);
            _input.text = text;

            if (eventType == 3)
            {
                _input.DeactivateInputField();
            }
        }
    }
}
