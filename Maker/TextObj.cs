using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace DS2DEngine
{
	public class TextObj : GameObj
	{
		private SpriteFont m_defaultFont;

		private SpriteFont m_font;

		private string m_text = "";

		private Vector2 m_textSize = Vector2.Zero;

		public Types.TextAlign Align;

		private float m_fontSize = 1f;

		protected Vector2 m_internalFontSizeScale = new Vector2(1f, 1f);

		private string m_typewriteText = "";

		private bool m_isTypewriting;

		private float m_typewriteSpeed;

		private float m_typewriteCounter;

		private int m_typewriteCharLength;

		private string m_tapSFX = "";

		private bool m_isPaused;

		private Color m_outlineColour = Color.Black;

		private int m_fontIndex = -1;

		private Texture2D m_textureValue;

		public bool LimitCorners;

		public static TextObj.DisposeDelegate disposeMethod;

		public override Vector2 Anchor
		{
			get
			{
                bool is_zh = (this.GetFixedScale() > 1.1f);

				Vector2 vector2 = Vector2.Zero;

                if (this.GetFixedScale() > 1.1f) {
                    vector2 = -Vector2.UnitY;
                }

				if (this.Align == Types.TextAlign.None)
				{
                    vector2 += base.Anchor;
                    return vector2;
				}
				float scaleX = 1f / (this.ScaleX * this.m_internalFontSizeScale.X);
				if (this.Align != Types.TextAlign.Left)
				{
					vector2 += (this.Align != Types.TextAlign.Centre ? new Vector2((float)this.Width * scaleX, this._anchor.Y) : new Vector2((float)(this.Width / 2) * scaleX, this._anchor.Y));
				}
				else
				{
					vector2 += new Vector2(0f, this._anchor.Y);
				}
				if (this.Flip == SpriteEffects.FlipHorizontally)
				{
					vector2.X += (float)this.Width * scaleX - vector2.X;
				}

				return vector2;
			}
			set
			{
				base.Anchor = value;
			}
		}

		public override float AnchorX
		{
			get
			{
				return this.Anchor.X;
			}
			set
			{
				base.AnchorX = value;
			}
		}

		public override float AnchorY
		{
			get
			{
				return this._anchor.Y;
			}
			set
			{
				this._anchor.Y = value;
			}
		}

		public SpriteFont defaultFont
		{
			get
			{
				return this.m_defaultFont;
			}
		}

		public Vector2 DropShadow
		{
			get;
			set;
		}

		public SpriteFont Font
		{
			get
			{
				return this.m_font;
			}
			set
			{
				if (value != null)
				{
					this.m_fontIndex = SpriteFontArray.SpriteFontList.IndexOf(value);
					if (this.m_fontIndex == -1)
					{
						throw new Exception("Cannot find font in SpriteFontArray");
					}
					FieldInfo field = value.GetType().GetField("textureValue", BindingFlags.Instance | BindingFlags.NonPublic);
					this.m_textureValue = (Texture2D)field.GetValue(value);
				}
				this.m_defaultFont = value;
				this.m_font = value;
			}
		}

		public virtual float FontSize
		{
			get
			{
				return this.m_fontSize;
			}
			set
			{
				Vector2 vector2 =  this.Font.MeasureString("0");
				float single = value * GetFixedScale() / vector2.X;
				this.m_internalFontSizeScale = new Vector2(single, single);
				this.m_fontSize = value;
			}
		}

		public override int Height
		{
			get
			{
				return (int)(this.m_textSize.Y * this.ScaleY * this.m_internalFontSizeScale.Y);
			}
		}

		public bool isLogographic
		{
			get;
			set;
		}

		public bool IsPaused
		{
			get
			{
				return this.m_isPaused;
			}
		}

		public bool IsTypewriting
		{
			get
			{
				return this.m_isTypewriting;
			}
		}

		public string locStringID
		{
			get;
			set;
		}

		public Color OutlineColour
		{
			get
			{
				if (base.Parent == null || base.Parent != null && base.Parent.OutlineColour == Color.Black)
				{
					return this.m_outlineColour;
				}
				return base.Parent.OutlineColour;
			}
			set
			{
				this.m_outlineColour = value;
			}
		}

		public int OutlineWidth
		{
			get;
			set;
		}

		public virtual string Text
		{
			get
			{
				return this.m_text;
			}
			set
			{
				this.m_text = value;
				if (this.Font != null)
				{
					this.m_textSize = this.Font.MeasureString(this.m_text);
				}
			}
		}

		public override int Width
		{
			get
			{
				return (int)(this.m_textSize.X * this.ScaleX * this.m_internalFontSizeScale.X);
			}
		}

		public TextObj(SpriteFont font = null)
		{
			this.m_defaultFont = font;
			this.m_font = font;
			this.isLogographic = false;
			if (font != null)
			{
				this.m_fontIndex = SpriteFontArray.SpriteFontList.IndexOf(font);
				if (this.m_fontIndex == -1)
				{
					throw new Exception("Cannot find font in SpriteFontArray");
				}
				FieldInfo field = font.GetType().GetField("textureValue", BindingFlags.Instance | BindingFlags.NonPublic);
				this.m_textureValue = (Texture2D)field.GetValue(font);
			}
		}

		public void BeginTypeWriting(float duration, string sound = "")
		{
			this.m_isTypewriting = true;
			this.m_typewriteText = this.Text;
			this.m_typewriteSpeed = duration / (float)this.Text.Length;
			this.m_typewriteCounter = this.m_typewriteSpeed;
			this.m_typewriteCharLength = 0;
			this.Text = "";
			this.m_tapSFX = sound;
		}

		public void ChangeFontNoDefault(SpriteFont font)
		{
			this.m_font = font;
			this.FontSize = this.m_fontSize;
		}

		protected override GameObj CreateCloneInstance()
		{
			TextObj textObj = new TextObj(this.m_defaultFont);
			textObj.ChangeFontNoDefault(this.m_font);
			return textObj;
		}

		public override void Dispose()
		{
			if (!base.IsDisposed)
			{
				this.m_font = null;
				if (TextObj.disposeMethod != null)
				{
					TextObj.disposeMethod(this);
				}
				base.Dispose();
			}
		}

		public override void Draw(Camera2D camera)
		{
			if (this.m_textureValue.IsDisposed)
			{
				this.m_font = SpriteFontArray.SpriteFontList[this.m_fontIndex];
			}
			if (this.IsTypewriting && this.m_typewriteCounter > 0f && !this.IsPaused)
			{
				TextObj totalSeconds = this;
				float mTypewriteCounter = totalSeconds.m_typewriteCounter;
				TimeSpan elapsedGameTime = camera.GameTime.ElapsedGameTime;
				totalSeconds.m_typewriteCounter = mTypewriteCounter - (float)elapsedGameTime.TotalSeconds;
				if (this.m_typewriteCounter <= 0f && this.Text != this.m_typewriteText)
				{
					if (this.m_tapSFX != "")
					{
						SoundManager.PlaySound(this.m_tapSFX);
					}
					this.m_typewriteCounter = this.m_typewriteSpeed;
					this.m_typewriteCharLength++;
					this.Text = this.m_typewriteText.Substring(0, this.m_typewriteCharLength);
				}
			}
			if (this.IsTypewriting && this.Text == this.m_typewriteText)
			{
				this.m_isTypewriting = false;
			}
			if (this.DropShadow != Vector2.Zero)
			{
				this.DrawDropShadow(camera);
			}
			if (this.OutlineWidth > 0 && (base.Parent == null || base.Parent.OutlineWidth == 0))
			{
				this.DrawOutline(camera);
			}
			if (base.Visible)
			{
				if (base.Parent == null || base.OverrideParentScale)
				{
					camera.DrawString(this.m_font, this.m_text, base.AbsPosition, base.TextureColor * base.Opacity, MathHelper.ToRadians(base.Rotation), this.Anchor, this.Scale * this.m_internalFontSizeScale, SpriteEffects.None, base.Layer);
					return;
				}
				camera.DrawString(this.m_font, this.m_text, base.AbsPosition, base.TextureColor * base.Opacity, MathHelper.ToRadians(base.Parent.Rotation + base.Rotation), this.Anchor, base.Parent.Scale * this.Scale * this.m_internalFontSizeScale, SpriteEffects.None, base.Layer);
			}
		}

		public void DrawDropShadow(Camera2D camera)
		{
			if (base.Visible)
			{
				if (base.Parent == null || base.OverrideParentScale)
				{
					camera.DrawString(this.m_font, this.m_text, base.AbsPosition + this.DropShadow, Color.Black * base.Opacity, MathHelper.ToRadians(base.Rotation), this.Anchor, this.Scale * this.m_internalFontSizeScale, SpriteEffects.None, base.Layer);
					return;
				}
				camera.DrawString(this.m_font, this.m_text, base.AbsPosition + this.DropShadow, Color.Black * base.Opacity, MathHelper.ToRadians(base.Parent.Rotation + base.Rotation), this.Anchor, base.Parent.Scale * this.Scale * this.m_internalFontSizeScale, SpriteEffects.None, base.Layer);
			}
		}

		public override void DrawOutline(Camera2D camera)
		{
			if (this.m_textureValue.IsDisposed)
			{
				this.m_font = SpriteFontArray.SpriteFontList[this.m_fontIndex];
			}
			int outlineWidth = this.OutlineWidth;
			if (this.m_font != null && base.Visible)
			{
				Vector2 absPosition = base.AbsPosition;
				float x = absPosition.X;
				float y = absPosition.Y;
				SpriteEffects flip = this.Flip;
				float radians = MathHelper.ToRadians(base.Rotation);
				Color outlineColour = this.OutlineColour * base.Opacity;
				Vector2 anchor = this.Anchor;
				float layer = base.Layer;
				Vector2 scale = this.Scale * this.m_internalFontSizeScale;
				if (base.Parent == null || base.OverrideParentScale)
				{
					camera.DrawString(this.m_font, this.m_text, new Vector2(x - (float)outlineWidth, y), outlineColour, radians, anchor, scale, flip, layer);
					camera.DrawString(this.m_font, this.m_text, new Vector2(x + (float)outlineWidth, y), outlineColour, radians, anchor, scale, flip, layer);
					camera.DrawString(this.m_font, this.m_text, new Vector2(x, y - (float)outlineWidth), outlineColour, radians, anchor, scale, flip, layer);
					camera.DrawString(this.m_font, this.m_text, new Vector2(x, y + (float)outlineWidth), outlineColour, radians, anchor, scale, flip, layer);
					if (!this.LimitCorners)
					{
						camera.DrawString(this.m_font, this.m_text, new Vector2(x - (float)outlineWidth, y - (float)outlineWidth), outlineColour, radians, anchor, scale, flip, layer);
						camera.DrawString(this.m_font, this.m_text, new Vector2(x + (float)outlineWidth, y + (float)outlineWidth), outlineColour, radians, anchor, scale, flip, layer);
						camera.DrawString(this.m_font, this.m_text, new Vector2(x + (float)outlineWidth, y - (float)outlineWidth), outlineColour, radians, anchor, scale, flip, layer);
						camera.DrawString(this.m_font, this.m_text, new Vector2(x - (float)outlineWidth, y + (float)outlineWidth), outlineColour, radians, anchor, scale, flip, layer);
						return;
					}
				}
				else
				{
					Vector2 vector2 = (base.Parent.Scale * this.Scale) * this.m_internalFontSizeScale;
					radians = MathHelper.ToRadians(base.Parent.Rotation + base.Rotation);
					camera.DrawString(this.m_font, this.m_text, new Vector2(x - (float)outlineWidth, y), outlineColour, radians, anchor, vector2, flip, layer);
					camera.DrawString(this.m_font, this.m_text, new Vector2(x + (float)outlineWidth, y), outlineColour, radians, anchor, vector2, flip, layer);
					camera.DrawString(this.m_font, this.m_text, new Vector2(x, y - (float)outlineWidth), outlineColour, radians, anchor, vector2, flip, layer);
					camera.DrawString(this.m_font, this.m_text, new Vector2(x, y + (float)outlineWidth), outlineColour, radians, anchor, vector2, flip, layer);
					if (!this.LimitCorners)
					{
						camera.DrawString(this.m_font, this.m_text, new Vector2(x - (float)outlineWidth, y - (float)outlineWidth), outlineColour, radians, anchor, vector2, flip, layer);
						camera.DrawString(this.m_font, this.m_text, new Vector2(x + (float)outlineWidth, y + (float)outlineWidth), outlineColour, radians, anchor, vector2, flip, layer);
						camera.DrawString(this.m_font, this.m_text, new Vector2(x + (float)outlineWidth, y - (float)outlineWidth), outlineColour, radians, anchor, vector2, flip, layer);
						camera.DrawString(this.m_font, this.m_text, new Vector2(x - (float)outlineWidth, y + (float)outlineWidth), outlineColour, radians, anchor, vector2, flip, layer);
					}
				}
			}
		}

		protected override void FillCloneInstance(object obj)
		{
			base.FillCloneInstance(obj);
			TextObj text = obj as TextObj;
			text.Text = this.Text;
			text.FontSize = this.FontSize;
			text.OutlineColour = this.OutlineColour;
			text.OutlineWidth = this.OutlineWidth;
			text.DropShadow = this.DropShadow;
			text.Align = this.Align;
			text.LimitCorners = this.LimitCorners;
		}

		public void PauseTypewriting()
		{
			this.m_isPaused = true;
		}

		public void RandomizeSentence(bool randomizeNumbers)
		{
			foreach (Match match in Regex.Matches(this.Text, "\\b[\\w']*\\b"))
			{
				string value = match.Value;
				int num = 0;
				if (!randomizeNumbers && (randomizeNumbers || int.TryParse(value, out num)) || value.Length <= 3)
				{
					continue;
				}
				List<char> list = value.ToList<char>();
				char item = list[0];
				char chr = list[list.Count - 1];
				list.RemoveAt(list.Count - 1);
				list.RemoveAt(0);
				CDGMath.Shuffle<char>(list);
				string str = new string(list.ToArray());
				str = string.Concat(item, str, chr);
				this.Text = this.Text.Replace(value, str);
			}
		}

		public void ResumeTypewriting()
		{
			this.m_isPaused = false;
		}

		public void StopTypeWriting(bool completeText)
		{
			this.m_isTypewriting = false;
			if (completeText)
			{
				this.Text = this.m_typewriteText;
			}
		}

		public virtual void WordWrap(int width)
		{
			string[] strArrays;
			if (this.Width > width)
			{
				string empty = string.Empty;
				string str = string.Empty;
				strArrays = (!this.isLogographic ? this.Text.Split(new char[] { ' ' }) : (
					from x in this.Text
					select x.ToString()).ToArray<string>());
				string[] strArrays1 = strArrays;
				for (int i = 0; i < (int)strArrays1.Length; i++)
				{
					string str1 = strArrays1[i];
					if (this.Font.MeasureString(string.Concat(empty, str1)).X * (this.ScaleX * this.m_internalFontSizeScale.X) > (float)width)
					{
						str = string.Concat(str, empty, '\n');
						empty = string.Empty;
					}
					empty = (!this.isLogographic ? string.Concat(empty, str1, ' ') : string.Concat(empty, str1));
				}
				this.Text = string.Concat(str, empty);
			}
		}

		public delegate void DisposeDelegate(TextObj textObj);

        public float GetFixedScale()
        {
            try
            {
                this.m_font.MeasureString("µ¡‘Ù“≈≤˙");
                return 1.2f;
            }
            catch
            {
                return 1f;
            }
        }
    }
}