# DStartAlgorithm
This algorithms solves the assumption-based path planning problems, including planning with the freespace assumption where a robot has to navigate to given goal coordinates in unknown terrain. It makes assumptions about the unknown part of the terrain (for example: that it contains no obstacles) and finds a shortest path from its current coordinates to the goal coordinates under these assumptions. The robot then follows the path. When it observes new map information (such as previously unknown obstacles), it adds the information to its map and, if necessary, replans a new shortest path from its current coordinates to the given goal coordinates. It repeats the process until it reaches the goal coordinates or determines that the goal coordinates cannot be reached. When traversing unknown terrain, new obstacles may be discovered frequently, so this replanning needs to be fast. Incremental (heuristic) search algorithms speed up searches for sequences of similar search problems by using experience with the previous problems to speed up the search for the current one. Assuming the goal coordinates do not change, all three search algorithms are more efficient than repeated A* searches.

D* has been widely used for mobile robot and autonomous vehicle navigation. Current systems are typically based on D* Lite rather than the original D* or Focused D*. In fact, even Stentz's lab uses D* Lite rather than D* in some implementations. Such navigation systems include a prototype system tested on the Mars rovers Opportunity and Spirit and the navigation system of the winning entry in the DARPA Urban Challenge, both developed at Carnegie Mellon University.

##How does it work?
The algorithm works by iteratively selecting a node from the OPEN list and evaluating it. It then propagates the node's changes to all of the neighboring nodes and places them on the OPEN list. This propagation process is termed "expansion". In contrast to canonical A*, which follows the path from start to finish, D* begins by searching backwards from the goal node. Each expanded node has a backpointer which refers to the next node leading to the target, and each node knows the exact cost to the target. When the start node is the next node to be expanded, the algorithm is done, and the path to the goal can be found by simply following the backpointers.

##Obstacle handling
When an obstruction is detected along the intended path, all the points that are affected are again placed on the OPEN list, this time marked RAISE. Before a RAISED node increases in cost, however, the algorithm checks its neighbors and examines whether it can reduce the node's cost. If not, the RAISE state is propagated to all of the nodes' descendants, that is, nodes which have backpointers to it. These nodes are then evaluated, and the RAISE state is passed on, forming a wave. When a RAISED node can be reduced, its backpointer is updated, and passes the LOWER state to its neighbors. These waves of RAISE and LOWER states are the heart of D*.

By this point, a whole series of other points are prevented from being "touched" by the waves. The algorithm has therefore only worked on the points which are affected by change of cost.

##Scrrenshots
![ph01](https://cloud.githubusercontent.com/assets/17988691/21746267/d1bc9490-d50c-11e6-978e-c72746c72dbc.JPG)

![ph02](https://cloud.githubusercontent.com/assets/17988691/21746269/f2dd845e-d50c-11e6-9aaa-bf459254df91.JPG)

![ph03](https://cloud.githubusercontent.com/assets/17988691/21746275/06da8ae2-d50d-11e6-9ab8-92110b089a74.JPG)

![ph05](https://cloud.githubusercontent.com/assets/17988691/21746276/06dc3fe0-d50d-11e6-82e4-d49f7db12302.JPG)
