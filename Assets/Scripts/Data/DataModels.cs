using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Player data structure for local storage
/// </summary>
[Serializable]
public class PlayerData
{
    public string playerId;
    public string characterName;
    public int level;
    public int experience;
    public int experienceToNextLevel;
    
    // Position
    public Vector2 position;
    public string currentScene;
    
    // Stats
    public int health;
    public int maxHealth;
    public int mana;
    public int maxMana;
    public int strength;
    public int defense;
    public int agility;
    
    // Inventory
    public List<ItemData> inventory;
    public List<int> equippedItemSlots; // Indices into inventory
    
    // Skill tree
    public List<string> unlockedSkills;
    public Dictionary<string, int> skillLevels;
    
    // Tutorial and progress
    public bool hasCompletedTutorial;
    public List<string> completedQuests;
    public List<string> activeQuests;
    
    // Timestamps
    public string createdAt;
    public string lastPlayedAt;
    public float totalPlayTime; // seconds

    public PlayerData()
    {
        playerId = System.Guid.NewGuid().ToString();
        characterName = "Adventurer";
        level = 1;
        experience = 0;
        experienceToNextLevel = 100;
        
        position = Vector2.zero;
        currentScene = "MainMenu";
        
        health = 100;
        maxHealth = 100;
        mana = 50;
        maxMana = 50;
        strength = 10;
        defense = 10;
        agility = 10;
        
        inventory = new List<ItemData>();
        equippedItemSlots = new List<int>();
        unlockedSkills = new List<string>();
        skillLevels = new Dictionary<string, int>();
        
        hasCompletedTutorial = false;
        completedQuests = new List<string>();
        activeQuests = new List<string>();
        
        createdAt = DateTime.UtcNow.ToString("o");
        lastPlayedAt = DateTime.UtcNow.ToString("o");
        totalPlayTime = 0f;
    }
}

/// <summary>
/// Item data structure
/// </summary>
[Serializable]
public class ItemData
{
    public string itemId;
    public string itemName;
    public ItemType type;
    public int quantity;
    public int rarity; // 0=common, 1=uncommon, 2=rare, 3=epic, 4=legendary
    
    // Stats modifiers
    public int healthBonus;
    public int manaBonus;
    public int strengthBonus;
    public int defenseBonus;
    public int agilityBonus;

    public ItemData()
    {
        itemId = System.Guid.NewGuid().ToString();
        itemName = "Unknown Item";
        type = ItemType.Consumable;
        quantity = 1;
        rarity = 0;
    }
}

/// <summary>
/// Item types
/// </summary>
public enum ItemType
{
    Weapon,
    Armor,
    Accessory,
    Consumable,
    Material,
    QuestItem
}

/// <summary>
/// Base layout data structure for local storage
/// </summary>
[Serializable]
public class BaseLayoutData
{
    public string baseId;
    public List<BuildingData> buildings;
    public List<string> unlockedTechnologies;
    public ResourceInventory resources;
    
    public BaseLayoutData()
    {
        baseId = System.Guid.NewGuid().ToString();
        buildings = new List<BuildingData>();
        unlockedTechnologies = new List<string>();
        resources = new ResourceInventory();
    }
}

/// <summary>
/// Building data structure
/// </summary>
[Serializable]
public class BuildingData
{
    public string buildingId;
    public string buildingType; // e.g., "Workshop", "Storage", "Forge"
    public Vector2Int position;
    public int level;
    public bool isConstructed;
    public float constructionProgress; // 0.0 to 1.0

    public BuildingData()
    {
        buildingId = System.Guid.NewGuid().ToString();
        buildingType = "Unknown";
        position = Vector2Int.zero;
        level = 1;
        isConstructed = false;
        constructionProgress = 0f;
    }
}

/// <summary>
/// Resource inventory
/// </summary>
[Serializable]
public class ResourceInventory
{
    public int wood;
    public int stone;
    public int iron;
    public int gold;
    public int crystals;
    
    public ResourceInventory()
    {
        wood = 0;
        stone = 0;
        iron = 0;
        gold = 0;
        crystals = 0;
    }
}

/// <summary>
/// Game settings
/// </summary>
[Serializable]
public class GameSettings
{
    // Audio
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;
    
    // Graphics
    public int resolutionWidth;
    public int resolutionHeight;
    public bool fullscreen;
    public int targetFrameRate;
    public bool vSync;
    
    // Gameplay
    public bool showTutorials;
    public bool showDamageNumbers;
    public float cameraShakeIntensity;
    
    // Controls
    public Dictionary<string, KeyCode> keyBindings;

    public GameSettings()
    {
        // Audio defaults
        masterVolume = 0.8f;
        musicVolume = 0.7f;
        sfxVolume = 0.8f;
        
        // Graphics defaults
        resolutionWidth = 1920;
        resolutionHeight = 1080;
        fullscreen = true;
        targetFrameRate = 60;
        vSync = true;
        
        // Gameplay defaults
        showTutorials = true;
        showDamageNumbers = true;
        cameraShakeIntensity = 0.5f;
        
        // Default key bindings
        keyBindings = new Dictionary<string, KeyCode>
        {
            { "MoveUp", KeyCode.W },
            { "MoveDown", KeyCode.S },
            { "MoveLeft", KeyCode.A },
            { "MoveRight", KeyCode.D },
            { "Attack", KeyCode.Mouse0 },
            { "Skill1", KeyCode.Alpha1 },
            { "Skill2", KeyCode.Alpha2 },
            { "Skill3", KeyCode.Alpha3 },
            { "Interact", KeyCode.E },
            { "Inventory", KeyCode.I },
            { "Map", KeyCode.M },
            { "Pause", KeyCode.Escape }
        };
    }
}
