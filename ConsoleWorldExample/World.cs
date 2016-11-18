using System;
using System.IO;
using System.Collections.Generic;

namespace ConsoleWorldExample
{
    //  Define Tile class; keeps track of what type of tile is at a given space based on the character on the map
    class Tile
    {
        private char c;
        public Tile(char c)
        {
            this.c = c;
        }
        //  Override ToString() method so Tile.ToString() will return the type of tile instead of garbleflobbish
        public override string ToString()
        {
            switch (c)
            {
                case ' ': return "empty";
                case 'X': return "wall";
                case '#': return "item";
                default: return "invalid";
            }
        }
    }

    //  Defines the World class; this is the meat of the program
    class World
    {
        //  Define default path for map files
        public string worldMapPath = "/maps";
        //  Define default filename for the map
        public string worldMapName = "default.txt";
        //  Create a list to aid in converting the text file into a 2D character array
        private List<char[]> mapList = new List<char[]>();
        //  2D character array for the final map shown to the player
        private char[,] map;

        //  A 2D player array that keeps track of the player's (Y, X) position; (Y, X) because the first index
        //  of the 2D array goes down and the second index goes right. Eg a 2D array int[1,3] has two rows and 3 columns.
        public int[,] playerIndex;

        public World(string mapPath, string mapName)
        {
            //  If map path passed in is null, use the default path; otherwise, use specified path
            if (mapPath != null)
                worldMapPath = mapPath;
            //  If map name passed in is null, use the default map name; otherwise, use specified name
            if (mapName != null)
                worldMapName = mapName + ".txt";

            //  Initialize player index 2D array with one row and 2 columns for (Y, X) coordinates
            playerIndex = new int[1, 2];

            //  Try/Catch will take care of any errors within
            try
            {
                //  StreamReader to read the text file which contains the raw map information
                using (StreamReader sr = new StreamReader(Directory.GetCurrentDirectory().ToString() + worldMapPath + "\\" + worldMapName))
                {
                    //  Keep track of X length, for use in map initialization
                    int xLength = -1;
                    //  Read and store the first line
                    string line = sr.ReadLine();
                    //  Do this until there are no more lines in the text file to read
                    while (line != null)
                    {
                        //  Add each line of the text file to the mapList as a character array
                        mapList.Add(line.ToCharArray());
                        //  This lets us make sure our map[,] array has an appropriate width later
                        if (line.Length > xLength)
                            xLength = line.Length;
                        //  Read the next line in the raw map data, of course
                        line = sr.ReadLine();
                    }

                    //  Initialize the 2D array which will hold the processed map data. The Y length is equal to the number of lines
                    //  in the text file for the raw map. The X length is the max width of the map in the raw text file.
                    map = new char[mapList.Count, xLength];

                    //  BEGIN MAP PROCESSING OMG OMG OMG
                    for (int x = 0; x < map.GetLength(0); x++)
                    {
                        for (int y = 0; y < map.GetLength(1); y++)
                        {
                            //  Keep track of these specific errors so we know where we went wrong.
                            try
                            {
                                switch (mapList[x][y])
                                {
                                    //  If character is O, replace it with blank space; X, replace it with... X; #, replace it with... #; S, replace it with O
                                    //  This is kind of a silly way to do it, but this lets the map creator better keep track of empty spaces and the player
                                    //  start position.
                                    case 'O':
                                        map[x, y] = ' ';
                                        break;
                                    case 'X':
                                        map[x, y] = 'X';
                                        break;
                                    case '#':
                                        map[x, y] = '#';
                                        break;
                                    //  This is the player start position, so we're going to set the player index accordingly
                                    case 'S':
                                        map[x, y] = 'O';
                                        playerIndex[0, 0] = x;
                                        playerIndex[0, 1] = y;
                                        break;
                                }
                            }
                            catch
                            {
                                Console.WriteLine("Error occurred at X = " + x + ", Y = " + y);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
        }

        //  This handles displaying the map to the player
        public void RenderMap()
        {
            //  2D arrays go top to bottom, left to right, so this nested for loop is reversed
            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    //  Write the character from the 2D array which holds the map
                    //  Get the map character in the reverse because of the inverse nature of 2D array alignment (top->bottom, left->right)
                    Console.Write(map[y, x]);
                }
                //  When we finish a horizontal line in the map, we make a new line so we draw the map correctly
                Console.Write("\n");
            }
        }

        //  Get the player's corrected position; converts from (Y, X) to the more typical (X, Y)
        public string getPlayerIndex()
        {
            return "(" + (playerIndex[0, 1] + 1) + ", " + (playerIndex[0, 0] + 1) + ")";
        }

        //  Get the character of the tile the player is trying to walk onto
        public char getTile(string direction)
        {
            int x = playerIndex[0, 0], y = playerIndex[0, 1];
            switch (direction)
            {
                case "up":
                    return map[x - 1, y];
                case "down":
                    return map[x + 1, y];
                case "left":
                    return map[x, y - 1];
                case "right":
                    return map[x, y + 1];
            }
            //  Need a default return, just using 0 for this as it is unused
            return '0';
        }

        //  Handle player movement
        internal void movePlayer(string direction)
        {
            //  Get the tile the player is attempting to move onto
            Tile t = new Tile(getTile(direction));
            //  If the tile is valid (not a wall, not out of bounds), move the player
            if (t.ToString() != "wall" || t.ToString() != "invalid")
            {
                //  Erase the player marker from its current position on the map; it is moving
                map[playerIndex[0, 0], playerIndex[0, 1]] = ' ';

                //  Keep track of the player's new X and Y position
                int newX = 0, newY = 0;

                //  Set the newX and newY values based on the player's move direction
                switch (direction)
                {
                    case "up":
                        newX = playerIndex[0, 0] - 1;
                        newY = playerIndex[0, 1];
                        playerIndex[0, 0] = newX;
                        break;
                    case "down":
                        newX = playerIndex[0, 0] + 1;
                        newY = playerIndex[0, 1];
                        playerIndex[0, 0] = newX;
                        break;
                    case "left":
                        newX = playerIndex[0, 0];
                        newY = playerIndex[0, 1] - 1;
                        playerIndex[0, 1] = newY;
                        break;
                    case "right":
                        newX = playerIndex[0, 0];
                        newY = playerIndex[0, 1] + 1;
                        playerIndex[0, 1] = newY;
                        break;
                    default: break;
                }

                //  The player has now moved, so let's update the map to reflect this fact by placing 'O' at the new
                //  position in the 2D map array!
                map[newX, newY] = 'O';
            }
        }
    }
}


//  Badeep-badeep-badeep-that's all folks!
//  :)