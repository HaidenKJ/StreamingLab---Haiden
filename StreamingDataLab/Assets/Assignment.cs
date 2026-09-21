
/*
This RPG data streaming assignment was created by Fernando Restituto.
Pixel RPG characters created by Sean Browning.
*/

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.IO;
#region Assignment Instructions

/*  Hello!  Welcome to your first lab :)

Wax on, wax off.

    The development of saving and loading systems shares much in common with that of networked gameplay development.  
    Both involve developing around data which is packaged and passed into (or gotten from) a stream.  
    Thus, prior to attacking the problems of development for networked games, you will strengthen your abilities to develop solutions using the easier to work with HD saving/loading frameworks.

    Try to understand not just the framework tools, but also, 
    seek to familiarize yourself with how we are able to break data down, pass it into a stream and then rebuild it from another stream.


Lab Part 1

    Begin by exploring the UI elements that you are presented with upon hitting play.
    You can roll a new party, view party stats and hit a save and load button, both of which do nothing.
    You are challenged to create the functions that will save and load the party data which is being displayed on screen for you.

    Below, a SavePartyButtonPressed and a LoadPartyButtonPressed function are provided for you.
    Both are being called by the internal systems when the respective button is hit.
    You must code the save/load functionality.
    Access to Party Character data is provided via demo usage in the save and load functions.

    The PartyCharacter class members are defined as follows.  */
public partial class PartyCharacter
{
    public int classID;

    public int health;
    public int mana;

    public int strength;
    public int agility;
    public int wisdom;

    public LinkedList<int> equipment;
}



/*
    Access to the on screen party data can be achieved via …..

    Once you have loaded party data from the HD, you can have it loaded on screen via …...

    These are the stream reader/writer that I want you to use.
    https://docs.microsoft.com/en-us/dotnet/api/system.io.streamwriter
    https://docs.microsoft.com/en-us/dotnet/api/system.io.streamreader

    Alright, that’s all you need to get started on the first part of this assignment, here are your functions, good luck and journey well!
*/


#endregion


#region Assignment Part 1

static public class AssignmentPart1
{

    static public void SavePartyButtonPressed()
    {
        // Writing to a file (Serialization transforms data you have stored away in class instances with that of a data stream, a sequence.)
        using (StreamWriter sw = new StreamWriter("Schmungus.txt"))
        {
            sw.WriteLine(GameContent.partyCharacters.Count);

            foreach (PartyCharacter pc in GameContent.partyCharacters)
            {
                Debug.Log("PC class id == " + pc.classID + " was saved to Schmungus.txt");
                sw.WriteLine(pc.classID);
                sw.WriteLine(pc.health);
                sw.WriteLine(pc.mana);
                sw.WriteLine(pc.strength);
                sw.WriteLine(pc.agility);
                sw.WriteLine(pc.wisdom);
                sw.WriteLine(pc.equipment.Count);

                foreach (int eq in pc.equipment)
                {
                    sw.WriteLine(eq);
                }
            }
        }
    }

    static public void LoadPartyButtonPressed()
    {
        GameContent.partyCharacters.Clear();

        // Reading from a file (Deserialization transforms a data stream back into class instances)
        using (StreamReader sr = new StreamReader("Schmungus.txt"))
        {
            string line = sr.ReadLine();
            int count = int.Parse(line);
            for (int i = 0; i < count; i++) 
            {;
                PartyCharacter pc = new PartyCharacter();

                // The parse function is used to convert a string into a number.  It will throw an exception if the string is not a valid number. ( Basically the opposite of ToString() )
                pc.classID = int.Parse(sr.ReadLine());
                Debug.Log("PC class id == " + pc.classID + " was loaded from Schmungus.txt");

                pc.health = int.Parse(sr.ReadLine());
                pc.mana = int.Parse(sr.ReadLine());
                pc.strength = int.Parse(sr.ReadLine());
                pc.agility = int.Parse(sr.ReadLine());
                pc.wisdom = int.Parse(sr.ReadLine());

                int equipmentCount = int.Parse(sr.ReadLine());
                pc.equipment.Clear();

                for (int j = 0; j < equipmentCount; j++)
                {
                    pc.equipment.AddLast(int.Parse(sr.ReadLine()));
                }

                GameContent.partyCharacters.AddLast(pc);
            }
        }
        GameContent.RefreshUI();
    }
}


#endregion


#region Assignment Part 2

//  Before Proceeding!
//  To inform the internal systems that you are proceeding onto the second part of this assignment,
//  change the below value of AssignmentConfiguration.PartOfAssignmentInDevelopment from 1 to 2.
//  This will enable the needed UI/function calls for your to proceed with your assignment.


static public class AssignmentConfiguration
{
    public const int PartOfAssignmentThatIsInDevelopment = 2;
}

/*

In this part of the assignment you are challenged to expand on the functionality that you have already created.  
    You are being challenged to save, load and manage multiple parties.
    You are being challenged to identify each party via a string name (a member of the Party class).

To aid you in this challenge, the UI has been altered.  

    The load button has been replaced with a drop down list.  
    When this load party drop down list is changed, LoadPartyDropDownChanged(string selectedName) will be called.  
    When this drop down is created, it will be populated with the return value of GetListOfPartyNames().

    GameStart() is called when the program starts.

    For quality of life, a new SavePartyButtonPressed() has been provided to you below.

    An new/delete button has been added, you will also find below NewPartyButtonPressed() and DeletePartyButtonPressed()

Again, you are being challenged to develop the ability to save and load multiple parties.
    This challenge is different from the previous.
    In the above challenge, what you had to develop was much more directly named.
    With this challenge however, there is a much more predicate process required.
    Let me ask you,
        What do you need to program to produce the saving, loading and management of multiple parties?
        What are the variables that you will need to declare?
        What are the things that you will need to do?  
    So much of development is just breaking problems down into smaller parts.
    Take the time to name each part of what you will create and then, do it.

Good luck, journey well.

*/

static public class AssignmentPart2
{
    static string fileName = null;
    static string partyName = null;


    static public void GameStart()
    {
        Directory.CreateDirectory("SavedParties"); // Creates the folder called SavedParties if it doesn't exist.
        if (Directory.Exists("SavedParties"))
        {
            Debug.Log("The SavedParties_folder has been created.");
        }
        GameContent.RefreshUI();
    }

    static public List<string> GetListOfPartyNames()
    {
        List<string> names = new List<string>();

        foreach (string path in Directory.GetFiles("SavedParties", "*.txt")) // an array of every file path matching *.txt in SavedParties folder
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string trueName = sr.ReadLine();
                names.Add(trueName);
            }
        }
        return names;
    }

    static public void LoadPartyDropDownChanged(string selectedName)
    {
        GameContent.partyCharacters.Clear();
        foreach (string path in Directory.GetFiles("SavedParties", "*.txt"))
        {
            using (StreamReader sr = new StreamReader(path))
            {
            string trueName = sr.ReadLine();
            if (trueName != selectedName)
            {
                continue;
            }
            partyName = trueName;
            fileName = path;
            // Something tells me I might have overcomplicated this


            string line = sr.ReadLine();
            int count = int.Parse(line);
            for (int i = 0; i < count; i++) 
                {
                    PartyCharacter pc = new PartyCharacter();

                    // The parse function is used to convert a string into a number.  It will throw an exception if the string is not a valid number. ( Basically the opposite of ToString() )
                    pc.classID = int.Parse(sr.ReadLine());
                    Debug.Log("PC class id == " + pc.classID + " was loaded from " + fileName);

                    pc.health = int.Parse(sr.ReadLine());
                    pc.mana = int.Parse(sr.ReadLine());
                    pc.strength = int.Parse(sr.ReadLine());
                    pc.agility = int.Parse(sr.ReadLine());
                    pc.wisdom = int.Parse(sr.ReadLine());

                    int equipmentCount = int.Parse(sr.ReadLine());
                    pc.equipment.Clear();

                    for (int j = 0; j < equipmentCount; j++)
                    {
                        pc.equipment.AddLast(int.Parse(sr.ReadLine()));
                    }

                    GameContent.partyCharacters.AddLast(pc);
                }
            }
        }
        GameContent.RefreshUI();

    }

    static public void SavePartyButtonPressed()
    {
        Directory.CreateDirectory("SavedParties");
        // Having a second CreateDirectory is just a failsafe, I heard that if the directory already exists, it will just ignore the command, so I put it here just in case my paranoia is right.

        
        partyName = GameContent.GetPartyNameFromInput();
        fileName = GenerateUniqueFileName();
        // My mouse literally just broke :]

        if (fileName == null)
        {
            Debug.Log("Cap reached, cannot save more than 100 parties.");
            return; // This activates automatically when all save slots are filled, and prevents the user from saving more than 100 parties.
        }

        using (StreamWriter sw = new StreamWriter(fileName))
        {
            sw.WriteLine(partyName);
            sw.WriteLine(GameContent.partyCharacters.Count);

            foreach (PartyCharacter pc in GameContent.partyCharacters)
            {
                Debug.Log("PC class id == " + pc.classID + " was saved to " + partyName + ".txt"); // It actually saves to fileName, but this Debug is just for simplicity's sake.
                sw.WriteLine(pc.classID);
                sw.WriteLine(pc.health);
                sw.WriteLine(pc.mana);
                sw.WriteLine(pc.strength);
                sw.WriteLine(pc.agility);
                sw.WriteLine(pc.wisdom);
                sw.WriteLine(pc.equipment.Count);

                foreach (int eq in pc.equipment)
                {
                    sw.WriteLine(eq);
                }
            }
        }
        GameContent.RefreshUI();
    }

    static public void NewPartyButtonPressed()
    {

    }

    static public void DeletePartyButtonPressed()
    {

    }

    static public string GenerateUniqueFileName()
    {
        int SmurbleMax = Directory.GetFiles("SavedParties", "*.txt").Length;

        if (SmurbleMax >= 100) // This is the cap
        {
            Debug.Log("Congratulations, if you’re seeing this Debug.Log message… Why did you do this? I’m revoking your saving privileges; overwrite an existing save instead!");
            return null;
        }
        
        string Smurble;
        do {Smurble = "SavedParties/Schmungus" + UnityEngine.Random.Range(0, 100) + ".txt";} // The Smurble saves the SavedParties/Schmungus, then it adds a random integer from 1 to 100 + ".txt" 
        while (File.Exists(Smurble)); // Then, if a file named Smurble exists, then :[
        return Smurble; // If not, then you get a Smurble
    }

}

#endregion


