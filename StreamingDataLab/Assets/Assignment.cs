
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

#endregion


#region Assignment Part 1

static public class AssignmentPart1 // saving and loading a single party
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

static public class AssignmentConfiguration
{
    public const int PartOfAssignmentThatIsInDevelopment = 2;
}

static public class AssignmentPart2 // saving and loading multiple parties, with unique names, and a cap of 100 parties.
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

    static public void NewPartyButtonPressed() // Is the new party function supposed to be a overwrite? We already have a save and reroll function?
    {
    if (fileName != null)
        {
            File.Delete(fileName);
            fileName = null;
        }
        if (partyName != null)
        {
            partyName = null;
        }
        
    // This func just deletes the current party, so the user can start fresh with a new party.
    GameContent.RerollParty();
    // Call RerollParty() to generate a new party, so the user doesn't need to press 2 buttons. I'm lazy
    SavePartyButtonPressed();
    // Automatically saves the parties.
    GameContent.RefreshUI();
    // Refresh the UI to actually show the new party, and to clear the input field for the party name.
    }

    static public void DeletePartyButtonPressed()
    {
        GameContent.partyCharacters.Clear();

        if (fileName != null)
        {
            File.Delete(fileName);
            Debug.Log("...");
            fileName = null;
        }

        if (partyName != null)
        {
            Debug.Log("~~~~~ The heroic party of " + partyName + ", has been lost to the sands of time, just as all names shall be lost. ~~~~~");
            partyName = null;
        }

        GameContent.RefreshUI();
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


