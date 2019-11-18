namespace csp_api.CSP
{
	public class Edge
	{
		public (Vertex start, Vertex end) EndVertices { get; set; }

		public Edge(Vertex v, Vertex u)
		{
			EndVertices = (v, u);
		}
	}
}
