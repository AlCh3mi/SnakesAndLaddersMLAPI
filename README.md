# Snakes And Ladders MLAPI
 Network Multiplayer Game made with Unity's MLAPI

### Technology:

- MLAPI is Unity's new Multiplayer API, its still experimental and came with many little issues that were not well documented and still in development. 

### Process:

- I started out researching, reading, and watching all available documentation and tutorials on MLAPI that I could find.
- I downloaded a few sample projects provided by Unity to examine to try better my understanding of its framework.
- I made prototypes of mulitple types of games, and features of MLAPI for testing, and trying to synchronise animations, and network transforms. 
- Movement almost always seemed laggy no matter what i did, with the exception of using NetworkNavMesh.
- I got a system working for authenticated login, but decided to not use it in my final game.
- There were many new concepts for me to wrap my head around, and it took LOTS of time before i became more comfortable with what i was doing.
 - RPC's (Remote Procedure Calls)
 - Network Serializing
 - Client and Server side logic
- Overall it felt very limiting and challenging to work within that multiplayer framework.


### Game:

- I ended up choosing snakes and ladders because i thought it would demonstrate some knowledge of my chosen new Technology, and due to time restraints and too many discarded prototypes and tests. 


### Features Used:

- NetworkVariables (built in types)
- NetworkVariables (Made my own type with their INetworkSerializable interface
- Setting the read and write access of these network sync'd variables
- NetworkTransforms (for the player movement)
- Remote Procedure Calls (Client and Server RPC's)
- Attributes such as [ClientRpc] and using their parameters to set delivery methods to reliable for eg. Or if players require Ownership of the said object in order to be able to use said RPC
