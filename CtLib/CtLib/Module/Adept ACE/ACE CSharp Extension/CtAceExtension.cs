using System;
using System.Collections.Generic;
using System.Linq;

using Ace.Adept.Server.Desktop;
using Ace.Core.Server;
using Ace.HSVision.Client.ImageDisplay;
using Ace.HSVision.Server.Parameters;
using Ace.HSVision.Server.Tools;

namespace CtLib.Module.Adept.Extension {

	/// <summary>提供 Omron Adept ACE 之相關物件擴充</summary>
	public static class CtAceExtend {

		#region VisionTransform Extensions
		/// <summary>將 <see cref="VisionTransform"/> 數值加上定量，並非向量操作</summary>
		/// <param name="visTrans">加數 <see cref="VisionTransform"/></param>
		/// <param name="adder">欲加至 X, Y, Degrees 的數值</param>
		public static void NumericAdd(this VisionTransform visTrans, double adder) {
			visTrans.X += adder;
			visTrans.Y += adder;
			visTrans.Degrees += adder;
		}

		/// <summary>將兩個 <see cref="VisionTransform"/> 數值相加，並非向量操作</summary>
		/// <param name="visTrans">加數 <see cref="VisionTransform"/></param>
		/// <param name="adder">被加數 <see cref="VisionTransform"/></param>
		public static void NumericAdd(this VisionTransform visTrans, VisionTransform adder) {
			visTrans.X += adder.X;
			visTrans.Y += adder.Y;
			visTrans.Degrees += adder.Degrees;
		}

		/// <summary>將兩個 <see cref="VisionTransform"/> 數值相加，並非向量操作</summary>
		/// <param name="visTrans">加數 <see cref="VisionTransform"/></param>
		/// <param name="adder">被加數 <see cref="VisionTransform"/></param>
		public static void NumericAdd(this VisionTransform visTrans, VisionPoint adder) {
			visTrans.X += adder.X;
			visTrans.Y += adder.Y;
		}

		/// <summary>將兩個 <see cref="VisionTransform"/> 數值相加，並非向量操作</summary>
		/// <param name="visTrans">加數 <see cref="VisionTransform"/></param>
		/// <param name="adder">被加數 <see cref="VisionTransform"/></param>
		/// <param name="deg">角度分量</param>
		public static void NumericAdd(this VisionTransform visTrans, VisionPoint adder, double deg) {
			visTrans.X += adder.X;
			visTrans.Y += adder.Y;
			visTrans.Degrees += deg;
		}

		/// <summary>將 <see cref="VisionTransform"/> 加上特定的數值，並非向量操作</summary>
		/// <param name="visTrans">加數 <see cref="VisionTransform"/></param>
		/// <param name="x">X 分量</param>
		/// <param name="y">Y 分量</param>
		/// <param name="deg">角度分量</param>
		public static void NumericAdd(this VisionTransform visTrans, double x, double y, double deg) {
			visTrans.X += x;
			visTrans.Y += y;
			visTrans.Degrees += deg;
		}

		/// <summary>將 <see cref="VisionTransform"/> 數值減去定量，並非向量操作</summary>
		/// <param name="visTrans">減數 <see cref="VisionTransform"/></param>
		/// <param name="subtractor">欲減少 X, Y, Degrees 的數值</param>
		public static void NumericSubtract(this VisionTransform visTrans, double subtractor) {
			visTrans.X -= subtractor;
			visTrans.Y -= subtractor;
			visTrans.Degrees -= subtractor;
		}

		/// <summary>將兩個 <see cref="VisionTransform"/> 數值相減，並非向量操作</summary>
		/// <param name="visTrans">減數 <see cref="VisionTransform"/></param>
		/// <param name="subtractor">被減數 <see cref="VisionTransform"/></param>
		public static void NumericSubtract(this VisionTransform visTrans, VisionTransform subtractor) {
			visTrans.X -= subtractor.X;
			visTrans.Y -= subtractor.Y;
			visTrans.Degrees -= subtractor.Degrees;
		}

		/// <summary>將 <see cref="VisionTransform"/> 數值乘上定量，並非向量操作</summary>
		/// <param name="visTrans">乘數 <see cref="VisionTransform"/></param>
		/// <param name="multiplier">欲乘至 X, Y, Degrees 的數值</param>
		public static void NumericMultiply(this VisionTransform visTrans, double multiplier) {
			visTrans.X *= multiplier;
			visTrans.Y *= multiplier;
			visTrans.Degrees *= multiplier;
		}

		/// <summary>將兩個 <see cref="VisionTransform"/> 數值相乘，並非向量操作，如需向量操作請直接使用「*」</summary>
		/// <param name="visTrans">乘數 <see cref="VisionTransform"/></param>
		/// <param name="multiplier">被乘數 <see cref="VisionTransform"/></param>
		public static void NumericMultiply(this VisionTransform visTrans, VisionTransform multiplier) {
			visTrans.X *= multiplier.X;
			visTrans.Y *= multiplier.Y;
			visTrans.Degrees *= multiplier.Degrees;
		}

		/// <summary>將 <see cref="VisionTransform"/> 數值除以定量，並非向量操作</summary>
		/// <param name="visTrans">除數 <see cref="VisionTransform"/></param>
		/// <param name="divider">欲除至 X, Y, Degrees 的數值</param>
		public static void NumericDivide(this VisionTransform visTrans, double divider) {
			visTrans.X /= divider;
			visTrans.Y /= divider;
			visTrans.Degrees /= divider;
		}

		/// <summary>將兩個 <see cref="VisionTransform"/> 數值相除，並非向量操作</summary>
		/// <param name="visTrans">除數 <see cref="VisionTransform"/></param>
		/// <param name="divider">被除數 <see cref="VisionTransform"/></param>
		public static void NumericDivide(this VisionTransform visTrans, VisionTransform divider) {
			visTrans.X /= divider.X;
			visTrans.Y /= divider.Y;
			visTrans.Degrees /= divider.Degrees;
		}

		/// <summary>計算兩個 <see cref="VisionTransform"/> 之平面距離</summary>
		/// <param name="visTrans">起始 <see cref="VisionTransform"/></param>
		/// <param name="otherVT">終點 <see cref="VisionTransform"/></param>
		/// <returns>兩點距離</returns>
		public static double DistanceFrom(this VisionTransform visTrans, VisionTransform otherVT) {
			double x = Math.Pow(Math.Abs(visTrans.X - otherVT.X), 2);
			double y = Math.Pow(Math.Abs(visTrans.Y - otherVT.Y), 2);
			return Math.Sqrt(x + y);
		}

		/// <summary>回傳 <see cref="Ace.HSVision.Server.Parameters.VisionTransform"/> 之 X、Y 座標為 <see cref="Ace.HSVision.Server.Parameters.VisionPoint"/></summary>
		/// <param name="visTrans">欲轉換的 <see cref="Ace.HSVision.Server.Parameters.VisionTransform"/></param>
		/// <returns>相對應的 <see cref="Ace.HSVision.Server.Parameters.VisionPoint"/></returns>
		public static VisionPoint VisionPoint(this VisionTransform visTrans) {
			return new VisionPoint(visTrans);
		}

        ///<summary>回傳<see cref="Ace.HSVision.Server.Parameters.VisionTransform"/>副本</summary>
        ///<param name="visTrans">欲複製之<see cref="Ace.HSVision.Server.Parameters.VisionTransform"/>來源</param>
        ///<returns>由來源建立的<see cref="Ace.HSVision.Server.Parameters.VisionTransform"/>副本</returns>
        public static VisionTransform Clone(this VisionTransform visTrans) {
            return new VisionTransform(visTrans.X, visTrans.Y, visTrans.Degrees);
        }
        
		#endregion

		#region Transform3D Extensions
		/// <summary>將兩個 <see cref="Transform3D"/> 數值相加，非向量操作</summary>
		/// <param name="trans3D">加數 <see cref="Transform3D"/></param>
		/// <param name="adder">被加數 <see cref="Transform3D"/></param>
		/// <returns>相加後的 <see cref="Transform3D"/></returns>
		public static Transform3D NumericAdd(this Transform3D trans3D, Transform3D adder) {
			return new Transform3D(
				trans3D.DX + adder.DX,
				trans3D.DY + adder.DY,
				trans3D.DZ + adder.DZ,
				trans3D.Yaw + adder.Yaw,
				trans3D.Pitch + adder.Pitch,
				trans3D.Roll + adder.Roll
			);
		}

		/// <summary>將兩個 <see cref="Transform3D"/> 數值相減，並非向量操作</summary>
		/// <param name="trans3D">減數 <see cref="Transform3D"/></param>
		/// <param name="subtractor">被減數 <see cref="Transform3D"/></param>
		/// <returns>相減後的 <see cref="Transform3D"/></returns>
		public static Transform3D NumericSubtract(this Transform3D trans3D, Transform3D subtractor) {
			return new Transform3D(
				trans3D.DX - subtractor.DX,
				trans3D.DY - subtractor.DY,
				trans3D.DZ - subtractor.DZ,
				trans3D.Yaw - subtractor.Yaw,
				trans3D.Pitch - subtractor.Pitch,
				trans3D.Roll - subtractor.Roll
			);
		}

		/// <summary>將兩個 <see cref="Transform3D"/> 數值相乘，並非向量操作</summary>
		/// <param name="trans3D">乘數 <see cref="Transform3D"/></param>
		/// <param name="multiplier">被乘數 <see cref="Transform3D"/></param>
		/// <returns>相乘後的 <see cref="Transform3D"/></returns>
		public static Transform3D NumericMultiply(this Transform3D trans3D, double multiplier) {
			return new Transform3D(
				trans3D.DX * multiplier,
				trans3D.DY * multiplier,
				trans3D.DZ * multiplier,
				trans3D.Yaw * multiplier,
				trans3D.Pitch * multiplier,
				trans3D.Roll * multiplier
			);
		}

		/// <summary>將兩個 <see cref="Transform3D"/> 數值相乘，並非向量操作，如需向量操作請直接使用「*」</summary>
		/// <param name="trans3D">乘數 <see cref="Transform3D"/></param>
		/// <param name="multiplier">被乘數 <see cref="Transform3D"/></param>
		/// <returns>相乘後的 <see cref="Transform3D"/></returns>
		public static Transform3D NumericMultiply(this Transform3D trans3D, Transform3D multiplier) {
			return new Transform3D(
				trans3D.DX * multiplier.DX,
				trans3D.DY * multiplier.DY,
				trans3D.DZ * multiplier.DZ,
				trans3D.Yaw * multiplier.Yaw,
				trans3D.Pitch * multiplier.Pitch,
				trans3D.Roll * multiplier.Roll
			);
		}

		/// <summary>將兩個 <see cref="Transform3D"/> 數值相除，並非向量操作</summary>
		/// <param name="trans3D">除數 <see cref="Transform3D"/></param>
		/// <param name="divider">被除數 <see cref="Transform3D"/></param>
		/// <returns>相除後的 <see cref="Transform3D"/></returns>
		public static Transform3D NumericDivide(this Transform3D trans3D, double divider) {
			return new Transform3D(
				trans3D.DX / divider,
				trans3D.DY / divider,
				trans3D.DZ / divider,
				trans3D.Yaw / divider,
				trans3D.Pitch / divider,
				trans3D.Roll / divider
			);
		}

		/// <summary>將兩個 <see cref="Transform3D"/> 數值相除，並非向量操作</summary>
		/// <param name="trans3D">除數 <see cref="Transform3D"/></param>
		/// <param name="divider">被除數 <see cref="Transform3D"/></param>
		/// <returns>相除後的 <see cref="Transform3D"/></returns>
		public static Transform3D NumericDivide(this Transform3D trans3D, Transform3D divider) {
			return new Transform3D(
				trans3D.DX / divider.DX,
				trans3D.DY / divider.DY,
				trans3D.DZ / divider.DZ,
				trans3D.Yaw / divider.Yaw,
				trans3D.Pitch / divider.Pitch,
				trans3D.Roll / divider.Roll
			);
		}

		/// <summary>將 <see cref="Transform3D"/> 轉換為 <see cref="List{T}"/><para>T 為 <see cref="double"/></para></summary>
		/// <param name="trans3D">欲轉換的 <see cref="Transform3D"/></param>
		/// <returns>相對應的 <see cref="List{T}"/></returns>
		public static List<double> ToList(this Transform3D trans3D) {
			return new List<double> {
				trans3D.DX,
				trans3D.DY,
				trans3D.DZ,
				trans3D.Yaw,
				trans3D.Pitch,
				trans3D.Roll
			};
		}

		/// <summary>將 <see cref="double"/> 數值集合轉換為 <see cref="Transform3D"/></summary>
		/// <param name="src">數值集合</param>
		/// <returns>對應的 <see cref="Transform3D"/></returns>
		public static Transform3D ToTransform3D(this IEnumerable<double> src) {
			Transform3D tran3d = null;
			if (src.Count() == 3) {
				tran3d = new Transform3D(
					src.ElementAt(0),
					src.ElementAt(1),
					src.ElementAt(2)
				);
			} else if (src.Count() == 6) {
				tran3d = new Transform3D(
					src.ElementAt(0),
					src.ElementAt(1),
					src.ElementAt(2),
					src.ElementAt(3),
					src.ElementAt(4),
					src.ElementAt(5)
				);
			} else throw new ArgumentOutOfRangeException("Transform3D", "資料來源集合的元素數量錯誤");
			return tran3d;
		}
		#endregion

		#region VisionPoint Extensions
		/// <summary>回傳 <see cref="Ace.HSVision.Server.Parameters.VisionPoint"/> 之 X、Y 座標為 <see cref="Ace.HSVision.Server.Parameters.VisionTransform"/></summary>
		/// <param name="visPoint">欲轉換的 <see cref="Ace.HSVision.Server.Parameters.VisionPoint"/></param>
		/// <returns>相對應的 <see cref="Ace.HSVision.Server.Parameters.VisionTransform"/></returns>
		public static VisionTransform VisionTransform(this VisionPoint visPoint) {
			return new VisionTransform(visPoint);
		}

        ///<summary>回傳<see cref="Ace.HSVision.Server.Parameters.VisionPoint"/>副本</summary>
        ///<param name="visPoint">要複製的<see cref="Ace.HSVision.Server.Parameters.VisionPoint"/>來源</param>
        ///<returns>由來源複製的<see cref="Ace.HSVision.Server.Parameters.VisionPoint"/>副本</returns> 
        public static VisionPoint Clone(this VisionPoint visPoint) {
            return new VisionPoint(visPoint.X, visPoint.Y);
        }

        ///<summary>座標點平面旋轉</summary>
        ///<param name="visPoint">要旋轉的<see cref="Ace.HSVision.Server.Parameters.VisionPoint"/></param>
        ///<param name="theta">要旋轉的角度</param>
        public static void Rotation(this VisionPoint visPoint,double theta) {
            VisionPoint ori = visPoint.Clone();
            double cos = Math.Cos(theta * Math.PI / 180);
            double sin = Math.Sin(theta * Math.PI / 180);
            visPoint.X = cos * ori.X - sin * ori.Y;
            visPoint.Y = sin * ori.X + sin * ori.Y;        
        }

		#endregion

		#region Precision Point Extensions
		/// <summary>將 <see cref="PrecisionPoint"/> 轉為 <see cref="Array"/><para>T 為 <see cref="double"/></para></summary>
		/// <param name="pp">欲轉換的 <see cref="PrecisionPoint"/></param>
		/// <returns>對應的 <see cref="Array"/></returns>
		public static double[] ToArray(this PrecisionPoint pp) {
			double[] loc = new double[pp.Length];
			for (int idx = 0; idx < pp.Length; idx++) {
				loc[idx] = pp.GetJoint(idx);
			}
			return loc;
		}

		/// <summary>將 <see cref="PrecisionPoint"/> 轉為 <see cref="List{T}"/><para>T 為 <see cref="double"/></para></summary>
		/// <param name="pp">欲轉換的 <see cref="PrecisionPoint"/></param>
		/// <returns>對應的 <see cref="List{T}"/></returns>
		public static List<double> ToList(this PrecisionPoint pp) {
			List<double> loc = new List<double>();
			for (byte idx = 0; idx < pp.Length; idx++) {
				loc.Add(pp.GetJoint(idx));
			}
			return loc;
		}

		/// <summary>將 <see cref="double"/> 數值集合轉換為 <see cref="PrecisionPoint"/></summary>
		/// <param name="src">數值來源</param>
		/// <returns>對應的 <see cref="PrecisionPoint"/></returns>
		public static PrecisionPoint ToPrecisionPoint(this IEnumerable<double> src) {
			PrecisionPoint pp = new PrecisionPoint();
			for (int idx = 0; idx < src.Count(); idx++) {
				pp.SetJoint(idx, (float)src.ElementAt(idx));
			}
			return pp;
		}
		#endregion

		#region Vision Tools

		/// <summary>繪出 Region Of Interest 於 <see cref="MarkerOverlayCollection"/> 裡</summary>
		/// <param name="tool">含有 ROI 之 <see cref="IVisionTool"/></param>
		/// <param name="markColl">欲新增的 <see cref="MarkerOverlayCollection"/>，通常於 <see cref="ICSharpCustomTool.OverlayMarkers"/></param>
		/// <param name="color">欲繪製的 ROI 顏色</param>
		public static void DrawRoi(this IVisionTool tool, MarkerOverlayCollection markColl, MarkerColor color) {
			/*-- Descriptor receiver --*/
			MarkerDescriptor descriptor = null;

			/*-- Check vision tool type and draw SearchRegion on CVT --*/
			if (tool is IRectangularRoiTool) {  //IVisionTool using rectangle region
				IRectangularRoiTool rctRoi = tool as IRectangularRoiTool;
				if (rctRoi.Origins.Length > 0) {
					descriptor = markColl.AddRectangleMarker(
						new VisionRectangle(
							rctRoi.Origins[0],
							rctRoi.SearchRegion
						)
					);
				} //else throw new ArgumentOutOfRangeException("Origins", "Vision tool offset not found. Ormon ACE may crashed.");

			} else if (tool is ILineRoiTool) {  //IVisionTool using line region
				ILineRoiTool lineRoi = tool as ILineRoiTool;
				if (lineRoi.Origins.Length > 0) {
					descriptor = markColl.AddRectangleMarker(
						new VisionRectangle(
							lineRoi.Origins[0],
							lineRoi.SearchRegion
						)
					);
				} //else throw new ArgumentOutOfRangeException("Origins", "Vision tool offset not found. Ormon ACE may crashed.");
			} else if (tool is IArcRoiTool) {   //IVisionTool using arc region
				IArcRoiTool arcRoi = tool as IArcRoiTool;
				VisionArc arc = arcRoi.SearchRegion;
				if (arcRoi.Origins.Length > 0) {
					descriptor = markColl.AddArcMarker(
						arcRoi.Origins[0].X,
						arcRoi.Origins[0].Y,
						arc.Radius,
						arc.Thickness,
						arcRoi.Origins[0].Degrees,
						arc.Opening
					);
				} //else throw new ArgumentOutOfRangeException("Origins", "Vision tool offset not found. Ormon ACE may crashed.");
			}

			/*-- Set descriptor's color and pen width if exist --*/
			if (descriptor != null) {
				descriptor.Color = color;
				descriptor.PenWidth = MarkerPenWidth.Thin;
			}
		}

        /// <summary>
        /// 繪出註解在MarkerOverlayCollection裡
        /// </summary>
        /// <param name="tool">要附加註解的<see cref="IVisionTool"/></param>
        /// <param name="markColl">欲加入的<see cref="MarkerOverlayCollection"/></param>
        /// <param name="comment">註解內容</param>
        /// <param name="color">註解顏色</param>
        public static void DrawComment(this IVisionTool tool,MarkerOverlayCollection markColl,string comment,MarkerColor color) {
            /*-- Descriptor receiver --*/
            MarkerDescriptor descriptor = null;
            VisionTransform pos = null;//註解要貼上的位置
            float ofsX = 0;//註解貼上位置X補償
            float ofsY = 0;//註解貼上位置Y補償


            /*-- Check vision tool type and draw SearchRegion on CVT --*/
            if (tool is IRectangularRoiTool) {  //IVisionTool using rectangle region
                IRectangularRoiTool rctRoi = tool as IRectangularRoiTool;
                if (rctRoi.Origins.Length > 0) {
                    ofsX = (float)rctRoi.SearchRegion.Width / 2;
                    ofsY = (float)rctRoi.SearchRegion.Height / 2;
                    pos = rctRoi.Origins[0];
                    pos = new VisionTransform(pos.X - ofsX, pos.Y + ofsY, pos.Degrees);//將註解位置設在ROI左上角
                    descriptor = markColl.AddLabelMarker(pos, comment);
                } //else throw new ArgumentOutOfRangeException("Origins", "Vision tool offset not found. Ormon ACE may crashed.");

            } else if (tool is ILineRoiTool) {  //IVisionTool using line region
                ILineRoiTool lineRoi = tool as ILineRoiTool;
                if (lineRoi.Origins.Length > 0) {
                    ofsX = (float)lineRoi.SearchRegion.Width / 2;
                    ofsY = (float)lineRoi.SearchRegion.Height / 2;
                    pos = lineRoi.Origins[0];
                    pos = new VisionTransform(pos.X - ofsX, pos.Y + ofsY, pos.Degrees);//將註解位置設在ROI左上角
                    descriptor = markColl.AddLabelMarker(pos, comment);
                } //else throw new ArgumentOutOfRangeException("Origins", "Vision tool offset not found. Ormon ACE may crashed.");
            } else if (tool is IArcRoiTool) {   //IVisionTool using arc region
                IArcRoiTool arcRoi = tool as IArcRoiTool;
                VisionArc arc = arcRoi.SearchRegion;
                if (arcRoi.Origins.Length > 0) {
                    descriptor = markColl.AddLabelMarker(arcRoi.Origins[0], comment);
                    
                } //else throw new ArgumentOutOfRangeException("Origins", "Vision tool offset not found. Ormon ACE may crashed.");
            }

            /*-- Set descriptor's color and pen width if exist --*/
            if (descriptor != null) {
                descriptor.Color = color;
                descriptor.PenWidth = MarkerPenWidth.Thin;
                
            }
        }

		/// <summary>繪出 <see cref="IVisionTool"/> 之執行結果</summary>
		/// <param name="tool">欲畫出結果的 <see cref="IVisionTool"/></param>
		/// <param name="markColl">欲新增的 <see cref="MarkerOverlayCollection"/>，通常於 <see cref="ICSharpCustomTool.OverlayMarkers"/></param>
		/// <param name="clrPass">含有正確執行結果(PASS)的顏色</param>
		/// <param name="clrNg">無執行結果(NG)的顏色</param>
		/// <param name="clrAxis">執行結果中心顏色</param>
		public static void DrawResult(this IVisionTool tool, MarkerOverlayCollection markColl, MarkerColor clrPass, MarkerColor clrNg, MarkerColor clrAxis) {
			/*-- If there have results, draw ROI with passed color --*/
			if (tool.ResultsAvailable) {
				/* Define style variables */
				double length = 1.5;
				MarkerColor resultColor = clrAxis;
				MarkerPenWidth resultWidth = MarkerPenWidth.Thick;

                /* Draw ROI */
                tool.DrawRoi(markColl, clrPass);

				/* Do action with specifed tool type */
				if (tool is ILineFinderTool) {
					ILineFinderTool lineTool = tool as ILineFinderTool;
					Array.ForEach(
						lineTool.Results,
						result => {
							MarkerDescriptor descriptor = markColl.AddLineMarker(
								result.StartPoint,
								result.EndPoint
							);

							descriptor.Color = resultColor;
							descriptor.PenWidth = resultWidth;
						}
					);
				} else if (tool is IPointFinderTool) {
					IPointFinderTool pointTool = tool as IPointFinderTool;
					Array.ForEach(
						pointTool.Results,
						result => {
							MarkerDescriptor descriptor = markColl.AddPointMarker(
								result.Point
							);

							descriptor.Color = resultColor;
							descriptor.PenWidth = resultWidth;
						}
					);
				} else if (tool is IArcFinderTool) {
					IArcFinderTool arcFinder = tool as IArcFinderTool;
					Array.ForEach(
						arcFinder.Results,
						result => {
							MarkerDescriptor descriptor = markColl.AddArcMarker(
								result.Center.X,
								result.Center.Y,
								result.Radius,
								0,
								result.Center.Degrees,
								result.Arc.Opening
							);

							descriptor.Color = resultColor;
							descriptor.PenWidth = resultWidth;
						}
					);
				} else {
					VisionTransform[] results = tool.GetTransformResults();
					Array.ForEach(
						results,
						visTrans => {
							MarkerDescriptor descriptor = markColl.AddAxesMarker(
								visTrans,
								length,
								length
							);

							descriptor.Color = resultColor;
							descriptor.PenWidth = resultWidth;
						}
					);
				}

				/*-- If there without results, draw ROI with ng color --*/
			} else tool.DrawRoi(markColl, clrNg);
		}

		#endregion
	}
}
