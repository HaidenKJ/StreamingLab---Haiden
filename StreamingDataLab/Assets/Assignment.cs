
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

static public class PartySerializer
{
    static public string SerializeParty(LinkedList<PartyCharacter> party)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine(party.Count.ToString());

        foreach (PartyCharacter character in party)
        {
            builder.AppendLine(character.classID.ToString());
            builder.AppendLine(character.health.ToString());
            builder.AppendLine(character.mana.ToString());
            builder.AppendLine(character.strength.ToString());
            builder.AppendLine(character.agility.ToString());
            builder.AppendLine(character.wisdom.ToString());
            builder.AppendLine(character.equipment.Count.ToString());

            foreach (int equipmentID in character.equipment)
            {
                builder.AppendLine(equipmentID.ToString());
            }
        }
        return builder.ToString();
    }
}

static public class PartyDeserializer
{
    static public LinkedList<PartyCharacter> DeserializeParty(string text)
{
    LinkedList<PartyCharacter> party = new LinkedList<PartyCharacter>();
    using (StringReader reader = new StringReader(text))
    {
        int characterCount = int.Parse(reader.ReadLine());
        for (int i = 0; i < characterCount; i++)
        {
            PartyCharacter character = new PartyCharacter();
            character.classID = int.Parse(reader.ReadLine());
            character.health = int.Parse(reader.ReadLine());
            character.mana = int.Parse(reader.ReadLine());
            character.strength = int.Parse(reader.ReadLine());
            character.agility = int.Parse(reader.ReadLine());
            character.wisdom = int.Parse(reader.ReadLine());

            character.equipment = new LinkedList<int>();
            int equipmentCount = int.Parse(reader.ReadLine());
            for (int j = 0; j < equipmentCount; j++)
                character.equipment.AddLast(int.Parse(reader.ReadLine()));

            party.AddLast(character);
        }
    }
    return party;
}
}


#endregion


#region Assignment Part 1

static public class AssignmentPart1 // saving and loading a single party
{

    static public void SavePartyButtonPressed()
    {
        string text = PartySerializer.SerializeParty(GameContent.partyCharacters);
        File.WriteAllText("Schmungus.txt", text);
        Debug.Log("Party saved to Schmungus.txt");
    }

    static public void LoadPartyButtonPressed()
    {
        if (File.Exists("Schmungus.txt"))
        {
            string text = File.ReadAllText("Schmungus.txt");
            GameContent.partyCharacters = PartyDeserializer.DeserializeParty(text);
            GameContent.RefreshUI();

            Debug.Log("Party loaded from Schmungus.txt");
        }
    }
}


#endregion

#region Assignment Part 2

static public class AssignmentConfiguration
{
    public const int PartOfAssignmentThatIsInDevelopment = 1;
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
        foreach (string path in Directory.GetFiles("SavedParties", "*.txt"))
        {
            SplitNameFromPartyText(File.ReadAllText(path), out string name);
            names.Add(name);
        }
        return names;
    }

    static public void LoadPartyDropDownChanged(string selectedName)
    {
        foreach (string path in Directory.GetFiles("SavedParties", "*.txt"))
        {
            string partyText = SplitNameFromPartyText(File.ReadAllText(path), out string savedName);
            if (savedName != selectedName)
                continue;

            partyName = savedName;
            fileName = path;
            GameContent.partyCharacters = PartyDeserializer.DeserializeParty(partyText);
            break; // found it, stop looking
        }
        GameContent.RefreshUI();
        Debug.Log("Party loaded from " + fileName);
    }

    static public void SavePartyButtonPressed()
    {
        Directory.CreateDirectory("SavedParties"); // Creates the folder called SavedParties if it doesn't exist.
        partyName = GameContent.GetPartyNameFromInput();
        fileName = GenerateUniqueFileName(); 
        if (fileName == null)
        {
            Debug.Log("Cap reached, cannot save more than 100 parties.");
            return; // This activates automatically when all save slots are filled, and prevents the user from saving more than 100 parties.
        }

        string text = partyName + "\n" + PartySerializer.SerializeParty(GameContent.partyCharacters);
        File.WriteAllText(fileName, text);
        GameContent.RefreshUI();

        Debug.Log("Party saved to " + fileName);
    }

    static public void NewPartyButtonPressed() 
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
            Debug.Log("Party deleted from " + fileName);
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
        int FileCountMax = Directory.GetFiles("SavedParties", "*.txt").Length;

        if (FileCountMax >= 100) // This is the cap
        {
            Debug.Log("Congratulations, if you’re seeing this Debug.Log message… Why did you do this? I’m revoking your saving privileges; overwrite an existing save instead!");
            return null;
        }
        
        string FileCount;
        do {FileCount = "SavedParties/Schmungus" + UnityEngine.Random.Range(0, 101) + ".txt";} // The FileCount saves the SavedParties/Schmungus, then it adds a random integer from 1 to 100 + ".txt" 
        while (File.Exists(FileCount)); // Then, if a file named FileCount exists, then :[
        return FileCount; // If not, then you get a FileCount
    }

    static string SplitNameFromPartyText(string fileText, out string name)
    {
        int firstNewline = fileText.IndexOf('\n');
        name = fileText.Substring(0, firstNewline).Trim(); // Trim removes the \r on Windows
        return fileText.Substring(firstNewline + 1); // everything after the name line
    }

}

#endregion



