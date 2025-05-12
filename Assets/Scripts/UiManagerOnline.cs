using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;
public class UiManagerOnline : MonoBehaviourPunCallbacks 
{
    [SerializeField] GameObject _player, _quizPanel, _waitingPanel, _resultPanel, _replayText;
    GameObject _localPlayer, _enemyPlayer;
    [SerializeField] TMP_Text _player1, _player2, _score1, _score2, _player1result, _player2result, _playerProgressInQuiz, _enemyProgressInQuiz, _enemyProgressWaiting, _correctAnswer1, _correctAnswer2;
    bool _waiting = false;
    public int _currentQuestionIndex = 0;

    void Start()
    {
        _localPlayer = PhotonNetwork.Instantiate(_player.name, _player.transform.position, _player.transform.rotation);
    }

    private void Update()
    {
        if (_waiting)
        {
            CheckForGameCompletion();
        }
        if (_enemyPlayer == null)
        {
            foreach (PlayerOnlineData _player in FindObjectsByType<PlayerOnlineData>(FindObjectsSortMode.None))
            {
                if (!_player.GetComponent<PhotonView>().IsMine)
                {
                    _enemyPlayer = _player.gameObject;
                }
            }
        }
        else
        {
            _enemyProgressWaiting.text = "Enemy Progress: " + _enemyPlayer.GetComponent<PlayerOnlineData>()._currentQuestion.ToString() + "/10";
            _enemyProgressInQuiz.text = "Enemy: " + _enemyPlayer.GetComponent<PlayerOnlineData>()._currentQuestion.ToString() + "/10";
            _playerProgressInQuiz.text = "Your: " + _localPlayer.GetComponent<PlayerOnlineData>()._currentQuestion.ToString() + "/10";
            _correctAnswer1.text = "Correct Answers: " + _localPlayer.GetComponent<PlayerOnlineData>()._correctAnswer.ToString();
            _correctAnswer2.text = "Correct Answers: " + _enemyPlayer.GetComponent<PlayerOnlineData>()._correctAnswer.ToString();
            if(_localPlayer.GetComponent<PlayerOnlineData>()._replay && _enemyPlayer.GetComponent<PlayerOnlineData>()._replay)
            {
                _replayText.SetActive(false);
                _quizPanel.SetActive(true);
                _resultPanel.SetActive(false);
                FindFirstObjectByType<QuizManager>().QuestionAnsweredReset();
                _localPlayer.GetComponent<PlayerOnlineData>()._score = 0;
                _localPlayer.GetComponent<PlayerOnlineData>()._correctAnswer = 0;
                _localPlayer.GetComponent<PlayerOnlineData>()._currentQuestion = 0;
                _localPlayer.GetComponent<PlayerOnlineData>()._correctAnswer = 0;
                _localPlayer.GetComponent<PlayerOnlineData>().ScoreUpdate();
                Invoke("ReplayReset", 2f);

            }
            if(_localPlayer.GetComponent<PlayerOnlineData>()._currentQuestion == 10 && _enemyPlayer.GetComponent<PlayerOnlineData>()._currentQuestion == 10)
            {
                _waiting = false;
                _waitingPanel.SetActive(false);
                _quizPanel.SetActive(false);
                _resultPanel.SetActive(true);
                Result();
            }
        }
        if (PhotonNetwork.CurrentRoom.Players.Count < 2)
        {
            PlayerPrefs.SetInt("exit", 1);
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LeaveLobby();
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene(0);
        }
        if(_enemyPlayer != null && !_localPlayer.GetComponent<PlayerOnlineData>()._replay && !_waiting)
        {
            if(_localPlayer.GetComponent<PlayerOnlineData>()._currentQuestion < _enemyPlayer.GetComponent<PlayerOnlineData>()._currentQuestion)
            {
                _currentQuestionIndex = _enemyPlayer.GetComponent<PlayerOnlineData>()._currentQuestion;
                _localPlayer.GetComponent<PlayerOnlineData>()._currentQuestion = _currentQuestionIndex;
                _localPlayer.GetComponent<PlayerOnlineData>().ScoreUpdate();
                FindFirstObjectByType<QuizManager>().QuestionUpdate(_currentQuestionIndex);
            }
        }
    }

    public void ReplayReset()
    {
        _localPlayer.GetComponent<PlayerOnlineData>()._replay = false;
        _localPlayer.GetComponent<PlayerOnlineData>().ScoreUpdate();
    }
    public int Score()
    {
        return _localPlayer.GetComponent<PlayerOnlineData>()._score;
    }

    public void QuestionIndexChange()
    {
        _localPlayer.GetComponent<PlayerOnlineData>()._replay = false;
        _currentQuestionIndex += 1;
        _localPlayer.GetComponent<PlayerOnlineData>()._currentQuestion += 1;
        _localPlayer.GetComponent<PlayerOnlineData>().ScoreUpdate();
        if (_currentQuestionIndex == 10)
        {
            _waiting = true;
        }
    }

    public void ScoreIncrease()
    {
        if (_localPlayer.GetComponent<PlayerOnlineData>()._currentQuestion >= _enemyPlayer.GetComponent<PlayerOnlineData>()._currentQuestion)
        {
            _localPlayer.GetComponent<PlayerOnlineData>()._score += 10;
        }
        _localPlayer.GetComponent<PlayerOnlineData>()._correctAnswer += 1;
        _localPlayer.GetComponent<PlayerOnlineData>().ScoreUpdate();
    }

    public void CheckForGameCompletion()
    {
        if (_enemyPlayer.GetComponent<PlayerOnlineData>()._currentQuestion < 10)
        {
            _waiting = true;
            _waitingPanel.SetActive(true);
        }
        else
        {
            _waiting = false;
            _waitingPanel.SetActive(false);
            _resultPanel.SetActive(true);
            Result();
        }
    }

    void Result()
    {
        _player1.text = _localPlayer.GetComponent<PhotonView>().Owner.NickName;
        _player2.text = _enemyPlayer.GetComponent<PhotonView>().Owner.NickName;
        _score1.text = _localPlayer.GetComponent<PlayerOnlineData>()._score.ToString();
        _score2.text = _enemyPlayer.GetComponent<PlayerOnlineData>()._score.ToString();
        if(_localPlayer.GetComponent<PlayerOnlineData>()._score > _enemyPlayer.GetComponent<PlayerOnlineData>()._score)
        {
            _player1result.text = "Winner";
            _player2result.text = "Losser";
        }
        else
        {
            _player2result.text = "Winner";
            _player1result.text = "Losser";
        }
        _currentQuestionIndex = 0;
    }

    public void RePlay()
    {
        _localPlayer.GetComponent<PlayerOnlineData>()._currentQuestion = 0;
        _localPlayer.GetComponent<PlayerOnlineData>()._replay = true;
        _localPlayer.GetComponent<PlayerOnlineData>().ScoreUpdate();
        _replayText.SetActive(true);
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("exit", 0);
    }
}
