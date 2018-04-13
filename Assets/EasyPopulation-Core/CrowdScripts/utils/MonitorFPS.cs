//#define ProfileAstar
using UnityEngine;
using System.Collections;
using System.Text;

namespace Crowd {
public class MonitorFPS: MonoBehaviour {
	
//	public int yOffset = 5;
	
//	public bool show = false;
//	public bool showInEditor = false;
	
//	public bool showFPS = true;
//	public bool showPathProfile =true;
//	public bool showMemProfile = true;
//	public bool showGraph = true;

//	public int graphBufferSize = 200;

//	/** Font to use.
//	 * A monospaced font is the best
//	 */
//	public Font font = null;
//	public int fontSize = 12;
	
//	StringBuilder text = new StringBuilder ();
//	string cachedText;
//	float lastUpdate = -999;

	
//	private GraphPoint[] graph;
	
//	struct GraphPoint {
//		public float fps, memory;
//		public bool collectEvent;
//	}
	
//	private float delayedDeltaTime = 1;
//	private float lastCollect = 0;
//	private float lastCollectNum = 0;
//	private float delta = 0;
//	private float lastDeltaTime = 0;
//	private int allocRate = 0;
//	private int lastAllocMemory = 0;
//	private float lastAllocSet = -9999;
//	private int allocMem = 0;
//	private int collectAlloc = 0;
//	private int peakAlloc = 0;
	
//	private int fpsDropCounterSize = 200;
//	private float[] fpsDrops;

//	private Rect boxRect;
	
//	private GUIStyle style;
	
//	private Camera cam;
	

//	float graphHeight = 100;
//	float graphOffset = 50;

	
//	//----------------------------------------------------------------------------------------
//	public void Start () {
		
//		useGUILayout = false;
		
//		fpsDrops = new float[fpsDropCounterSize];

//		cam = GetComponent<Camera>();
//		if (cam == null) {
//			cam = Camera.main;
//		}
		
//		graph = new GraphPoint[graphBufferSize];
		
//		for (int i=0;i<fpsDrops.Length;i++) {
//			fpsDrops[i] = 1F / Time.deltaTime;
//		}
//	}
//	int maxVecPool = 0;
//	int maxNodePool = 0;

//	//----------------------------------------------------------------------------------------
//	public void Update () {

//		//if (Global.isMonitoringFPS )
//		//		show=!show;

//		if (!Global.isMonitoringFPS  || (!Application.isPlaying && !showInEditor)) return;
		
//		int collCount = System.GC.CollectionCount (0);
		
//		if (lastCollectNum != collCount) {
//			lastCollectNum = collCount;
//			delta = Time.realtimeSinceStartup-lastCollect;
//			lastCollect = Time.realtimeSinceStartup;
//			lastDeltaTime = Time.deltaTime;
//			collectAlloc = allocMem;
//		}
		
//		allocMem = (int)System.GC.GetTotalMemory (false);
		
//		bool collectEvent = allocMem < peakAlloc;
//		peakAlloc = !collectEvent ? allocMem : peakAlloc;
		
//		if (Time.realtimeSinceStartup - lastAllocSet > 0.3F || !Application.isPlaying) {
//			int diff = allocMem - lastAllocMemory;
//			lastAllocMemory = allocMem;
//			lastAllocSet = Time.realtimeSinceStartup;
//			delayedDeltaTime = Time.deltaTime;
			
//			if (diff >= 0) {
//				allocRate = diff;
//			}
//		}
		
//		if (Application.isPlaying) {
//			fpsDrops[Time.frameCount % fpsDrops.Length] = Time.deltaTime != 0 ? 1F / Time.deltaTime : float.PositiveInfinity;
//			int graphIndex = Time.frameCount % graph.Length;
//			graph[graphIndex].fps = Time.deltaTime < Mathf.Epsilon ? 0 : 1F / Time.deltaTime;
//			graph[graphIndex].collectEvent = collectEvent;
//			graph[graphIndex].memory = allocMem;
//		}
		
//		if (Application.isPlaying && cam != null && showGraph) {
			
////			graphWidth = cam.pixelWidth*0.8f;			
			
//			float minMem = float.PositiveInfinity, maxMem = 0, minFPS = float.PositiveInfinity, maxFPS = 0;
//			for (int i=0;i<graph.Length;i++) {
//				minMem = Mathf.Min (graph[i].memory, minMem);
//				maxMem = Mathf.Max (graph[i].memory, maxMem);
//				minFPS = Mathf.Min (graph[i].fps, minFPS);
//				maxFPS = Mathf.Max (graph[i].fps, maxFPS);
//			}
			
//	//			int currentGraphIndex = Time.frameCount % graph.Length;			
//	//		Matrix4x4 m = Matrix4x4.TRS (new Vector3 ((cam.pixelWidth - graphWidth)/2f, graphOffset,1), Quaternion.identity, new Vector3 (graphWidth, graphHeight, 1));
//		}
		
//	}
//	//-----------------------------------------------------------------------------------------------------
//	public void OnGUI () {
		
//			if (Global.isMonitoringFPS ) {
//				if (style == null) {
//					style = new GUIStyle ();
//					style.normal.textColor = Color.white;
//					style.padding = new RectOffset (5, 5, 5, 5);
//				}
//                //	GUILayout.FlexibleSpace();
//                if (Time.realtimeSinceStartup - lastUpdate > 0.5f || cachedText == null || !Application.isPlaying)
//                {
//                    lastUpdate = Time.realtimeSinceStartup;

//                    boxRect = new Rect(5, yOffset, 310, 40);

//                    text.Length = 0;


//                    if (showMemProfile)
//                    {
//                        boxRect.height += 200;

//                        text.AppendLine();
//                        text.AppendLine();
//                        text.Append("Currently allocated".PadRight(25));
//                        text.Append((allocMem / 1000000F).ToString("0.0 MB"));
//                        text.AppendLine();

//                        text.Append("Peak allocated".PadRight(25));
//                        text.Append((peakAlloc / 1000000F).ToString("0.0 MB")).AppendLine();

//                        text.Append("Last collect peak".PadRight(25));
//                        text.Append((collectAlloc / 1000000F).ToString("0.0 MB")).AppendLine();

//                        text.Append("Allocation rate".PadRight(25));
//                        text.Append((allocRate / 1000000F).ToString("0.0 MB")).AppendLine();

//                        text.Append("Collection frequency".PadRight(25));
//                        text.Append(delta.ToString("0.00"));
//                        text.Append("s\n");

//                        text.Append("Last collect fps".PadRight(25));
//                        text.Append((1F / lastDeltaTime).ToString("0.0 fps"));
//                        text.Append(" (");
//                        text.Append(lastDeltaTime.ToString("0.000 s"));
//                        text.Append(")");
//                    }

//                    if (showFPS)
//                    {
//                        text.AppendLine();
//                        text.AppendLine();
//                        text.Append("FPS".PadRight(25)).Append((1F / delayedDeltaTime).ToString("0.0 fps"));

//                        float minFps = Mathf.Infinity;

//                        for (int i = 0; i < fpsDrops.Length; i++)
//                            if (fpsDrops[i] < minFps)
//                                minFps = fpsDrops[i];

//                        text.AppendLine();
//                        text.Append(("Lowest fps (last " + fpsDrops.Length + ")").PadRight(25)).Append(minFps.ToString("0.0"));
//                    }

//                    /*deprecated					if (showPathProfile) {
//                                            AstarPath astar = AstarPath.active;

//                                            text.AppendLine ();

//                                            if (astar == null) {
//                                                text.Append ("\nNo AstarPath Object In The Scene");
//                                            } else {

//                                                if (Pathfinding.Util.ListPool<Vector3>.GetSize () > maxVecPool)
//                                                    maxVecPool = Pathfinding.Util.ListPool<Vector3>.GetSize ();
//                                                if (Pathfinding.Util.ListPool<Pathfinding.GraphNode>.GetSize () > maxNodePool)
//                                                    maxNodePool = Pathfinding.Util.ListPool<Pathfinding.GraphNode>.GetSize ();

//                                                text.Append ("\nPool Sizes (size/total created)");
//                                            }
//                                        }


//                                        cachedText = text.ToString ();
//                                    }

//                                    if (font != null) {
//                                        style.font = font;
//                                        style.fontSize = fontSize;
//                                    }

//                                    boxRect.height = style.CalcHeight (new GUIContent (cachedText), boxRect.width);

//                                    GUI.Box (boxRect, "");
//                                    GUI.Label (boxRect, cachedText, style);



//                                    float minMem = float.PositiveInfinity, maxMem = 0, minFPS = float.PositiveInfinity, maxFPS = 0;
//                                    for (int i=0; i<graph.Length; i++) {
//                                        minMem = Mathf.Min (graph [i].memory, minMem);
//                                        maxMem = Mathf.Max (graph [i].memory, maxMem);
//                                        minFPS = Mathf.Min (graph [i].fps, minFPS);
//                                        maxFPS = Mathf.Max (graph [i].fps, maxFPS);
//                                    }

//                                    float line;
//                                    GUI.color = Color.blue;

//                                    //Round to nearest x.x MB
//                                    line = Mathf.RoundToInt (maxMem / (100.0f * 1000)); // *1000*100
//                                    GUI.Label (new Rect (5, Screen.height - AstarMath.MapTo (minMem, maxMem, 0 + graphOffset, graphHeight + graphOffset, line * 1000 * 100) - 10, 100, 20), (line / 10.0f).ToString ("0.0 MB"));

//                                    line = Mathf.Round (minMem / (100.0f * 1000)); // *1000*100
//                                    GUI.Label (new Rect (5, Screen.height - AstarMath.MapTo (minMem, maxMem, 0 + graphOffset, graphHeight + graphOffset, line * 1000 * 100) - 10, 100, 20), (line / 10.0f).ToString ("0.0 MB"));

//                                    GUI.color = Color.green;
//                                    //Round to nearest x.x MB
//                                    line = Mathf.Round (maxFPS); // *1000*100
//                                    GUI.Label (new Rect (55, Screen.height - AstarMath.MapTo (minFPS, maxFPS, 0 + graphOffset, graphHeight + graphOffset, line) - 10, 100, 20), (line).ToString ("0 FPS"));

//                                    line = Mathf.Round (minFPS); // *1000*100
//                                    GUI.Label (new Rect (55, Screen.height - AstarMath.MapTo (minFPS, maxFPS, 0 + graphOffset, graphHeight + graphOffset, line) - 10, 100, 20), (line).ToString ("0 FPS"));

//                        */
//                }
//            }
//		}
	}//eo class
}//eo package

