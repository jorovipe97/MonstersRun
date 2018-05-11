#include <ESP8266WiFi.h>

// UNE_HFC_5D50
// ADADC4F8
const char* ssid = "UPBWiFi";
const char* password = "";

const char* host = "10.8.88.55";
const uint8_t port = 8888;

#define MOVEPIN 1
int a, b, c;
// Pins guide https://tttapa.github.io/ESP8266/Chap04%20-%20Microcontroller.html
// put your main code here, to run repeatedly:
WiFiClient client;
void setup() {
  // put your setup code here, to run once:
  // Connects to wifi
  Serial.begin(9600);
  Serial.println();

  Serial.printf("Connecting to %s ", ssid);
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
    while (1) {}
  }
  // pinMode(16, INPUT_PULLDOWN_16);

  pinMode(D6,OUTPUT);
}

#define BUFTXSIZE 255
char bufTx[BUFTXSIZE];
uint8_t posTx = 0;
bool canSendMsg = false;

bool flagOnEnter = true;
void loop() {
  digitalWrite(D6,HIGH);    // Turning ON LED
  delayMicroseconds(500);  //wait
  a = analogRead(A0);        //take reading from photodiode(pin A3) :noise+signal
  digitalWrite(D6,LOW);     //turn Off LED
  delayMicroseconds(500);  //wait
  b = analogRead(A0);        // again take reading from photodiode :noise
  c=a-b;                    //taking differnce:[ (noise+signal)-(noise)] just signal

  if (c > 560)
  {
    if (flagOnEnter)
    {
      String command = String("oldman 10\n");
      client.print(command);
      Serial.println(command);
      flagOnEnter = false;
    }    
  }
  else
  {
    flagOnEnter = true;
  }
  
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
