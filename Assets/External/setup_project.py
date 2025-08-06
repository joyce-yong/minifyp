import os

folders = [
    "Assets/Art/Models",
    "Assets/Art/Textures",
    "Assets/Art/Materials",
    "Assets/Art/UI",
    "Assets/Audio/Music",
    "Assets/Audio/SFX",
    "Assets/Scripts/Core",
    "Assets/Scripts/Systems",
    "Assets/Scripts/UI",
    "Assets/Scenes/Main",
    "Assets/Scenes/Levels",
    "Assets/Prefabs/Characters",
    "Assets/Prefabs/Environment",
    "Assets/Prefabs/UI",
    "Assets/Resources",
    "Assets/Shaders",
    "Assets/Plugins",
    "ProjectSettings",
    "Packages"
]

for folder in folders:
    os.makedirs(folder, exist_ok=True)
