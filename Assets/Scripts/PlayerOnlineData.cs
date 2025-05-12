using Photon.Pun;
using UnityEngine;

public class PlayerOnlineData : MonoBehaviourPunCallbacks
{
    public int _score, _currentQuestion, _correctAnswer;
    public bool _replay = false;

    public void ScoreUpdate()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("NewScore", RpcTarget.AllBuffered, _score);
            photonView.RPC("CurrentQuestion", RpcTarget.AllBuffered, _currentQuestion);
            photonView.RPC("CorrectAnswer", RpcTarget.AllBuffered, _correctAnswer);
            photonView.RPC("Replay", RpcTarget.AllBuffered, _replay);
        }
    }

    [PunRPC]
    void NewScore(int _newScore)
    {
        _score = _newScore;
    }

    [PunRPC]
    void CurrentQuestion(int _newCurrentQuestionValue)
    {

        _currentQuestion = _newCurrentQuestionValue;
    }

    [PunRPC]
    void CorrectAnswer(int _correctAnswerValue)
    {
        _correctAnswer = _correctAnswerValue;
    }

    [PunRPC]
    void Replay(bool _yesNo)
    {
        _replay = _yesNo;
    }
}
