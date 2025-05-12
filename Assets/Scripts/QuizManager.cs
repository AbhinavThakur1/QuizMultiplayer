using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    [SerializeField] TMP_Text _question, _score, _currentQuestion, _option1Text, _option2Text, _option3Text, _option4Text;
    UiManagerOnline _uiManagerOnline;
    const int _MAXQUESTION = 10;
    int _correctOption;
    [SerializeField] List<Question> _questions;

    [System.Serializable]
    public class Question
    {
        public string questionText;
        public string[] options;
        public int correctAnswerIndex;
    }

    private void Start()
    {
        _uiManagerOnline = FindFirstObjectByType<UiManagerOnline>();
        QuestionUpdate(_uiManagerOnline._currentQuestionIndex);
    }

    public void QuestionAnsweredReset()
    {
        _currentQuestion.text = "Your: " + "0" + "/" + _MAXQUESTION.ToString();
        _score.text = "Score: " + "0";
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
            QuestionUpdate(_uiManagerOnline._currentQuestionIndex);
        }
        else
        {
            _uiManagerOnline.CheckForGameCompletion();
        }
    }

    public void QuestionUpdate(int _index)
    {
        _question.text = _questions[_index].questionText;
        _correctOption = _questions[_index].correctAnswerIndex;
        _option1Text.text = _questions[_index].options[0];
        _option2Text.text = _questions[_index].options[1];
        _option3Text.text = _questions[_index].options[2];
        _option4Text.text = _questions[_index].options[3];
    }
}
