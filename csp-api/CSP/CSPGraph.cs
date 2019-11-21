using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace csp_api.CSP
{
	public class CSPGraph
	{
		public List<Vertex> Vertices { get; set; }
		public List<Edge> Edges { get; set; }

		private Func<Dictionary<string, byte>, CSPGraph, Vertex> _selectUnassignedVariable;
		private Func<Vertex, Dictionary<string, byte>, CSPGraph, List<byte>> _orderDomainValues;
		private Func<Vertex, byte, Dictionary<string, byte>, CSPGraph, bool>? _inference;

		internal CSPGraph(List<string> nodes, Dictionary<string, List<string>> constraints, List<string> domain,
			Func<Dictionary<string, byte>, CSPGraph, Vertex> selectUnassignedVariable,
			Func<Vertex, Dictionary<string, byte>, CSPGraph, List<byte>> orderDomainValues,
			Func<Vertex, byte, Dictionary<string, byte>, CSPGraph, bool>? inference)
		{
			this.Vertices = nodes.Select(node => new Vertex(node, Enumerable.Range(0, domain.Count).Select(num => (byte)num).ToList())).ToList();
			this.Edges = new List<Edge>();

			_initEdges(constraints);

			this._selectUnassignedVariable = selectUnassignedVariable;
			this._orderDomainValues = orderDomainValues;
			this._inference = inference;
		}

		private void _initEdges(Dictionary<string, List<string>> constraints)
		{
			foreach (var (node, neighbors) in constraints)
			{
				var nodeVertex = this.Vertices.Find(vertex => vertex.Element == node);
				if (nodeVertex is null)
				{
					throw new Exception("Node vertex not found, this should not happen");
				}

				foreach (var neighbor in neighbors)
				{
					var neighborVertex = this.Vertices.Find(vertex => vertex.Element == neighbor);
					if (neighborVertex is null)
					{
						throw new Exception("Neighbor vertex not found, this should not happen");
					}

					this.Edges.Add(new Edge(nodeVertex, neighborVertex));
				}
			}
		}

		public CSPResult RunCSP()
		{
			var success = false;
			var assignments = new Dictionary<string, byte>();
			var numBacktracks = 0;
			var stopwatch = new Stopwatch();

			stopwatch.Start();
			success = _cspHelper(assignments, ref numBacktracks);
			stopwatch.Stop();

			var elapsedTime = stopwatch.ElapsedTicks / 10000;

			var results = new CSPResult(success, assignments, numBacktracks, elapsedTime);

			return results;
		}

		private bool _cspHelper(Dictionary<string, byte> assignments, ref int numBacktracks)
		{
			if (assignments.Count == this.Vertices.Count)
			{
				return true;
			}

			var unassigned = _selectUnassignedVariable(assignments, this);

			foreach (var value in _orderDomainValues(unassigned, assignments, this))
			{
				List<Vertex>? previousVertices = null;
				List<Edge>? previousEdges = null;
				if (_inference != null)
				{
					previousVertices = this.Vertices.Select(vertex => new Vertex(vertex.Element, new List<byte>(vertex.Domain))).ToList();
					previousEdges = this.Edges.Select(edge =>
					{
						var vVertex = previousVertices.Find(vertex => vertex.Element == edge.EndVertices.start.Element);
						var uVertex = previousVertices.Find(vertex => vertex.Element == edge.EndVertices.end.Element);

						if (vVertex is null || uVertex is null)
						{
							throw new Exception("A vertex is null, this should not happen");
						}

						return new Edge(vVertex, uVertex);
					}).ToList();

					_assign(assignments, unassigned.Element, value);
					var inferenceSuccess = _inference(unassigned, value, assignments, this);
					if (!inferenceSuccess)
					{
						_backtrack(assignments, unassigned.Element, previousVertices, previousEdges, ref unassigned, ref numBacktracks);
						continue;
					}
					var success = _cspHelper(assignments, ref numBacktracks);

					if (!success)
					{
						_backtrack(assignments, unassigned.Element, previousVertices, previousEdges, ref unassigned, ref numBacktracks);
					}
					else
					{
						return true;
					}
				}
				else
				{
					_assign(assignments, unassigned.Element, value);
					if (_isConsistent(unassigned, value, assignments))
					{
						var success = _cspHelper(assignments, ref numBacktracks);
						if (!success)
						{
							_backtrack(assignments, unassigned.Element, previousVertices, previousEdges, ref unassigned, ref numBacktracks);
						}
						else
						{
							return true;
						}
					}
					else
					{
						_backtrack(assignments, unassigned.Element, previousVertices, previousEdges, ref unassigned, ref numBacktracks);
					}
				}

			}

			return false;
		}

		private bool _isConsistent(Vertex unassigned, byte value, Dictionary<string, byte> assignments)
		{
			return this.Edges.Where(edge => edge.EndVertices.start == unassigned)
							.Select(edge => edge.EndVertices.end)
							.Where(vertex => assignments.ContainsKey(vertex.Element))
							.Select(neighbor => assignments[neighbor.Element])
							.All(assignedValues => assignedValues != value);
		}

		private void _assign(Dictionary<string, byte> assignments, string element, byte value)
		{
			assignments.Add(element, value);
		}

		private void _backtrack(Dictionary<string, byte> assignments, string element,
			List<Vertex>? previousVertices, List<Edge>? previousEdges, ref Vertex unassigned, ref int numBacktracks)
		{
			numBacktracks++;
			assignments.Remove(element);

			if (previousVertices != null && previousEdges != null)
			{
				this.Vertices = previousVertices;
				this.Edges = previousEdges;

				var unElement = unassigned.Element;
				var newVertex = this.Vertices.Find(vertex => unElement == vertex.Element);
				if (newVertex is null)
				{
					throw new Exception("New vertex could not be found, this should not HAPPEN");
				}
				unassigned = newVertex;
			}
		}
	}
}
