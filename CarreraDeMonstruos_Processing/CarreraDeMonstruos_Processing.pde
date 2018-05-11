import processing.net.*;

int port = 10003;
Server myServer;


class Player
{
  /*
  The name of the player, can exists 3 names:
  1. oldman (A todo le dice que no)
  2. dj ()
  3. bird
  */
  public String name;
  public PVector actualPos;
  private color col;
  float easing = 0.07;
  float target = 0;
  float playerWidth = 30;
  
  public Player(PVector initialPos, color c)
  {
    this.actualPos = initialPos;
    this.target = initialPos.x;
    this.col = c;
  }
    
  public void moveStep(int stepVal)
  {
    this.target = this.actualPos.x + stepVal;
    //this.actualPos.x += stepVal;
  }
  
  public boolean checkForWin(int winLine)
  {
    boolean retVal = false;
    
    if (this.actualPos.x - this.playerWidth/2> winLine)
    {
      retVal = true;
    }    
    return retVal;
  }
  
  public void update()
  {
    float targetLoc = this.target - this.actualPos.x;
    this.actualPos.x += targetLoc*this.easing;
  }
  
  public void draw()
  {
    fill(this.col);  // Use color variable 'c' as fill color
    ellipse(this.actualPos.x, this.actualPos.y, this.playerWidth, this.playerWidth);
  }
}

Player[] players;

color[] colors = new color[]{
  color(255, 200, 100),
  color(100, 255, 0),
  color(15, 100, 255)
};

PFont f;


void setup()
{
  
  f = createFont("Arial",16,true); // Arial, 16 point, anti-aliasing on
  
  players = new Player[3];
  
  /*Initializes array*/
  for (int i = 0; i < players.length; i++)
  {
    players[i] = new Player(new PVector(20, 50 + 50*i), colors[i]);
  } 
  
  players[0].name = "oldman";
  players[1].name = "dj";
  players[2].name = "bird";  
  
  size(640,480);
  // Starts a myServer on port PORT
  myServer = new Server(this, port);
  println("Server started at: " + Server.ip() + ":" + port);
}

void draw()
{
  int winLine = 400;
  background(235);
  line(winLine, 0, winLine, height);
  
  // Get the next available client
  Client thisClient = myServer.available();
  // If the client is not null, and says something, display what it said
  if (thisClient != null) {
    // Read string until new line (\n)
    String whatClientSaid = thisClient.readString();
    if (whatClientSaid != null) {
      // Prints the client message
      print(thisClient.ip() + "t: " + whatClientSaid);
      
      String msgs[] = whatClientSaid.split(" ");      
      if (msgs.length == 1)
        return;     
      
      // Exits if the message is not a number
      if (!isInteger(trim(msgs[1])))
        return;
      
      String receivedName = trim(msgs[0]);
      int receivedStep = Integer.parseInt(trim(msgs[1]));
      
      // Iterates over all the players for apply the command
      for (int i = 0; i < players.length; i++)
      {
        if (players[i].name.equals(receivedName))
        {
          players[i].moveStep(receivedStep);
        }
      }
      
      print("Name: " + receivedName + ", Step: " + receivedStep);
      print("\n\n");
    }
  }
  
  boolean someoneWin = false;
  for (int i = 0; i < players.length; i++)
  {
    someoneWin |= players[i].checkForWin(400);
  }
  
  if (someoneWin)
  {
    
    for (int i = 0; i < players.length; i++)
    {
      if (players[i].checkForWin(400))
      {
        textFont(f,16);                  // STEP 3 Specify font to be used
        fill(0);                         // STEP 4 Specify font color 
        textAlign(CENTER);
        text(players[i].name + " Wins",width/2,height/2);   // STEP 5 Display Text
      }  
      players[i].draw();
    }
  }
  else
  {
    // Draw players
    for (int i = 0; i < players.length; i++)
    {    
      players[i].update();
      players[i].draw();
    } 
  }
  
}

boolean isInteger(String str)
{
  /*
  https://stackoverflow.com/questions/5439529/determine-if-a-string-is-an-integer-in-java
  */
  return str.matches("^[+-]?\\d+$");
}
