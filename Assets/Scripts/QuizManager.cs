using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    [SerializeField] TMP_Text _question, _score, _currentQuestion, _option1Text, _option2Text, _option3Text, _option4Text;
    UiManagerOnline _uiManagerOnline;
    const int _MAXQUESTION = 10;
    int _correctOption;

    private void Start()
    {
        _uiManagerOnline = FindFirstObjectByType<UiManagerOnline>();
        NewQuestion();
    }

    public void QuestionAnsweredReset()
    {
        _currentQuestion.text = "Your: " + "0" + "/" + _MAXQUESTION.ToString();
    }

    public void OptionClicked(int _optionNumber)
    {
        if(_correctOption == _optionNumber)
        {
            _uiManagerOnline.ScoreIncrease();
            _score.text = "Score: " + _uiManagerOnline.Score().ToString();
        }
        NewQuestion();
    }

    public void NewQuestion()
    {
        if (_uiManagerOnline._currentQuestionIndex < _MAXQUESTION)
        {
            _uiManagerOnline.QuestionIndexChange();
            _currentQuestion.text = "Your: " + _uiManagerOnline._currentQuestionIndex.ToString() + "/" + _MAXQUESTION.ToString();
            QuestionUpdate();
        }
        else
        {
            _uiManagerOnline.CheckForGameCompletion();
        }
    }

    void QuestionUpdate()
    {
        int _randomValue = Random.Range(0, 10);
        int _firstValue = _randomValue - Random.Range(0, 5);
        int _secondValue = _randomValue - _firstValue;
        _question.text = _firstValue.ToString() + "+" + _secondValue.ToString();
        _correctOption = Random.Range(1,5);
        if (_correctOption == 1)
        {
            _option1Text.text = (_randomValue).ToString();
            _option2Text.text = (_randomValue - 2).ToString();
            _option3Text.text = (_randomValue + 1).ToString();
            _option4Text.text = (_randomValue - 1).ToString();
        }
        else if (_correctOption == 2)
        {
            _option1Text.text = (_randomValue + 2).ToString();
            _option2Text.text = (_randomValue).ToString();
            _option3Text.text = (_randomValue + 1).ToString();
            _option4Text.text = (_randomValue - 1).ToString();
        }
        else if (_correctOption == 3)
        {
            _option1Text.text = (_randomValue + 2).ToString();
            _option2Text.text = (_randomValue - 2).ToString();
            _option3Text.text = (_randomValue).ToString();
            _option4Text.text = (_randomValue - 1).ToString();
        }
        else if (_correctOption == 4)
        {
            _option1Text.text = (_randomValue + 2).ToString();
            _option2Text.text = (_randomValue - 2).ToString();
            _option3Text.text = (_randomValue + 1).ToString();
            _option4Text.text = (_randomValue).ToString();
        }
    }
}
