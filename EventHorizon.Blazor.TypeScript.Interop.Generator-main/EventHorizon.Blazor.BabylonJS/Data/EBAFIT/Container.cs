using System;
using System.Collections.Generic;
using System.Linq;

namespace EventHorizon.Blazor.BabylonJS.Data.EBAFIT
{
    public class Container
    {
        private decimal volume;

		public Container(int id, decimal length, decimal width, decimal height)
		{
			this.ID = id;
			this.Length = length;
			this.Width = width;
			this.Height = height;
			this.Volume = length * width * height;
		}

		public int ID { get; set; }

		public decimal Length { get; set; }

		public decimal Width { get; set; }

		public decimal Height { get; set; }

		public decimal Volume
		{
			get
			{
				return this.volume;
			}
			set
			{
				this.volume = value;
			}
		}
	}
}
