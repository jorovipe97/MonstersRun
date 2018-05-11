/*
This code is to Remove ambience noise from sensor data.
IR LED connected to Digital pin: 6
IR diode connected to analog input:A3

by-Abhilash Patel
*/
int a, b, c;
void setup() 
{
  Serial.begin(9600);
  pinMode(D6,OUTPUT);
}

bool flagOnEnter = true;
void loop()
{
  digitalWrite(D6,HIGH);    // Turning ON LED
  delayMicroseconds(500);  //wait
  a = analogRead(A0);        //take reading from photodiode(pin A3) :noise+signal
  digitalWrite(D6,LOW);     //turn Off LED
  delayMicroseconds(500);  //wait
  b = analogRead(A0);        // again take reading from photodiode :noise
  c=a-b;                    //taking differnce:[ (noise+signal)-(noise)] just signal

  /*Serial.print(a);         //noise+signal
  Serial.print("\t");
  Serial.print(b);         //noise
  Serial.print("\t");
  Serial.println(c);       // denoised signal*/

  if (c > 100)
  {
    if (flagOnEnter)
    {
      Serial.println("Point scored");
      flagOnEnter = false;
    }    
  }
  else
  {
    flagOnEnter = true;
  }
 
}
