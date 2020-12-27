#include <Arduino.h>
#include "src/SerialMessageParse.h"
using namespace SerialMessageHelper;

#define buzzer 11

void setup() {
  // put your setup code here, to run once:
  Serial.begin(115200);
}

void loop() {
  // put your main code here, to run repeatedly:
  const String received = Serial.readStringUntil('\n');
  const int receiveIndex = received.indexOf(SerialMessageSet::receiveStart);
  const int readStartIndex = receiveIndex + String(SerialMessageSet::receiveStart).length();
  const int readEndIndex = received.indexOf(SerialMessageSet::receiveEnd);

  if(received.length() > 0 && receiveIndex != -1 && readEndIndex != -1)
  {
    const String extract = received.substring(readStartIndex, readEndIndex);

    if(extract.indexOf(SerialMessageSet::endSign) != -1)
    {
      noTone(buzzer);
    }
    else if(SerialMessageParse::checkType(extract, "frequency") == receivedType::Float && SerialMessageParse::checkType(extract, "time") == receivedType::Float)
    {
      float frequency = -1;
      SerialMessageParse::readFromSerial<float>(extract, "frequency", &frequency);

      if (frequency != -1)
      {
        tone(buzzer, frequency);
      }
    }
  }
}
