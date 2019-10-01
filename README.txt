GameAI
Homework (Project) 2: 
Haoran Hu
Eric Xu

Usage:
Numberkeys: 1, 2, 3, 4.
Key 'R': (capitalize) Regenerate trees. (can be used in every state)
Arrow keys: apply force to the player(white ball) in the corresponding direction(Desired to be used in state 2 and 4).
Mouse: select and move the player. (carefull not to drag it outside of the map)

	To achieve wall and tree avoidence, the agent(wolf/hunter) will cast 2 sets of rays in face and velocity direction. Each direction has 3 rays which will
be displayed when hitting obstacles. The user might not be able to see the rays sometime since the following up drawing hint will be displayed right after
the ray collision.

The brief idea about the algorthms used in this project:
Wall avoidence:
	Calculate a new target position on the normal vector of the hit surface starting from hit point, then seek this new target.
	Rays will be displayed when wall is detected.
Tree avoidence:
	Evade from the hit point on the tree.
	The tree that the agent want to evade from will be in the origin of the circle. The radius of the circle is considered dangerous zone. 
	The circle disappears once the agent gets out.

Collision prediction:
	This algorithm aims to predict whether a collision might occur or not in the future. It calculates the time when in the future that the two agents will
	be at nearest distance.
	In order to use this algorithm more intelligently, the "time in the future" and the nearest distance will be compared with certain threshold. If the distance and
	the time is within the threshold, the evade algorthim will be used for agents to evade from the calculated nearest position respectively.
	


<State 0> 
Description:
	Initial state


<State 1> 
Description:
	The wolf agent perform dynamic wander with wall/tree avoidence.
	
<State 2>
Description:
	The wolf agent perform dynamic chase and arrive with wall/tree avoidence.

<State 3>
Description:
	The wolf agent perform dynamic evade with with wall/tree avoidence.
	The hunter agent perform dynamic pursue with with wall/tree avoidence.

<State 4>
Description:
	In order to see collision prediction algorithm more clearly, tress are destroyed in this stage. 
	Both wolf and hunter chase the player. When they calculate the collision will happen based on the collision prediction algorithm, they will perform evade based
	on the calculated nearest position respectively. Once they're furthur apart, they go back to chase algorithm.
	

How to be more intelligent?
	Since Eric and I are still in learning stage about unity and c# and limited time table, some cornor case beyond above algorithms might occur and will not 
	be handled properly. But here's some idea that might be implemented in the future.

	For example, agents might get stucked at a right angle shaped wall. To solve this, an array of positions in the passed several seconds can be recorded to 
	derive a recent average position. If the position in the next frame is still close to this average recent position, then we apply a force that is on the opposite
	direction of the wall therefore escape the cornor.

	

