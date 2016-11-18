using System;
using System.Linq;

namespace ConsoleWorldExample
{
    class Program
    {
        //  Declaring variables; First is a World for the map
        private static World test;
        //  Track the move direction; tracking outside of main program for ease of access and to cut down on redundant code
        private static string moveString;
        //  Track whether player was able to move last time they pressed a key
        private static bool canMove = true;
        //  Track the tile the player is about to move into; similar to moveString
        private static Tile t;

        //  Keep a list of acceptable player inputs to handle input properly and with ease
        private static ConsoleKey[] acceptableInput = { ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.LeftArrow, ConsoleKey.RightArrow, ConsoleKey.Escape };
        //  Keep track of the key the player presses; similar to moveString/canMove
        private static ConsoleKey key;

        static void Main(string[] args)
        {
            //  Initialize World "test" with null path and name; will use defaults
            test = new World(null, null);
            //  Render the map; see World.RenderMap()
            test.RenderMap();
            //  Tell the player their (X, Y) coordinates
            Console.WriteLine("Player location: " + test.getPlayerIndex());
            //  Tell the player what is in the spaces around them
            printNeighboringTiles();
            //  Beginning instruction
            Console.WriteLine("You are the 'O' on the map. Go on then, get moving! Check the controls below.");
            //  Print the controls for the player
            showControls();
            //  You know...
            showCopyright();
            //  Use as a shortcut for a functional loop
            getInput:
            //  Get player input
            key = Console.ReadKey(true).Key;
            //  Clear the console when input is received in order to prepare for rendering an updated map
            Console.Clear();

            //  Check acceptable input for player's key
            if (acceptableInput.Contains(key))
                //  Set the move string based on the key pressed
                moveString = getMoveString(key);
            else
                moveString = null;

            //  Get the tile the player is trying to move to, based on input
            t = new Tile(test.getTile(moveString));
            //  Check the type of tile; if empty, player can move into it...
            if (t.ToString() == "empty")
            {
                test.movePlayer(moveString);
                canMove = true;
            }
            //  ...Otherwise, the player can't move into it
            else if (t.ToString() == "wall" || t.ToString() == "item")
            {
                canMove = false;
            }
            //  Render the (hopefully) updated map.
            test.RenderMap();
            //  Show information about the player
            showInfo();
            //  If player hits escape, close the program
            if (key != ConsoleKey.Escape)
            {
                goto getInput;
            }
        }

        //  Displays important information to the player
        static void showInfo()
        {
            //  Show the player's (X, Y) coordinates
            Console.WriteLine("Player location: " + test.getPlayerIndex());
            //  If the player was able to move with the last key press, tell them their move...
            if (canMove)
            {
                printNeighboringTiles();
                //  If the player's input was invalid, tell them, and refer to the controls
                if (acceptableInput.Contains(key))
                    Console.WriteLine("You moved " + moveString);
                else
                    Console.WriteLine("Invalid input; see controls, please.");
            }
            //  ...Otherwise, tell them something else
            else
            {
                printNeighboringTiles();
                if (acceptableInput.Contains(key))
                {
                    //  If the tile into which the player tried to move was an item, inform them; Not yet implemented, like it says
                    if (t.ToString() == "item")
                        Console.WriteLine("You don't have a backpack! Find the backpack first! (Not yet implemented, sucks to be you!)");
                    else
                        //  Tell the player they can't move into a wall
                        Console.WriteLine("You can't move " + moveString + ", try another direction! Use the map!");
                }
                else
                    Console.WriteLine("Invalid input; see controls, please.");
            }
            //  Display controls and copyright information because yeah
            showControls();
            showCopyright();
        }

        //  This is the method which displays neighboring tile information
        static void printNeighboringTiles()
        {
            //  Get the tile UP, DOWN, LEFT, and RIGHT, and tell the player what type they are
            Console.WriteLine("UP: " + new Tile(test.getTile("up")).ToString() +
                              "\t DOWN: " + new Tile(test.getTile("down")).ToString() +
                              "\tLEFT: " + new Tile(test.getTile("left")).ToString() +
                              "\tRIGHT: " + new Tile(test.getTile("right")).ToString());
        }

        //  Show the player the controls
        static void showControls() { Console.Write("\n\nCONTROLS\nLeft Arrow: Move left\nRight arrow: Move right\nUp arrow: Move up\nDown arrow: Move down\nEscape: Quit"); }

        //  Yeah
        static void showCopyright() { Console.Write("\n\nCopyright Nicholas Jackson, 2016. Don't know why you'd want to steal this, though. :)"); }

        //  Get move direction based on player key input; Fairly self-explanatory, isn't it? Yeah, yeah it is
        static string getMoveString(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.UpArrow: return "up";
                case ConsoleKey.DownArrow: return "down";
                case ConsoleKey.LeftArrow: return "left";
                case ConsoleKey.RightArrow: return "right";
            }
            //  If the player didn't input any of those, return null
            return "null";
        }
    }
}
