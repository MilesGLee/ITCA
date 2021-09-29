using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ITCA
{
    class Game
    {
        //Create variables.
        //Matrix with a 5 by 5 dimension
        public static string[,] map = new string[5, 5];
        private Entity _player = new Entity();
        private int _currentFloor = 0;
        private Entity[] _enemies = new Entity[0]; //List of all current enemies on the floor the player is on.
        private Object[] _items = new Object[0]; //List of all current items on the floor the player is on.
        private Entity _currentEnemy; //The current enemy the player is in combat with.
        private int _currentEnemyIndex; //The index of the current enemy in the _enemies array.
        private Object _exit; //The exit hole, that lets the player progress stages.
        private SceneList _currentScene = SceneList.MAINMENU;
        private bool _gameOver = false;

        public enum SceneList
        {
            MAINMENU,
            INTRO,
            REST,
            FLOOR1,
            FLOOR2,
            FLOOR3,
            FLOOR4,
            FLOOR5
        }

        public void Run()
        {
            //The basic command line game loop
            Start();
            while (!_gameOver)
            {
                Update();
            }
            End();
        }

        void Start()
        {
            //Setup player stats
            _player.icon = "0";
            _player.PositionX = 2;
            _player.PositionY = 2;
            _player.Health = 10;
            _player.AttackPower = 5;
            _player.DefensePower = 1;
        }

        void End()
        {
            //When you end the game.
            Console.WriteLine("Farewell traveler, may we meet again...");
            Console.ReadKey(true);
        }

        void Update() //This is called every frame.
        {
            SceneManager();
        }

        int GetInput(string description, params string[] options)
        {
            string input = "";
            int inputReceived = -1;
            while (inputReceived == -1)
            {
                Console.WriteLine(description);
                for (int i = 0; i < options.Length; i++)
                {
                    Console.WriteLine($"{(i + 1)}. {options[i]}");
                }
                Console.Write(">");
                input = Console.ReadLine();

                if (int.TryParse(input, out inputReceived))
                {
                    inputReceived--;
                    if (inputReceived < 0 || inputReceived >= options.Length)
                    {
                        inputReceived = -1;

                        Console.WriteLine("Invalid Input");
                        Console.ReadKey(true);
                    }
                }
                else
                {
                    inputReceived = -1;
                    Console.WriteLine("Invalid Input.");
                    Console.ReadKey(true);

                    Console.Clear();
                }

                Console.Clear();
            }

            return inputReceived;
        } //Getting an int input based on a parameter string array.

        Entity[] AppendToEnemies(Entity[] arr, Entity value)
        {
            //Create a dummy array with one more element than our previous array.
            Entity[] tempArray = new Entity[arr.Length + 1];

            //Copy the old array values to the new array.
            for (int i = 0; i < arr.Length; i++)
            {
                tempArray[i] = arr[i];
            }

            //Set the last index of array to be the new value
            tempArray[tempArray.Length - 1] = value;

            //Return new array.
            return tempArray;
        } //Adds to the enemies array.
        Object[] AppendToObjects(Object[] arr, Object value)
        {
            //Create a dummy array with one more element than our previous array.
            Object[] tempArray = new Object[arr.Length + 1];

            //Copy the old array values to the new array.
            for (int i = 0; i < arr.Length; i++)
            {
                tempArray[i] = arr[i];
            }

            //Set the last index of array to be the new value
            tempArray[tempArray.Length - 1] = value;

            //Return new array.
            return tempArray;
        } //Adds to the object array.
        string[] AppendToString(string[] arr, string value)
        {
            //Create a dummy array with one more element than our previous array.
            string[] tempArray = new string[arr.Length + 1];

            //Copy the old array values to the new array.
            for (int i = 0; i < arr.Length; i++)
            {
                tempArray[i] = arr[i];
            }

            //Set the last index of array to be the new value
            tempArray[tempArray.Length - 1] = value;

            //Return new array.
            return tempArray;
        } //Adds to any inputted string array
        public Entity[] RemoveAt(Entity[] source, int index)
        {
            //Create new array that is one element smaller than the source
            Entity[] dest = new Entity[source.Length - 1];
            if (index > 0) //Copy arrays over if the index is not the origin.
                Array.Copy(source, 0, dest, 0, index);

            //IF the index is the last element in the source
            if (index < source.Length - 1)
                Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

            return dest;
        } //Removes from the enemies array at a specific index.
        public Object[] RemoveAtObject(Object[] source, int index)
        {
            //Create new array that is one element smaller than the source
            Object[] dest = new Object[source.Length - 1];
            if (index > 0) //Copy arrays over if the index is not the origin.
                Array.Copy(source, 0, dest, 0, index);

            //IF the index is the last element in the source
            if (index < source.Length - 1)
                Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

            return dest;
        } //Removes from the enemies array at a specific index.

        public void Save() 
        {
            StreamWriter writer = new StreamWriter("SaveData.txt");
            writer.WriteLine(_currentFloor);
            writer.WriteLine(_player.AttackPower);
            writer.WriteLine(_player.DefensePower);
            writer.Close();
        } //Saving the game

        public bool Load() 
        {
            bool loadSuccessful = true;
            if (!File.Exists("SaveData.txt"))
                loadSuccessful = false;
            StreamReader reader = new StreamReader("SaveData.txt");
            if (!int.TryParse(reader.ReadLine(), out _currentFloor))
                loadSuccessful = false;
            if (!int.TryParse(reader.ReadLine(), out _player.AttackPower))
                loadSuccessful = false;
            if (!int.TryParse(reader.ReadLine(), out _player.DefensePower))
                loadSuccessful = false;
            reader.Close();

            return loadSuccessful;
        } //Loading the game

        void SceneManager() //Updates the current scene based on a enum that can be set.
        {
            if (_currentScene == SceneList.MAINMENU)
                DisplayMainMenu();
            if (_currentScene == SceneList.INTRO)
                IntroScene();
            if (_currentScene == SceneList.REST)
                RestArea();
            if (_currentScene == SceneList.FLOOR1)
                Floor1();
            if (_currentScene == SceneList.FLOOR2)
                Floor2();
            if (_currentScene == SceneList.FLOOR3)
                Floor3();
            if (_currentScene == SceneList.FLOOR4)
                Floor4();
            if (_currentScene == SceneList.FLOOR5)
                Floor5();
        }

        void DisplayMainMenu()
        {
            //Display the ascii title card =)
            Console.WriteLine("██████████████████████████████████████████████████████████████");
            Console.WriteLine("██▄─█▀▀▀█─▄█▄─▄▄─█▄─▄███─▄▄▄─█─▄▄─█▄─▀█▀─▄█▄─▄▄─███─▄─▄─█─▄▄─█");
            Console.WriteLine("███─█─█─█─███─▄█▀██─██▀█─███▀█─██─██─█▄█─███─▄█▀█████─███─██─█");
            Console.WriteLine("█▀▀▄▄▄▀▄▄▄▀▀▄▄▄▄▄▀▄▄▄▄▄▀▄▄▄▄▄▀▄▄▄▄▀▄▄▄▀▄▄▄▀▄▄▄▄▄▀▀▀▀▄▄▄▀▀▄▄▄▄█");
            Console.WriteLine("██████████████████████████████████████████████████████████████");
            Console.WriteLine("█░░░░░░░░░░█░░░░░░░░░░░░░░█░░░░░░░░░░░░░░█░░░░░░░░░░░░░░██████");
            Console.WriteLine("█░░▄▀▄▀▄▀░░█░░▄▀▄▀▄▀▄▀▄▀░░█░░▄▀▄▀▄▀▄▀▄▀░░█░░▄▀▄▀▄▀▄▀▄▀░░██████");
            Console.WriteLine("█░░░░▄▀░░░░█░░░░░░▄▀░░░░░░█░░▄▀░░░░░░░░░░█░░▄▀░░░░░░▄▀░░██████");
            Console.WriteLine("███░░▄▀░░███████░░▄▀░░█████░░▄▀░░█████████░░▄▀░░██░░▄▀░░██████");
            Console.WriteLine("███░░▄▀░░███████░░▄▀░░█████░░▄▀░░█████████░░▄▀░░░░░░▄▀░░██████");
            Console.WriteLine("███░░▄▀░░███████░░▄▀░░█████░░▄▀░░█████████░░▄▀▄▀▄▀▄▀▄▀░░██████");
            Console.WriteLine("███░░▄▀░░███████░░▄▀░░█████░░▄▀░░█████████░░▄▀░░░░░░▄▀░░██████");
            Console.WriteLine("███░░▄▀░░███████░░▄▀░░█████░░▄▀░░█████████░░▄▀░░██░░▄▀░░██████");
            Console.WriteLine("█░░░░▄▀░░░░█████░░▄▀░░█████░░▄▀░░░░░░░░░░█░░▄▀░░██░░▄▀░░██████");
            Console.WriteLine("█░░▄▀▄▀▄▀░░█████░░▄▀░░█████░░▄▀▄▀▄▀▄▀▄▀░░█░░▄▀░░██░░▄▀░░██████");
            Console.WriteLine("█░░░░░░░░░░█████░░░░░░█████░░░░░░░░░░░░░░█░░░░░░██░░░░░░██████");
            Console.WriteLine("██████████████████████████████████████████████████████████████");
            //Give the player the choice to play the game.
            int choice = GetInput("(Totally an original name and not an acronym for Introduction to C# Assessment)", "Play Game", "Load Game", "Quit Game");
            if (choice == 0)
            {
                Console.Clear();
                _currentScene = SceneList.INTRO;
                _player.Health = 10;
                _player.AttackPower = 5;
                _player.DefensePower = 1;
            }
            else if (choice == 1)
            {
                if (Load()) //If player has a load saved, load game at the floor they were at.
                {
                    _player.Health = 10;
                    Console.Clear();
                    Console.WriteLine($"Load Successful. You are currently resting before floor #{_currentFloor}");
                    PreloadMap();
                    SetupFloor();
                    if (_currentFloor == 2)
                    {
                        _currentScene = SceneList.FLOOR2;
                        CreateWalls("1,0", "2,3", "0,3");
                        PlacePlayer();
                    }
                    if (_currentFloor == 3)
                    {
                        _currentScene = SceneList.FLOOR3;
                        CreateWalls("1,1", "1,2", "1,3", "2,2", "4,4", "0,4");
                        PlacePlayer();
                    }
                    if (_currentFloor == 4)
                    {
                        _currentScene = SceneList.FLOOR4;
                        CreateWalls("2,0", "2,1", "2,2", "4,0", "4,1", "4,2", "4,3");
                        PlacePlayer();
                    }
                    if (_currentFloor == 5)
                    {
                        _currentScene = SceneList.FLOOR5;
                        CreateWalls("2,2", "2,3", "2,1", "3,2", "1,2");
                        PlacePlayer();
                    }
                    Console.WriteLine(_currentScene);
                    Console.ReadKey();
                    Console.Clear();
                }
            }
            else if (choice == 2)
            {
                Console.Clear();
                _gameOver = true;
            }
        }

        void IntroScene() //The setup for the first floor.
        {
            Console.WriteLine($"You awake in an unknown dungeon. With nothing on your back you feel compelled to move forward.");
            Console.ReadKey(true);
            //Base level for the floors. Cannot be 0.
            _currentFloor = 1;
            PreloadMap();
            CreateWalls("0,0", "4,4");
            GetEntityPositions();
            SetupFloor();
            Console.Clear();
            _currentScene = SceneList.FLOOR1;
        }

        void SetupFloor() //Randomly creates enemies and places them around the map.
        {
            Random rand = new Random();
            int enemyAmount = rand.Next(1, _currentFloor); //Choses a random amount of enemies to spawn
            int itemAmount = rand.Next(1, _currentFloor); //Choses a random amount of items to spawn
            for (int i = 0; i < enemyAmount; i++)
            {
                Entity enemy = new Entity();
                //Sets the new enemies stats
                enemy.AttackPower = 2 + _currentFloor; //This is so the enemies will be harder, the further the player progresses.
                enemy.DefensePower = 1;
                enemy.Health = 5 + _currentFloor;
                enemy.icon = "I";
                enemy.name = "Skeleton";
                int x = rand.Next(0, 5); //These pick a random spot on the map.
                int y = rand.Next(0, 5);
                if (map[x, y] != ".") //If that random spot does not have a different object in place, place the enemy there.
                {
                    i--;
                    continue;
                }
                else 
                {
                    enemy.PositionX = x;
                    enemy.PositionY = y;
                    _enemies = AppendToEnemies(_enemies, enemy); //Adds the new enemy to the array of enemies.
                }
            }
            for (int i = 0; i < itemAmount; i++) 
            {
                int itemType = rand.Next(1,4);
                if (itemType == 1) 
                {
                    Object item = new Object();
                    item.ItemPower = 2;
                    item.icon = "♥";
                    item.name = "Health Vial";
                    int x = rand.Next(0, 5); //These pick a random spot on the map.
                    int y = rand.Next(0, 5);
                    if (map[x, y] != ".") //If that random spot does not have a different object in place, place the item there.
                    {
                        i--;
                        continue;
                    }
                    else
                    {
                        item.PositionX = x;
                        item.PositionY = y;
                        _items = AppendToObjects(_items, item); //Adds the new enemy to the array of enemies.
                    }
                }
                else if (itemType == 2)
                {
                    Object item = new Object();
                    item.ItemPower = 1;
                    item.icon = "♦";
                    item.name = "Sheen";
                    int x = rand.Next(0, 5); //These pick a random spot on the map.
                    int y = rand.Next(0, 5);
                    if (map[x, y] != ".") //If that random spot does not have a different object in place, place the item there.
                    {
                        i--;
                        continue;
                    }
                    else
                    {
                        item.PositionX = x;
                        item.PositionY = y;
                        _items = AppendToObjects(_items, item); //Adds the new enemy to the array of enemies.
                    }
                }
                else if (itemType == 3)
                {
                    Object item = new Object();
                    item.ItemPower = 1;
                    item.icon = "▲";
                    item.name = "Some armor";
                    int x = rand.Next(0, 5); //These pick a random spot on the map.
                    int y = rand.Next(0, 5);
                    if (map[x, y] != ".") //If that random spot does not have a different object in place, place the item there.
                    {
                        i--;
                        continue;
                    }
                    else
                    {
                        item.PositionX = x;
                        item.PositionY = y;
                        _items = AppendToObjects(_items, item); //Adds the new enemy to the array of enemies.
                    }
                }
            }
        }

        void CreateWalls(params string[] positions) //Takes in a param of string arrays for visual pleasure. Input positions of walls and this will create and draw them to the map.
        {
            for (int i = 0; i < positions.Length; i++)
            {
                int Xpos;
                int Ypos;
                int.TryParse($"{positions[i][0]}", out Xpos); //Because the input string is meant to be formatted like "x,y" the Y is on the *3rd position of the string.
                int.TryParse($"{positions[i][2]}", out Ypos);
                map[Xpos, Ypos] = "[]";
            }
        }

        void PlaceExit() //Creates the exit hole and randomly places it in a suitable location.
        {
            Random rand = new Random();
            _exit = new Object();
            _exit.icon = "X";
            _exit.name = "Exit";
            bool goodToSpawn = false;
            while (goodToSpawn == false) //This while loop checks through random positions until an open position is found.
            {
                int x = rand.Next(0, 5);
                int y = rand.Next(0, 5);
                if (map[x, y] != ".")
                {
                    goodToSpawn = false;
                }
                else
                {
                    goodToSpawn = true;
                    _exit.PositionX = x;
                    _exit.PositionY = y;
                }
            }
        }

        void PlacePlayer() //Sets the player in an open position on the map
        {
            Random rand = new Random();
            bool goodToSpawn = false;
            while (goodToSpawn == false) //This while loop checks through random positions until an open position is found.
            {
                int x = rand.Next(0, 5);
                int y = rand.Next(0, 5);
                if (map[x, y] != ".")
                {
                    goodToSpawn = false;
                }
                else
                {
                    goodToSpawn = true;
                    _player.PositionX = x;
                    _player.PositionY = y;
                }
            }
        }

        void PreloadMap() //Loads and draws the base empty map matrix.
        {
            map = new string[5, 5] { { ".", ".", ".", ".", "." }, 
                                    { ".", ".", ".", ".", "." }, 
                                    { ".", ".", ".", ".", "." }, 
                                    { ".", ".", ".", ".", "." }, 
                                    { ".", ".", ".", ".", "." } };
        }

        void DisplayMap() //This displays the map matrix on the screen to view.
        {
            int x, y;
            for (x = 0; x < 5; x++) //Both the for loops need to be kept at 5, being the matrix's bounds.
            {
                Console.Write("\n");
                for (y = 0; y < 5; y++)
                    Console.Write("{0}\t", map[x, y]);
            }
            Console.Write("\n\n");
        }

        void GetEntityPositions() //Loads and draws all of the entity positions on the map.
        {
            map[_player.PositionY, _player.PositionX] = _player.icon; //Player position
            if(_exit != null)
                map[_exit.PositionX, _exit.PositionY] = _exit.icon; //If exit exists, display it.
            if (_enemies.Length > 0) //This loops for each enemy in the enemies array and displays them.
            {
                for (int i = 0; i < _enemies.Length; i++)
                {
                    Entity e = _enemies[i];
                    map[e.PositionY, e.PositionX] = e.icon;
                }
            }
            if (_items.Length > 0) //This loops for each enemy in the enemies array and displays them.
            {
                for (int i = 0; i < _items.Length; i++)
                {
                    Object o = _items[i];
                    map[o.PositionY, o.PositionX] = o.icon;
                }
            }
        }

        void Combat() //When the player is in combat, stop drawin the map and work through combat.
        {
            Console.Clear();
            Console.WriteLine($"You are in combat with {_currentEnemy.name}!"); //Display the name of the enemy
            bool inCombat = true;
            while (inCombat == true) 
            {
                int input = GetInput("What do you do:", "Attack", "Run away"); //Player can choose to fight the enemy or exit combat.
                if (input == 0)
                {
                    float damageDealt = _player.Attack(_currentEnemy); //Attacks the enemy
                    Console.WriteLine($"You attacked the {_currentEnemy.name} for {damageDealt} damage.");
                }
                else if (input == 1)
                {
                    inCombat = false;
                }
                float damageTaken = _currentEnemy.Attack(_player); // Enemy attacks player
                Console.WriteLine($"You were attacked by the {_currentEnemy.name} for {damageTaken} damage.");
                if (_player.Health <= 0) //If player dies
                {
                    Console.WriteLine($"You were slain by the {_currentEnemy.name}");
                    Console.ReadKey();
                    Console.Clear();
                    _currentScene = SceneList.MAINMENU;
                    inCombat = false;
                }
                else if (_currentEnemy.Health <= 0) //If player defeats the enemy
                {
                    Console.WriteLine($"You slayed the {_currentEnemy.name}");
                    _enemies = RemoveAt(_enemies, _currentEnemyIndex);
                    _currentEnemy = null;
                    _currentEnemyIndex = 0;
                    inCombat = false;
                    if (_enemies.Length == 0) //if this current enemy was the last one left in the enemies array.
                    {
                        Console.WriteLine("All enemies in the floor defeated! The exit has appeared!");
                        PlaceExit();
                    }
                    if (_currentFloor == 5) 
                    {
                        Console.ReadKey();
                        Console.Clear();
                        Console.WriteLine("You see a seperate pathway leading outside to the daylight! You've escaped this dungeon! Congradulations!");
                        _currentScene = SceneList.MAINMENU;
                    }
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        void RestArea() //Scene inbetween floors, allows player to save and quit if needed.
        {
            Console.WriteLine("You find yourself in a cozy camp that someone long before you has left. Your health has been restored.");
            if (_player.Health < 10) //If player is below 10hp, bring them up to 10 hp
                _player.Health += (10 - _player.Health);
            Console.WriteLine($"Health: {_player.Health} Attack: {_player.AttackPower} Defense: {_player.DefensePower}");
            int input = GetInput("What do you do...", "Venture forth", "Take a nap (Save&Quit)");
            if (input == 0) //Resets player health and proceeds to next floor
            {
                for (int i = 0; i < _items.Length; i++) 
                {
                    _items = RemoveAtObject(_items, i);
                }
                PreloadMap();
                SetupFloor();
                if (_currentFloor == 2)
                {
                    _currentScene = SceneList.FLOOR2;
                    CreateWalls("1,0", "2,3", "0,3");
                    PlacePlayer();
                }
                if (_currentFloor == 3)
                {
                    _currentScene = SceneList.FLOOR3;
                    CreateWalls("1,1", "1,2", "1,3", "2,2", "4,4", "0,4");
                    PlacePlayer();
                }
                if (_currentFloor == 4)
                {
                    _currentScene = SceneList.FLOOR4;
                    CreateWalls("2,0", "2,1", "2,2", "4,0", "4,1", "4,2", "4,3");
                    PlacePlayer();
                }
                if (_currentFloor == 5)
                {
                    _currentScene = SceneList.FLOOR5;
                    CreateWalls("2,2", "2,3", "2,1", "3,2", "1,2");
                    PlacePlayer();
                }
            }
            else if (input == 1) 
            {
                Console.WriteLine("Saved game, exiting to main menu...");
                Console.ReadKey();
                Save();
                _currentScene = SceneList.MAINMENU;
            }
        }

        void Floor1() //The first floor of the game. Initialized in the Intro void. Allows the player to view the map and move around it.
        {
            PreloadMap();
            CreateWalls("0,0", "4,4"); //Creates the walls/bounds of the current floor.
            GetEntityPositions();
            DisplayMap(); //Draws the map after loading all the entities and walls.
            Console.WriteLine($"Health: {_player.Health} Attack: {_player.AttackPower} Defense: {_player.DefensePower}"); //Display the players health.
            Console.WriteLine("");
            int choice = GetInput("Move which direction", "Up", "Down", "Right", "Left"); //Lets the player move in any direction.
            if (choice == 0)
                _player.Move("Up");
            if (choice == 1)
                _player.Move("Down");
            if (choice == 2)
                _player.Move("Right");
            if (choice == 3)
                _player.Move("Left");
            Random rand = new Random(); //Has enemy move in a random direction.
            for (int i = 0; i < _enemies.Length; i++)
            {
                int dir = rand.Next(1, 7);
                if (dir == 1)
                    _enemies[i].Move("Up");
                else if (dir == 2)
                    _enemies[i].Move("Down");
                else if (dir == 3)
                    _enemies[i].Move("Left");
                else if (dir == 4)
                    _enemies[i].Move("Right");
            }
            for (int i = 0; i < _enemies.Length; i++) //If player is colliding with an enemy, start a combat sequence.
            {
                if (_enemies[i].PositionX == _player.PositionX && _enemies[i].PositionY == _player.PositionY)
                {
                    _currentEnemyIndex = i;
                    _currentEnemy = _enemies[i];
                    Combat();
                }
            }
            if (map[_player.PositionY, _player.PositionX] == "X") //If the player is colliding with an exit, move onto the next floor.
            {
                Console.Clear();
                Console.WriteLine("You open the hatch and fall down, decending deeper into this dungeon...");
                Console.ReadKey();
                _exit = null;
                _currentFloor++;
                Console.Clear();
                _currentScene = SceneList.REST;
            }
            for (int i = 0; i < _items.Length; i++) //If player is colliding with an item, add item stats to player.
            {
                if (_items[i].PositionX == _player.PositionX && _items[i].PositionY == _player.PositionY)
                {
                    Console.Clear();
                    if (map[_player.PositionY, _player.PositionX] == "♥") { //Add health to player if they found a heart
                        Console.WriteLine($"You found a health vial and drank it. +{_items[i].ItemPower} health restored.");
                        Console.ReadKey();
                        _player.Health += _items[i].ItemPower;
                        _items = RemoveAtObject(_items, i);
                    }
                    if (map[_player.PositionY, _player.PositionX] == "♦") //This one adds damage
                    {
                        Console.WriteLine($"You found a sheen and used it to sharpen your weapon. +{_items[i].ItemPower} damage increased.");
                        Console.ReadKey();
                        _player.AttackPower += _items[i].ItemPower;
                        _items = RemoveAtObject(_items, i);
                    }
                    if (map[_player.PositionY, _player.PositionX] == "▲") //and this one armor/defense.
                    {
                        Console.WriteLine($"You found a piece of armor and equipped it. +{_items[i].ItemPower} defense.");
                        Console.ReadKey();
                        _player.DefensePower += _items[i].ItemPower;
                        _items = RemoveAtObject(_items, i);
                    }
                }
            }
            Console.Clear();
        }

        void Floor2() //The second floor of the game.
        {
            PreloadMap();
            CreateWalls("1,0", "2,3", "0,3");
            GetEntityPositions();
            DisplayMap(); //Draws the map after loading all the entities and walls.
            Console.WriteLine($"Health: {_player.Health} Attack: {_player.AttackPower} Defense: {_player.DefensePower}"); //Display the players health.
            Console.WriteLine("");
            int choice = GetInput("Move which direction", "Up", "Down", "Right", "Left"); //Lets the player move in any direction.
            if (choice == 0)
                _player.Move("Up");
            if (choice == 1)
                _player.Move("Down");
            if (choice == 2)
                _player.Move("Right");
            if (choice == 3)
                _player.Move("Left");
            Random rand = new Random(); //Has enemy move in a random direction.
            for (int i = 0; i < _enemies.Length; i++)
            {
                int dir = rand.Next(1, 7);
                if (dir == 1)
                    _enemies[i].Move("Up");
                else if (dir == 2)
                    _enemies[i].Move("Down");
                else if (dir == 3)
                    _enemies[i].Move("Left");
                else if (dir == 4)
                    _enemies[i].Move("Right");
            }
            for (int i = 0; i < _enemies.Length; i++) //If player is colliding with an enemy, start a combat sequence.
            {
                if (_enemies[i].PositionX == _player.PositionX && _enemies[i].PositionY == _player.PositionY)
                {
                    _currentEnemyIndex = i;
                    _currentEnemy = _enemies[i];
                    Combat();
                }
            }
            if (map[_player.PositionY, _player.PositionX] == "X") //If the player is colliding with an exit, move onto the next floor.
            {
                Console.Clear();
                Console.WriteLine("You open the hatch and fall down, decending deeper into this dungeon...");
                Console.ReadKey();
                _exit = null;
                _currentFloor++;
                Console.Clear();
                _currentScene = SceneList.REST;
            }
            for (int i = 0; i < _items.Length; i++) //If player is colliding with an item, add item stats to player.
            {
                if (_items[i].PositionX == _player.PositionX && _items[i].PositionY == _player.PositionY)
                {
                    Console.Clear();
                    if (map[_player.PositionY, _player.PositionX] == "♥")
                    {
                        Console.WriteLine($"You found a health vial and drank it. +{_items[i].ItemPower} health restored.");
                        _player.Health += _items[i].ItemPower;
                        _items = RemoveAtObject(_items, i);
                        Console.ReadKey();
                    }
                    if (map[_player.PositionY, _player.PositionX] == "♦")
                    {
                        Console.WriteLine($"You found a sheen and used it to sharpen your weapon. +{_items[i].ItemPower} damage increased.");
                        _player.AttackPower += _items[i].ItemPower;
                        _items = RemoveAtObject(_items, i);
                        Console.ReadKey();
                    }
                    if (map[_player.PositionY, _player.PositionX] == "▲")
                    {
                        Console.WriteLine($"You found a piece of armor and equipped it. +{_items[i].ItemPower} defense.");
                        _player.DefensePower += _items[i].ItemPower;
                        _items = RemoveAtObject(_items, i);
                        Console.ReadKey();
                    }
                }
            }
            Console.Clear();
        }

        void Floor3() //The third floor of the game.
        {
            PreloadMap();
            CreateWalls("1,1", "1,2", "1,3", "2,2", "4,4", "0,4");
            GetEntityPositions();
            DisplayMap(); //Draws the map after loading all the entities and walls.
            Console.WriteLine($"Health: {_player.Health} Attack: {_player.AttackPower} Defense: {_player.DefensePower}"); //Display the players health.
            Console.WriteLine("");
            int choice = GetInput("Move which direction", "Up", "Down", "Right", "Left"); //Lets the player move in any direction.
            if (choice == 0)
                _player.Move("Up");
            if (choice == 1)
                _player.Move("Down");
            if (choice == 2)
                _player.Move("Right");
            if (choice == 3)
                _player.Move("Left");
            Random rand = new Random(); //Has enemy move in a random direction.
            for (int i = 0; i < _enemies.Length; i++)
            {
                int dir = rand.Next(1, 7);
                if (dir == 1)
                    _enemies[i].Move("Up");
                else if (dir == 2)
                    _enemies[i].Move("Down");
                else if (dir == 3)
                    _enemies[i].Move("Left");
                else if (dir == 4)
                    _enemies[i].Move("Right");
            }
            for (int i = 0; i < _enemies.Length; i++) //If player is colliding with an enemy, start a combat sequence.
            {
                if (_enemies[i].PositionX == _player.PositionX && _enemies[i].PositionY == _player.PositionY)
                {
                    _currentEnemyIndex = i;
                    _currentEnemy = _enemies[i];
                    Combat();
                }
            }
            if (map[_player.PositionY, _player.PositionX] == "X") //If the player is colliding with an exit, move onto the next floor.
            {
                Console.Clear();
                Console.WriteLine("You open the hatch and fall down, decending deeper into this dungeon...");
                Console.ReadKey();
                _exit = null;
                _currentFloor++;
                Console.Clear();
                _currentScene = SceneList.REST;
            }
            for (int i = 0; i < _items.Length; i++) //If player is colliding with an item, add item stats to player.
            {
                if (_items[i].PositionX == _player.PositionX && _items[i].PositionY == _player.PositionY)
                {
                    Console.Clear();
                    if (map[_player.PositionY, _player.PositionX] == "♥")
                    {
                        Console.WriteLine($"You found a health vial and drank it. +{_items[i].ItemPower} health restored.");
                        _player.Health += _items[i].ItemPower;
                        _items = RemoveAtObject(_items, i);
                        Console.ReadKey();
                    }
                    if (map[_player.PositionY, _player.PositionX] == "♦")
                    {
                        Console.WriteLine($"You found a sheen and used it to sharpen your weapon. +{_items[i].ItemPower} damage increased.");
                        _player.AttackPower += _items[i].ItemPower;
                        _items = RemoveAtObject(_items, i);
                        Console.ReadKey();
                    }
                    if (map[_player.PositionY, _player.PositionX] == "▲")
                    {
                        Console.WriteLine($"You found a piece of armor and equipped it. +{_items[i].ItemPower} defense.");
                        _player.DefensePower += _items[i].ItemPower;
                        _items = RemoveAtObject(_items, i);
                        Console.ReadKey();
                    }
                }
            }
            Console.Clear();
        }

        void Floor4() //The fourth floor of the game.
        {
            PreloadMap();
            CreateWalls("2,0", "2,1", "2,2", "4,0", "4,1", "4,2", "4,3");
            GetEntityPositions();
            DisplayMap(); //Draws the map after loading all the entities and walls.
            Console.WriteLine($"Health: {_player.Health} Attack: {_player.AttackPower} Defense: {_player.DefensePower}"); //Display the players health.
            Console.WriteLine("");
            int choice = GetInput("Move which direction", "Up", "Down", "Right", "Left"); //Lets the player move in any direction.
            if (choice == 0)
                _player.Move("Up");
            if (choice == 1)
                _player.Move("Down");
            if (choice == 2)
                _player.Move("Right");
            if (choice == 3)
                _player.Move("Left");
            Random rand = new Random(); //Has enemy move in a random direction.
            for (int i = 0; i < _enemies.Length; i++)
            {
                int dir = rand.Next(1, 7);
                if (dir == 1)
                    _enemies[i].Move("Up");
                else if (dir == 2)
                    _enemies[i].Move("Down");
                else if (dir == 3)
                    _enemies[i].Move("Left");
                else if (dir == 4)
                    _enemies[i].Move("Right");
            }
            for (int i = 0; i < _enemies.Length; i++) //If player is colliding with an enemy, start a combat sequence.
            {
                if (_enemies[i].PositionX == _player.PositionX && _enemies[i].PositionY == _player.PositionY)
                {
                    _currentEnemyIndex = i;
                    _currentEnemy = _enemies[i];
                    Combat();
                }
            }
            if (map[_player.PositionY, _player.PositionX] == "X") //If the player is colliding with an exit, move onto the next floor.
            {
                Console.Clear();
                Console.WriteLine("You open the hatch and fall down, decending deeper into this dungeon...");
                Console.ReadKey();
                _exit = null;
                _currentFloor++;
                Console.Clear();
                _currentScene = SceneList.REST;
            }
            for (int i = 0; i < _items.Length; i++) //If player is colliding with an item, add item stats to player.
            {
                if (_items[i].PositionX == _player.PositionX && _items[i].PositionY == _player.PositionY)
                {
                    Console.Clear();
                    if (map[_player.PositionY, _player.PositionX] == "♥")
                    {
                        Console.WriteLine($"You found a health vial and drank it. +{_items[i].ItemPower} health restored.");
                        _player.Health += _items[i].ItemPower;
                        _items = RemoveAtObject(_items, i);
                        Console.ReadKey();
                    }
                    if (map[_player.PositionY, _player.PositionX] == "♦")
                    {
                        Console.WriteLine($"You found a sheen and used it to sharpen your weapon. +{_items[i].ItemPower} damage increased.");
                        _player.AttackPower += _items[i].ItemPower;
                        _items = RemoveAtObject(_items, i);
                        Console.ReadKey();
                    }
                    if (map[_player.PositionY, _player.PositionX] == "▲")
                    {
                        Console.WriteLine($"You found a piece of armor and equipped it. +{_items[i].ItemPower} defense.");
                        _player.DefensePower += _items[i].ItemPower;
                        _items = RemoveAtObject(_items, i);
                        Console.ReadKey();
                    }
                }
            }
            Console.Clear();
        }

        void Floor5() //The fifth floor of the game.
        {
            PreloadMap();
            CreateWalls("2,2", "2,3", "2,1", "3,2", "1,2");
            GetEntityPositions();
            DisplayMap(); //Draws the map after loading all the entities and walls.
            Console.WriteLine($"Health: {_player.Health} Attack: {_player.AttackPower} Defense: {_player.DefensePower}"); //Display the players health.
            Console.WriteLine("");
            int choice = GetInput("Move which direction", "Up", "Down", "Right", "Left"); //Lets the player move in any direction.
            if (choice == 0)
                _player.Move("Up");
            if (choice == 1)
                _player.Move("Down");
            if (choice == 2)
                _player.Move("Right");
            if (choice == 3)
                _player.Move("Left");
            Random rand = new Random(); //Has enemy move in a random direction.
            for (int i = 0; i < _enemies.Length; i++)
            {
                int dir = rand.Next(1, 7);
                if (dir == 1)
                    _enemies[i].Move("Up");
                else if (dir == 2)
                    _enemies[i].Move("Down");
                else if (dir == 3)
                    _enemies[i].Move("Left");
                else if (dir == 4)
                    _enemies[i].Move("Right");
            }
            for (int i = 0; i < _enemies.Length; i++) //If player is colliding with an enemy, start a combat sequence.
            {
                if (_enemies[i].PositionX == _player.PositionX && _enemies[i].PositionY == _player.PositionY)
                {
                    _currentEnemyIndex = i;
                    _currentEnemy = _enemies[i];
                    Combat();
                }
            }
            if (map[_player.PositionY, _player.PositionX] == "X") //If the player is colliding with an exit, move onto the next floor.
            {
                Console.Clear();
                Console.WriteLine("You open the hatch and fall down, decending deeper into this dungeon...");
                Console.ReadKey();
                _exit = null;
                _currentFloor++;
                Console.Clear();
                _currentScene = SceneList.REST;
            }
            for (int i = 0; i < _items.Length; i++) //If player is colliding with an item, add item stats to player.
            {
                if (_items[i].PositionX == _player.PositionX && _items[i].PositionY == _player.PositionY)
                {
                    Console.Clear();
                    if (map[_player.PositionY, _player.PositionX] == "♥")
                    {
                        Console.WriteLine($"You found a health vial and drank it. +{_items[i].ItemPower} health restored.");
                        _player.Health += _items[i].ItemPower;
                        _items = RemoveAtObject(_items, i);
                        Console.ReadKey();
                    }
                    if (map[_player.PositionY, _player.PositionX] == "♦")
                    {
                        Console.WriteLine($"You found a sheen and used it to sharpen your weapon. +{_items[i].ItemPower} damage increased.");
                        _player.AttackPower += _items[i].ItemPower;
                        _items = RemoveAtObject(_items, i);
                        Console.ReadKey();
                    }
                    if (map[_player.PositionY, _player.PositionX] == "▲")
                    {
                        Console.WriteLine($"You found a piece of armor and equipped it. +{_items[i].ItemPower} defense.");
                        _player.DefensePower += _items[i].ItemPower;
                        _items = RemoveAtObject(_items, i);
                        Console.ReadKey();
                    }
                }
            }
            Console.Clear();
        }
    }
}
