// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Microsoft.MSInternal", "CA905:SystemAndMicrosoftNamespacesRequireApproval", Scope = "namespace", Target = "System.Windows.Forms.VisualStyles")]

namespace System.Windows.Forms.VisualStyles
{
    public struct TextMetrics
	{
		private int height; 
		private int ascent; 
		private int descent; 
		private int internalLeading; 
		private int externalLeading; 
		private int aveCharWidth; 
		private int maxCharWidth; 
		private int weight; 
		private int overhang; 
		private int digitizedAspectX; 
		private int digitizedAspectY; 
		private char firstChar; 
		private char lastChar; 
		private char defaultChar; 
		private char breakChar; 
		private bool italic; 
		private bool underlined; 
		private bool struckOut; 
		private TextMetricsPitchAndFamilyValues pitchAndFamily; 
		private TextMetricsCharacterSet charSet; 

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.Height;"]/*' />
		public int Height 
		{ 
			get
			{
				return height;
			} set 
			  { 
				  height = value;
			  }
		} 

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.Ascent;"]/*' />
		public int Ascent 
		{ 
			get
			{
				return ascent;
			} 
			set 
			{ 
				ascent = value;
			}
		} 
		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.Descent;"]/*' />
		public int Descent 
		{ 
			get
			{
				return descent;
			} 
			set 
			{ 
				descent = value;
			}
		} 

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.InternalLeading;"]/*' />
		public int InternalLeading 
		{ 
			get
			{
				return internalLeading;
			} 
			set 
			{ 
				internalLeading = value;
			}
		} 

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.ExternalLeading;"]/*' />
		public int ExternalLeading 
		{ 
			get
			{
				return externalLeading;
			} 
			set 
			{ 
				externalLeading = value;
			}
		} 

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.AveCharWidth;"]/*' />
		public int AverageCharWidth 
		{ 
			get
			{
				return aveCharWidth;
			} 
			set 
			{ 
				aveCharWidth = value;
			}
		} 

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.MaxCharWidth;"]/*' />
		public int MaxCharWidth 
		{ 
			get
			{
				return maxCharWidth;
			} 
			set 
			{ 
				maxCharWidth = value;
			}
		} 

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.Weight;"]/*' />
		public int Weight 
		{ 
			get
			{
				return weight;
			} 
			set 
			{ 
				weight = value;
			}
		} 

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.Overhang;"]/*' />
		public int Overhang 
		{ 
			get
			{
				return overhang;
			} 
			set 
			{ 
				overhang = value;
			}
		} 

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.DigitizedAspectX;"]/*' />
		public int DigitizedAspectX 
		{ 
			get
			{
				return digitizedAspectX;
			} 
			set 
			{ 
				digitizedAspectX = value;
			}
		} 

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.DigitizedAspectY;"]/*' />
		public int DigitizedAspectY 
		{ 
			get
			{
				return digitizedAspectY;
			} 
			set 
			{ 
				digitizedAspectY = value;
			}
		} 

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.FirstChar;"]/*' />
		public char FirstChar 
		{ 
			get
			{
				return firstChar;
			} 
			set 
			{ 
				firstChar = value;
			}
		} 

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.LastChar;"]/*' />
		public char LastChar 
		{ 
			get
			{
				return lastChar;
			} 
			set 
			{ 
				lastChar = value;
			}
		} 

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.DefaultChar;"]/*' />
		public char DefaultChar 
		{ 
			get
			{
				return defaultChar;
			} 
			set 
			{ 
				defaultChar = value;
			}
		} 

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.BreakChar;"]/*' />
		public char BreakChar 
		{ 
			get
			{
				return breakChar;
			} 
			set 
			{ 
				breakChar = value;
			}
		} 

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.Italic;"]/*' />
		public bool Italic 
		{ 
			get
			{
				return italic;
			} 
			set 
			{ 
				italic = value;
			}
		} 

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.Underlined;"]/*' />
		public bool Underlined 
		{ 
			get
			{
				return underlined;
			} 
			set 
			{ 
				underlined = value;
			}
		} 

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.StruckOut;"]/*' />
		public bool StruckOut 
		{ 
			get
			{
				return struckOut;
			} 
			set 
			{ 
				struckOut = value;
			}
		} 

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.PitchAndFamily;"]/*' />
		public TextMetricsPitchAndFamilyValues PitchAndFamily 
		{ 
			get
			{
				return pitchAndFamily;
			} 
			set 
			{ 
				pitchAndFamily = value;
			}
		} 

		/// <include file='doc\VisualStyleTypesAndProperties.uex' path='docs/doc[@for="TextMetrics.CharSet;"]/*' />
		public TextMetricsCharacterSet CharSet 
		{ 
			get
			{
				return charSet;
			} 
			set 
			{ 
				charSet = value;
			}
		}
	}

    internal struct VisualStyleDocProperty
    {
        internal static string DisplayName = "DisplayName";
        internal static string Company = "Company";
        internal static string Author = "Author";
        internal static string Copyright = "Copyright";
        internal static string Url = "Url";
        internal static string Version = "Version";
        internal static string Description = "Description";
    }

    internal struct VisualStyleSystemProperty
    {
        internal static int SupportsFlatMenus = 1001;
        internal static int MinimumColorDepth = 1301;
    }
}
