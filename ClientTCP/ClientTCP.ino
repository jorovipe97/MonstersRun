#include <ESP8266WiFi.h>

// UNE_HFC_5D50
// ADADC4F8
const char* ssid = "UNE_HFC_5D50";
const char* password = "ADADC4F8";

const char* host = "192.168.1.26";
const uint8_t port = 23;

// put your main code here, to run repeatedly:
WiFiClient client;
void setup() {
  // put your setup code here, to run once:
  // Connects to wifi
  Serial.begin(115200);
  Serial.println();

  Serial.printf("Connecting to %s ", ssid);
  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED)
  {
    delay(500);
    Serial.print(".");
  }
  Serial.println("WiFi connected");

  Serial.printf("\n[Connecting to %s ... ", host);
  if (client.connect(host, port))
  {
    Serial.println("connected]");
  }
  else
  {
    Serial.println("connection failed!]");
    client.stop();
  }
}

#define BUFTXSIZE 255
char bufTx[BUFTXSIZE];
uint8_t posTx = 0;
bool canSendMsg = false;
void loop() {
  
  uint8_t data;
  // Reads serial
  if (Serial.available() > 0) {
    data = Serial.read();
    if (posTx == BUFTXSIZE) bufTx[posTx - 1] = data;
    else {
      bufTx[posTx++] = data;
    }

    if (data == '\n') {
      bufTx[posTx] = 0;
      posTx = 0;
      Serial.println(bufTx);
      client.print(String("") + bufTx);
    }
  }
  
  // Serial.println("[Response:]");
  if (client.connected())
  {
    if (client.available())
    { 
      String line = client.readStringUntil('\n');
      char userId = line.charAt(0);
      // Removes user id from message
      line.remove(0, 1);

      String out = String("User ") + userId + ": " + line;
      Serial.println(out);
    }
  }
  else
  {
    client.stop();
    // Serial.println("\n[Disconnected]");
  }
}
