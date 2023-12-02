using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// <para>Level manager script that controls events occurring in the level scene</para>
/// </summary>
public class LevelManager : MonoBehaviour
{
    /// <summary>
    /// <para>Action triggered when the game starts</para>
    /// </summary>
    public event UnityAction GameStarted;

    /// <summary>
    /// <para>Action triggered when the game ends</para>
    /// </summary>
    public event UnityAction GameEnded;

    [SerializeField] private UIHPBar _hpBar;
    [SerializeField] private MultipleButtons _multipleButtons;
    [Range(1, 100)] 
    [SerializeField] private int _hpCount;
    [SerializeField] private UISmoothChangeNumberParameterBar _scoreBar;
    [SerializeField] private AbstractBiofeedbackManager _biofeedbackManager;
    [SerializeField] private AbstractSpeedChangingComponent[] _speedChangingComponents;
    [SerializeField] private bool _startGame;

    private void Start()
    {
        Cursor.visible = false;
        _hpBar.ResetHP();
        _scoreBar.SetParameterValue(0);
        _hpBar.OutOfHP += EndGame;
        _multipleButtons.ButtonsPressed += StartGame;
        if (_biofeedbackManager)
        {
            _biofeedbackManager.PulseConditionChange += ChangeGameSpeedAccordingToPulse;
        }
        else
        {
            Debug.LogError($"[{name}]: BiofeedbackManager не установлен");
        }
    }

    private void Update()
    {
        if (_startGame)
        {
            _startGame = false;
            StartGame();
        }
    }

    private void OnDestroy()
    {
        _hpBar.OutOfHP -= EndGame;
        _multipleButtons.ButtonsPressed -= StartGame;
        if (_biofeedbackManager)
        {
            _biofeedbackManager.PulseConditionChange -= ChangeGameSpeedAccordingToPulse;
        }
    }

    /// <summary>
    /// <para>Start game method</para>
    /// </summary>
    public void StartGame()
    {
        _multipleButtons.Hide();
        if (_biofeedbackManager?.GetCurrentPulseCondition() == PulseCondition.CRITICAL)
        {
            Debug.Log("Критическое состояние пульса. Запрет на старт игры");
            _multipleButtons.Show();
            return;
        }

        Debug.Log("Игра началась");
        _hpBar.SetHPCount(_hpCount);
        _scoreBar.SetParameterValue(0);
        GameStarted?.Invoke();
    }

    /// <summary>
    /// <para>End game method</para>
    /// </summary>
    public void EndGame()
    {
        Debug.Log("Игра окончена");
        GameEnded?.Invoke();
        _multipleButtons.Show();
    }

    /// <summary>
    /// <para>Method that takes away selected count of health points from player</para>
    /// </summary>
    /// <param name="count">Taken away health points count</param>
    public void DrainHealthPoint(int count)
    {
        _hpBar.AddHPToCurrent(-count);
    }

    /// <summary>
    /// <para>Method that increases a player's score by selected number of points</para>
    /// </summary>
    /// <param name="count">Points count added to the player's score</param>
    public void AddScore(int count)
    {
        _scoreBar.AddValue(count);
    }

    /// <summary>
    /// <para>Method that give selected count of health points to player</para>
    /// </summary>
    /// <param name="count">Given health points count</param>
    public void AddHealthPoint(int count)
    {
        _hpBar.AddHPToCurrent(count);
    }

    /// <summary>
    /// <para>Method that changes game speed according to <see cref="PulseCondition"/></para>
    /// </summary>
    /// <param name="pulseCondition"><see cref="PulseCondition"/> depending on which the game speed will be changed</param>
    public void ChangeGameSpeedAccordingToPulse(PulseCondition pulseCondition)
    {
        switch (pulseCondition)
        {
            case PulseCondition.CRITICAL:
                ChangeGameSpeed(Speed.STOP);
                break;
            case PulseCondition.SLOW:
                ChangeGameSpeed(Speed.FAST);
                break;
            case PulseCondition.NORMAL:
                ChangeGameSpeed(Speed.NORMAL);
                break;
            case PulseCondition.FAST:
                ChangeGameSpeed(Speed.SLOW);
                break;
        }
    }

    private void ChangeGameSpeed(Speed newSpeed)
    {
        foreach (AbstractSpeedChangingComponent speedChangingComponent in _speedChangingComponents)
        {
            speedChangingComponent.ChangeSpeed(newSpeed);
        }
    }
}