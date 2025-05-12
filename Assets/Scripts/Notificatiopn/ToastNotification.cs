using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ToastNotification : MonoBehaviour
{
    public static ToastNotification Instance { get; private set; }

    public Animator anim;
    private Queue<string> popupQueue;
    private Coroutine queueChecker;

    public TMP_Text text;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        popupQueue = new Queue<string>();
    }


    public void SendMessages(string text)
    {
        popupQueue.Enqueue(text);
        if(queueChecker == null)
        {
            queueChecker = StartCoroutine(CheckQueue());
        }
    }

    public void ShowMessage(string message)
    {
        text.text = message;
        anim.Play("MessageAnimation");
    }

    private IEnumerator CheckQueue()
    {
        do
        {
            ShowMessage(popupQueue.Dequeue());
            do
            {
                yield return null;
            } while (!anim.GetCurrentAnimatorStateInfo(0).IsTag("Idle"));
        } while (popupQueue.Count > 0);

        queueChecker = null;
    }

   
}
