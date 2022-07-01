using System;
using System.Collections.Generic;
using System.Linq;

namespace EventHorizon.Blazor.BabylonJS.Data.EBAFIT
{
	public class Item
	{


		private decimal volume;

		public Item(int id, decimal dim1, decimal dim2, decimal dim3, int quantity)
		{
			this.ID = id;
			this.Dim1 = dim1;
			this.Dim2 = dim2;
			this.Dim3 = dim3;
			this.volume = dim1 * dim2 * dim3;
			this.Quantity = quantity;
		}

		public int ID { get; set; }


		public bool IsPacked { get; set; }


		public decimal Dim1 { get; set; }


		public decimal Dim2 { get; set; }


		public decimal Dim3 { get; set; }


		public decimal CoordX { get; set; }


		public decimal CoordY { get; set; }

		public decimal CoordZ { get; set; }

		public int Quantity { get; set; }

		public decimal PackDimX { get; set; }

		public decimal PackDimY { get; set; }

		public decimal PackDimZ { get; set; }

		public decimal Volume
		{
			get
			{
				return volume;
			}
		}
	}
}
