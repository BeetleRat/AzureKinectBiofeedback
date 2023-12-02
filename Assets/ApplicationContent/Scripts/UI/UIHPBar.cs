using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Quaternion = LightBuzz.Quaternion;

/// <summary>
/// <para>Script for displaying health bar in UI. Health bar consists of health points - UIHealthPoint</para>
/// <see cref="UIHealthPoint"/>
/// </summary>
public class UIHPBar : MonoBehaviour
{
    /// <summary>
    /// <para>Action triggered when health points run out</para>
    /// </summary>
    public event UnityAction OutOfHP;

    [SerializeField] private int _hpCount;
    [SerializeField] private UIHealthPoint _uiHealthPointPrefab;
    [Range(0, 100)] [SerializeField] private float _prefabSpacing;
    private UIHealthPoint[] hpImagesArray;
    private int currentHP;
    private float hpImageWidth;
    private float hpImageHeight;

    private void Awake()
    {
        CreateNewBar();
    }

    /// <summary>
    /// <para>Set new health point count</para>
    /// </summary>
    /// <param name="count">New health point count</param>
    public void SetHPCount(int count)
    {
        if (_hpCount != count)
        {
            RemoveOldBar();
            _hpCount = count;
            CreateNewBar();
        }
        else
        {
            ResetHP();
        }
    }

    private void RemoveOldBar()
    {
        if (hpImagesArray != null)
        {
            for (int i = _hpCount - 1; i >= 0; i--)
            {
                Destroy(hpImagesArray[i].gameObject);
            }
        }
    }

    private void CreateNewBar()
    {
        hpImagesArray = new UIHealthPoint[_hpCount];
        currentHP = _hpCount;

        Transform prefabTransform = _uiHealthPointPrefab.transform;
        Rect imageRectTransform = ((RectTransform)prefabTransform).rect;
        hpImageWidth = imageRectTransform.width * prefabTransform.localScale.x;
        hpImageHeight = imageRectTransform.height * prefabTransform.localScale.y;
        for (int i = 0; i < _hpCount; i++)
        {
            var spawnedHeart = SpawnHeart(i);
            hpImagesArray[i] = spawnedHeart;
            spawnedHeart.SetActive(true);
        }
    }

    /// <summary>
    /// <para>Set all health points to the active state without changing their quantity</para>
    /// </summary>
    public void ResetHP()
    {
        ChangeHP(_hpCount);
    }

    /// <summary>
    /// <para>Add health point to current. If you want to subtract from the current state just add a negative number</para>
    /// </summary>
    /// <param name="value">Added value</param>
    public void AddHPToCurrent(int value)
    {
        int newHP = currentHP + value;
        ChangeHP(newHP);
    }

    private void ChangeHP(int newHP)
    {
        if (newHP > _hpCount || newHP < 0)
        {
            return;
        }

        bool activeState = newHP > currentHP;
        while (currentHP != newHP)
        {
            currentHP = activeState ? currentHP : currentHP - 1;
            hpImagesArray[currentHP].SetActive(activeState);
            currentHP = activeState ? currentHP + 1 : currentHP;
        }

        if (currentHP == 0)
        {
            OutOfHP?.Invoke();
        }
    }

    private UIHealthPoint SpawnHeart(int xOffset)
    {
        float fullXOffset = -xOffset * hpImageWidth - _prefabSpacing - hpImageWidth / 2;
        float fullYOffset = -hpImageHeight / 2;
        Vector3 spawnPosition = transform.position + new Vector3(fullXOffset, fullYOffset);

        UIHealthPoint spawnedUIHealthPoint =
            Instantiate(_uiHealthPointPrefab.gameObject, spawnPosition, Quaternion.Identity)
                .GetComponent<UIHealthPoint>();
        spawnedUIHealthPoint.transform.SetParent(transform);

        return spawnedUIHealthPoint;
    }
}