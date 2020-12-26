using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTTTLhelper;

class KeyTag : MonoBehaviour
{
    public TagInfo tagInfo;

    public IEnumerator Destroydelay()
    {
        yield return new WaitForSeconds(tagInfo.delay);
        Destroy(gameObject);
    }
}

struct TagInfo
{
    public readonly int keyIndex;
    public readonly int delay;
    public readonly bool isLast;
    public Note note;

    public TagInfo(int _keyIndex, bool _isLast, Note _note)
    {
        this.keyIndex = _keyIndex;
        this.delay = 2;
        this.isLast = _isLast;
        this.note = _note;
    }
}
