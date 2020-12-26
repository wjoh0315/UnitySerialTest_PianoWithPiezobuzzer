#include "SerialMessageParse.h"

namespace SerialMessageHelper
{
  const char* SerialMessageSet::receiveStart = "<Unity>";
  const char* SerialMessageSet::receiveEnd = "</Unity>";
  const char* SerialMessageSet::sendStart = "<Arduino>";
  const char* SerialMessageSet::sendEnd = "</Arduino>";
  const char* SerialMessageSet::endSign = "end";
  const char* SerialMessageSet::okSign = "ok";
  const char SerialMessageSet::readStart = ':';
  const char SerialMessageSet::readEnd = ';';
  const char SerialMessageSet::readTypeStart = '(';
  const char SerialMessageSet::readTypeEnd = ')';

  receivedType SerialMessageParse::checkType(const String rawData, const char* Name)
  {
    const int index = rawData.indexOf(Name);
    const int readStartIndex = rawData.indexOf(SerialMessageSet::readTypeStart, index) + 1;
    const int readEndIndex = rawData.indexOf(SerialMessageSet::readTypeEnd, index);
    String readType = rawData.substring(readStartIndex, readEndIndex);
    readType.trim();
  
    if (readStartIndex == -1 || readEndIndex == -1 || readType.length() == 0)
      return receivedType::Another;
  
    if(readType.equals("System.Int16") || readType.equals("System.Int32") || readType.equals("System.Int64")
       || readType.equals("System.UInt16") || readType.equals("System.UInt32") || readType.equals("System.UInt64"))
      return receivedType::Int;
    else if(readType.equals("System.Single") || readType.equals("System.Double"))
      return receivedType::Float;
    else if(readType.equals("System.Boolean"))
      return receivedType::Bool;
    else
      return receivedType::Another;
  }
}
