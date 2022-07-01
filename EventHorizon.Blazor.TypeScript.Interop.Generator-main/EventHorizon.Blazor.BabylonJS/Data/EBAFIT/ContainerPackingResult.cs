using System;
using System.Collections.Generic;
using System.Linq;

namespace EventHorizon.Blazor.BabylonJS.Data.EBAFIT
{

	public class ContainerPackingResult
	{


		public ContainerPackingResult()
		{
			this.AlgorithmPackingResults = new List<AlgorithmPackingResult>();
		}

		public int ContainerID { get; set; }


		public List<AlgorithmPackingResult> AlgorithmPackingResults { get; set; }


	}
}
