using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTTTLhelper;

class PianoKey : MonoBehaviour
{
    public KeyInfo keyInfo;
    private IEnumerator touchEvent = null;

    private void OnCollisionEnter(Collision col)
    {
        KeyTag keyTag = col.gameObject.GetComponent<KeyTag>();

        if (touchEvent != null)
            StopCoroutine(touchEvent);

        if (keyTag != null)
        {
            Rigidbody rigid = col.gameObject.GetComponent<Rigidbody>();
            TrailRenderer trail = col.gameObject.GetComponent<TrailRenderer>();

            if (keyTag.tagInfo.keyIndex == keyInfo.index)
            {
                keyInfo.useManager.PlayNoteMessage(keyTag.tagInfo.note, true);
                if (keyTag.tagInfo.isLast)
                    StartCoroutine(isEnd(keyTag.tagInfo.note.time));
            }

            if (rigid != null)
                rigid.AddForce(new Vector3(0, -rigid.velocity.y * 120, -150));
            if (trail != null)
                trail.emitting = false;

            touchEvent = touch(50, 0.3f);
            StartCoroutine(touchEvent);
        }
    }

    private IEnumerator isEnd(float time)
    {
        yield return new WaitForSeconds(time);
        keyInfo.useManager.EndNoteMessage(true);
    }

    private IEnumerator touch(int repeat, float t)
    {
        Vector3 targetPosLocal1 = keyInfo.keyPoslocal - gameObject.transform.up * 0.1f;
        Vector3 targetPosLocal2 = keyInfo.keyPoslocal;
        MeshRenderer mesh = gameObject.GetComponent<MeshRenderer>();

        for (int i = 0; i < repeat; i++)
        {
            if (i < repeat / 2)
            {
                gameObject.transform.localPosition = Vector3.Lerp(gameObject.transform.localPosition, targetPosLocal1, t);

                if (mesh != null && keyInfo.selectedMat != null)
                    mesh.material = keyInfo.selectedMat;
                yield return null;
            }
            else
            {
                gameObject.transform.localPosition = Vector3.Lerp(gameObject.transform.localPosition, targetPosLocal2, t);

                if (mesh != null && keyInfo.defaultMat != null)
                    mesh.material = keyInfo.defaultMat;
                yield return null;
            }
        }
    }
}

struct KeyInfo
{
    public readonly double Hz;
    public readonly int index;
    public readonly bool isWhite;
    public readonly Vector3 keyPoslocal;
    public readonly Material defaultMat;
    public readonly Material selectedMat;
    public readonly RTTTLmanager useManager;

    public KeyInfo(double _Hz, int _index, bool _isWhite, Vector3 _keyPoslocal, Material _defaultMat, Material _selectedMat, RTTTLmanager _useManager)
    {
        this.Hz = _Hz;
        this.index = _index;
        this.isWhite = _isWhite;
        this.keyPoslocal = _keyPoslocal;
        this.useManager = _useManager;
        this.defaultMat = _defaultMat;
        this.selectedMat = _selectedMat;
    }
}