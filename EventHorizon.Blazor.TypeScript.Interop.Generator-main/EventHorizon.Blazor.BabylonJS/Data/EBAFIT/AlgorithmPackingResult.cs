using System;
using System.Collections.Generic;
using System.Linq;

namespace EventHorizon.Blazor.BabylonJS.Data.EBAFIT
{

	public class AlgorithmPackingResult
	{
		#region Constructors

		public AlgorithmPackingResult()
		{
			this.PackedItems = new List<Item>();
			this.UnpackedItems = new List<Item>();
		}

		#endregion Constructors

		#region Public Properties


		public int AlgorithmID { get; set; }

	
		public string AlgorithmName { get; set; }


		public bool IsCompletePack { get; set; }


		public List<Item> PackedItems { get; set; }

		public long PackTimeInMilliseconds { get; set; }


		public decimal PercentContainerVolumePacked { get; set; }

	
		public decimal PercentItemVolumePacked { get; set; }

		public List<Item> UnpackedItems { get; set; }

		#endregion Public Properties
	}
}
