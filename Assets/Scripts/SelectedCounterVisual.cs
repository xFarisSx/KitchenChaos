using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] private BaseCounter baseCounter;
    [SerializeField] private GameObject[] visualGameObjectArray;

    private void Start()
    {
        Player.Instance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
    }

    private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e)
    {
        if (e.selectedCounter == baseCounter)
        {
            Show();
          
        } else
        {
            Hide();
        }
    }

    private void Show()
    {
        for (int i = 0; i < visualGameObjectArray.Length; i++)
        {

        visualGameObjectArray[i].SetActive(true);
        }
    }
    private void Hide()
    {
        for (int i = 0; i < visualGameObjectArray.Length; i++)
        {

            visualGameObjectArray[i].SetActive(false);
        }
    }
}
