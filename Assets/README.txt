Message for Snowprint Team:

I developed this project about three months ago as a case project. The goal was to create a mock mobile game with a liveops event
called "Ticket Hunt". The requirements were:

- Implement a currency system to be used with the event that can support multiple currencies and the ability to add new currencies easily.
- An event system that can be setup externally through a json, has data for rewards to be given and has data persistence for player progress.
Interfaces:
- A top bar UI to show player currencies. XP and Settings can just be visuals.
- Mock Gameplay UI: 
    - Simulate Gameplay button which removes 10 money and rewards a random amount of tickets, with a 5 second cooldown.
    - Ability to modify the time remaining for the event for testing purposes.
    - Ability to add money for testing purposes.
- Event Popup UI to show event progress and rewards.
- Event HUD UI to show event progress and time remaining, should be properly when the owned ticket amount changes with proper animations.
It should open the event popup when tapped.
- Ticket animations to be nicely animated and fly on the screen when earned.
Third party plugins were allowed.

--------------------------------

This README contains some information about how to navigate the project

// Project Structure
The project is organized into GlobalContext and GameContext.
GlobalContext is initialized during the InitScene, GameContext is initialized during the MainScene.

// Launching the game
The game must be launched from the InitScene, which will set up the GlobalContext and then transition to the MainScene where the GameContext is initialized.

// Save Game Handling
Save game data is managed through IGameStateHandler, which currently has the PlayerPrefs implementation for the sake of simplicity.
The save game can be reset through Edit -> Clear PlayerPrefs in the Unity Editor.

// Asset Handling
Assets are mainly managed through Resources for simplicity, ideally this would be handled through an interface to allow for different implementations (e.g., Addressables).
I initially intended to implemented an IAssetHandler interface, but integrating it with Zenject installations would've been too complex for this project.

// Event Data
Event data is loaded from a JSON file located in the Resources folder, through an implementation of the ILiveOpsLoader interface.
The file is called "TicketHuntData.json", it can be edited directly. Alternatively, the "Json Helper.asset" scriptable object can be used to edit the data in the inspector.

// Game Data
Game data, including currencies, is handled through assets under the "Data" folder. These assets are ScriptableObjects that can be edited in the inspector.


For this project, I paid special attention to UI responsiveness and adaptability to different screen sizes and aspect ratios,
and draw call optimizations through the use of Sprite Atlases. This is usually a trade-off between memory usage and performance,
for this project I prioritized performance as an example and kept each panel in a separate atlas (meaning there's multiple copies of the same sprites
in the project) to minimize draw calls.

I also paid attention to separation of game logic and UI logic, to make the code more maintainable and testable. (Although I didn't write unit tests for this project due to time constraints).