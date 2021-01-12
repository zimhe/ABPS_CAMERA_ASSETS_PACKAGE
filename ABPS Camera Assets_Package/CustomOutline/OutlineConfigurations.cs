using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace CustomOutline
{
	[Serializable]
	public struct OutlineStyle
	{
		[Range(1.0f, 6.0f)]
		public float lineThickness;
		[Range(0, 10)]
		public float lineIntensity;
		[Range(0, 1)]
		public float fillIntensity;

		public Color lineColor;

		public Color fillColor;

		public bool useFillColor;
	}
   
	[CreateAssetMenu(fileName ="Outline Configurations",menuName ="Outline/Outline Configuratation")]
    public  class OutlineConfigurations:ScriptableObject
    {
		public OutlineStyle[] outlineStyles;

		
	}
}
