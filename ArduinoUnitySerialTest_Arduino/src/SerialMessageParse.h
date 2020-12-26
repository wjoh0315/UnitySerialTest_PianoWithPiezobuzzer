#ifndef SERIAL_MESSAGE_PARSE_H
#define SERIAL_MESSAGE_PARSE_H

#include <Arduino.h>

namespace SerialMessageHelper
{
  enum class receivedType
  {
    Int,
    Float,
    Bool,
    Another
  };
    
  struct SerialMessageSet
  {
    static const char* receiveStart;
    static const char* receiveEnd;
    static const char* sendStart;
    static const char* sendEnd;
    static const char* endSign;
    static const char* okSign;
  
    static const char readStart,
    readEnd,
    readTypeStart,
    readTypeEnd;
  };
  
  class SerialMessageParse
  {
  private:
    template<typename T> static inline bool isCompare(T a, T b);
    template<typename T1, typename T2> static inline bool isCompare(T1 a, T2 b);
    
  public:
    static receivedType checkType(const String rawData, const char* Name);
    template<typename T> static T readFromSerial(const String rawData, const char* Name, T* buf);
  };

  template<typename T>
  bool SerialMessageParse::isCompare(T a, T b)
  {
      return true;
  }

  template<typename T1, typename T2>
  bool SerialMessageParse::isCompare(T1 a, T2 b)
  {
    return false;
  }

  template<typename T>
  T SerialMessageParse::readFromSerial(const String rawData, const char* Name, T* buf)
  {
    const int index = rawData.indexOf(Name);
    const int readStartIndex = rawData.indexOf(SerialMessageSet::readStart, index) + 1;
    const int readEndIndex = rawData.indexOf(SerialMessageSet::readEnd, index);
    String readValue = rawData.substring(readStartIndex, readEndIndex);
    readValue.trim();
  
    const int checkInt = 0;
    const float checkFloat = 0;
    const bool checkBool = false;
  
    if(readStartIndex == -1 || readEndIndex == -1 || readValue == 0)
      return *buf;
    
    if(isCompare(*buf, checkInt))
    {
      if(readValue.toInt() != 0 || readValue.equals("0"))
        *buf = readValue.toInt();
    }
    else if (isCompare(*buf, checkFloat))
    {
      if(readValue.toFloat() != 0 || readValue.equals("0") || readValue.equals("0.000000"))
        *buf =  readValue.toFloat();
    }
    else if (isCompare(*buf, checkBool))
    {
      if(readValue.equalsIgnoreCase("true"))
        *buf = true;
    }

    return *buf;
  }
}

#endif
