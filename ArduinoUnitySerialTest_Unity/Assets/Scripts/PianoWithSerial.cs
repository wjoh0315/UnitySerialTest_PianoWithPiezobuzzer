using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
using RTTTLhelper;

public class PianoWithSerial : MonoBehaviour
{
    // 컴포넌트 세팅

    [Header("SerialSetting")]
    public string serialPortName;
    public int baudRate;
    public Parity parity;
    public int dataBits;
    public StopBits stopBits;

    [Header("RTTTLmanager")]
    public TextAsset RTTTLFile;

    [Header("PianoSetting")]
    public GameObject piano;
    public int keyLayer;

    [Header("KeySetting")]
    public GameObject white_key;
    public GameObject black_key;
    public Material selectedMat;
    public LayerMask clickLayer;
    public float pianoKeyRange;
    public float touchDepth;

    [Header("EventObj")]
    public GameObject eventObj;
    public Vector3 instantPosLocal;

    private SerialPort serialPort = null;
    private RTTTLmanager rtttlmanager = null;
    private RTTTL rtttl = null;
    private GameObject[] keys = null;

    private bool isStart = false;

    // Start is called before the first frame update
    private void Start()
    {
        try
        {
            serialPort = new SerialPort(serialPortName, baudRate, parity, dataBits, stopBits);
            serialPort.ReadTimeout = 50;
            serialPort.Open();
        }
        catch (Exception exception)
        {
            Debug.Log("serial setting is wrong\r\n" + exception);
        }

        GameObject rtttlobj = new GameObject();
        rtttlobj.name = "RTTTLmanager";
        rtttlmanager = rtttlobj.AddComponent<RTTTLmanager>();
        rtttlmanager.Initialize(serialPort);
        rtttl = rtttlmanager.RTTTLparse(RTTTLFile.text);

        if (white_key.GetComponent<PianoKey>() == null || black_key.GetComponent<PianoKey>() == null || white_key.GetComponent<MeshRenderer>() == null || black_key.GetComponent<MeshRenderer>() == null)
            throw new Exception("key hasn't \"PianoKey\" class or mesh renderer");

        keys = SettingPianoKeys(piano, white_key, black_key, piano.transform.position, piano.transform.right, keyLayer, pianoKeyRange, piano.transform.rotation);
        Debug.Log("piano key length: " + keys.Length);
    }

    // Update is called once per frame
    private void Update()
    {
        if (!isStart)
            StartCoroutine(rtttlmanager.PlayRTTTLsong(rtttl, false, (int i, int noteCount, bool isLast) =>
            {
                if (i < keys.Length)
                {
                    GameObject clone = Instantiate(eventObj, keys[i].transform.position + instantPosLocal, Quaternion.identity);
                    clone.SetActive(true);

                    KeyTag keyTag = clone.GetComponent<KeyTag>();
                    TrailRenderer trail = clone.GetComponent<TrailRenderer>();

                    if (keyTag == null)
                        keyTag = clone.AddComponent<KeyTag>();
                    /*if (trail != null)
                    {
                        float H = 360 * (float) i / 88, S = 100, V = 100;
                        trail.startColor = Color.HSVToRGB(H, S, V);
                    }*/

                    keyTag.tagInfo = new TagInfo(i, isLast, rtttl.notes[noteCount]);
                    StartCoroutine(keyTag.Destroydelay());
                }
            }));

        isStart = true;
    }

    private void OnApplicationQuit()
    {
        serialPort.Close();
    }

    private GameObject GetMouseClickedKey(Camera camera, Vector3 screenPoint, LayerMask layer, float range = Mathf.Infinity)
    {
        GameObject clickedObj = null;
        Ray mousePointToRay = camera.ScreenPointToRay(screenPoint);
        RaycastHit hit;

        if (Physics.Raycast(mousePointToRay, out hit, range, layer))
            clickedObj = hit.transform.gameObject;

        return clickedObj;
    }

    private GameObject[] SettingPianoKeys(GameObject _piano, GameObject _key_white, GameObject _key_black, Vector3 set, Vector3 dir, int keyLayer, float range, Quaternion rot)
    {
        int patternIndex = 0;
        int count_white = 0;
        int count_black = 0;
        int check = 0;
        short order = 1;

        double termHz = 27.5;
        double keyHzAzeo = 1.059463;

        const int Length = 88;
        const int whiteLength = 52;
        int[] pattern = new int[3] { 1, 2, 3 };
        GameObject[] objs = new GameObject[Length]; 

        for (int i = 0; i < Length; i++)
        {
            if (order > 0)
            {
                Vector3 posAssignment = dir * (-(range / 2) + (range / whiteLength) * count_white);
                GameObject obj = Instantiate(_key_white, set + posAssignment, rot);

                PianoKey targetKey = obj.GetComponent<PianoKey>();
                targetKey.keyInfo = new KeyInfo(termHz, i, true, obj.transform.localPosition, white_key.GetComponent<MeshRenderer>().material, selectedMat, rtttlmanager);

                obj.layer = keyLayer;
                obj.SetActive(true);
                obj.transform.SetParent(_piano.transform);
                objs[i] = obj;

                count_white++;

                order--;
            }
            else
            {
                if (check == pattern[patternIndex])
                {
                    if (patternIndex == 1)
                        patternIndex = 2;
                    else if (patternIndex == 0 || patternIndex == 2)
                        patternIndex = 1;

                    check = 0;
                    count_black += 1;
                    order++;
                }

                Vector3 posAssignment = dir * (-(range / 2) + (range / whiteLength) / 2 + (range / (whiteLength)) * count_black);
                GameObject obj = Instantiate(_key_black, set + posAssignment + _piano.transform.up * _key_white.transform.lossyScale.y / 2 + _piano.transform.forward * ((_key_white.transform.lossyScale.z - _key_black.transform.lossyScale.z) / 2), rot);

                PianoKey targetKey = obj.GetComponent<PianoKey>();
                targetKey.keyInfo = new KeyInfo(termHz, i, false, obj.transform.localPosition, black_key.GetComponent<MeshRenderer>().material, selectedMat, rtttlmanager);

                obj.layer = keyLayer;
                obj.SetActive(true);
                obj.transform.SetParent(_piano.transform);
                objs[i] = obj;

                count_black++;
                check++;
                order++;
            }

            termHz *= keyHzAzeo;

            if (i == Length - 2)
                order++;
        }

        return objs;
    }
}
