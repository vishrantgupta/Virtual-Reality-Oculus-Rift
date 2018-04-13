=====================================HOW TO SETUP A SCENE WITH EASY POPULATION=============================================================================

=====================================PREPARATORY STEPS=====================================================================================================

1- Define the Navmesh (in Window>Navmesh) to configure the walkable areas of the terrain. Before backing, in the Bake tab, check the "Height Mesh" option under Advanced.  
2- Import the package 

3.1- Import all the models of characters you will need. These should be rigged and stored in the folder Assets>Resources>EasyPopulation . 
3.2- Once you have imported the models, set them to be rigged as Humanoid (Animation type=Humanoid, in the Rig separator of the animation settings). 

4.1- If you need workers in your scene, import the fbx models corresponding to their actions. 
4.2- Set the animation type of these animations to be marked as Humanoid, as well (Animation type=Humanoid, in the Rig separator of the animation settings). 
4.3- Define the unique name of these clips. These names are the identifiers you will be using (In the separator Animations of the animation settings). 

=====================================HOW TO CREATE THE POPULATION?=========================================================================================

Be sure you have completed the step 3.2 

*Create the AI generator
5.1 - Create an empty gameObject. 
5.2 - Then, add a script component: Population Manager.
5.3 - Double check if both the gameobject and the script are active. This step can save you hours of debugging.  

*Define the anchors - this is where individuals will appear and will constitute locomotion waypoints
6.1 - Add an anchor to the scene (Press Add anchor and click on the scene) 
6.2 - You can calibrate the radius of the area where character will show up, with influence radius.
6.3 - You can also calibrate the height of the anchors with the Height Anchors
6.4 - Anchors can be hidden, by unchecking Visualize Anchors.  

*Define the characters constituting the population
7.1 - In Quantity, define the total amount of character you will need  
7.2 - In Templates Citizens, assign the models prepared in 3.2. Say how many templates you will be using and specify their models.   

=====================================HOW TO CREATE A SINGLE ACTION WORKER?=================================================================================
==This is a worker (or group of workers) performing the same repetitive action in one spot ================================================================

Make sure you have completed steps 3.2, 4.2 and 4.3

8- Add an Achor Key to define where the character or group of characters will show up. Press Add Keys, and place the anchor in the desired location in the 
stage. 
9- Define the type as ONE-state
10- In Count, specify how many characters you want at this place
11- In Actor, specify the model you want to use  
12- Define where it will show up, dragging the anchor placed at step 8 to Base Anchor.
13- In Animation Producer, define what clip will you be using as animation (make sure this is marked as humanoid, as specified in step 4.2). 

=====================================HOW TO CREATE A TWO STATE ACTION WORKER?===============================================================================
==This is a worker performing one action in one spot, then walking to a second spot and performing a second action over there. Then he returns to the first
spot and repeats ===========================================================================================================================================

Be sure you have completed the steps 3.2, 4.2 and 4.3

14- Add two Achor Keys to define where the character(s) will show up and where he will move to.
15- Define type as TWO-state
16- Specify how many characters you want at this place, in Count.
17- Specify the model you want to use, in Actor. 
18- Specify where it will show up, dragging the first anchor placed at step 14 to Base Anchor.
19- In Animation Producer, define what clip will you be using for the initial stage (make sure this is marked as humanoid, as specified in step 4.2).   
20- Define the second location where this character will be moving, after completing the animation of state 19. To do this, drag the second anchors placed at 
step 14 to Delivery Anchor.
21- In Animation Transaction, define what clip will you be using for the delivery stage (make sure this is marked as humanoid). 

============================================================================================================================================================
Disclaimer 
This project is free for use in research and educational projects. If you publish results obtained using this data, we would appreciate if you would send the 
citation to your published paper to rfantunes@fc.ul.pt, and also would add this text to your acknowledgments section:
The animation generator was created with funding from the European Union's Horizon 2020 research and innovation programme under the
 Marie Sklodowska-Curie grant agreement No 655226. 
Some of the motion data used in this project was obtained from mocap.cs.cmu.edu, and rigged with Mixamo.com. 
The database was created with funding from NSF EIA-0196217. 

Citation
Antunes, R.F and Cláudio, A.P and Carmo, M.B. and Correia Luís, Animating with a Self-Organizing Population the Reconstruction of Medieval Mértola, in Eurographics Workshop in Graphics and Cultural Heritage, p. 1-10, 2017.

============================================================================================================================================================
This project has received funding from the European Union's Horizon 2020 research and innovation programme under the Marie Sklodowska-Curie grant agreement 
No 655226. 
============================================================================================================================================================
