using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Serialhelper
{
    public static class SerialCommunicationSet
    {
        public static readonly string sendStart = "<Unity>",
        sendEnd = "</Unity>",
        receiveStart = "<Arduino>",
        receiveEnd = "</Arduino>",
        endSign = "end",
        okSign = "ok";

        public static readonly char readStart = ':',
        readEnd = ';',
        readTypeStart = '(',
        readTypeEnd = ')';

        public static List<Type> able = new List<Type>() { typeof(string), typeof(int), typeof(double), typeof(float), typeof(bool) };
    };

    public static class SerialSendHelper
    {
        public static string MakeSendMesageData(string[] dataName, object[] data)
        {
            bool LengthIsNotEquals = (dataName.Length - data.Length) != 0;
            bool LengthIsNegative = (dataName.Length + data.Length) != Mathf.Abs(dataName.Length + data.Length);

            if (LengthIsNotEquals || LengthIsNegative)
                return "\0";

            string sendData = "";

            for (int i = 0; i < dataName.Length; i++)
            {
                if (SerialCommunicationSet.able.IndexOf(data[i].GetType()) == -1)
                    return "\0";

                sendData += dataName[i]
                    + SerialCommunicationSet.readTypeStart + data[i].GetType().ToString() + SerialCommunicationSet.readTypeEnd
                    + SerialCommunicationSet.readStart + data[i].ToString() + SerialCommunicationSet.readEnd;
            }

            return SerialCommunicationSet.sendStart + sendData + SerialCommunicationSet.sendEnd;
        }

        public static string MakeSendMesageSign(string sign)
        {
            return SerialCommunicationSet.sendStart + sign + SerialCommunicationSet.sendEnd;
        }
    }
}
