﻿using System;
using System.Runtime.InteropServices;

namespace Tesseract
{
	/// <summary>
	/// Represents a colormap.
	/// </summary>
	/// <remarks>
	/// Once the colormap is assigned to a pix it is owned by that pix and will be disposed off automatically 
	/// when the pix is disposed off.
	/// </remarks>
	public sealed class PixColormap: IDisposable
	{
		private HandleRef handle;

		internal PixColormap(IntPtr handle)
		{
			this.handle = new HandleRef(this,handle);
		}

		public static PixColormap Create(int depth)
		{
			if (!(depth == 1 || depth == 2 || depth == 4 || depth == 8))
			{
				throw new ArgumentOutOfRangeException(nameof(depth),"Depth must be 1, 2, 4, or 8 bpp.");
			}

			var handle = Interop.LeptonicaApiSignatures.pixcmapCreate(depth);
			if (handle == IntPtr.Zero)
			{
				throw new InvalidOperationException("Failed to create colormap.");
			}
			return new PixColormap(handle);
		}

		public static PixColormap CreateLinear(int depth,int levels)
		{
			if (!(depth == 1 || depth == 2 || depth == 4 || depth == 8))
			{
				throw new ArgumentOutOfRangeException(nameof(depth),"Depth must be 1, 2, 4, or 8 bpp.");
			}
			if (levels < 2 || levels > (2 << depth))
				throw new ArgumentOutOfRangeException(nameof(levels),"Depth must be 2 and 2^depth (inclusive).");

			var handle = Interop.LeptonicaApiSignatures.pixcmapCreateLinear(depth,levels);
			if (handle == IntPtr.Zero)
			{
				throw new InvalidOperationException("Failed to create colormap.");
			}
			return new PixColormap(handle);
		}

		public static PixColormap CreateLinear(int depth,bool firstIsBlack,bool lastIsWhite)
		{
			if (!(depth == 1 || depth == 2 || depth == 4 || depth == 8))
			{
				throw new ArgumentOutOfRangeException(nameof(depth),"Depth must be 1, 2, 4, or 8 bpp.");
			}

			var handle = Interop.LeptonicaApiSignatures.pixcmapCreateRandom(depth,firstIsBlack ? 1 : 0,lastIsWhite ? 1 : 0);
			if (handle == IntPtr.Zero)
			{
				throw new InvalidOperationException("Failed to create colormap.");
			}
			return new PixColormap(handle);
		}

		internal HandleRef Handle
		{
			get { return handle; }
		}

		public int Depth
		{
			get { return Interop.LeptonicaApiSignatures.pixcmapGetDepth(handle); }
		}

		public int Count
		{
			get { return Interop.LeptonicaApiSignatures.pixcmapGetCount(handle); }
		}

		public int FreeCount
		{
			get { return Interop.LeptonicaApiSignatures.pixcmapGetFreeCount(handle); }
		}

		public bool AddColor(PixColor color)
		{
			return Interop.LeptonicaApiSignatures.pixcmapAddColor(handle,color.Red,color.Green,color.Blue) == 0;
		}

		public bool AddNewColor(PixColor color,out int index)
		{
			return Interop.LeptonicaApiSignatures.pixcmapAddNewColor(handle,color.Red,color.Green,color.Blue,out index) == 0;
		}

		public bool AddNearestColor(PixColor color,out int index)
		{
			return Interop.LeptonicaApiSignatures.pixcmapAddNearestColor(handle,color.Red,color.Green,color.Blue,out index) == 0;
		}

		public bool AddBlackOrWhite(int color,out int index)
		{
			return Interop.LeptonicaApiSignatures.pixcmapAddBlackOrWhite(handle,color,out index) == 0;
		}

		public bool SetBlackOrWhite(bool setBlack,bool setWhite)
		{
			return Interop.LeptonicaApiSignatures.pixcmapSetBlackAndWhite(handle,setBlack ? 1 : 0,setWhite ? 1 : 0) == 0;
		}

		public bool IsUsableColor(PixColor color)
		{
			int usable;
			if (Interop.LeptonicaApiSignatures.pixcmapUsableColor(handle,color.Red,color.Green,color.Blue,out usable) == 0)
			{
				return usable == 1;
			}
			else
			{
				throw new InvalidOperationException("Failed to detect if color was usable or not.");
			}
		}

		public void Clear()
		{
			if (Interop.LeptonicaApiSignatures.pixcmapClear(handle) != 0)
			{
				throw new InvalidOperationException("Failed to clear color map.");
			}
		}

		public PixColor this[int index]
		{
			get
			{
				int color;
				if (Interop.LeptonicaApiSignatures.pixcmapGetColor32(handle,index,out color) == 0)
				{
					return PixColor.FromRgb((uint)color);
				}
				else
				{
					throw new InvalidOperationException("Failed to retrieve color.");
				}
			}
			set
			{
				if (Interop.LeptonicaApiSignatures.pixcmapResetColor(handle,index,value.Red,value.Green,value.Blue) != 0)
				{
					throw new InvalidOperationException("Failed to reset color.");
				}
			}
		}

		public void Dispose()
		{
			var tmpHandle = Handle.Handle;
			Interop.LeptonicaApiSignatures.pixcmapDestroy(ref tmpHandle);
			this.handle = new HandleRef(this,IntPtr.Zero);
		}
	}
}
